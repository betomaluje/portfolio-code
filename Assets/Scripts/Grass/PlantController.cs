using BerserkPixel.Health;
using UnityEngine;

namespace Grass {
    [RequireComponent(typeof(CharacterHealth))]
    public class Plantcontroller : MonoBehaviour {
        [SerializeField]
        private ParticleSystem _leavesParticles;

        private CharacterHealth _characterHealth;

        private void Awake() {
            _characterHealth = GetComponent<CharacterHealth>();
        }

        private void OnEnable() {
            _characterHealth.OnDie += Die;
        }

        private void OnDisable() {
            _characterHealth.OnDie -= Die;
        }

        private void Die() {
            if (_leavesParticles != null) {
                Instantiate(_leavesParticles, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}