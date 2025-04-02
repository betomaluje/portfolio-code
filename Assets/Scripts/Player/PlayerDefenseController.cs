using UnityEngine;

namespace Player {
    public class PlayerDefenseController : MonoBehaviour {
        private float _reducedDamage = 0f;
        public float ReducedDamageMultiplier => _reducedDamage;

        public void SetReduceDamageInfluence(float amount) {
            if (amount == _reducedDamage) {
                return;
            }

            _reducedDamage = Mathf.Clamp01(amount);
        }

        public void ResetReduceDamageInfluence() {
            _reducedDamage = 0f;
        }
    }
}