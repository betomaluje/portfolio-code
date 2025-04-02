using UnityEngine;
using UnityEditor;
using Detection;

[CustomEditor(typeof(MultipleTargetDetection))]
public class MultipleTargetDetectionEditor : UnityEditor.Editor {

    private MultipleTargetDetection _detector;

    private float _lastDistance = 0;
    private Transform _lastTarget = null;

    private void OnEnable() {
        _detector = (MultipleTargetDetection)target;
        if (_detector != null) {
            _detector.OnClosestDetected.AddListener(HandleTargetDetected);
        }
    }

    private void OnDisable() {
        if (_detector != null) {
            _detector.OnClosestDetected.RemoveListener(HandleTargetDetected);
        }
        _detector = null;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (_detector == null || _lastTarget == null) {
            return;
        }

        GUILayout.Label($"{_lastTarget.name} is {_lastDistance} away");
    }

    public void HandleTargetDetected(Transform target, float distance) {
        if (_detector == null || target == null) {
            return;
        }

        _lastDistance = distance;
        _lastTarget = target;

        Repaint();
    }
}