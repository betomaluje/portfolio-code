using UnityEngine;
using Utils;

namespace Coins {
    public class MagnetTowardsObject : MonoBehaviour {
        [SerializeField]
        private LayerMask _targetMask;

        [SerializeField]
        private Transform _detectionPosition;

        [SerializeField]
        private float _detectionRadius = 2;

        [SerializeField]
        private float _speed = 5.0f;

        [Header("Debug")]
        [SerializeField]
        private Color _debugColor = new(.1f, .1f, .1f, .4f);

        private bool _canMove;
        private CountdownTimer _timer;

        private void Awake() {
            _canMove = true;
            _timer = new CountdownTimer(2f);
            _timer.OnTimerStop += ResetMovement;
        }

        public void StopMoving() {
            _canMove = false;
            _timer.Start();
        }

        private void ResetMovement() {
            _timer.Reset();
            _canMove = true;
        }

        private void OnDestroy() {
            _timer.OnTimerStop -= ResetMovement;
        }

        private void Update() {
            if (!_canMove) {
                _timer.Tick(Time.deltaTime);
                return;
            }

            var detectCollider = Physics2D.OverlapCircle(_detectionPosition.position, _detectionRadius, _targetMask);
            if (!detectCollider) {
                return;
            }

            var step = _speed * Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position, detectCollider.transform.position, step);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = _debugColor;
            Gizmos.DrawWireSphere(_detectionPosition.position, _detectionRadius);
        }

        private void OnValidate() {
            if (_detectionPosition == null) {
                _detectionPosition = transform;
            }
        }
    }
}