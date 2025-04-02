using UnityEngine;

namespace Interactable {
    public class InteractTag : MonoBehaviour, IInteractTag {
        [SerializeField]
        private InteractAction _interactAction = InteractAction.None;

        [SerializeField]
        private string _objectName;

        public InteractAction Action => _interactAction;

        public Sprite ItemIcon => null;

        public string ObjectName {
            get {
                if (string.IsNullOrEmpty(_objectName)) {
                    return transform.parent != null ? transform.parent.name.Replace("(Clone)", "") : transform.name.Replace("(Clone)", "");
                }
                else {
                    return _objectName;
                }
            }
            set { _objectName = value; }
        }

        public Color TagColor => Color.white;
    }
}