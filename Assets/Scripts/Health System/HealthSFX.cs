using UnityEngine;

namespace BerserkPixel.Health {
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(CharacterHealth))]
    public class HealthSFX : MonoBehaviour {
        [Header("Hurt")]
        [SerializeField]
        private AudioClip[] hurtSounds;

        [Header("Death")]
        [SerializeField]
        private AudioClip[] deathSounds;

        private AudioSource audioSource;

        private CharacterHealth health;

        private void Awake() {
            audioSource = GetComponent<AudioSource>();
            health = GetComponent<CharacterHealth>();
        }

        private void OnEnable() {
            health.OnDamagePerformed += HandleDamage;
            health.OnDie += HandleDeath;
        }

        private void OnDisable() {
            health.OnDamagePerformed -= HandleDamage;
            health.OnDie -= HandleDeath;
        }

        private void HandleDamage(HitData hitData) {
            var index = Random.Range(0, hurtSounds.Length);
            audioSource.clip = hurtSounds[index];
            audioSource.Play();
        }

        private void HandleDeath() {
            var index = Random.Range(0, deathSounds.Length);
            audioSource.clip = deathSounds[index];
            audioSource.Play();
        }
    }
}