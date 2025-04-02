using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;

public class ObjectReplacer : EditorWindow {
    private ObjectField _transformContainer;

    private ObjectField _targetObject;

    [MenuItem("Tools/Aurora/ObjectReplacer")]
    private static void ShowWindow() {
        var window = GetWindow<ObjectReplacer>();
        window.titleContent = new GUIContent("ObjectReplacer");
        window.Show();
    }

    public void CreateGUI() {
        var root = rootVisualElement;

        var containerPane = new VisualElement();
        root.Add(containerPane);

        var titleLabel = new Label("Prefab to Clone");
        containerPane.Add(titleLabel);

        _targetObject = new ObjectField {
            objectType = typeof(Transform),
            allowSceneObjects = false,
            style ={
                marginTop = 2,
                minWidth = 250,
                width = 250,
                marginBottom = 8
            }
        };

        containerPane.Add(_targetObject);

        var titleContent2 = new Label("Previous Container");
        containerPane.Add(titleContent2);

        _transformContainer = new ObjectField {
            objectType = typeof(Transform),
            allowSceneObjects = true,
            style = {
                marginTop = 2,
                minWidth = 250,
                width = 250,
                marginBottom = 8
            }
        };

        containerPane.Add(_transformContainer);

        DrawCopy(containerPane);
    }

    private void DrawCopy(VisualElement containerPane) {
        var copyAllButton = CreateButton("Copy All Transform", () => {
            if (_transformContainer == null) {
                // show error
                var error = new Label("Please select a container");
                containerPane.Add(error);
                return;
            }

            if (_targetObject == null) {
                // show error
                var error = new Label("Please select a prefab");
                containerPane.Add(error);
                return;
            }

            var prefab = _targetObject.value as Transform;

            var toSearch = prefab.name;

            var container = _transformContainer.value as Transform;
            var sameTransforms = container.GetComponentsInChildren<Transform>().Where(t => t.name.StartsWith(toSearch)).ToArray();

            foreach (var source in sameTransforms) {
                var target = PrefabUtility.InstantiatePrefab(prefab) as Transform;

                Undo.RecordObject(target, "Undo set position"); target.position = source.position;
                Undo.RecordObject(target, "Undo set rotation"); target.rotation = source.rotation;
                Undo.RecordObject(target, "Undo set scale"); target.localScale = source.localScale;
                target.parent = source.parent;
            }


        });

        containerPane.Add(copyAllButton);
    }

    private Button CreateButton(string text, Action action) {
        var createButton = new Button {
            text = text
        };

        createButton.RegisterCallback<ClickEvent>(e => { action?.Invoke(); });

        createButton.style.position = Position.Relative;
        createButton.style.height = 24;
        createButton.style.alignSelf = Align.FlexStart;

        return createButton;
    }

    public static Transform FindChildren(Transform self, string exactName) => FindRecursive(self, child => child.name == exactName);

    public static Transform FindRecursive(Transform self, Func<Transform, bool> selector) {
        foreach (Transform child in self) {
            if (selector(child)) {
                return child;
            }

            var finding = FindRecursive(child, selector);

            if (finding != null) {
                return finding;
            }
        }

        return null;
    }
}