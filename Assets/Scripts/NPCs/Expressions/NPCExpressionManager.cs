using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NPCs.Expressions {
    public class NPCExpressionManager : MonoBehaviour {
        private const float ExpressionTime = .8f;

        [SerializeField]
        protected SpriteRenderer _expressionRenderer;

        [SerializeField]
        private List<Expression> _expressions;

        private float _elapsedTime;
        private bool _isWorking;

        private void Awake() {
            ResetExpression();
        }

        private void Update() {
            if (!_isWorking) {
                return;
            }

            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= ExpressionTime) {
                ResetExpression();
            }
        }

        private void SetExpression(Sprite sprite) {
            _elapsedTime = 0f;
            _isWorking = true;
            _expressionRenderer.sprite = sprite;
            _expressionRenderer?.gameObject?.SetActive(true);
        }

        public void ResetExpression() {
            _expressionRenderer?.gameObject?.SetActive(false);
        }

        public void SetExpression(ExpressionType type) {
            var expression = _expressions.FirstOrDefault(e => e.Type == type);
            SetExpression(expression.Sprite);
        }
    }

    [Serializable]
    public struct Expression {
        public ExpressionType Type;
        public Sprite Sprite;
    }
}