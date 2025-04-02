using Sirenix.OdinInspector;
using UnityEngine;
using Stats;
using System;
using Utils;
using Modifiers.Conditions;

namespace Modifiers.Powerups {
    /// <summary>
    /// Base class for all powerups.
    /// By default, this are activated automatically after being setup.
    /// It almost always goes like this:
    /// Whenever X happens, you do Y (like get better Stats or spawn something)
    /// </summary>
    [InlineEditor]
    [DisallowMultipleComponent]
    public abstract class PowerupConfig : ScriptableObject, IModifier {
        [FoldoutGroup("General")]
        public int ID;

        [FoldoutGroup("General")]
        public string Name;

        [FoldoutGroup("General")]
        [SerializeField]
        [Required]
        public StatType _statType;

        [FoldoutGroup("General")]
        [TextArea(0, 5)]
        public string Description;

        [FoldoutGroup("General")]
        [PreviewField(100, ObjectFieldAlignment.Right)]
        public Sprite Icon;

        [FoldoutGroup("General")]
        [Tooltip("The value to be added to the player's stats")]
        public float EndValue = 1.0f;

        [FoldoutGroup("General")]
        [Tooltip("If true, it will be saved in preferences")]
        public bool SaveInPreferences = false;

        [FoldoutGroup("General")]
        [Tooltip("Which color will represent the powerup in the UI")]
        [SerializeField]
        private Color _tagColor = new(166 / 255f, 38 / 255f, 84 / 255f);

        #region Conditions

        [FoldoutGroup("Conditions")]
        [Tooltip("Conditions that need to be met for this Powerup to be activated")]
        [SerializeField]
        private bool _hasConditions = false;

        [FoldoutGroup("Conditions")]
        [ShowIf("_hasConditions")]
        [InfoBox("@\"Whenever: \" + BaseConditionExt.GetConditionText(_conditions)", "_hasConditions")]
        [SerializeField]
        private BaseCondition[] _conditions;

        #endregion

        #region Timer

        [BoxGroup("Timer")]
        [Tooltip("If true, the timer will run indefinitely")]
        public bool Indefinite = true;

        [BoxGroup("Timer")]
        [Tooltip("How long does the powerup last in seconds")]
        [Min(0f)]
        public float Duration = 3f;

        [BoxGroup("Timer")]
        [Tooltip("The cooldown between powerups in seconds.")]
        [Min(0f)]
        public float Cooldown = 1f;

        public bool MarkedAsDone { get; private set; }

        public bool IsCurrentlyActive => _timer.IsRunning;

        private CountdownTimer _timer;

        #endregion

        #region Cooldown

        protected float _nextPowerupTime;

        public bool IsCoolingDown() => Time.time < _nextPowerupTime;
        protected void StartCooldown() => _nextPowerupTime = Time.time + Cooldown;

        #endregion

        protected IStatsModifier _playerStatsManager;

        public Color GetTagColor() => _tagColor;

        public string GetDescription() => Description;

        private bool HasConditions() => _hasConditions && _conditions != null && _conditions.Length > 0;

        #region PowerupRemoteLoaderEditor
        public bool NeedNewConditions() => _conditions == null || _conditions.Length == 0;

        public void SetConditions(BaseCondition[] conditions) {
            _conditions = conditions;
            _hasConditions = conditions != null && conditions.Length > 0;
        }
        #endregion

        /// <summary>
        /// Returns true if the powerup should be removed after it's first use. Replacing the OneTime boolean we had before.
        /// If a Powerup is not indefinite, it should be removed after it's first use.
        /// </summary>
        /// <returns>True if the powerup should be removed, false otherwise.</returns>
        public bool ShouldBeRemoved() => !Indefinite;

        private void OnValidate() {
            _tagColor = new(166 / 255f, 38 / 255f, 84 / 255f);
        }

        public virtual void Setup(Transform owner) {
            _playerStatsManager = owner.GetComponent<IStatsModifier>();

            if (_playerStatsManager == null) {
                throw new NullReferenceException("PlayerStatsManager (a.k.a IStatsModifier implementation) is null.");
            }

            if (SaveInPreferences) {
                _playerStatsManager.SaveStats();
            }

            MarkedAsDone = false;
            _nextPowerupTime = 0;

            _timer = new CountdownTimer(Duration);
            _timer.OnTimerStart += TimerStart;
            _timer.OnTimerStop += TimerStop;

            if (HasConditions()) {
                foreach (var condition in _conditions) {
                    condition.Setup(owner);
                }
            }
        }

        public virtual void Activate(Transform target) {
            if (!CheckConditions()) {
                DebugTools.DebugLog.Log($"Activation Failed! Not all conditions were met for {Name}");
                return;
            }

            if (IsCoolingDown()) {
                DebugTools.DebugLog.Log($"Activation Failed! {Name} is still on cooldown.");
                return;
            }

            // DebugTools.DebugLog.Log($"Starting timer for {Name}: {_timer.Duration}");
            _timer.Start();
        }

        private void ResetConditions() {
            if (HasConditions()) {
                foreach (var condition in _conditions) {
                    condition.ResetCondition();
                }
            }
        }

        public bool CheckConditions() {
            bool allConditionsMet = true;

            if (HasConditions()) {
                foreach (var condition in _conditions) {
                    if (!condition.Check(Time.deltaTime)) {
                        allConditionsMet = false;
                        break;
                    }
                }
            }

            return allConditionsMet;
        }

        public virtual void Tick(float deltaTime) => _timer.Tick(deltaTime);

        private void TimerStart() {
            MarkedAsDone = false;

            if (HasConditions()) {
                foreach (var condition in _conditions) {
                    condition.OnPowerupActivated();
                }
            }
        }

        private void TimerStop() {
            // DebugTools.DebugLog.Log($"Time's up!: {Name}. Starting Cooldown");
            MarkedAsDone = true;

            ResetConditions();

            StartCooldown();
        }

        public virtual void Deactivate() {
            // DebugTools.DebugLog.Log($"Deactivate: {Name}");
            ResetConditions();
            _timer.Reset();
        }

        public virtual void ResetPowerup() {
            if (!_timer.IsRunning) {
                return;
            }

            // DebugTools.DebugLog.Log($"Reset: {Name}");
            ResetConditions();

            _timer.Reset();
        }

        public virtual void Cleanup() {
            _nextPowerupTime = 0;
            MarkedAsDone = false;

            _timer.OnTimerStart -= TimerStart;
            _timer.OnTimerStop -= TimerStop;

            if (HasConditions()) {
                foreach (var condition in _conditions) {
                    condition.Cleanup();
                }
            }
        }
    }
}