using BerserkPixel.Health;
using BerserkPixel.Health.FX;
using Camera;
using Sounds;
using UnityEngine;

namespace Dummy {
    public class DummyController : MonoBehaviour, IHealth {
        [SerializeField]
        private Animator _animator;

        private readonly int Hit1 = Animator.StringToHash("Dummy_Hit1");
        private readonly int Hit2 = Animator.StringToHash("Dummy_Hit2");
        private readonly int Hit3 = Animator.StringToHash("Dummy_Hit3");

        private IFX[] _allFxs;

        private void Awake() {
            _allFxs = GetComponentsInChildren<IFX>();
        }

        public void SetupHealth(int maxHealth) { }

        public void PerformDamage(HitData hitData) {
            // play random animation
            var index = Random.Range(0, 3);
            var hashName = index switch {
                0 => Hit1,
                1 => Hit2,
                2 => Hit3,
                _ => Hit1
            };
            SoundManager.instance.Play("hit");
            CinemachineCameraShake.Instance.ShakeCamera(transform);

            foreach (var fx in _allFxs) {
                fx.DoFX(hitData);
            }

            _animator.CrossFadeInFixedTime(hashName, .1f);
        }

        public void GiveHealth(int health) { }

        public bool CanGiveHealth() => false;
    }
}