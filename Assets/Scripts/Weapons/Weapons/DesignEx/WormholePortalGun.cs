using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A tactical tool weapon that fires two linked portals.
    /// Mastery: Teleports any projectile (friendly or hostile) between the two portals.
    /// Skill is using it to redirect boss projectiles or extend your own range.
    /// </summary>
    [RequiredBullet(typeof(PortalProjectile))]
    [CreateAssetMenu(fileName = "WormholePortalGun", menuName = "Aurora/Weapons/Expanded/Wormhole Portal Gun")]
    public class WormholePortalGun : BaseShootingWeapon {

        private PortalNode _bluePortal;
        private PortalNode _orangePortal;
        private bool _nextIsOrange = false;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            animations?.Play(AttackAnimation);

            if (BulletPrefab != null) {
                var bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
                if (bullet.TryGetComponent<PortalProjectile>(out var portalProj)) {
                    portalProj.SetWeapon(this);
                    portalProj.SetOwner(animations?.Transform.root);
                    portalProj.InitializePortal(_nextIsOrange, this);
                    portalProj.Fire(direction.normalized);
                    
                    _nextIsOrange = !_nextIsOrange; // Toggle between blue and orange
                }
            }

            StartCooldown();
        }

        /// <summary>
        /// Registers a new portal node and links it to its counterpart.
        /// </summary>
        public void RegisterPortal(PortalNode newNode, bool isOrange) {
            if (isOrange) {
                if (_orangePortal != null) Destroy(_orangePortal.gameObject);
                _orangePortal = newNode;
            } else {
                if (_bluePortal != null) Destroy(_bluePortal.gameObject);
                _bluePortal = newNode;
            }

            // Link the two portals if both exist
            if (_bluePortal != null && _orangePortal != null) {
                _bluePortal.SetLink(_orangePortal);
                _orangePortal.SetLink(_bluePortal);
            }
        }
    }
}
