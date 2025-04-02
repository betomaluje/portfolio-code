using DG.Tweening;
using Extensions;
using NPCs;
using TMPro;
using UnityEngine;

namespace UI {
    public class DisablePanelComponent {
          private readonly RectTransform _container;

        public DisablePanelComponent(RectTransform container) {
            _container = container;
        }          

          public void Hide() {
            _container?.HideChildren();
          }
        
    }
}