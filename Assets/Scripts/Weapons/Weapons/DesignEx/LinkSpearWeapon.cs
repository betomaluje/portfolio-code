using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A spear that sticks to walls and sustains a damaging electric beam back to the owner.
    /// Mastery: Moving the player "sweeps" the beam through enemies to clothesline them.
    /// Recasting the attack while a spear is lodged will recall it.
    /// </summary>
    [CreateAssetMenu(fileName = "LinkSpearWeapon", menuName = "Aurora/Weapons/Expanded/Link-Beam Spear")]
    public class LinkSpearWeapon : BaseShootingWeapon {
        
        [Header("Beam Properties")]
        [Tooltip("The time in seconds between damage ticks of the link beam.")]
        [SerializeField] private float _damageTickRate = 0.1f;

        [Tooltip("Prefab for the beam visual (e.g., a LineRenderer setup).")]
        [SerializeField] private GameObject _beamVisualPrefab;

        private TetherSpearBullet _activeSpear;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            // Recast Logic: Recall the spear if it's already lodged
            if (_activeSpear != null && _activeSpear.IsLodged) {
                animations?.Play(AttackAnimation); // Could utilize a "Recall" string
                _activeSpear.Recall();
                StartCooldown();
                return;
            }

            // Normal firing cycle
            if (HasAmmo()) {
                animations?.Play(AttackAnimation);

                if (BulletPrefab != null) {
                    var bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (bullet.TryGetComponent<TetherSpearBullet>(out var spear)) {
                        _activeSpear = spear;
                        spear.SetWeapon(this);
                        spear.SetOwner(animations?.Transform.root);
                        spear.InitializeBeam(_beamVisualPrefab, _damageTickRate);
                        spear.Fire(direction.normalized);
                    }
                }
                
                StartCooldown();
            } else {
                Reload(animations);
            }
        }

        protected override void OnValidate() {
            base.OnValidate();
            if (BulletPrefab != null && BulletPrefab.GetComponent<TetherSpearBullet>() == null) {
                Debug.LogWarning($"[{name}] Link-Beam Spear requires a BulletPrefab with TetherSpearBullet component!");
                BulletPrefab = null;
            }
        }
    }
}
