using DG.Tweening;
using Enemies;
using UnityEngine;

namespace Utils {
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(EnemyStateMachine))]
    public class JumpOnAppear : MonoBehaviour {
        [Tooltip("The min and max distance from emitter of this bullet.")]
        [SerializeField]
        private Vector2 _distance = new(.5f, 2.5f);

        [Tooltip("The jump force of this bullet.")]
        [SerializeField]
        private float _jumpForce = 10f;

        [Tooltip("The how fast this jump will be.")]
        [SerializeField]
        private float _jumpDuration = .25f;

        private Rigidbody2D _rb;
        private Collider2D _collider;

        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();

            _collider.enabled = false;
        }

        private void Start() {
            if (_rb == null) {
                return;
            }

            Vector2 direction = Random.insideUnitCircle.normalized;

            var extraDistance = direction * Random.Range(_distance.x, _distance.y);

            var enemyStateMachine = GetComponent<EnemyStateMachine>();

            _rb.DOJump((Vector2)transform.position + extraDistance, jumpPower: _jumpForce, numJumps: 1, duration: _jumpDuration)
            .OnStart(() => {
                _collider.enabled = false;
                enemyStateMachine.enabled = false;
            })
            .OnComplete(() => {
                _collider.enabled = true;
                enemyStateMachine.enabled = true;
            });
        }
    }
}