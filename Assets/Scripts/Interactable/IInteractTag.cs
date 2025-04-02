
using UnityEngine;

namespace Interactable {
    public interface IInteractTag {
        public InteractAction Action { get; }
        public string ObjectName { get; }

        public Sprite ItemIcon { get; }

        public Color TagColor { get; }
    }
}