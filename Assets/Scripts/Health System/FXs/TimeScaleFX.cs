using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BerserkPixel.Health.FX {
    [DisallowMultipleComponent]
    public class TimeScaleFX : MonoBehaviour, IFX {
        private bool _isBusy;

        public FXType GetFXType() => FXType.OnlyNotImmune;

        public FXLifetime LifetimeFX => FXLifetime.OnlyAlive;

        public void DoFX(HitData hitData) {
            if (_isBusy) {
                return;
            }

            if (hitData.timescaleData == null) {
                return;
            }

            if (!hitData.timescaleData.IsValid()) {
                return;
            }

            DoTimescale(hitData);
        }

        private async void DoTimescale(HitData hitData) {
            float time = hitData.timescaleData.TimeInSeconds;
            float timeScale = hitData.timescaleData.TimeScale;

            Time.timeScale = timeScale;
            _isBusy = true;
            await UniTask.Delay(TimeSpan.FromSeconds(time), ignoreTimeScale: true);
            Time.timeScale = 1;
            _isBusy = false;
        }
    }
}