using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(menuName = "Aurora/ThrowablePaths/Parabolic Path")]
    public class ParabolicPath : ThrowablePath {
        [SerializeField]
        private float _gravity = 9.8f;

        public override UniTask<Vector2> GetPosition(float elapsedTime, Vector2 startPoint, Vector2 direction, float speed) {
            Vector2 horizontalMovement = direction.normalized * speed * elapsedTime;
            float verticalMovement = -_gravity * Mathf.Pow(elapsedTime, 2) / 2;
            return UniTask.FromResult(startPoint + horizontalMovement + new Vector2(0, verticalMovement));
        }
    }
}