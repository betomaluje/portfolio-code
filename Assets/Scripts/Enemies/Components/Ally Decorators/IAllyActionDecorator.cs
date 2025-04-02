using System;
using UnityEngine;

namespace Enemies.Components {
    public interface IAllyActionDecorator {
        Action<Transform, Transform> OnPerformed { get; set; }

        bool ConditionsMet(Transform target, AllyActionsTypes allowedTypes);

        void DoAction(Transform actor, Transform target);
    }
}