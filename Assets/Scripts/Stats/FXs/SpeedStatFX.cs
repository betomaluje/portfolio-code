using UnityEngine;

namespace Stats.FXs {
    public class SpeedStatFX : BaseStatFX {
        public override StatType StatType => StatType.Speed;
        private Animator _animator;
        private float _originalSpeed;

        public override void Awake() {
            base.Awake();
            _animator = transform.parent.GetComponentInChildren<Animator>();
            _originalSpeed = _animator.speed;
        }

        public override void DoFX(StatType type, float amount) {
            base.DoFX(type, amount);
            _animator.speed = amount;
        }

        public override void ResetFX(StatType type, float amount) {
            base.ResetFX(type, amount);
            _animator.speed = _originalSpeed;
        }
    }
}