﻿using UnityEngine;

namespace BerserkPixel.Extensions {
    public static class ScriptableObjectExt {
        /// <summary>
        ///     Creates and returns a clone of any given scriptable object.
        /// </summary>
        public static T Clone<T>(this T scriptableObject) where T : ScriptableObject {
            if (scriptableObject == null) {
                Debug.LogWarning($"ScriptableObject was null. Returning default {typeof(T)} object.");
                return (T) ScriptableObject.CreateInstance(typeof(T));
            }

            var instance = Object.Instantiate(scriptableObject);
            instance.name = scriptableObject.name; // remove (Clone) from name
            return instance;
        }
    }
}