using UnityEngine;

namespace BerserkPixel.Health.FX {
    [RequireComponent(typeof(ForceReceiver))]
    public class KnockbackFX : MonoBehaviour, IFX {
        private ForceReceiver _forceReceiver;

        private void Awake() => _forceReceiver = GetComponent<ForceReceiver>();

        public FXType GetFXType() => FXType.Always;

        public FXLifetime LifetimeFX => FXLifetime.Always;

        public void DoFX(HitData hitData) {
            Weapons.Weapon weapon = hitData.weapon;

            if (weapon == null) {
                return;
            }

            Vector3 direction = hitData.direction;
            _forceReceiver.Knockback(direction, weapon.GetKnockback());
        }

        public void DoFX(float knockbackForce, Vector2 direction) {
            if (knockbackForce <= 0) {
                return;
            }

            _forceReceiver.Knockback(direction, knockbackForce);
        }

    }
}