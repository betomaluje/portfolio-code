using Sounds;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    public class ButtonSounds : MonoBehaviour, ISelectHandler, ISubmitHandler {
        public void OnSelect(BaseEventData eventData) {
            SoundManager.instance.Play("button_select");
        }

        public void OnSubmit(BaseEventData eventData) {
            SoundManager.instance.Play("button_click");
        }
    }
}