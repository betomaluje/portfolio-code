using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Utils;

namespace DebugTools {
    public class VersionWindow : PersistentSingleton<VersionWindow> {
        [SerializeField]
        private TextMeshProUGUI _versionLabel;

        [OnValueChanged("UpdateLabel")]
        [SerializeField]
        private TMP_FontAsset _font;

        [OnValueChanged("UpdateLabel")]
        [SerializeField]
        private int _fontSize = 16;

        [OnValueChanged("UpdateLabel")]
        [SerializeField]
        private Color _textColor = Color.black;

        private bool _showInBuild = true;

        private readonly PreferencesStorage _preferencesStorage = new();

        private void OnEnable() {
            _showInBuild = _preferencesStorage.GetShowVersion();
            PreferencesStorage.OnPreferenceChanged += Toggle_ShowVersion;
        }

        private void OnDisable() {
            PreferencesStorage.OnPreferenceChanged -= Toggle_ShowVersion;
        }

        private void Start() {
            _versionLabel.text = $"v{Application.version}";
            UpdateLabel();
        }

        private void OnDestroy() {
            PreferencesStorage.OnPreferenceChanged -= Toggle_ShowVersion;
        }

        private void UpdateLabel() {
            if (_versionLabel == null) {
                return;
            }
            _versionLabel.gameObject.SetActive(_showInBuild);
            _versionLabel.color = _textColor;
            _versionLabel.font = _font;
            _versionLabel.fontSize = _fontSize;
        }

        #region Editor Callbacks

        public void Toggle_ShowVersion(string preferenceName, bool isChecked) {
            if (preferenceName.Equals(PreferencesStorage.EVENT_VERSION)) {
                _showInBuild = isChecked;
                UpdateLabel();
            }
        }

        #endregion
    }
}