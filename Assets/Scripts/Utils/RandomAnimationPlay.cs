using UnityEngine;

namespace Utils {
    [RequireComponent(typeof(Animator))]
    public class RandomAnimationPlay : MonoBehaviour {
        private Animator _animator;

        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        private void Start() {
            var state = _animator.GetCurrentAnimatorStateInfo(0);
            _animator.Play(state.fullPathHash, 0, Random.value);
        }
    }
}