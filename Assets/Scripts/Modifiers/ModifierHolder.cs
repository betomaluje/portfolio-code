using Extensions;
using Interactable;
using Sprites;
using UnityEngine;

namespace Modifiers {
    public abstract class ModifierHolder<T> : ModifierHolderBase where T : IModifier {
        [SerializeField]
        protected T[] _itemsConfigs;

        [SerializeField]
        protected Transform _container;

        public T FirstItem => _itemsConfigs.Length > 0 ? _itemsConfigs[0] : default(T);

        public void SetItems(T[] items) {
            _itemsConfigs = items;
        }

        public void SetItem(T item) {
            _itemsConfigs = new T[] { item };
        }

        private void OnValidate() {
            if (_container == null) {
                _container = transform.FindChildren("Powerup");
            }

            if (FirstItem != null && this.FindInChildren(out ItemInteractTag interactTag)) {
                interactTag.UpdateItemDetailsButton();
            }
        }

        private void Start() {
            if (FirstItem != null && this.FindInChildren(out ItemInteractTag interactTag)) {
                interactTag.UpdateItemDetailsButton();
            }
        }

        /// <summary>
        /// Called from the Editor as UnityEvent on children.
        /// </summary>
        /// <param name="target"></param>
        protected abstract void HandleTargetDetected(Transform target);

        public override void DisableItem() {
            enabled = false;
            if (this.FindInChildren(out DissolveFX dissolveFx)) {
                dissolveFx.Dissolve();
            }
            if (this.FindInChildren(out ItemInteractTag interactTag)) {
                interactTag.gameObject.SetActive(false);
            }
        }
    }
}