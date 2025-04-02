using System;
using Enemies;
using UnityEngine;

namespace Player {
    public class PlayerExperienceController : MonoBehaviour {
        [SerializeField]
        private int _currentLevel = 1;

        [SerializeField]
        private int _currentExperience = 0;

        [SerializeField]
        private int _experienceToNextLevel = 300;

        [SerializeField]
        // Add 20% more experience needed to level up each time
        private float _experienceGrowthMultiplier = 1.2f;

        public event Action<int> OnExperienceGained = delegate { };
        public event Action<int> OnLevelUp = delegate { };

        private void OnEnable() {
            EnemyStateMachine.OnEnemyDefeated += GainExperience;
        }

        private void OnDisable() {
            EnemyStateMachine.OnEnemyDefeated -= GainExperience;
        }

        private void OnDestroy() {
            EnemyStateMachine.OnEnemyDefeated -= GainExperience;

        }

        public void SetupLevel(int level) {
            _currentLevel = level;
        }

        public void SetupExperience(int experience) {
            _currentExperience = experience;
        }

        public void GainExperience(int amount) {
            _currentExperience += amount;
            if (_currentExperience >= _experienceToNextLevel) {
                LevelUp();
                OnLevelUp?.Invoke(_currentLevel);
            }
            OnExperienceGained?.Invoke(_currentExperience);
        }

        private void LevelUp() {
            _currentLevel++;
            _currentExperience -= _experienceToNextLevel;
            _experienceToNextLevel = Mathf.RoundToInt(_experienceToNextLevel * _experienceGrowthMultiplier);
        }
    }
}