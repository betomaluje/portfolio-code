using UnityEngine;

namespace BerserkPixel.Prata {
    [AddComponentMenu("Prata/Interactions/Auto Reset Interaction")]
    public class AutoResetInteraction : MonoBehaviour, IInteractionHolder {
        [SerializeField] private Interaction interaction;

        public Interaction GetInteraction() {
            return interaction;
        }

        public void Reset() {
            var lastInteraction = GetInteraction();
            if (lastInteraction != null && !lastInteraction.HasAnyDialogLeft()) {
                // if there's not any more text to show we change the interaction index to the next one
                lastInteraction.Reset();
            }
        }
    }
}