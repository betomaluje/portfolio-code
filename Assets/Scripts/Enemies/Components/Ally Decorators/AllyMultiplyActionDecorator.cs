using System;
using BerserkPixel.Health;
using UnityEngine;

namespace Enemies.Components {
    public class AllyMultiplyActionDecorator : MonoBehaviour, IAllyActionDecorator {
        [SerializeField]
        [Min(1f)]
        private int _amountToMultiply = 2;

        [SerializeField]
        private float _distanceFromOrigin = 1.5f;

        [SerializeField]
        private Transform _actionParticles;

        public Action<Transform, Transform> OnPerformed { get; set; }

        public bool ConditionsMet(Transform target, AllyActionsTypes allowedTypes) =>
            allowedTypes.HasFlag(AllyActionsTypes.Multiply) &&
            target.TryGetComponent<CharacterHealth>(out var health) &&
            !health.IsDead;

        public void DoAction(Transform actor, Transform target) {
            if (_actionParticles != null) {
                Instantiate(_actionParticles, target.position, Quaternion.identity);
            }

            var enemySwarm = FindFirstObjectByType<EnemySwarm>();

            for (int i = 0; i < _amountToMultiply; i++) {
                var position = RandomUtils.NextDirection3D() * _distanceFromOrigin + target.position;
                var enemy = Instantiate(target, position, Quaternion.identity);
                if (enemySwarm != null) {
                    enemySwarm.AddEnemy(enemy);
                }
            }

            OnPerformed?.Invoke(actor, target);
        }
    }
}