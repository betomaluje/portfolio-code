using UnityEngine;

namespace Sounds {
    public class SceneThemeSound : MonoBehaviour {
        [SerializeField]
        private string ThemeSound;

        private void Start() {
            UnPauseMusic();
        }

        private void OnDestroy() {
            PauseMusic();
        }

        public void PauseMusic() {
            SoundManager.instance.StopAllBackground();
        }

        public void UnPauseMusic() {
            SoundManager.instance.StopAllBackground();
            SoundManager.instance.Play(ThemeSound);
        }
    }
}