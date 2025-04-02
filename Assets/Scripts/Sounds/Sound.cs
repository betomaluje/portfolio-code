using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sounds {
    [Serializable]
    public class Sound {
        public string name;

        public SoundType soundType;
#if UNITY_EDITOR
        [InlineButton(nameof(Stop), "Stop")]
        [InlineButton(nameof(PlayPreview), "Preview")]
#endif
        public AudioClip clip;

        public List<AudioClip> alternatives;

        [Range(0f, 1f)]
        public float volume = .5f;

        [Range(.1f, 3f)]
        public float pitch = 1;

        [Range(0f, 1f)]
        public float spatialBlend;

        public bool loop;

        [HideInInspector]
        public AudioSource source;

        private AudioSource previewSource;

        private GameObject previewObject;

#if UNITY_EDITOR
        ~Sound() {
            Stop();
            if (previewObject != null) {
                UnityEngine.Object.DestroyImmediate(previewObject);
            }
            if (previewSource != null) {
                UnityEngine.Object.DestroyImmediate(previewSource);
            }
        }

        public async void PlayPreview() {
            if (previewObject == null) {
                previewObject = new GameObject("Sound Preview");
            }

            if (previewSource == null) {
                previewSource = previewObject.AddComponent<AudioSource>();
            }
            previewSource.clip = clip;
            previewSource.volume = volume;
            previewSource.Play();
            await UniTask.Delay((int)(previewSource.clip.length * 1000));
            UnityEngine.Object.DestroyImmediate(previewObject);
        }

        public void Stop() {
            if (previewObject == null) {
                return;
            }
            previewSource = previewObject.GetComponent<AudioSource>();
            previewSource.Stop();
            UnityEngine.Object.DestroyImmediate(previewObject);
        }
#endif
    }
}