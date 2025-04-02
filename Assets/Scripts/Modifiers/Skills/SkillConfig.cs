using Sirenix.OdinInspector;
using Base;
using UnityEngine;
using Utils;
using Modifiers.Conditions;
using System;

namespace Modifiers.Skills {
    [InlineEditor]
    [DisallowMultipleComponent]
    public abstract class SkillConfig : ScriptableObject, IModifier {
        [FoldoutGroup("General")]
        public int ID;

        [FoldoutGroup("General")]
        public string Name;

        [FoldoutGroup("General")]
        public SkillType SkillType;

        [FoldoutGroup("General")]
        [TextArea(0, 3)]
        public string Description;

        [FoldoutGroup("General")]
        [PreviewField(100, ObjectFieldAlignment.Right)]
        public Sprite Icon;

        [FoldoutGroup("General")]
        public float EndValue = 2f;

        [FoldoutGroup("General")]
        [Tooltip("In seconds")]
        public float Duration = 3f;

        [FoldoutGroup("General")]
        [Tooltip("It will be disposed after the first use")]
        public bool OneTime = false;

        [FoldoutGroup("General")]
        [Tooltip("Which color will represent the powerup in the UI")]
        [SerializeField]
        private Color _tagColor = new(127 / 255f, 211 / 255f, 230 / 255f);

        #region Conditions

        [FoldoutGroup("Conditions")]
        [Tooltip("Conditions that need to be met for this Skill to be activated")]
        [SerializeField]
        private bool _hasConditions = false;

        [FoldoutGroup("Conditions")]
        [ShowIf("_hasConditions")]
        [InfoBox("@\"Whenever: \" + BaseConditionExt.GetConditionText(_conditions)", "_hasConditions")]
        [SerializeField]
        private BaseCondition[] _conditions;

        #endregion

        protected const int MAX_DURATION = 120;

        protected CountdownTimer _timer;

        public bool MarkedAsDone { get; private set; }

        public bool IsCurrentlyActive => _timer.IsRunning;

        protected ICharacterHolder _holder;

        public Color GetTagColor() => _tagColor;

        public string GetDescription() => Description;

        private bool HasConditions() => _hasConditions && _conditions != null && _conditions.Length > 0;

        private void OnValidate() {
            _tagColor = new(127 / 255f, 211 / 255f, 230 / 255f);
        }

        public virtual void Setup(Transform owner) {
            _holder = owner.GetComponent<ICharacterHolder>();

            if (_holder == null) {
                throw new NullReferenceException("CharacterStateMachine (a.k.a ICharacterHolder implementation) is null.");
            }

            MarkedAsDone = false;

            _timer = new CountdownTimer(Duration < 0 ? MAX_DURATION : Duration);
            _timer.OnTimerStart += TimerStart;
            _timer.OnTimerStop += TimerStop;

            if (HasConditions()) {
                foreach (var condition in _conditions) {
                    condition.Setup(owner);
                }
            }
        }

        /// <summary>
        /// Activates the skill.
        /// </summary>
        /// <param name="target">Who is going to be the target of this skill? Player (eg. Dash) or Enemy (eg. Fireball)</param>
        public virtual void Activate(Transform target) {
            if (!CheckConditions()) {
                DebugTools.DebugLog.Log($"Activation Failed! Not all conditions were met for {Name}");
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

        private void TimerStart() {
            MarkedAsDone = false;

            if (HasConditions()) {
                foreach (var condition in _conditions) {
                    condition.OnPowerupActivated();
                }
            }
        }

        private void TimerStop() {
            MarkedAsDone = true;

            ResetConditions();
        }

        public virtual void Deactivate() {
            ResetConditions();
            _timer.Reset();
        }

        public virtual void Tick(float deltaTime) => _timer.Tick(deltaTime);

        public virtual void Cleanup() {
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

    /// <summary>
    /// Enum representing the type of skill.
    /// Remember to ADD it to the SkillFactory.cs script
    /// </summary>
    [Serializable]
    public enum SkillType {
        Health,
        Immunity,
        OnHit,
        Size,
        SpawnObject,
        Strength,
        BigAttack,
        TimeWarp,
        DetectAndSpawn
    }
}