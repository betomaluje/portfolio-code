using Camera;
using Level;
using Sounds;
using UnityEngine;

namespace Enemies.Bosses {
    public class BossAppear : MonoBehaviour {
        [Header("Camera Focus")]
        [SerializeField]
        private Transform _bossTransform;

        [SerializeField]
        private float _focusTime = 3f;

        [Header("Camera Shake")]
        [SerializeField]
        private float _shakeIntensity = 2f;

        [SerializeField]
        private float _shakeDuration = 2f;

        public void FocusBoss() {
            SoundManager.instance.Play("big-woosh");
            CinemachineCameraHighlight.Instance.Highlight(_bossTransform, _focusTime);
        }

        public void OnBossAppear() {
            SoundManager.instance.Play("explosion");
            SoundManager.instance.Play("roar");
        }

        public void BossShakeCamera() {
            CinemachineCameraShake.Instance.ShakeCamera(_bossTransform, _shakeIntensity, _shakeDuration, true);
        }

        public void SetPostProcessingProfile() {
            PostProcessingManager.Instance.SetProfile("Boss");
        }

        public void ResetPostProcessingProfile() {
            PostProcessingManager.Instance.Reset();
        }
    }
}