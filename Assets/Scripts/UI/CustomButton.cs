using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class CustomButton : Selectable, IPointerClickHandler, ISubmitHandler {

        [SerializeField]
        private UnityEvent _onClick;

        [SerializeField]
        private UnityEvent _onHoverEnter;

        [SerializeField]
        private UnityEvent _onHoverExit;

#if UNITY_EDITOR
        protected override void OnValidate() {
            if (!TryGetComponent<Image>(out var imageComponent))
                imageComponent = gameObject.AddComponent<Image>();

            targetGraphic = imageComponent;
        }
#endif

        #region Mouse & Keyboard
        public void OnPointerClick(PointerEventData eventData) {
            DoStateTransition(SelectionState.Pressed, true);

            _onClick?.Invoke();

            DoStateTransition(currentSelectionState, false);
        }

        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);
            _onHoverEnter?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData) {
            base.OnPointerExit(eventData);
            _onHoverExit?.Invoke();
        }
        #endregion

        #region Gamepad
        public void OnSubmit(BaseEventData eventData) {
            _onClick?.Invoke();
        }

        public override void OnSelect(BaseEventData eventData) {
            base.OnSelect(eventData);
            _onHoverEnter?.Invoke();
        }

        public override void OnDeselect(BaseEventData eventData) {
            base.OnDeselect(eventData);
            _onHoverExit?.Invoke();
        }
        #endregion
    }
}