using System.Collections.Generic;
using System.Linq;
using BerserkPixel.Health;
using Sirenix.OdinInspector;
using Stats;
using UnityEngine;
using Weapons;

namespace Modifiers.Skills {
    [CreateAssetMenu(menuName = "Aurora/Skills/Detect And Spawn Skill")]
    public class DetectAndSpawnSkill : SkillConfig {
        [BoxGroup("Detection")]
        [SerializeField]
        private LayerMask _targetMask;

        [BoxGroup("Detection")]
        [SerializeField]
        private float _detectionRadius = 0.5f;

        [SerializeField]
        private Transform _spawnPrefab;

        [SerializeField]
        private StatType _statType = StatType.Speed;

        [SerializeField]
        [Range(0f, 1f)]
        private float _statValue = .8f;

        private Transform _owner;
        private PlayerStatsManager _statsManager;
        private readonly List<Transform> _spawnedObjects = new();

        public override void Setup(Transform owner) {
            base.Setup(owner);
            _owner = owner;
            _statsManager = owner.GetComponent<PlayerStatsManager>();
        }

        public override void Activate(Transform target) {
            base.Activate(target);
            _statsManager.AddStatModifier(_statType, _statValue);

            var points = DetectEnemies();

            _spawnedObjects.Clear();

            foreach (var point in points) {
                var instance = Instantiate(_spawnPrefab, _owner.position, Quaternion.identity);
                instance.SetParent(_owner);
                _spawnedObjects.Add(instance);

                if (instance.TryGetComponent<Tentacle>(out var tentacle)) {
                    var hitData = new HitDataBuilder()
                        .WithDamage((int)EndValue)
                        .WithDirection(Vector2.zero)
                        .Build(_owner, point);
                    tentacle.Setup(hitData, OnObjectDestroyed);
                }
            }
        }

        private void OnObjectDestroyed(GameObject obj) {
            if (_spawnedObjects.Contains(obj.transform)) {
                _spawnedObjects.Remove(obj.transform);
            }
        }

        public override void Deactivate() {
            base.Deactivate();
            _statsManager.ResetStatModifier(_statType);
            ClearList();
        }

        private void ClearList() {
            foreach (var obj in _spawnedObjects) {
                Destroy(obj.gameObject);
            }
            _spawnedObjects.Clear();
        }

        public override void Cleanup() {
            base.Cleanup();
            ClearList();
        }

        private List<Transform> DetectEnemies() => Physics2D.OverlapCircleAll(_owner.position, _detectionRadius, _targetMask)
                                                    .Where(x => x.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead)
                                                    .Select(x => x.transform)
                                                    .ToList();
    }
}