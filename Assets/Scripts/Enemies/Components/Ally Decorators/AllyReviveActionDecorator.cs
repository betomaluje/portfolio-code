using System;
using BerserkPixel.Health;
using UnityEngine;

namespace Enemies.Components {
    public class AllyReviveActionDecorator : MonoBehaviour, IAllyActionDecorator {
        [SerializeField]
        private EnemyStateMachine _skeletonPrefab;

        [SerializeField]
        private Transform _actionParticles;

        public Action<Transform, Transform> OnPerformed { get; set; }

        public bool ConditionsMet(Transform target, AllyActionsTypes allowedTypes) =>
            allowedTypes.HasFlags(AllyActionsTypes.Revive)
                && target.TryGetComponent<CharacterHealth>(out var health) && health.IsDead;

        public void DoAction(Transform actor, Transform target) {
            if (_actionParticles != null) {
                var particles = Instantiate(_actionParticles, actor.position, Quaternion.identity);
                Destroy(particles.gameObject, 2f);
            }

            // instantiate a skeleton prefab
            if (_skeletonPrefab != null) {
                Instantiate(_skeletonPrefab, actor.position, Quaternion.identity);
            }

            OnPerformed?.Invoke(actor, target);

            // destroy gameObject
            Destroy(target.gameObject);

            // we add it to the spawn list
            var enemySwarm = FindFirstObjectByType<EnemySwarm>();
            if (enemySwarm != null) {
                enemySwarm.AddEnemy(actor);
            }
        }
    }
}