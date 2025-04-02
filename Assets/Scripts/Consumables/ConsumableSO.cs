using UnityEngine;

namespace Consumables {
    public abstract class ConsumableSO : ScriptableObject, IConsumable {
        public abstract void Consume(Transform owner, Transform target);

        public abstract void Consume(Transform target);
    }
}