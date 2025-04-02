using UnityEngine;

namespace Extensions {
    public static class GameObjectExt {
        public static void DestroyNow(this GameObject gameObject) {
#if UNITY_EDITOR
            Object.DestroyImmediate(gameObject);
#else
            Object.Destroy(gameObject);
#endif
        }

        public static void DestroyNow(this Component component) {
#if UNITY_EDITOR
            Object.DestroyImmediate(component);
#else
            Object.Destroy(component);
#endif
        }

        public static bool FindInParent<T>(this GameObject parent, out T component) where T : Component {
            return parent.transform.parent.TryGetComponent<T>(out component);
        }

        public static bool FindInParent<T>(this MonoBehaviour parent, out T component) where T : Component {
            return parent.transform.parent.TryGetComponent<T>(out component);
        }

        public static bool FindAllInParent<T>(this MonoBehaviour parent, out T[] components) where T : Component {
            components = parent.transform.parent.GetComponentsInChildren<T>();
            return components != null && components.Length > 0;
        }

        public static T[] FindAllInParent<T>(this MonoBehaviour parent) where T : Component {
            return parent.transform.parent.GetComponentsInChildren<T>();
        }

        public static bool FindInChildren<T>(this GameObject parent, out T component) where T : Component {
            component = parent.GetComponentInChildren<T>();
            return component;
        }

        public static bool FindInChildren<T>(this MonoBehaviour parent, out T component) where T : Component {
            component = parent.GetComponentInChildren<T>();
            return component;
        }

        public static bool FindAllInChildren<T>(this GameObject parent, out T[] components) where T : Component {
            components = parent.GetComponentsInChildren<T>();
            return components != null && components.Length > 0;
        }

        public static V GetOrAdd<V>(this GameObject parent) where V : Component {
            if (parent.TryGetComponent<V>(out var component)) {
                return component;
            }
            else {
                return parent.AddComponent<V>();
            }
        }

        public static V GetOrAdd<V>(this Transform parent) where V : Component {
            if (parent.gameObject.TryGetComponent<V>(out var component)) {
                return component;
            } else {
                return parent.gameObject.AddComponent<V>();
            }
        }
    }
}