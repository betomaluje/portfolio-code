using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Weapons;
using Stats;

namespace Traps {
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class Goo : BaseBullet {
        [SerializeField]
        private float _speedReduceFactor = 0.2f;

        [SerializeField]
        private float _duration = .25f;

        [Header("Influence")]
        [SerializeField]
        private float _timeToLive = 5f;

        [Tooltip("Time to make the rigidbody static in millisecods. It will be randomized between this value and it's double.")]
        [SerializeField]
        private int _timeForStatic = 100;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [Header("Sizing")]
        [SerializeField]
        private Vector2 _scaleRange = new(1, 4);

        [SerializeField]
        private CapsuleCollider2D _capsuleCollider;

        private void Start() {
            var factor = Random.Range(_scaleRange.x, _scaleRange.y);
            var currentScale = _spriteRenderer.transform.localScale;
            _spriteRenderer.transform.localScale = new Vector3(currentScale.x * factor, currentScale.y * factor, 1);

            _capsuleCollider.size = _spriteRenderer.transform.localScale;

            _timeToLive = Mathf.Max(_duration, _timeToLive);

            _spriteRenderer.DOFade(0, _timeToLive).SetEase(Ease.InExpo).SetId(this).OnComplete(() => {
                Destroy(gameObject);
            });
        }

        private void OnDestroy() {
            DOTween.Kill(this);
        }

        protected override void OnValidate() {
            base.OnValidate();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
        }

        public override async void Fire(Vector2 direction) {
            if (_rb == null) {
                return;
            }

            _rb.AddForce(direction * _speed, ForceMode2D.Impulse);

            // we make it a little bit randomly
            var randomTime = Random.Range(_timeForStatic, 2 * _timeForStatic);
            await UniTask.Delay(randomTime);
            if (_rb == null) {
                return;
            }
            _rb.bodyType = RigidbodyType2D.Static;
        }

        private void OnTriggerStay2D(Collider2D other) {
            if (other.TryGetComponent(out IStatsModifier statsModifier)) {
                statsModifier.AddStatModifier(StatType.Speed, _speedReduceFactor, _duration);

                CheckCollision(other);
            }
        }
    }
}