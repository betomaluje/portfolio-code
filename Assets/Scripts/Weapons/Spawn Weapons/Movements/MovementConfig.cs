using System;
using UnityEngine;

namespace Weapons {
    public abstract class MovementConfig {
        public Transform transform;

        public abstract void Move(float deltaTime);

        protected Action OnMoveEnd;
        protected Action OnTimerEnd;

        public void SetMovementEndAction(Action onMoveEnd) {
            OnMoveEnd = onMoveEnd;
        }

        public void SetTimerEndAction(Action onTimerEnd) {
            OnTimerEnd = onTimerEnd;
        }

        public MovementConfig(Transform transform) {
            this.transform = transform;
        }
    }
}
