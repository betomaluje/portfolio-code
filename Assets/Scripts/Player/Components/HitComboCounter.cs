using System;
using UnityEngine;

namespace Player.Components {
    public class HitComboCounter {
        // Time window to consider hits as part of the combo
        public float ComboTimeWindow { get; set; } = 1.5f;
        public int ComboHitsRequired { get; set; } = 3; // Number of hits required for the combo

        public int CurrentHits => _hitCount;

        // Event triggered when the combo stops counting
        public event Action<int> OnHitAdded;
        public event Action OnHitReset;
        // Event triggered when a certain amount of combos is completed
        public event Action CombosCompleted;

        public int TargetsHit => _targetsHit;
        private int _targetsHit;

        // Time of the last successful hit
        private float _lastHitTime;
        // Number of consecutive hits
        private int _hitCount;

        public HitComboCounter() {
            _lastHitTime = -ComboTimeWindow;
            _hitCount = 0;
            _targetsHit = 0;
        }

        public void Tick() {
            if (_hitCount > 0 && Time.time - _lastHitTime > ComboTimeWindow) {
                _hitCount = 0;
                _targetsHit = 0;
                // DebugTools.DebugLog.Log($"Reset Hit combo count");
                OnHitReset?.Invoke();
            }
        }

        public void Hit(int targetsCount) {
            _targetsHit = targetsCount;
            float currentTime = Time.time;
            if (currentTime - _lastHitTime <= ComboTimeWindow) {
                // If within the combo time window
                _hitCount++;

                OnHitAdded?.Invoke(_hitCount);

                if (_hitCount >= ComboHitsRequired) {
                    // DebugLog.Log("Combo achieved!");
                    CombosCompleted?.Invoke();
                }
            }
            else {
                // If outside the combo time window, reset hit count
                if (_hitCount > 0) {
                    _hitCount = 0;
                    OnHitReset?.Invoke();
                }
                _hitCount = 1;
            }

            _lastHitTime = currentTime;
        }
    }
}