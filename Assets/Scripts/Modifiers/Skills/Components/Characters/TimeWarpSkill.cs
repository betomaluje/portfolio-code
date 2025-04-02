using System.Collections.Generic;
using System.Linq;
using Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Skills {
    [CreateAssetMenu(menuName = "Aurora/Skills/Time Warp Skill")]
    public class TimeWarpSkill : SkillConfig {
        [BoxGroup("Detection")]
        [SerializeField]
        private LayerMask _targetMask;

        [BoxGroup("Detection")]
        [SerializeField]
        private float _detectionRadius = 0.5f;

        [BoxGroup("Power")]
        [SerializeField]
        private float _attackPower = 2f;

        [BoxGroup("Power")]
        [SerializeField]
        private float _speedPower = 1.8f;

        private Transform _owner;
        private PlayerStatsManager _statsManager;
        private List<EnemyStatsManager> _detectedEnemies;

        public override void Setup(Transform owner) {
            base.Setup(owner);
            _owner = owner;
            _statsManager = owner.GetComponent<PlayerStatsManager>();
            _detectedEnemies = new();
        }

        public override void Activate(Transform target) {
            base.Activate(target);
            _statsManager.AddAttack(_attackPower);
            _statsManager.AddSpeed(_speedPower);

            DetectEnemies();
        }

        public override void Deactivate() {
            base.Deactivate();
            _statsManager.ResetAttack();
            _statsManager.ResetSpeed();

            foreach (var enemy in _detectedEnemies) {
                enemy.ResetSpeed();
            }

            _detectedEnemies.Clear();
        }

        private void DetectEnemies() {
            var hits = Physics2D.OverlapCircleAll(_owner.position, _detectionRadius, _targetMask);
            _detectedEnemies.Clear();

            if (hits.Length == 0) {
                return;
            }

            _detectedEnemies = hits.Select(h => h.GetComponent<EnemyStatsManager>()).Where(stats => stats != null).ToList();

            foreach (var enemy in _detectedEnemies) {
                enemy.AddSpeed(EndValue);
            }
        }
    }
}