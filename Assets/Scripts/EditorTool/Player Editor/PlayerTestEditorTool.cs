using Player.Input;
using UnityEngine;

namespace EditorTool.PlayerEditor {
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerTestEditorTool : MonoBehaviour {
        private PlayerInput _playerInput;
        private BattleGrid _battleGrid;

        private void Awake() {
            _playerInput = GetComponent<PlayerInput>();
            _battleGrid = BattleGrid.Instance;
        }

        private void OnEnable() {
            _battleGrid?.LoadGrid();
        }

        private void OnDisable() {
            _battleGrid?.RemoveAllObjects();
        }

        private void Start() {
            _playerInput.EnableGameplayInput();
        }
    }
}