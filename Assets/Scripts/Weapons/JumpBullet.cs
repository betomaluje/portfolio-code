using DG.Tweening;
using Extensions;
using UnityEngine;

namespace Weapons {
    public class JumpBullet : BaseBullet {
        [Tooltip("The min and max distance from emitter of this bullet.")]
        [SerializeField]
        private Vector2 _distance = new(.5f, 2.5f);

        [Tooltip("The jump force of this bullet.")]
        [SerializeField]
        private float _jumpForce = 10f;

        [Tooltip("The how fast this jump will be.")]
        [SerializeField]
        private float _jumpDuration = .25f;

        [SerializeField]
        private float _lifetime = 1f;

        [SerializeField]
        private Collider2D _collider;

        protected override void OnValidate() {
            base.OnValidate();
            _collider = GetComponent<Collider2D>();
        }

        private void Awake() {
            _collider.enabled = false;
        }

        public override void Fire(Vector2 direction) {
            if (_rb == null) {
                return;
            }

            transform.RotateTo(direction);

            var extraDistance = direction * Random.Range(_distance.x, _distance.y);

            _rb.DOJump((Vector2)transform.position + extraDistance, _jumpForce, 1, _jumpDuration).OnComplete(() => {
                _collider.enabled = true;
                Destroy(gameObject, _lifetime);
            });
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckCollision(other)) {
                Destroy(gameObject);
            }
        }
    }
}