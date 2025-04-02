using UnityEngine;

namespace BerserkPixel.Prata
{
    [AddComponentMenu("Prata/Interactions/Sequence Interaction")]
    public class SequenceInteraction : MonoBehaviour, IInteractionHolder
    {
        [SerializeField] private Interaction[] interactions;

        private int _lastIndex = 0;

        public Interaction GetInteraction()
        {
            if (interactions != null && _lastIndex < interactions.Length)
            {
                var interaction = interactions[_lastIndex];
                return interaction;
            }
            else
            {
                return null;
            }
        }

        public void Reset()
        {
            var lastInteraction = GetInteraction();
            if (lastInteraction != null && !lastInteraction.HasAnyDialogLeft())
            {
                // if there's not any more text to show we change the interaction index to the next one
                _lastIndex++;
            }
        }
    }
}