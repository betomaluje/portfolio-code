using UnityEngine;

namespace BerserkPixel.Prata
{
    [AddComponentMenu("Prata/Interactions/Simple Interaction")]
    public class SimpleInteraction : MonoBehaviour, IInteractionHolder
    {
        [SerializeField] private Interaction interaction;

        public Interaction GetInteraction()
        {
            return interaction;
        }

        public void Reset()
        {

        }
    }
}