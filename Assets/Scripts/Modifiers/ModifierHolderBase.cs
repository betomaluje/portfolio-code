using System;
using Interactable;
using UnityEngine;

namespace Modifiers {
	public abstract class ModifierHolderBase : MonoBehaviour, IHolderInteraction, IInteract {
		public abstract event Action<int> OnModifierSelected;
		public abstract void DisableItem();

		public abstract void DoInteract();
		public abstract void CancelInteraction();
	}
}