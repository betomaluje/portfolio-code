using Base;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(menuName = "Aurora/Weapons/Place Trap Weapon")]
    public class PlaceTrapWeapon : BaseShootingWeapon {
        [SerializeField]
        private LayerMask _whereToPlace;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) {
                return;
            }

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                HandleShooting(position);
                StartCooldown();
            }
            else {
                Reload(animations);
            }
        }

        private void HandleShooting(Vector3 position) {
            var positionNear = Random.insideUnitCircle * Range;

            var currentPosition = (Vector2)position;

            var endPoint = currentPosition + positionNear;

            var hit = Physics2D.OverlapCircle(
                endPoint,
                .2f,
                _whereToPlace
            );

            if (hit != null) {
                ShootBullet(positionNear, Vector2.zero);
            }
        }
    }
}