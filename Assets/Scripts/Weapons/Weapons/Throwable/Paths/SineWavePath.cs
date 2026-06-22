using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(menuName = "Aurora/ThrowablePaths/Sine Wave Path")]
    public class SineWavePath : ThrowablePath {
        [SerializeField]
        private float _amplitude = 1.0f;
        [SerializeField]
        private float _frequency = 1.0f;

        public override UniTask<Vector2> GetPosition(float elapsedTime, Vector2 startPoint, Vector2 direction, float speed) {
            Vector2 forwardMovement = direction.normalized * speed * elapsedTime;
            float sineOffset = Mathf.Sin(elapsedTime * _frequency) * _amplitude;
            return UniTask.FromResult(startPoint + forwardMovement + new Vector2(0, sineOffset));
        }
    }
}