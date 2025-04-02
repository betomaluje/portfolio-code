using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DebugTools {
    public class CameraScreenshot : MonoBehaviour {
        [SerializeField]
        private float _timeBetweenCaptures = .5f;

        private string _capturesFilePath;

        private string GetCapturePath(string fileName) => Path.Combine(_capturesFilePath, $"{fileName}.png");

        private bool _isRecording;

        private void Awake() {
            _capturesFilePath = Application.dataPath + "/Captures/";
            if (!Directory.Exists(_capturesFilePath)) {
                Directory.CreateDirectory(_capturesFilePath);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Update() {
            if (!_isRecording && Input.GetKeyDown(KeyCode.O)) {
                _isRecording = true;
            }
        }

        private void LateUpdate() {
            if (_isRecording) {
                TakeScreenshot();
            }
        }

        private async void TakeScreenshot() {
            await UniTask.WaitForEndOfFrame();
            var fileName = System.DateTime.Now.ToString("MM-dd-yy (HH-mm-ss)");
            ScreenCapture.CaptureScreenshot(GetCapturePath(fileName));
            await UniTask.Delay((int)_timeBetweenCaptures * 1000);

            _isRecording = false;
        }
    }

}