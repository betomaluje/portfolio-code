using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace RemoteSync {
    public static class NetworkManager {
        private static UnityWebRequest _request;

        public static void DownloadFile(string url, string localPath, Action<bool> callback) {
            GetRequest(url, request => {
                if (request.result == UnityWebRequest.Result.Success) {
                    try {
                        // Save the file locally
                        File.WriteAllBytes(localPath, request.downloadHandler.data);
                        callback(true);
                    }
                    catch (Exception e) {
                        Debug.LogError($"Failed to save file: {e.Message}");
                        callback(false);
                    }
                }
                else {
                    Debug.LogError($"File download error: {request.error}");
                    callback(false);
                }
            });
        }

        private static void GetRequest(string url, Action<UnityWebRequest> callback) {
            if (_request != null) return;

            _request = UnityWebRequest.Get(url);
            var op = _request.SendWebRequest();
            op.completed += operation => {
                callback(_request);
                _request.Dispose();
                _request = null;
            };
        }
    }

}