using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils {
    public class InternetConnection {
        private static string URL = "https://google.com";

        public InternetConnection(string url) {
            URL = url;
        }

        public static async UniTask<bool> HasInternet() {
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                return false;
            }
            else {
                var request = new UnityWebRequest(URL) {
                    timeout = 10,
                    downloadHandler = new DownloadHandlerBuffer()
                };
                try {
                    await request.SendWebRequest().ToUniTask();
                    return request.error == null && request.responseCode == 200;
                }
                catch (Exception) {
                    return false;
                }
                finally {
                    request.Dispose();
                }
            }
        }
    }
}