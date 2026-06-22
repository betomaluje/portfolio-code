using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Shot by the Portal Gun to place a portal node on a wall.
    /// Mastery: Placement geometry matters for bullet redirection.
    /// </summary>
    public class PortalProjectile : BaseBullet {
        
        [Tooltip("Prefab for the actual portal node spawned on hit.")]
        [SerializeField] private PortalNode _portalNodePrefab;

        private bool _isOrange;
        private WormholePortalGun _weaponRef;

        /// <summary>
        /// Initializer for portal specifics.
        /// </summary>
        public void InitializePortal(bool isOrange, WormholePortalGun weapon) {
            _isOrange = isOrange;
            _weaponRef = weapon;
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            // Spawn node at impact point and face it towards normal
            if (_portalNodePrefab != null) {
                Vector2 point = collision.contacts[0].point;
                Vector2 normal = collision.contacts[0].normal;
                
                var node = Instantiate(_portalNodePrefab, point, Quaternion.identity);
                // Rotate to face away from the wall
                node.transform.right = normal;
                
                if (node.TryGetComponent<PortalNode>(out var portalNode)) {
                    portalNode.SetColor(_isOrange);
                    _weaponRef.RegisterPortal(portalNode, _isOrange);
                }
            }

            Destroy(gameObject);
        }
    }
}
