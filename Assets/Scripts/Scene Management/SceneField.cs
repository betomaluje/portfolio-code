using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scene_Management {
    [Serializable]
    public class SceneField {
        [SerializeField]
        private Object m_SceneAsset;

        [SerializeField]
        private string m_SceneName = "";

        public string SceneName => m_SceneName;

        // makes it work with the existing Unity methods (LoadLevel/LoadScene)
        public static implicit operator string(SceneField sceneField) {
            return sceneField.SceneName;
        }

        // implicit operator for null
        public static implicit operator bool(SceneField sceneField) {
            return sceneField != null && sceneField.m_SceneAsset != null;
        }
    }
}