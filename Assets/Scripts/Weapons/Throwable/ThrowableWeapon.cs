using Base;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(menuName = "Aurora/Weapons/Throwable Weapon")]
    public class ThrowableWeapon : Weapon, IThrowable, ICharge {
        [Tooltip("Prefab of the throwable weapon.")]
        [SerializeField]
        private ThrowableProjectile _projectilePrefab;

        [Tooltip("The path to follow when throwing the projectile.")]
        [SerializeField]
        private ThrowablePath _path;

        [Tooltip("The time it takes to fully charge the weapon.")]
        public float ChargeTime = 3f;

        public float Charge { set => _charge = Mathf.Clamp01(value); }
        private float _charge = 0f;

        private GameObject _originalWeapon;
        private bool _hasBeenThrown = false;

        public void SetOriginalWeapon(GameObject weaponObject) {
            _originalWeapon = weaponObject;
        }

        private void OnEnable() {
            if (_path is IRangeLimitable rangeLimitable) {
                rangeLimitable.OnOutOfRange += InternalCooldown;
            }

            InternalCooldown();
        }

        private void OnDisable() {
            _originalWeapon = null;

            if (_path is IRangeLimitable rangeLimitable) {
                rangeLimitable.OnOutOfRange -= InternalCooldown;
            }

            InternalCooldown();
        }

        private void InternalCooldown() {
            _hasBeenThrown = false;
        }

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (_projectilePrefab == null || _path == null) {
                return;
            }

            if (_hasBeenThrown || IsCoolingDown()) {
                return;
            }

            animations?.Play(AttackAnimation);
            StartCooldown();
            _hasBeenThrown = true;

            // v = d / t
            float minSpeed = Range / ChargeTime;
            float maxSpeed = Range / (0.1f * ChargeTime);
            var speed = Mathf.Lerp(minSpeed, maxSpeed, _charge);

            // Instantiate the projectile and configure its path
            var throwableProjectile = Instantiate(_projectilePrefab, position, Quaternion.identity);
            throwableProjectile.Launch(this, position, direction, _path, speed, _originalWeapon);
        }

        private void OnValidate() {
            AttackType = AttackType.Throwable;
        }
    }
}
