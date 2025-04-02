using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    [RequireComponent(typeof(RectTransform))]
    public class ButtonAnimations : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField]
        private float _angleToRotate = 15f;

        [SerializeField]
        private float _scale = 1.2f;

        [SerializeField]
        private float _duration = 0.2f;

        private RectTransform _rectTransform;

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnSelect(BaseEventData eventData) {
            DoHover();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            DoHover();
        }

        public void OnDeselect(BaseEventData eventData) {
            UndoHover();
        }

        public void OnPointerExit(PointerEventData eventData) {
            UndoHover();
        }

        private void DoHover() {
            if (_angleToRotate != 0)
                _rectTransform.DORotate(new Vector3(0, 0, _angleToRotate), _duration).SetEase(Ease.OutBack).SetUpdate(true);

            if (_scale != 1)
                _rectTransform.DOScale(_scale, _duration).SetEase(Ease.OutBack).SetUpdate(true);
        }

        private void UndoHover() {
            if (_angleToRotate != 0)
                _rectTransform.DORotate(Vector3.zero, _duration).SetEase(Ease.OutBack).SetUpdate(true);

            if (_scale != 1)
                _rectTransform.DOScale(1, _duration).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }
}