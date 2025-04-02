using UnityEngine;

namespace UI {
    public class MinimapUIToggle : MonoBehaviour {
        [SerializeField]
        private GameObject[] _objectsToToggle;

        private PreferencesStorage _preferencesStorage = new();

        private bool _storedShowMinimap = true;

        /// <summary>
        /// Gets or sets if the minimap should be shown
        /// </summary>
        public bool ShowMinimap {
            get => _storedShowMinimap;
            set {
                _storedShowMinimap = value;
                ToggleObjects();
            }
        }

        private void OnEnable() {
            PreferencesStorage.OnPreferenceChanged += HandlePreferenceChanged;

            _storedShowMinimap = _preferencesStorage.GetShowMinimap();
        }

        private void OnDisable() {
            PreferencesStorage.OnPreferenceChanged -= HandlePreferenceChanged;

            _preferencesStorage.SetShowMinimap(_storedShowMinimap);
        }

        private void OnDestroy() {
            PreferencesStorage.OnPreferenceChanged -= HandlePreferenceChanged;
        }

        private void Start() {
            ToggleObjects();
        }

        private void HandlePreferenceChanged(string eventKey, bool active) {
            if (eventKey.Equals(PreferencesStorage.EVENT_MINIMAP)) {
                ShowMinimap = active;
            }
        }

        private void ToggleObjects() {
            foreach (var obj in _objectsToToggle) {
                obj.SetActive(_storedShowMinimap);
            }
        }

    }
}