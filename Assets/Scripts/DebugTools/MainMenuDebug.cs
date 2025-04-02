using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace DebugTools {
    public class MainMenuDebug : MonoBehaviour {
        [SerializeField]
        private RectTransform _mainContainer;

        [SerializeField]
        private RectTransform _settingsContainer;

        [SerializeField]
        private RectTransform _controllerContainer;

#if UNITY_EDITOR

        [Button]
        private void ShowMain() {
            _mainContainer.gameObject.SetActive(true);
            _settingsContainer.gameObject.SetActive(false);
            _controllerContainer.gameObject.SetActive(false);
        }

        [Button]
        private void ShowSettings() {
            _mainContainer.gameObject.SetActive(false);
            _settingsContainer.gameObject.SetActive(true);
            _controllerContainer.gameObject.SetActive(false);
        }

        [Button]
        private void ShowController() {
            _mainContainer.gameObject.SetActive(false);
            _settingsContainer.gameObject.SetActive(false);
            _controllerContainer.gameObject.SetActive(true);
        }

        [Button]
        private void ClearAllGameplayPrefs() {
            DeleteConfirmDialog();
        }

        private void DeleteConfirmDialog() {
            if (EditorUtility.DisplayDialog($"Delete Saved Preferences?",
                $"Are you sure you want to delete all your saved preferences?", "Delete", "Do Not Delete")) {

                PlayerPrefs.DeleteKey("player_key");
                PlayerPrefs.DeleteKey("RealmName");
                PlayerPrefs.DeleteKey("intro-dungeon_Wins");
                PlayerPrefs.DeleteKey("intro-dungeon_Losses");
                PlayerPrefs.DeleteKey("villagescene_Wins");
                PlayerPrefs.DeleteKey("villagescene_Losses");
                PlayerPrefs.DeleteKey("forest-dungeon_Wins");
                PlayerPrefs.DeleteKey("forest-dungeon_Losses");
                PlayerPrefs.DeleteKey("dessert-dungeon_Wins");
                PlayerPrefs.DeleteKey("dessert-dungeon_Losses");
                PlayerPrefs.DeleteKey("sky-dungeon_Wins");
                PlayerPrefs.DeleteKey("sky-dungeon_Losses");
                PlayerPrefs.DeleteKey("boss1scene_Wins");
                PlayerPrefs.DeleteKey("boss1scene_Losses");

                PlayerPrefs.DeleteKey("test-dungeon_Wins");
                PlayerPrefs.DeleteKey("test-dungeon_Losses");
                PlayerPrefs.DeleteKey("testscene_Wins");
                PlayerPrefs.DeleteKey("testscene_Losses");

                PlayerPrefs.SetInt("first_time", 1);
            }
        }

#endif

    }
}