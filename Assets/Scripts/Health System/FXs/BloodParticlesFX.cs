using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BerserkPixel.Health.FX {
    public class BloodParticlesFX : MonoBehaviour, IFX {
        [SerializeField]
        private ParticleSystem[] _bloodParticles;

        public FXType GetFXType() => FXType.OnlyNotImmune;

        public FXLifetime LifetimeFX => FXLifetime.OnlyAlive;

        private GameObject previewObject;

        private ParticleSystem GetRandomBloodParticle() => _bloodParticles[UnityEngine.Random.Range(0, _bloodParticles.Length)];

        private bool _preferenceShowBlood = true;

        private void OnEnable() {
            PreferencesStorage.OnPreferenceChanged += HandlePreferenceChanged;
        }

        private void OnDisable() {
            PreferencesStorage.OnPreferenceChanged -= HandlePreferenceChanged;
        }

        private void OnDestroy() {
            PreferencesStorage.OnPreferenceChanged -= HandlePreferenceChanged;
        }

        private void HandlePreferenceChanged(string eventKey, bool active) {
            if (eventKey.Equals(PreferencesStorage.EVENT_BLOOD)) {
                _preferenceShowBlood = active;
            }
        }

        public void DoFX(HitData hitData) {
            if (!_preferenceShowBlood) {
                return;
            }

            if (_bloodParticles == null || _bloodParticles.Length == 0) {
                Destroy(this);
                return;
            }

            Vector3 direction = hitData.direction;

            ParticleSystem bloodParticle = Instantiate(GetRandomBloodParticle(), transform.position, Quaternion.identity, transform);

            Quaternion rotation = Quaternion.FromToRotation(Vector2.right, direction);
            bloodParticle.transform.rotation = rotation;
            bloodParticle.Play();
        }

        [Button("Play Blood Particles FX")]
        private async void PlayBloodParticlesFX() {
            if (_bloodParticles == null || _bloodParticles.Length == 0) {
                return;
            }

            if (previewObject == null) {
                previewObject = new GameObject("Blood Preview");
            }

            var bloodParticles = Instantiate(GetRandomBloodParticle(), transform.position, Quaternion.identity, previewObject.transform);
            bloodParticles.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            DestroyImmediate(previewObject);
        }

    }
}