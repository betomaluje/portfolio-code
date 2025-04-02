using System;
using Cinemachine;
using EditorTool.PlayerEditor;
using Scene_Management;
using TMPro;
using UnityEngine;
using Utils;

namespace EditorTool {
    public class EditorStateManager : Singleton<EditorStateManager> {
        [SerializeField]
        private PlayerEditorTool _playerEditorTool;

        [SerializeField]
        private EditorLoadSaveUI _editorLoadSaveUI;

        [SerializeField]
        private SceneField _testScene;

        [SerializeField]
        private TextMeshProUGUI _toggleButtonText;

        [SerializeField]
        private CinemachineVirtualCamera _virtualCamera;

        [Space]
        [Header("Extras to Hide")]
        [SerializeField]
        private Transform[] _hideOnPlay;

        private bool _isInEditMode = true;

        private void Start() {
            _playerEditorTool.gameObject.SetActive(_isInEditMode);
        }

        public void ToggleEdition() {
            _isInEditMode = !_isInEditMode;
            _playerEditorTool.gameObject.SetActive(_isInEditMode);

            Array.ForEach(_hideOnPlay, t => t.gameObject.SetActive(_isInEditMode));

            if (_isInEditMode) {
                Unload();
            }
            else {
                Load();
            }
        }

        public void Load() {
            void onSceneLoaded() {
                Time.timeScale = 1;
                var player = FindFirstObjectByType<PlayerTestEditorInput>();
                if (player != null && _virtualCamera != null) {
                    _virtualCamera.m_Follow = player.transform;
                }
            }

            void onSceneStartLoad() {
                _toggleButtonText.text = "Build more!";
                _editorLoadSaveUI.Hide();
                BattleGrid.Instance.HideAllMockObjects();
                Time.timeScale = 0;
            }

            SceneLoader.Instance.LoadSceneAdditive(_testScene, onSceneStartLoad, onSceneLoaded);
        }

        public void Unload() {
            _toggleButtonText.text = "Play test!";
            SceneLoader.Instance.UnloadScene(_testScene, () => {
                _editorLoadSaveUI.Hide();
                BattleGrid.Instance.ShowAllMockObjects();
                _virtualCamera.m_Follow = _playerEditorTool.transform;
            });
        }
    }
}