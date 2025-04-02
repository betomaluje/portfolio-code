using Camera;
using UnityEngine;
using Weapons;

namespace Bomb {
    public class Bomb : BaseBullet {
        [SerializeField, Min(0)]
        private float _radius;

        [SerializeField, Min(0)]
        private float _timeToExplode;

        [SerializeField]
        private Animator _animator;

        private bool _started = false;

        private readonly int anim_explode = Animator.StringToHash("Bomb_Explode");

        private void Update() {
            if (!_started) return;

            // after the time is up, explode the bomb
            if (_timeToExplode <= 0) {
                Explode();
            }
            else {
                _timeToExplode -= Time.deltaTime;
            }
        }

        public override void Fire(Vector2 direction) {
            _started = true;

            base.Fire(direction);
        }

        private void Explode() {
            _animator.Play(anim_explode);
            CinemachineCameraShake.Instance.ShakeCameraWithIntensity(transform, .5f);
        }

        // Called from Animation
        private void Detect() {
            var colliders = Physics2D.OverlapCircleAll(transform.position, _radius, _targetMask);
            foreach (var collider in colliders) {
                CheckCollision(collider);
            }
        }

        // Called from Animation
        private void RemoveInHierarchy() {
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}