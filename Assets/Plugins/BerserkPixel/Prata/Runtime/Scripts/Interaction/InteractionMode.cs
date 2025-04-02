using UnityEngine;

namespace BerserkPixel.Prata
{
    [System.Serializable]
    public enum InteractionMode
    {
        Mode2D = 0,
        Mode3D = 1
    }

    public struct InteractionHit
    {
        public Transform hitTransform;

        public InteractionHit(Transform t)
        {
            hitTransform = t;
        }

        public bool IsValid => hitTransform != null;
    }
}