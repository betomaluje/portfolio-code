using UnityEngine;

namespace Modifiers {
    public interface IModifier {
        void Setup(Transform owner);

        void Activate(Transform target);

        void Tick(float deltaTime);

        bool CheckConditions();

        string GetDescription();

        Color GetTagColor();

        void Deactivate();
    }
}