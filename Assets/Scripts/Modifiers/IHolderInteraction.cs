using System;

namespace Modifiers {
    public interface IHolderInteraction {
        /// <summary>
        /// Called when a holder is interacted with. It sends the objects HashCode
        /// </summary>
        public event Action<int> OnModifierSelected;

        void DisableItem();
    }
}