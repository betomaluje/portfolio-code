using BerserkPixel.Health;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Extensions;
using Sprites;
using UnityEngine;

namespace Modifiers.Skills {
    /// <summary>
    /// A Powerup that is triggered when the target is hit. It spawns an object at the target's position when hitted.
    /// </summary>
    [CreateAssetMenu(menuName = "Aurora/Skills/On Hit Skill")]
    public class OnHitSkill : SkillConfig {
        [SerializeField]
        private Transform _spawnObject;

        [SerializeField]
        private float _destroyObjectTime = 5f;

        [Header("Animation FX")]
        [SerializeField]
        private float _jumpPower = 5f;
        [SerializeField]
        private float _distance = 1f;
        [SerializeField]
        private float _animDuration = 1f;

        private CharacterHealth _characterHealth;

        public override void Activate(Transform target) {
            base.Activate(target);
            if (CheckConditions() && target.TryGetComponent(out _characterHealth)) {
                _characterHealth.OnDamagePerformed += HandleHurt;
            }
        }

        private void HandleHurt(HitData hitData) {
            if (_spawnObject != null) {
                Vector3 direction = hitData.direction;

                var spawn = Instantiate(_spawnObject, _characterHealth.transform.position, Quaternion.identity);
                var target = _characterHealth.transform.position + -direction * _distance;
                spawn.DOJump(target, _jumpPower, 1, _animDuration)
                    .OnComplete(() => {
                        DestroyObject(spawn);
                    });
            }
        }

        private async void DestroyObject(Transform target) {
            await UniTask.Delay((int)_destroyObjectTime * 1000);
            if (target != null && target.gameObject != null && target.gameObject.FindInChildren<DissolveFX>(out var dissolveFx)) {
                dissolveFx.Dissolve();
            }
        }

        public override void Deactivate() {
            base.Deactivate();
            if (_characterHealth != null) {
                _characterHealth.OnDamagePerformed -= HandleHurt;
            }
        }
    }
}