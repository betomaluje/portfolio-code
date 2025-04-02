using BerserkPixel.Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Powerups {
    [CreateAssetMenu(menuName = "Aurora/Powerups/Damage Others Powerup")]
    [TypeInfoBox("Powerup that damages other entities near by EndValue amount")]
    public class DamageOthersPowerup : PowerupConfig {
        [SerializeField]
        private LayerMask _targetMask;

        [SerializeField]
        [Min(0)]
        private float _radius = 3;

        public override void Activate(Transform target) {
            base.Activate(target);

            var hits = Physics2D.OverlapCircleAll(target.position, _radius, _targetMask);

            foreach (var hit in hits) {
                var dir = (hit.transform.position - target.transform.position).normalized;

                var hitData = new HitDataBuilder()
                    .WithDamage((int)EndValue)
                    .WithDirection(dir)
                    .Build(target.transform, hit.gameObject.transform);

                hitData.PerformDamage(hit);
            }
        }

        private void OnValidate() {
            _statType = Stats.StatType.DamageOthers;
        }
    }
}