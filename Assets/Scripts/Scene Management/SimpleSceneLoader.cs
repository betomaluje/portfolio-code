using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene_Management {
    public class SimpleSceneLoader : MonoBehaviour {
        // the actual percentage while scene is fully loaded
        private const float LOAD_READY_PERCENTAGE = 0.9f;

        [SerializeField]
        private SceneField _mainMenuScene;

        private Coroutine _currentCoroutine;

        private void Start() {
            Cursor.visible = false;
        }

        private void OnDestroy() {
            if (_currentCoroutine != null) {
                StopCoroutine(_currentCoroutine);
            }
        }

        public void LoadMainMenu() {
            _currentCoroutine = StartCoroutine(LoadScene());
        }

        private IEnumerator LoadScene() {
            var asyncLoad = SceneManager.LoadSceneAsync(_mainMenuScene, LoadSceneMode.Single);

            asyncLoad.allowSceneActivation = false;
            //wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone) {
                // _loadingProgbar.fillAmount = _sceneAO.progress;
                // scene has loaded as much as possible,
                // the last 10% can't be multi-threaded
                if (asyncLoad.progress >= LOAD_READY_PERCENTAGE) {
                    // _loadingProgbar.fillAmount = 1f;
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}