using System;
using System.Text;
using UnityEngine;

namespace DebugTools {
    public class FPSWindow : MonoBehaviour {
        [SerializeField]
        private Font _font;

        [SerializeField]
        private int _fontSize = 16;

        [SerializeField]
        private Color _textColor = Color.black;

        [SerializeField]
        private Color _backgroundColor = Color.white;

        [SerializeField]
        private float _marginHorizontal = 100;

        [SerializeField]
        private float _marginVertical = 50;

        [SerializeField]
        [Tooltip("Padding for the version label. X is horizontal padding, Y is vertical padding.")]
        private Vector2Int _padding = new(10, 6);

        private bool _showInBuild = true;

        private float _pollingTime = .5f;
        private float _time;
        private int _frameCount;
        private readonly StringBuilder _sb = new();

        private readonly PreferencesStorage _preferencesStorage = new();

        private void Awake() {
            transform.parent = null;

            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            _marginVertical = Screen.height;
        }

        private void OnEnable() {
            _showInBuild = _preferencesStorage.GetShowFPS();
        }

        private void Update() {
            if (!_showInBuild) {
                return;
            }

            _frameCount++;
            _time += Time.unscaledDeltaTime;

            if (_time >= _pollingTime) {
                var frameRate = Math.Round(_frameCount / _time, 2);

                _sb.Clear();
                _sb.Append("FPS: ");
                _sb.Append(frameRate.ToString("0.")); // Convert float to string

                _time = 0f;
                _frameCount = 0;
            }
        }

        private void OnGUI() {
            if (!_showInBuild) {
                return;
            }

            // create a label with the version number and place it on the bottom right of the screen
            var content = new GUIContent($"{_sb}");
            var size = GUI.skin.box.CalcSize(content);
            var position = new Rect(Screen.width - _marginHorizontal, Screen.height - _marginVertical, size.x + _padding.x, size.y + _padding.y);

            position.y = Mathf.Clamp(position.y, 0f, Screen.height - size.y);

            var style = new GUIStyle(GUI.skin.box) {
                richText = true,
                fontSize = _fontSize,
                normal = {
                    textColor = _textColor,
                    background = MakeTex((int)size.x, (int)size.y, _backgroundColor)
                }
            };

            if (_font != null) {
                style.font = _font;
            }

            GUI.Box(position, content, style);
        }

        private Texture2D MakeTex(int width, int height, Color col) {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i) {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        #region Editor Callbacks

        public void Toggle_ShowFPS(bool isChecked) {
            _showInBuild = isChecked;
        }

        #endregion
    }
}