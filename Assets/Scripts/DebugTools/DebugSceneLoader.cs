using System;
using Interactable;
using Scene_Management;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace DebugTools {
    public class DebugSceneLoader : MonoBehaviour, IInteract {
        [SerializeField]
        private SceneField _targetScene;

        private Action onSceneLoaded = null;

        public void CancelInteraction() {

        }

        public void DoInteract() {
#if UNITY_EDITOR
            LoadScene();
#endif
        }

#if UNITY_EDITOR
        public void LoadScene() {
            if (_targetScene != null) {
                var guid = AssetDatabase.FindAssets($"{_targetScene.SceneName} t:scene")[0];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asyncOperation = EditorSceneManager.LoadSceneAsyncInPlayMode(path, new LoadSceneParameters(LoadSceneMode.Single));
                asyncOperation.completed += (AsyncOperation obj) => {
                    onSceneLoaded?.Invoke();
                };
            }
        }
#endif
    }
}