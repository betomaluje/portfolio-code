using Extensions;
using UnityEngine;

namespace Level {
    public class MiniMapHider : MonoBehaviour {
        [SerializeField]
        private GameObject _miniMap;

        private void OnValidate() {
            if (_miniMap == null) {
                _miniMap = transform.FindChildren("Minimap").gameObject;
            }
        }

        private void Start() {
            PreferencesStorage preferencesStorage = new();
            _miniMap.SetActive(preferencesStorage.GetShowMinimap());
        }

        private void OnEnable() {
            PreferencesStorage.OnPreferenceChanged += HandlePreferenceChanged;
        }

        private void OnDisable() {
            PreferencesStorage.OnPreferenceChanged -= HandlePreferenceChanged;
        }

        private void OnDestroy() {
            PreferencesStorage.OnPreferenceChanged -= HandlePreferenceChanged;
        }

        private void HandlePreferenceChanged(string eventKey, bool active) {
            if (eventKey.Equals(PreferencesStorage.EVENT_MINIMAP)) {
                _miniMap.SetActive(active);
            }
        }
    }
}