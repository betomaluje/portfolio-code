using UnityEngine;
using UnityEngine.UI;

namespace UI.Options {
    public class GameplayOptions : OptionsPanel {
        [SerializeField]
        private Toggle _showMinimap;

        [SerializeField]
        private Toggle _enableAimAssist;

        [SerializeField]
        private Toggle _showFPS;

        [SerializeField]
        private Toggle _showVersion;

        [SerializeField]
        private Toggle _showBlood;

        [SerializeField]
        private Toggle _showFogOfWar;

        private readonly PreferencesStorage _preferencesStorage = new();

        private void Start() {
            _showMinimap.isOn = _preferencesStorage.GetShowMinimap();
            _enableAimAssist.isOn = _preferencesStorage.GetAimAssist();
            _showFPS.isOn = _preferencesStorage.GetShowFPS();
            _showVersion.isOn = _preferencesStorage.GetShowVersion();
            _showBlood.isOn = _preferencesStorage.GetShowBlood();
            _showFogOfWar.isOn = _preferencesStorage.GetShowFogOfWar();
        }

        #region Editor UI Callbacks
        public void Toggle_ShowMinimap(bool isChecked) {
            _showMinimap.isOn = isChecked;
            _preferencesStorage.SetShowMinimap(isChecked);
        }

        public void Toggle_EnableAimAssist(bool isChecked) {
            _enableAimAssist.isOn = isChecked;
            _preferencesStorage.SetAimAssist(isChecked);
        }

        public void Toggle_ShowBlood(bool isChecked) {
            _showBlood.isOn = isChecked;
            _preferencesStorage.SetShowBlood(isChecked);
        }

        public void Toggle_ShowFogOfWar(bool isChecked) {
            _showFogOfWar.isOn = isChecked;
            _preferencesStorage.SetShowFogOfWar(isChecked);
        }

        public void Toggle_ShowFPS(bool isChecked) {
            _showFPS.isOn = isChecked;
            _preferencesStorage.SetShowFPS(isChecked);
        }

        public void Toggle_ShowVersion(bool isChedked) {
            _showVersion.isOn = isChedked;
            _preferencesStorage.SetShowVersion(isChedked);
        }
        #endregion
    }
}