using UnityEngine;

namespace Base {
    public interface IMove {
        void Move(Vector2 velocity);

        void Stop();

        void ApplyForce(Vector2 direction, float speed, float duration);

        void MoveToPoint(Vector2 endPoint, float duration);

        void SetMovementInfluence(float amount);

        void ResetMovementInfluence() { }

        void SetScaleInfluence(float amount);

        void ResetScaleInfluence() { }

        void FlipSprite(Vector2 direction);

        public float LastX { get; }

        public ParticleSystem MovementParticles { get; set; }
    }
}