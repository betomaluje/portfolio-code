using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using Scene_Management;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace UI {
    public class InGameUILoader : MonoBehaviour {
        [SerializeField]
        private SceneField _playerUI;

        [SerializeField]
        private bool _loadOnStart = false;

        private void Start() {
            if (_loadOnStart)
                LoadUI();
        }

        [Button]
        public void LoadUI() {
            if (SceneManager.GetSceneByName(_playerUI).isLoaded)
                return;

            if (Application.isPlaying) {
                SceneManager.LoadSceneAsync(_playerUI, LoadSceneMode.Additive);
            }
#if UNITY_EDITOR
            else {
                EditorSceneManager.OpenScene($"Assets/Scenes/UI Scenes/{_playerUI.SceneName}.unity", OpenSceneMode.Additive);
            }
#endif
        }

        [Button]
        public void UnloadUI() {
            if (!SceneManager.GetSceneByName(_playerUI).isLoaded)
                return;

            if (Application.isPlaying)
                SceneManager.UnloadSceneAsync(_playerUI);
#if UNITY_EDITOR
            else
                EditorSceneManager.CloseScene(SceneManager.GetSceneByName(_playerUI), true);
#endif
        }
    }
}