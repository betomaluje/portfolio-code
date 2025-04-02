using System;
using UnityEngine;

namespace DebugTools {
    public static class DebugLog {

#if UNITY_EDITOR
        public static bool IsDebug = true;
#else
        public static bool IsDebug = false;
#endif

        public static void ChangeDebug(bool debug) {
            IsDebug = debug;
        }

        public static void Log(Exception e) {
            LogException(e);
        }

        public static void Log(string message) {
            if (IsDebug) {
                Debug.Log(message);
            }
        }

        public static void Log(string message, UnityEngine.Object unityObject) {
            if (IsDebug) {
                Debug.Log(message, unityObject);
            }
        }

        public static void LogException(Exception exception) {
            if (IsDebug) {
                Debug.LogException(exception);
            }
        }

        internal static void LogError(string error) {
            if (IsDebug) {
                Debug.LogError(error);
            }
        }

        internal static void LogWarning(string message) {
            if (IsDebug) {
                Debug.LogWarning(message);
            }
        }
    }
}