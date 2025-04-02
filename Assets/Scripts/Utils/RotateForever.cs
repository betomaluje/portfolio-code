using UnityEngine;

namespace Utils {
    public class RotateForever : MonoBehaviour {
        [SerializeField]
        private Transform _target;

        [SerializeField]
        [Min(0)]
        private float _speed = 10f;

        [SerializeField]
        private bool _clockwise = true;

        private void Update() {
            // this is (0, 0, 1) -> so it's 2D rotation
            var dir = _clockwise ? -1 : 1;
            _target.Rotate(Vector3.forward, dir * _speed * Time.deltaTime);
        }
    }
}