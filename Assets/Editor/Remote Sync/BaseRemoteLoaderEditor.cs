using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RemoteSync {
    public abstract class BaseRemoteLoaderEditor : EditorWindow {
        // URL of the Excel file        
        protected abstract string RemoteExcelUrl { get; }

        // Local path to save the downloaded Excel file
        protected abstract string LocalFilePath { get; }

        // Where to save the ScriptableObjects
        protected abstract string OutputFolderPath { get; }

        private string _remoteExcelUrl;
        private string _localFilePath;
        private string _outputFolderPath;

        private Stopwatch _stopwatch = new();

        protected virtual void OnEnable() {
            _remoteExcelUrl = RemoteExcelUrl;
            _localFilePath = LocalFilePath;
            _outputFolderPath = OutputFolderPath;
        }

        protected void OnGUI() {
            _remoteExcelUrl = EditorGUILayout.TextField("Remote Excel URL", _remoteExcelUrl);
            _localFilePath = EditorGUILayout.TextField("Local File Path", _localFilePath);
            _outputFolderPath = EditorGUILayout.TextField("Output Folder Path", _outputFolderPath);

            if (GUILayout.Button("Sync with Remote")) {
                if (string.IsNullOrEmpty(_remoteExcelUrl)) {
                    Debug.LogError("Remote Excel URL is empty. Cannot download file.");
                    return;
                }

                if (string.IsNullOrEmpty(_localFilePath)) {
                    Debug.LogError("Local File Path is empty. Cannot download file.");
                    return;
                }

                if (string.IsNullOrEmpty(_outputFolderPath)) {
                    Debug.LogError("Output Folder Path is empty. Cannot download file.");
                    return;
                }

                DownloadExcelFile(_remoteExcelUrl, _localFilePath);
            }

            if (GUILayout.Button("Sync only Locally")) {
                _stopwatch.Start();
                ParseExcel(_localFilePath);
                _stopwatch.Stop();
                FinishSync(_stopwatch.Elapsed.Seconds);
            }
        }

        protected void DownloadExcelFile(string remoteUrl, string localPath) {
            _stopwatch.Start();
            Debug.Log("Starting file download...");

            // Use NetworkManager to download the Excel file
            NetworkManager.DownloadFile(remoteUrl, localPath, success => {
                if (success) {
                    Debug.Log($"File downloaded to: {localPath}");
                    ParseExcel(localPath);
                    _stopwatch.Stop();
                    FinishSync(_stopwatch.Elapsed.Seconds);
                }
                else {
                    _stopwatch.Stop();
                    Debug.LogError("File download failed. Cannot import data.");
                }
            });
        }

        protected abstract void ParseExcel(string localPath);

        protected abstract void FinishSync(int elapsedSeconds = 0);

        protected string[] ParseCsvLine(string line) {
            // Use a regex to split CSV lines by commas, respecting quoted fields
            var regex = new Regex("(?:^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
            var matches = regex.Matches(line);

            string[] values = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++) {
                // Remove surrounding quotes and unescape inner quotes
                values[i] = matches[i].Value.TrimStart(',').Trim('\"').Replace("\"\"", "\"");
            }

            return values;
        }

        protected Sprite LookupSprite(string modifierType, string folder, string spriteName) {
            if (string.IsNullOrEmpty(spriteName)) {
                spriteName = $"default_{modifierType.ToLower()}";
            }

            Sprite sprite = Resources.Load<Sprite>($"Modifiers/{folder}/{spriteName}");

            if (sprite == null) {
                // we load the default according to the stat type
                sprite = Resources.Load<Sprite>($"Modifiers/{folder}/default_{modifierType.ToLower()}");
            }

            return sprite;
        }
    }
}