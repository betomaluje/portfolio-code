using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Utils;

namespace Level {
    [RequireComponent(typeof(Volume))]
    public class PostProcessingManager : Singleton<PostProcessingManager> {
        [SerializeField]
        private Volume _extraVolume;

        [SerializeField]
        private float _timeToShow = 1f;

        [SerializeField]
        private VolumesDictionary _volumes;

        private bool _isBusy;

        private void Start() {
            if (_extraVolume == null) {
                _extraVolume = GetComponent<Volume>();
            }

            if (_extraVolume == null) {
                Destroy(this);
                return;
            }

            _extraVolume.weight = 0f;
            _extraVolume.profile = null;
        }

        [Button]
        public void SetProfile(string name) {
            if (_volumes.ContainsKey(name) && !_isBusy) {
                _extraVolume.profile = _volumes[name];
                StartCoroutine(ChangePostProcessing(0, 1));
            }
        }

        [Button]
        public void Reset() {
            StopAllCoroutines();

            StartCoroutine(ChangePostProcessing(1, 0));
        }

        private void OnDestroy() {
            _extraVolume.weight = 0f;
            _extraVolume.profile = null;
        }

        private IEnumerator ChangePostProcessing(float from, float to, Action onDone = null) {
            if (_extraVolume.weight != to) {
                _isBusy = true;
                var time = 0f;

                while (time < _timeToShow) {
                    _extraVolume.weight = Mathf.Lerp(from, to, time / _timeToShow);
                    time += Time.deltaTime;

                    yield return null;
                }

                _extraVolume.weight = to;
                _isBusy = false;
            }

            onDone?.Invoke();
        }
    }

    [Serializable]
    public class VolumesDictionary : UnitySerializedDictionary<string, VolumeProfile> { }
}