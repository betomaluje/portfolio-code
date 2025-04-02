using System;
using Extensions;
using UnityEngine;
using Utils;

namespace Companions {
    public class CompanionExpressionManager : MonoBehaviour {
        private const float ExpressionTime = .8f;

        [SerializeField]
        protected SpriteRenderer _expressionRenderer;

        [SerializeField]
        private SpritesDictionary _expressions;

        protected CountdownTimer _timer;

        private void Awake() {
            _timer = new CountdownTimer(ExpressionTime);
            _timer.OnTimerStop += ResetExpression;

            _expressionRenderer.gameObject.SetActive(false);
        }

        private void OnValidate() {
            if (_expressionRenderer == null) {
                var child = transform.FindChildren("Expression");
                if (child != null) {
                    _expressionRenderer = child.GetComponent<SpriteRenderer>();
                }
            }
        }

        private void OnDestroy() {
            _timer.OnTimerStop -= ResetExpression;
        }

        private void Update() {
            _timer?.Tick(Time.deltaTime);
        }

        private void SetExpression(Sprite sprite) {
            if (_timer.IsRunning) {
                return;
            }

            _timer.Start();
            _expressionRenderer.sprite = sprite;
            _expressionRenderer.gameObject.SetActive(true);
        }

        public void ResetExpression() {
            _expressionRenderer.gameObject.SetActive(false);
            _timer.Reset();
        }

        public void SetExpression(string expression) {
            if (_expressions.TryGetValue(expression, out var sprite)) {
                SetExpression(sprite);
            }
        }
    }

    [Serializable]
    public class SpritesDictionary : UnitySerializedDictionary<string, Sprite> { }
}