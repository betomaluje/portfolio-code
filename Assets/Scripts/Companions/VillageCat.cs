using UnityEngine;

namespace Companions {
    public class VillageCat : MonoBehaviour {
        [SerializeField]
        private Animator _animator;

        private readonly int PARAM_LOOKLEFT = Animator.StringToHash("LookLeft");
        private readonly int PARAM_LOOKRIGHT = Animator.StringToHash("LookRight");
        private readonly int PARAM_IDLE = Animator.StringToHash("Idle");

        private Transform _playerTransform;

        private void OnValidate() {
            if (_animator == null) {
                _animator = GetComponentInChildren<Animator>();
            }
        }

        private void LateUpdate() {
            if (_playerTransform == null) {
                return;
            }

            var dir = (_playerTransform.position - transform.position).normalized;

            if (dir.x < 0) {
                _animator.Play(PARAM_LOOKLEFT);
            }
            else {
                _animator.Play(PARAM_LOOKRIGHT);
            }
        }

        // Called from the TargetDetection Editor UnityEvent
        public void OnTargetDetected(Transform target) {
            _playerTransform = target;
        }

        public void OnTargetLost() {
            _animator.Play(PARAM_IDLE);
            _playerTransform = null;
        }
    }
}