using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EditorTool {
    public class ToolCategoryUI : MonoBehaviour {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private RectTransform _container;

        [Space]
        [Header("Icons")]
        [SerializeField]
        private Sprite _enemyIcon;
        [SerializeField]
        private Sprite _extraIcon;
        [SerializeField]
        private Sprite _playerIcon;

        [Space]
        [Header("FX")]
        [SerializeField]
        private float _animDuration = .5f;
        [SerializeField]
        private float _scaleFactor = 1.2f;

        public void SetupTool(ToolType category) {
            _image.sprite = GetCategoryIcon(category);
            _image.preserveAspect = true;
            SetSelected(false);
        }

        private Sprite GetCategoryIcon(ToolType category) {
            return category switch {
                ToolType.Enemy => _enemyIcon,
                ToolType.Extra => _extraIcon,
                ToolType.Player => _playerIcon,
                _ => null,
            };
        }

        public void SetSelected(bool selected) {
            if (!_container)
                return;

            var factor = selected ? _scaleFactor : 1f;
            _container.DOScale(Vector2.one * factor, _animDuration);
        }
    }
}