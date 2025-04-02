using System;
using UnityEngine;

namespace EditorTool {
    [CreateAssetMenu(fileName = "Tool", menuName = "Aurora/Tool", order = 0)]
    public class Tool : ScriptableObject {
        public string ID = Guid.NewGuid().ToString();

        public ToolType Type;
        public Transform Prefab;
        public Transform EditorPrefab;
        public Transform MockPrefab;
        public bool OnlyCopy = false;
        public float UIScale = 1f;
        public int CurrentAmount => _currentAmount;

        private int _currentAmount;

        private SpriteRenderer _spriteRenderer;

        public bool HasAny() => _currentAmount > 0;

        private void OnValidate() {
            if (EditorPrefab == null) {
                EditorPrefab = Prefab;
            }
        }

        public void Setup(int amount) {
            if (OnlyCopy) {
                _currentAmount = amount <= 0 ? 0 : 1;
            }
            else {
                _currentAmount = amount;
            }
        }

        public void DecreaseAmount(int amount) {
            _currentAmount -= amount;
        }

        public void IncreaseAmount(int amount) {
            _currentAmount += amount;
        }

        /// <summary>
        /// Returns the icon of the tool based on it's MockPrefab.
        /// </summary>
        /// <returns>The first sprite found from the MockPrefab</returns>
        public Sprite Icon() {
            if (_spriteRenderer != null) {
                return _spriteRenderer.sprite;
            }

            _spriteRenderer = MockPrefab.GetComponentInChildren<SpriteRenderer>();
            if (_spriteRenderer) {
                return _spriteRenderer.sprite;
            }
            else {
                return null;
            }
        }
    }
}