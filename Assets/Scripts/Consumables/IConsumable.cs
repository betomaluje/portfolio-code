using UnityEngine;

namespace Consumables {
    public interface IConsumable {
        void Consume(Transform owner, Transform target);
    }
}