using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Level.Props {
    public class Bird : MonoBehaviour {
        private static readonly int Fly = Animator.StringToHash("Fly");
        private static readonly int Eat = Animator.StringToHash("Eat");

        [SerializeField]
        private GameObject flyParticles;

        [SerializeField]
        private Transform childSprite;

        [SerializeField]
        private float deltaToMove = 20f;

        [SerializeField]
        private float animTime = 4f;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private float maxEatTime;

        private UnityEngine.Camera cam;
        private float _elapsedTime;
        private bool _isEating, _hasFled;

        private void Awake() => cam = UnityEngine.Camera.main;

        private void Update() {
            if (_elapsedTime <= 0 && !_isEating)
                // eat and start waiting for random time
                StartCoroutine(DoEat());

            _elapsedTime -= Time.deltaTime;
        }

        private IEnumerator DoEat() {
            _isEating = true;
            animator.SetTrigger(Eat);
            yield return new WaitForSeconds(2f);
            _elapsedTime = Random.Range(0, maxEatTime);
            _isEating = false;
        }

        // Called from child TragetDetection script
        public void HandleTargetDetected(Transform target) {
            if (flyParticles != null)
                Instantiate(flyParticles, transform.position, Quaternion.identity);

            animator.SetTrigger(Fly);
            if (!_hasFled) {
                _hasFled = true;
                FlyOffScreen(target.position);
            }
        }

        private void FlyOffScreen(Vector2 playerPos) {
            var flip = playerPos.x > transform.position.x ? -1 : 1;

            Vector2 newPos = cam.ScreenToWorldPoint(
                new Vector2(Random.Range(0, Screen.width), Random.Range(0, Screen.height))
            );
            newPos.x += deltaToMove * flip;
            newPos.y += deltaToMove;

            var transformLocalScale = childSprite.localScale;
            transformLocalScale.x = flip;
            childSprite.localScale = transformLocalScale;
            if (transform)
                transform.DOMove(newPos, animTime).SetEase(Ease.InOutSine).OnComplete(() => Destroy(gameObject));
        }
    }
}