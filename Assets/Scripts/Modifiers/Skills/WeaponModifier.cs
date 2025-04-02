using Sirenix.OdinInspector;
using Base;
using UnityEngine;
using Utils;
using System.Collections.Generic;

namespace Modifiers.Skills {
    [InlineEditor]
    /// <summary>
    /// Represents a weapon modifier. It's a passive effect that can be applied to a weapon
    /// </summary>
    public abstract class WeaponModifier : ScriptableObject, IModifier {
        [FoldoutGroup("General")]
        public string Name;

        [FoldoutGroup("General")]
        [TextArea(0, 3)]
        public string Description;

        [FoldoutGroup("General")]
        [PreviewField(100, ObjectFieldAlignment.Right)]
        public Sprite Icon;

        [FoldoutGroup("General")]
        public float EndValue = 2f;

        [FoldoutGroup("General")]
        [Tooltip("Which color will represent the powerup in the UI")]
        [SerializeField]
        private Color _tagColor = new(210 / 255f, 156 / 255f, 138 / 255f);
        
        [BoxGroup("Timer")]
        [Tooltip("If true, the timer will run indefinitely")]
        public bool Indefinite = false;

        [BoxGroup("Timer")]
        [HideIf("Indefinite")]
        [Tooltip("How long does the modifier last in seconds")]
        [Min(0f)]
        public float Duration = 8f;

        [Header("FX")]
        [SerializeField]
        private Transform _particleFX;

        protected const int MAX_DURATION = 120;

        protected CountdownTimer _timer;

        public bool MarkedForRemoval { get; private set; }

        public bool IsCurrentlyActive => !MarkedForRemoval;

        protected ICharacterHolder _target;

        private readonly Dictionary<Transform, Transform> _appliedFxs = new();

        public Color GetTagColor() => _tagColor;

        public string GetDescription() => Description;

        private void OnValidate() {
            _tagColor = new(210 / 255f, 156 / 255f, 138 / 255f);
        }

        public virtual void Setup(Transform owner) { }

        public virtual void Activate(Transform target) {
            MarkedForRemoval = false;
            _target = target.GetComponent<ICharacterHolder>();

            ApplyFX(target);

            if (Indefinite) {
                return;
            }

            _timer = new CountdownTimer(Duration < 0 ? MAX_DURATION : Duration);
            _timer.OnTimerStop += TimerStop;
            _timer.Start();
        }

        public bool CheckConditions() { return true; }

        private void ApplyFX(Transform parent) {
            // to avoid duplicated fxs
            if (!_appliedFxs.ContainsKey(parent) && _particleFX != null) {
                var appliedFX = Instantiate(_particleFX, parent);
                _appliedFxs.Add(parent, appliedFX);
            }
        }

        public virtual void Tick(float deltaTime) => _timer?.Tick(deltaTime);

        private void TimerStop() => MarkedForRemoval = true;

        public virtual void Deactivate() {
            if (_appliedFxs != null && _appliedFxs.Count > 0) {
                foreach (var fx in _appliedFxs) {
                    if (fx.Value != null && fx.Value.gameObject != null) {
                        fx.Value.gameObject.SetActive(false);
                    }
                }
                _appliedFxs.Clear();
            }

            if (_timer != null) {
                _timer.Reset(0);
                _timer.OnTimerStop -= TimerStop;
            }
        }
    }
}