using System;
using UnityEngine;
using Utils;

namespace Companions {
    [RequireComponent(typeof(AudioSource))]
    public class CompanionSounds : MonoBehaviour {
        [SerializeField]
        private Vector2 _pitchRange;

        [SerializeField]
        private SoundsDictionary _sounds;

        private AudioSource _audioSource;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnValidate() {
            if (TryGetComponent<AudioSource>(out var audioSource) && audioSource.playOnAwake) {
                audioSource.playOnAwake = false;
            }
        }

        public void PlayFollow() {
            if (_sounds.TryGetValue("follow", out var clip)) {
                var pitchLevel = UnityEngine.Random.Range(_pitchRange.x, _pitchRange.y);
                Play(pitchLevel, clip);
            }
        }

        public void PlaySleep() {
            if (_sounds.TryGetValue("sleep", out var clip)) {
                var pitchLevel = UnityEngine.Random.Range(_pitchRange.x, _pitchRange.y);
                Play(pitchLevel, clip);
            }
        }

        private void Play(float pitchLevel, AudioClip clip) {
            _audioSource.pitch = pitchLevel;
            _audioSource.clip = clip;
            _audioSource.PlayOneShot(clip);
        }
    }

    [Serializable]
    internal class SoundsDictionary : UnitySerializedDictionary<string, AudioClip> { }
}