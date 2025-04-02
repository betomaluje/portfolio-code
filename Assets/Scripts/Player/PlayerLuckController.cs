using System;
using UnityEngine;

namespace Player {
    public class PlayerLuckController : MonoBehaviour {
        [SerializeField]
        [Range(0f, 1f)]
        private float _criticalHitChance = 0f;

        [SerializeField]
        [Min(0f)]
        private float _criticalHitMultiplier = 2f;

        public event Action<float> OnCriticalHitChanceChanged = delegate { };

        public void SetCriticalHitChance(float hitChance) {
            _criticalHitChance = hitChance;
            OnCriticalHitChanceChanged(_criticalHitChance);
        }

        public float GetCriticalHitChance() => _criticalHitChance;

        public void SetCriticalHitMultiplier(float criticalHitMultiplier) {
            _criticalHitMultiplier = criticalHitMultiplier;
        }

        public float GetCriticalHitMultiplier() => _criticalHitMultiplier;
    }
}