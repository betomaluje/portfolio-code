using UnityEngine;

namespace EditorTool.PlayerEditor {
    [RequireComponent(typeof(PlayerCloudServices))]
    public class PlayerEditorCloudLoader : MonoBehaviour {
        [SerializeField]
        private PlayerEditorTool _playerEditorTool;

        [SerializeField]
        private BattleGrid _battleGrid;

        private PlayerCloudServices _cloudServices;

        private void Awake() {
            _cloudServices = GetComponent<PlayerCloudServices>();
        }

        private void OnEnable() {
            _cloudServices.OnSignedIn += OnSignedIn;
            _cloudServices.OnSignedInFailed += OnSignedInFailed;
        }

        private void OnDisable() {
            _cloudServices.OnSignedIn -= OnSignedIn;
            _cloudServices.OnSignedInFailed -= OnSignedInFailed;
        }

        private void OnSignedInFailed(string errorMessage) {
            Debug.LogError(errorMessage);
            _playerEditorTool.LoadToolsByCategory();
            _battleGrid.LoadMockGrid();
        }

        private void OnSignedIn() {
            _playerEditorTool.LoadToolsByCategory();
            _battleGrid.LoadMockGrid();
        }


    }
}