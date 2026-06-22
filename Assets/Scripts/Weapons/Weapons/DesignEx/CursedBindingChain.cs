using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Configuration for a binding weapon that roots enemies in place. 
    /// Mastery: Hitting an already bound enemy refreshes the stun duration 
    /// and adds a high-damage "Blight" explosion.
    /// </summary>
    [RequiredBullet(typeof(CursedChainBullet))]
    [CreateAssetMenu(fileName = "CursedBindingChain", menuName = "Aurora/Weapons/Expanded/Cursed Binding Chain")]
    public class CursedBindingChain : BaseShootingWeapon {
        
        [Header("Binding Mechanics")]
        [Tooltip("The duration (seconds) that an enemy is rooted.")]
        [SerializeField] private float _stunDuration = 2.5f;

        [Tooltip("Prefab for the chains that erupt from the ground.")]
        [SerializeField] private GameObject _groundChainPrefab;

        public float StunDuration => _stunDuration;
        public GameObject GroundChainPrefab => _groundChainPrefab;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);

                if (BulletPrefab != null) {
                    var bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (bullet.TryGetComponent<CursedChainBullet>(out var chain)) {
                        chain.SetWeapon(this);
                        chain.SetOwner(animations?.Transform.root);
                        chain.InitializeBinding(_stunDuration, _groundChainPrefab);
                        chain.Fire(direction.normalized);
                    }
                }

                StartCooldown();
            } else {
                Reload(animations);
            }
        }

    }
}
