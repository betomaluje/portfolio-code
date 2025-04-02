using BerserkPixel.Health;
using Coins;
using Sounds;
using UnityEngine;

namespace Consumables {
    [CreateAssetMenu(fileName = "Health Potion", menuName = "Aurora/Consumables/Health Potion")]
    public class HealthPotion : ConsumableSO {
        [SerializeField]
        private int _healthToGive = 10;

        [SerializeField]
        private Transform _particles;

        public override void Consume(Transform owner, Transform target) {
            if (target.TryGetComponent(out IHealth health) && health.CanGiveHealth()) {
                health.GiveHealth(_healthToGive);

                SoundManager.instance.Play("coin");

                if (_particles != null) {
                    Instantiate(_particles, owner.position, Quaternion.identity);
                }

                Destroy(owner.gameObject);
            } else {
                if (owner.TryGetComponent<MagnetTowardsObject>(out var magnet)) {
                    magnet.StopMoving();
                }
            }
        }

        public override void Consume(Transform target) {
            if (target.TryGetComponent(out IHealth health) && health.CanGiveHealth()) {
                health.GiveHealth(_healthToGive);
            }
        }
    }
}