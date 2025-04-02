using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System.Collections;

namespace Bezier {
    [RequireComponent(typeof(LineRenderer))]
    public class BezierRenderer : MonoBehaviour {
        [SerializeField]
        private LineRenderer _lineRenderer;

        [SerializeField]
        private int _pointCount = 20; //Number of points to plot the curve with

        [Header("Wave Offset Settings")]
        [Tooltip("Speed of the wave")]
        [SerializeField]
        private float _waveSpeed = 1f;

        [Tooltip("Amplitude of the wave")]
        [SerializeField]
        private float _waveAmplitude = 0.5f;

        [SerializeField]
        private float _animationDuration = 1f;

        [SerializeField]
        private WaveOffset _waveOffset = WaveOffset.Sine;

        private Vector2[] _controlPoints;
        private Vector2 _textureScale = Vector2.one;

        private void OnValidate() => _lineRenderer = gameObject.GetOrAdd<LineRenderer>();

        private void Awake() {
            _lineRenderer = gameObject.GetOrAdd<LineRenderer>();
        }

        /// <summary>
        /// Renders the bezier curve with the given control points. This is called on the Update method.
        /// </summary>
        /// <param name="positions">The control points of the bezier curve.</param>
        public void Begin() {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = _pointCount;
        }

        public void UpdatePositions(Vector2[] positions) {
            _controlPoints = positions;
        }

        private void Update() {
            if (!_lineRenderer.enabled || _lineRenderer.positionCount == 0 || _controlPoints == null) {
                return;
            }

            MakeBezier();
        }

        /// <summary>
        /// Renders the bezier curve with the given control points.
        /// </summary>
        private void MakeBezier() {
            var allPositions = new List<Vector3>(_pointCount);
            float time = Time.time;

            var direction = (_controlPoints[2] - _controlPoints[0]).normalized;
            _textureScale.y = direction.x < 0 ? 1 : -1;
            _lineRenderer.textureScale = _textureScale;

            for (int i = 0; i < _pointCount; i++) {
                float t = (float)i / (_pointCount - 1);

                Vector2 controlP1WithOffset = _controlPoints[1];
                if (i > 0 && i < _pointCount - 1) {
                    controlP1WithOffset = ApplyWaveOffset(_controlPoints[1], time, i);
                }

                allPositions.Add(BezierCurve.QuadraticBezierInterp(
                    _controlPoints[0],
                    controlP1WithOffset,
                    _controlPoints[2],
                    t)
                );
            }

            if (_lineRenderer.GetPosition(0) == default) {
                _lineRenderer.SetPositions(allPositions.ToArray());
            }
            else {
                StartCoroutine(SmoothlyUpdateLineRenderer(allPositions));
            }
        }

        private IEnumerator SmoothlyUpdateLineRenderer(List<Vector3> allPositions) {
            for (int i = 0; i < _pointCount; i++) {
                float elapsedTime = 0f;
                Vector3 startPosition = _lineRenderer.GetPosition(i);
                Vector3 targetPosition = allPositions[i];

                while (elapsedTime < _animationDuration) {
                    _lineRenderer.SetPosition(i, Vector3.Lerp(startPosition, targetPosition, elapsedTime / _animationDuration));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                _lineRenderer.SetPosition(i, targetPosition);
            }
        }

        private Vector2 ApplyWaveOffset(Vector2 controlPoint, float time, int index) {
            float offset = GetWaveOffset(time, index);
            return new Vector2(controlPoint.x + offset, controlPoint.y + offset);
        }

        private float GetWaveOffset(float time, int index) {
            switch (_waveOffset) {
                case WaveOffset.Sine:
                    return Mathf.Sin(time + index * _waveSpeed) * _waveAmplitude;

                case WaveOffset.PerlinNoise:
                    return Mathf.PerlinNoise(index * _waveSpeed, time) * _waveAmplitude;

                case WaveOffset.Square:
                    return Mathf.Sign(Mathf.Sin(time + index * _waveSpeed)) * _waveAmplitude;

                case WaveOffset.Triangle:
                    float triangleWave = Mathf.PingPong(time + index * _waveSpeed, 1f) * 2f - 1f;
                    return triangleWave * _waveAmplitude;

                case WaveOffset.Sawtooth:
                    float sawtoothWave = Mathf.Repeat(time + index * _waveSpeed, 1f);
                    return (sawtoothWave * 2f - 1f) * _waveAmplitude;

                case WaveOffset.Random:
                    return (Random.value * 2f - 1f) * _waveAmplitude;

                case WaveOffset.None:
                default:
                    return 0f;
            }
        }

        public void Finish() {
            _lineRenderer.enabled = false;
            _lineRenderer.positionCount = 0;
        }
    }

    [System.Serializable]
    public enum WaveOffset {
        None,
        Sine,
        PerlinNoise,
        Square,
        Triangle,
        Sawtooth,
        Random
    }
}