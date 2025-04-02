using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace BerserkPixel.Health {
    public class ForceReceiver : MonoBehaviour {
        [SerializeField]
        private Rigidbody2D _rb;

        [SerializeField]
        private float delay = .15f;

        public UnityEvent OnBegin;
        public UnityEvent OnDone;

        private float _originalDrag;

        private void OnValidate() {
            if (_rb == null){
                _rb = GetComponentInParent<Rigidbody2D>();
            }
        }

        private void Start() {
            _originalDrag = _rb.linearDamping;
        }

        private IEnumerator ResetVelocity() {
            yield return new WaitForSeconds(delay);
            _rb.linearVelocity = Vector2.zero;
            _rb.linearDamping = _originalDrag;
            OnDone?.Invoke();
        }

        public void Knockback(Vector2 dir, float force) {
            OnBegin?.Invoke();
            StopAllCoroutines();
            _rb.linearDamping = 0f;
            _rb.AddForce(dir * force, ForceMode2D.Impulse);
            StartCoroutine(ResetVelocity());
        }
    }
}