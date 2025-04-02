using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Camera {
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CinemachineCameraShake : Singleton<CinemachineCameraShake> {
        [SerializeField]
        private ShakeData _shakeData;

        private CinemachineBasicMultiChannelPerlin _channelPerlin;

        protected override void Awake() {
            base.Awake();
            var virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _channelPerlin =
                virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _channelPerlin.m_FrequencyGain = 1f;
        }

        private void OnValidate() {
            if (_shakeData == null) {
                _shakeData = new ShakeData.Builder()
                    .WithIntensity(DefaultIntensity)
                    .WithShakeTime(DefaultShakeTime)
                    .Build();
            }
            else if (_shakeData.intensity == 0f && _shakeData.shakeTime == 0f) {
                _shakeData = new ShakeData.Builder()
                    .WithIntensity(DefaultIntensity)
                    .WithShakeTime(DefaultShakeTime)
                    .Build();
            }
        }

        [Button]
        public void ShakeCamera(Transform target) {
            ShakeCamera(target, _shakeData);
        }

        public void ShakeCamera(Transform target, float intensity, float shakeTime, bool ignoreVisible = false) {
            var shakeData = new ShakeData.Builder()
                .WithIntensity(intensity)
                .WithShakeTime(shakeTime)
                .Build();
            ShakeCamera(target, shakeData, ignoreVisible);
        }

        public void ShakeCameraWithIntensity(Transform target, float intensity, bool ignoreVisible = false) {
            var shakeData = new ShakeData.Builder()
                .WithIntensity(intensity)
                .WithShakeTime(DefaultShakeTime)
                .Build();
            ShakeCamera(target, shakeData, ignoreVisible);
        }

        public void ShakeCameraWithTime(Transform target, float shakeTime, bool ignoreVisible = false) {
            var shakeData = new ShakeData.Builder()
                .WithIntensity(DefaultIntensity)
                .WithShakeTime(shakeTime)
                .Build();
            ShakeCamera(target, shakeData, ignoreVisible);
        }

        private void ShakeCamera(Transform target, ShakeData shakeData, bool ignoreVisible = false) {
            if (target == null) {
                return;
            }

            if (ignoreVisible) {
                ShakeLerp(shakeData.intensity, shakeData.shakeTime);
            }
            else {
                if (UnityEngine.Camera.main.IsObjectVisible(target)) {
                    ShakeLerp(shakeData.intensity, shakeData.shakeTime);
                }
            }

        }

        private async void ShakeLerp(float intensity, float duration) {
            var time = 0f;

            _channelPerlin.m_AmplitudeGain = intensity;

            while (time < duration) {
                var percentage = Mathf.Lerp(intensity, 0f, time / duration);
                _channelPerlin.m_AmplitudeGain = percentage;
                time += Time.deltaTime;
                await UniTask.Yield();
            }

            _channelPerlin.m_AmplitudeGain = 0f;
        }

        #region ShakeData

        private static readonly float DefaultIntensity = 2f;
        private static readonly float DefaultShakeTime = .8f;

        [Serializable]
        public class ShakeData {
            public float intensity;
            public float shakeTime;

            private ShakeData() { }

            private ShakeData(float intensity, float shakeTime) {
                this.intensity = intensity;
                this.shakeTime = shakeTime;
            }

            public static class ShakeDataExt {
                public static ShakeData GetDefault() {
                    return new ShakeData(DefaultIntensity, DefaultShakeTime);
                }
            }

            public class Builder {
                private float intensity = 2f;
                private float shakeTime = .8f;

                public Builder WithIntensity(float intensity) {
                    this.intensity = intensity;
                    return this;
                }

                public Builder WithShakeTime(float shakeTime) {
                    this.shakeTime = shakeTime;
                    return this;
                }

                public ShakeData Build() {
                    return new ShakeData(intensity, shakeTime);
                }
            }
        }
    }
    #endregion
}