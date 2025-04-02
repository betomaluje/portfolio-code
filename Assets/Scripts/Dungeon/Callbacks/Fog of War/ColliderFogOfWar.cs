using UnityEngine;
using UnityEngine.VFX;

namespace Dungeon.FogOfWar {
    public class ColliderFogOfWar : MonoBehaviour {
        private readonly int ColliderPos = Shader.PropertyToID("ColliderPos");

        private VisualEffect _fogOfWarVFX;

        public void Setup(VisualEffect fogOfWarVFX) {
            _fogOfWarVFX = fogOfWarVFX;

        }

        private void Update() {
            if (_fogOfWarVFX == null) {
                return;
            }

            _fogOfWarVFX.SetVector3(ColliderPos, transform.position);
        }

    }
}