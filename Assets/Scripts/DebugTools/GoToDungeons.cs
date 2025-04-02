using Scene_Management;
using UnityEngine;

namespace DebugTools {
    public class GoToDungeons : MonoBehaviour {
        [SerializeField]
        private SceneField _testScene, _forestScene, _dessertScene, _skyScene, _boss1Scene;

        private bool _isLoading;

        private void Update() {
            if (_isLoading) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.T)) {
                _isLoading = true;
                SceneLoader.Instance.LoadScene(_testScene);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                _isLoading = true;
                SceneLoader.Instance.LoadScene(_forestScene);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                _isLoading = true;
                SceneLoader.Instance.LoadScene(_dessertScene);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                _isLoading = true;
                SceneLoader.Instance.LoadScene(_skyScene);
            }

            if (Input.GetKeyDown(KeyCode.Alpha9)) {
                _isLoading = true;
                SceneLoader.Instance.LoadScene(_boss1Scene);
            }
        }
    }
}