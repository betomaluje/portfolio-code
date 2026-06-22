using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons {
    public abstract class ThrowablePath : ScriptableObject, IThrowablePath {
        public abstract UniTask<Vector2> GetPosition(float elapsedTime, Vector2 startPoint, Vector2 direction, float speed);
    }
}