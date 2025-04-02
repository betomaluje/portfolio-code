using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        private const float MAX_ALPHA = 1f;
        private const float MIN_ALPHA = .6f;
        private const float ANIM_DURATION = .3f;

        [BoxGroup("Elements")]
        [SerializeField]
        private CanvasGroup Container;

        [BoxGroup("Elements")]
        [SerializeField]
        private Image Background;

        [BoxGroup("Elements")]
        [SerializeField]
        private CanvasGroup _triangleContainer;

        [BoxGroup("Scale")]
        [SerializeField]
        private Vector2 _selectedScale = new(1.1f, 1.1f);

        [BoxGroup("Scale")]
        [SerializeField]
        private Vector2 _unselectedScale = new(1f, 1f);

        [BoxGroup("Color")]
        [SerializeField]
        private Color _selectedColor = new(1f, 1f, 1f, 1f);

        [BoxGroup("Color")]
        [SerializeField]
        private Color _unselectedColor = new(1f, 1f, 1f, 0.5f);

        [SerializeField]
        private UnityEvent _onClick;

        private RectTransform _rectTransform;

        private void Awake() {
            _rectTransform = Background.GetComponent<RectTransform>();
        }

        private void Start() {
            Container.DOFade(MIN_ALPHA, 0f).SetUpdate(true);
            Background.DOColor(_unselectedColor, 0f).SetUpdate(true);
            _rectTransform.DOScale(_unselectedScale, 0f).SetUpdate(true);
            _triangleContainer.alpha = 0f;
        }

        public void FadeIn() {
            Container.DOFade(MAX_ALPHA, ANIM_DURATION).SetUpdate(true);
            Background.DOColor(_selectedColor, ANIM_DURATION).SetUpdate(true);
            _rectTransform.DOScale(_selectedScale, ANIM_DURATION).SetUpdate(true);
            _triangleContainer.DOFade(1f, ANIM_DURATION).SetUpdate(true);
        }

        public void FadeOut() {
            Container.DOFade(MIN_ALPHA, ANIM_DURATION).SetUpdate(true);
            Background.DOColor(_unselectedColor, ANIM_DURATION).SetUpdate(true);
            _rectTransform.DOScale(_unselectedScale, ANIM_DURATION).SetUpdate(true);
            _triangleContainer.DOFade(0f, ANIM_DURATION).SetUpdate(true);
        }

        public void OnPointerClick(PointerEventData eventData) {
            _onClick?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            FadeIn();
        }

        public void OnPointerExit(PointerEventData eventData) {
            FadeOut();
        }
    }
}