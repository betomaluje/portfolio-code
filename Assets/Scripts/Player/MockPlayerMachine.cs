using BerserkPixel.Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player {
    public class MockPlayerMachine : MonoBehaviour {
        [SerializeField]
        [Required]
        protected CharacterHealth _characterHealth;

        [SerializeField]
        private Transform _destroyParticles;

        private void OnEnable() {
            _characterHealth.OnDie += HandleDeath;
        }

        private void OnDisable() {
            _characterHealth.OnDie -= HandleDeath;
        }

        private void OnValidate() {
            if (_characterHealth == null) {
                _characterHealth = GetComponent<CharacterHealth>();
            }
        }

        private void HandleDeath() {
            if (_destroyParticles != null) {
                Instantiate(_destroyParticles, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}