using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Options {
    public abstract class OptionsPanel : MonoBehaviour {
        [SerializeField]
        private TabButton Tab;

        [SerializeField]
        private GameObject Panel;

        [SerializeField]
        private GameObject FirstElement;

        protected virtual void Awake() {
            Panel.SetActive(false);
        }

        public void Activate() {
            Panel.SetActive(true);
            Tab.FadeIn();

            EventSystem.current.SetSelectedGameObject(FirstElement);
        }

        public void Deactivate() {
            Panel.SetActive(false);
            Tab.FadeOut();
        }
    }
}