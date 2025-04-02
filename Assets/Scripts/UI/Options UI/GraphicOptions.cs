using System.Linq;
#if UNITY_EDITOR
using System.Text;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Options {
    public class GraphicOptions : OptionsPanel {
        [SerializeField]
        private TMP_Dropdown _resolutionDropdown;

        [SerializeField]
        private Toggle _windowedToggle;

        [Header("Debug")]
        [SerializeField]
        private bool _debug = false;

        private void Start() {
            _resolutionDropdown.ClearOptions();
            var resolutions = ResolutionSingleton.Instance.Resolutions.ToList();
            _resolutionDropdown.AddOptions(resolutions);

            Vector2Int lastSelectedResolution = new();
            (lastSelectedResolution.x, lastSelectedResolution.y) = ResolutionSingleton.Instance.GetPrefsResolution();

            if (lastSelectedResolution.x == -1 && lastSelectedResolution.y == -1) {
                var lastAvailable = resolutions.Last().Split(ResolutionSingleton.MIDDLE_SUFFIX);
                lastSelectedResolution = new(int.Parse(lastAvailable[0]), int.Parse(lastAvailable[1]));

                ResolutionSingleton.Instance.SetResolution(lastSelectedResolution.x, lastSelectedResolution.y, _windowedToggle.isOn);
            }

            _resolutionDropdown.value = ResolutionSingleton.Instance.FindIndex(lastSelectedResolution);
            _resolutionDropdown.RefreshShownValue();
        }

        private void OnEnable() {
            _resolutionDropdown.onValueChanged.AddListener(DropDownValueChanged);
            _windowedToggle.onValueChanged.AddListener(SetWindowedToggle);

            _windowedToggle.isOn = ResolutionSingleton.Instance.GetPrefsWindowed();
        }

        private void OnDisable() {
            _resolutionDropdown.onValueChanged.RemoveListener(DropDownValueChanged);
            _windowedToggle.onValueChanged.RemoveListener(SetWindowedToggle);
        }

        private void SetWindowedToggle(bool toggled) {
            _windowedToggle.isOn = toggled;

            ResolutionSingleton.Instance.SetWindowed(toggled);
        }

        private void DropDownValueChanged(int index) {
            var res = _resolutionDropdown.options[index].text.Split(ResolutionSingleton.MIDDLE_SUFFIX);
            ResolutionSingleton.Instance.SetResolution(int.Parse(res[0]), int.Parse(res[1]), _windowedToggle.isOn);
        }

#if UNITY_EDITOR
        private void OnGUI() {
            if (!_debug || ResolutionSingleton.Instance == null) {
                return;
            }
            StringBuilder stringBuilder = new();
            var windowed = ResolutionSingleton.Instance.GetPrefsWindowed();
            var (width, height) = ResolutionSingleton.Instance.GetPrefsResolution();
            stringBuilder.AppendLine($"Windowed: {windowed}");
            stringBuilder.AppendLine($"Res: {width} x {height}");


            GUIStyle boxStyle = new(GUI.skin.box);
            boxStyle.normal.textColor = Color.white;
            boxStyle.fontSize = 20;

            GUILayout.Label($"{stringBuilder}", boxStyle);
        }
#endif
    }
}