using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons {
    public interface IThrowablePath {
        UniTask<Vector2> GetPosition(float elapsedTime, Vector2 startPoint, Vector2 direction, float speed);
    }

}