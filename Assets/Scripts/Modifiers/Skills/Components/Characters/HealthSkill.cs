using UnityEngine;
using Utils;

namespace Modifiers.Skills {
    [CreateAssetMenu(menuName = "Aurora/Skills/Health Skill")]
    public class HealthSkill : SkillConfig {
        [SerializeField]
        private float _interval = 1f;

        private NotifyingCountdownTimer _internalTimer;
        private float _duration;

        public override void Setup(Transform owner) {
            base.Setup(owner);
            _duration = Duration;
            _internalTimer = new NotifyingCountdownTimer(_duration, _interval);
            _internalTimer.OnInterval += IncreaseHealth;
        }

        private void IncreaseHealth() {
            if (_holder.Health.CanGiveHealth()) {
                _holder?.Health?.GiveHealth(Mathf.FloorToInt(EndValue));
            }
        }

        public override void Activate(Transform target) {
            base.Activate(target);

            if (CheckConditions()) {
                _internalTimer.Start();
            }
        }

        public override void Tick(float deltaTime) {
            base.Tick(deltaTime);
            _internalTimer?.Tick(deltaTime);
        }

        public override void Deactivate() {
            base.Deactivate();
            _internalTimer.Reset(_duration);
        }

        public override void Cleanup() {
            base.Cleanup();
            _internalTimer.OnInterval -= IncreaseHealth;
            _internalTimer = null;
        }
    }
}