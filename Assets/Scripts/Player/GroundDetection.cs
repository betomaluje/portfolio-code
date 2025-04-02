using UnityEngine;

namespace Player {
    public class GroundDetection : MonoBehaviour {
        [SerializeField]
        private float _detectionDistance = 1.25f;

        [SerializeField]
        private float _detectionRadius = 0.2f;

        [SerializeField]
        private LayerMask _groundLayer;

        [SerializeField]
        private LayerMask _secondGroundLayer;

        [SerializeField]
        private Transform _groundCheck;

        [SerializeField]
        private Transform _forwardCheck;

        [SerializeField]
        protected bool _debug = true;

        private Vector2 _lastDirection;

        public bool IsGrounded => Physics2D.OverlapCircle(_groundCheck.position, _detectionRadius, _groundLayer) || Physics2D.OverlapCircle(_groundCheck.position, _detectionRadius, _secondGroundLayer);

        /// <summary>
        /// It should jump if player is on second layer and going to ground or if player is on ground and going to second
        /// </summary>
        /// <param name="inputDirection"></param>
        /// <returns></returns>
        public bool ShouldJump(Vector2 inputDirection) {
            _lastDirection = inputDirection;
            _forwardCheck.localPosition = inputDirection * _detectionDistance;
            return IsInSecondButMovingToGround() || IsInGroundButMovingToSecond();
        }

        /// <summary>
        /// Check if player already on second layer, now check if going to normal
        /// </summary>
        private bool IsInSecondButMovingToGround() {
            return IsWalkableCell(_groundCheck.position, _secondGroundLayer) &&
               IsWalkableCell(_forwardCheck.position, _groundLayer);
        }

        private bool IsInGroundButMovingToSecond() {
            return IsWalkableCell(_groundCheck.position, _groundLayer) &&
                  IsWalkableCell(_forwardCheck.position, _secondGroundLayer);
        }

        private bool IsWalkableCell(Vector2 pointToCheck, LayerMask targetMask) {
            var hitOnEdge = Physics2D.OverlapCircle(pointToCheck, _detectionRadius, targetMask);
            return hitOnEdge != null;
        }

        private void OnDrawGizmos() {
            if (!_debug || _lastDirection == Vector2.zero) {
                return;
            }

            var endPoint = _groundCheck.position;
            var hitOnEdge = IsWalkableCell(endPoint, _groundLayer);

            Gizmos.color = hitOnEdge ? Color.cyan : Color.red;
            Gizmos.DrawWireSphere(endPoint, _detectionRadius);

            var endPoint2 = _forwardCheck.position;
            var hitOnEdge2 = IsWalkableCell(endPoint2, _secondGroundLayer);

            Gizmos.color = hitOnEdge2 ? Color.cyan : Color.red;
            Gizmos.DrawWireSphere(endPoint2, _detectionRadius);
            Gizmos.DrawLine(_forwardCheck.position, endPoint);
        }

        private void OnValidate() {
            if (_groundCheck == null) {
                _groundCheck = transform;
            }
            if (_forwardCheck == null) {
                _forwardCheck = transform;
            }
        }
    }
}