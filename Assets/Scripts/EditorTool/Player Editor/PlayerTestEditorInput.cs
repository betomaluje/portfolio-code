using Player.Input;
using UnityEngine;

namespace EditorTool.PlayerEditor {

    [RequireComponent(typeof(PlayerBattleInput))]
    public class PlayerTestEditorInput : MonoBehaviour {
        private PlayerBattleInput _playerInput;
        private EditorLoadSaveUI _editorLoadSaveUI;

        private void Awake() {
            _playerInput = GetComponent<PlayerBattleInput>();
            _editorLoadSaveUI = FindFirstObjectByType<EditorLoadSaveUI>();
        }

        private void OnEnable() {
            _playerInput.OpenMenuEvent += ToggleMenu;
        }

        private void OnDisable() {
            _playerInput.OpenMenuEvent -= ToggleMenu;
        }

        private void ToggleMenu() {
            _editorLoadSaveUI.ToggleMenu();
        }
    }
}