using UnityEngine;

namespace Utils {
    public class MoveTowards : MonoBehaviour {
        [SerializeField]
        private Transform _target;

        [SerializeField]
        private float _speed = 5.0f;

        [SerializeField]
        private float _minDistance = .2f;

        private void Update() {
            var distance = Vector2.Distance(transform.position, _target.position);

            if (distance <= _minDistance) {
                return;
            }

            var step = _speed * Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position, _target.position, step);
        }
    }
}