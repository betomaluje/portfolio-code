using System;
using System.Collections;
using System.Linq;
using DebugTools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Sounds {
    public class SoundManager : MonoBehaviour {
        public static SoundManager instance;

        [Searchable]
        [ListDrawerSettings(ShowFoldout = true, NumberOfItemsPerPage = 3, ShowPaging = true)]
        [SerializeField]
        private Sound[] sounds;

        [Space]
        [Header("Preferences")]
        [SerializeField]
        private string _preferencesMaster;

        [SerializeField]
        private string _preferencesSfx;

        [SerializeField]
        private string _preferencesSong;

        [Header("Audio Mixer")]
        [SerializeField]
        private AudioMixerGroup _masterGroup;

        [SerializeField]
        private AudioMixerGroup _backgroundGroup;

        [SerializeField]
        private AudioMixerGroup _sfxGroup;

        private Sound lastThemeSong;

        public string PREFS_MASTER => _preferencesMaster;

        public string PREFS_SFX => _preferencesSfx;

        public string PREFS_SONG => _preferencesSong;

        private void Awake() {
            if (instance == null) {
                instance = this;
            }
            else {
                Destroy(gameObject);
                return;
            }

            foreach (var s in sounds) {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.alternatives.Add(s.clip);

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.spatialBlend = s.spatialBlend;
                s.source.loop = s.loop;
                s.source.playOnAwake = false;

                switch (s.soundType) {
                    case SoundType.SONG:
                        s.source.outputAudioMixerGroup = _backgroundGroup;
                        break;
                    case SoundType.SFX:
                        s.source.outputAudioMixerGroup = _sfxGroup;
                        break;
                }
            }
        }

        private void Start() {
            RestoreVolumes();
        }

        public Sound GetSound(string soundName) {
            return Array.Find(sounds, sound => sound.name == soundName);
        }

        private void RestoreVolumes() {
            var storedMasterVolume = PlayerPrefs.GetFloat(PREFS_MASTER, .5f);
            var storedMusicVolume = PlayerPrefs.GetFloat(PREFS_SONG, .5f);
            var storedSFXVolume = PlayerPrefs.GetFloat(PREFS_SFX, .5f);

            SetMasterVolume(storedMasterVolume);
            SetMusicVolume(storedMusicVolume);
            SetSFXVolume(storedSFXVolume);
        }

        public void Play(string soundName) {
            var s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null) {
                Debug.LogWarning($"Sound {soundName} not found");
                return;
            }

            if (s.source == null) {
                return;
            }

            if (s.soundType == SoundType.SONG) {
                lastThemeSong = s;
            }

            if (s.alternatives != null && s.alternatives.Count > 0) {
                var randomIndex = Random.Range(0, s.alternatives.Count);
                var copy = s;
                copy.source.clip = s.alternatives[randomIndex];
                copy.source.Play();
            }
            else {
                s.source.Play();
            }
        }

        public void PlayOnSpot(string soundName, Vector2 position) {
            PlayOnSpot(soundName, (Vector3)position);
        }

        public void PlayOnSpot(string soundName, Vector3 position) {
            var s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null) {
                Debug.LogWarning($"Sound {soundName} not found");
                return;
            }

            if (s.source == null) {
                return;
            }

            if (s.soundType == SoundType.SONG) {
                lastThemeSong = s;
            }

            if (s.alternatives != null && s.alternatives.Count > 0) {
                var randomIndex = Random.Range(0, s.alternatives.Count);
                PlayClipAtPoint(s.alternatives[randomIndex], position, volume: s.volume);
            }
            else {
                PlayClipAtPoint(s.source.clip, position, volume: s.volume);
            }
        }

        public void PlayReversed(string soundName) {
            var s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null || s.source == null) {
                Debug.LogWarning($"Sound {soundName} not found");
                return;
            }

            if (s.source == null) {
                return;
            }

            s.source.timeSamples = s.source.clip.samples - 1;
            s.source.pitch = -1;

            s.source.Play();
        }

        public void PlayWithPitch(string soundName, float pitch) {
            var s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null) {
                Debug.LogWarning($"Sound {soundName} not found");
                return;
            }

            if (s.source == null) {
                return;
            }

            if (s.alternatives != null && s.alternatives.Count > 0) {
                var randomIndex = Random.Range(0, s.alternatives.Count);
                var copy = s;
                copy.source.clip = s.alternatives[randomIndex];
                copy.source.pitch = pitch;
                copy.source.Play();
            }
            else {
                s.source.pitch = pitch;
                s.source.Play();
            }
        }

        public void PlayWithPitch(string soundName) {
            // we do a random between the values from the Sound class
            var pitch = Random.Range(.1f, 3f);

            PlayWithPitch(soundName, pitch);
        }

        public void PlayWithPitchOnSpot(string soundName, Vector3 position) {
            var s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null) {
                Debug.LogWarning($"Sound {soundName} not found");
                return;
            }

            if (s.source == null) {
                return;
            }

            var pitch = Random.Range(.1f, 3f);

            if (s.alternatives != null && s.alternatives.Count > 0) {
                var randomIndex = Random.Range(0, s.alternatives.Count);
                PlayClipAtPoint(s.alternatives[randomIndex], position, pitch, s.volume);
            }
            else {
                PlayClipAtPoint(s.source.clip, position, pitch, s.volume);
            }
        }

        public void PlayClipAtPoint(AudioClip clip, Vector3 position, float pitch = 1, float volume = 1) {
            GameObject gameObject = new GameObject("One shot audio");
            gameObject.transform.position = position;
            AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.pitch = pitch;
            audioSource.volume = volume;
            audioSource.Play();
            Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
        }

        public void Stop(string soundName) {
            var s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null || s.source == null) {
                Debug.LogWarning($"Sound {soundName} not found");
                return;
            }

            s.source.Stop();
        }

        public void StopAll() {
            foreach (var s in sounds) {
                s.source.Stop();
            }
        }

        public void StopAllBackground() {
            var soundsOfType =
                Array.FindAll(sounds, sound => sound.soundType == SoundType.SONG && sound.source != null && sound.source.isPlaying);
            foreach (var s in soundsOfType) {
                s.source.Stop();
            }
        }

        public void PauseCurrentThemeSong() {
            if (lastThemeSong != null) {
                DebugLog.Log("Pausing song " + lastThemeSong.name);
                lastThemeSong.source.Pause();
            }
        }

        public void ResumeCurrentThemeSong() {
            if (lastThemeSong != null) {
                DebugLog.Log("Resuming song " + lastThemeSong.name);
                lastThemeSong.source.Play();
            }
        }

        public void SetVolumeForType(SoundType soundType, float volume) {
            var soundsOfType = Array.FindAll(sounds, sound => sound.soundType == soundType && sound.source != null);
            if (soundsOfType.Length <= 0) {
                Debug.LogWarning("Sounds of type " + soundType + " not found");
                return;
            }

            foreach (var s in soundsOfType) {
                s.source.volume = volume;
            }
        }

        public float GetVolumeForType(SoundType soundType) {
            var soundsOfType = Array.FindAll(sounds, sound => sound.soundType == soundType && sound.source != null);
            if (soundsOfType.Length <= 0) {
                Debug.LogWarning("Sounds of type " + soundType + " not found");
                return 0.4f;
            }

            soundsOfType = soundsOfType.OrderBy(x => x.volume).ToArray();

            return soundsOfType[0].volume;
        }

        public void PitchEverything(float pitch = .5f, bool onlyPlayingSounds = true) {
            foreach (var s in sounds) {
                if (onlyPlayingSounds && !s.source.isPlaying) {
                    continue;
                }
                s.source.pitch = pitch;
            }
        }

        public void PitchEverything(float time, float pitch = .5f) {
            StartCoroutine(Pitch(time, pitch));
        }

        private IEnumerator Pitch(float time, float pitch = .5f) {
            var savedPitches = new Hashtable();
            foreach (var s in sounds) {
                if (!s.source.isPlaying) {
                    continue;
                }

                savedPitches.Add(s.name, s.source.pitch);
                s.source.pitch = pitch;
            }

            yield return new WaitForSeconds(time);

            foreach (var s in sounds) {
                if (!savedPitches.ContainsKey(s.name)) {
                    continue;
                }

                var previousPitch = (float)savedPitches[s.name];
                s.source.pitch = previousPitch;
            }
        }

        public float GetMasterVolume() => PlayerPrefs.GetFloat(PREFS_MASTER, .5f);

        public void SetMasterVolume(float value) {
            _masterGroup.audioMixer.SetFloat("volume_master", GetActualVolume(value));
            PlayerPrefs.SetFloat(PREFS_MASTER, value);
        }

        public float GetMusicVolume() => PlayerPrefs.GetFloat(PREFS_SONG, .5f);

        public void SetMusicVolume(float value) {
            _backgroundGroup.audioMixer.SetFloat("volume_music", GetActualVolume(value));
            PlayerPrefs.SetFloat(PREFS_SONG, value);
        }

        public float GetSFXVolume() => PlayerPrefs.GetFloat(PREFS_SFX, .5f);

        public void SetSFXVolume(float value) {
            _sfxGroup.audioMixer.SetFloat("volume_sfx", GetActualVolume(value));
            PlayerPrefs.SetFloat(PREFS_SFX, value);
        }

        /// <summary>
        ///     Taken from https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float GetActualVolume(float value) {
            return Mathf.Log10(value) * 20;
        }
    }
}