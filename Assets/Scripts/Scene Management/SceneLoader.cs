using System;
using EasyTransition;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Random = UnityEngine.Random;

namespace Scene_Management {
    public class SceneLoader : PersistentSingleton<SceneLoader> {
        [SerializeField]
        private TransitionSettings[] _transitionSettings;

        protected override void Awake() {
            base.Awake();
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadCurrentScene(Action onSceneStartLoad = null, Action onSceneLoaded = null) {
            LoadScene(SceneManager.GetActiveScene().name, onSceneStartLoad, onSceneLoaded);
        }

        public void LoadScene(string sceneName, Action onSceneStartLoad = null, Action onSceneLoaded = null) {
            if (SceneManager.GetActiveScene().name != sceneName) {
                onSceneStartLoad?.Invoke();
                var setting = _transitionSettings[Random.Range(0, _transitionSettings.Length)];
                TransitionManager.Instance.Transition(sceneName, setting, 0.2f);
                TransitionManager.Instance.onTransitionEnd += () => {
                    onSceneLoaded?.Invoke();
                };
            }
        }

        public void LoadSceneAdditive(string sceneName, Action onSceneStartLoad = null, Action onSceneLoaded = null) {
            if (SceneManager.GetActiveScene().name != sceneName) {
                onSceneStartLoad?.Invoke();
                var setting = _transitionSettings[Random.Range(0, _transitionSettings.Length)];
                TransitionManager.Instance.AdditiveTransition(sceneName, setting, 0.2f);
                TransitionManager.Instance.onTransitionEnd += () => {
                    onSceneLoaded?.Invoke();
                };
            }
        }

        public void UnloadScene(string sceneName, Action onSceneLoaded = null) {
            var asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
            asyncOperation.completed += (AsyncOperation obj) => {
                onSceneLoaded?.Invoke();
            };
        }

        public void UnloadCurrentScene(Action onSceneLoaded = null) {
            UnloadScene(SceneManager.GetActiveScene().name, onSceneLoaded);
        }

        public void ReloadScene() {
            var currentScene = SceneManager.GetActiveScene().name;
            UnloadCurrentScene(() => { LoadScene(currentScene); });
        }
    }
}