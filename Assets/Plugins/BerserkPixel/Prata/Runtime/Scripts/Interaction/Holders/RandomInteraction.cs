using UnityEngine;

namespace BerserkPixel.Prata
{
    [AddComponentMenu("Prata/Interactions/Random Interaction")]
    public class RandomInteraction : MonoBehaviour, IInteractionHolder
    {
        [SerializeField] private Interaction[] interactions;

        private int _lastIndex = -1;

        public Interaction GetInteraction()
        {
            if (_lastIndex == -1)
            {
                _lastIndex = Random.Range(0, interactions.Length);
            }
            return interactions[_lastIndex];
        }

        public void Reset()
        {
            _lastIndex = -1;
        }
    }
}