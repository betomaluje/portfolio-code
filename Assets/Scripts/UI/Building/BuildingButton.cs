using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Building {
    public class BuildingButton : MonoBehaviour, ISelectHandler, IDeselectHandler {
        [SerializeField]
        private TextMeshProUGUI _label;

        public void OnDeselect(BaseEventData eventData) {
            OnButtonDeselect?.Invoke();
        }

        public void OnSelect(BaseEventData eventData) {
            OnButtonSelect?.Invoke();
        }

        private event Action OnButtonSelect = delegate { };
        private event Action OnButtonDeselect = delegate { };

        public void Setup(Buildings.Building building, Action onSelect = null, Action onDeselect = null) {
            _label.text = building.Name;
            OnButtonSelect = onSelect;
            OnButtonDeselect = onDeselect;
        }
    }
}