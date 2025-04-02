using BerserkPixel.Health;
using BerserkPixel.Health.FX;
using DG.Tweening;
using UI;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(PlayerStateMachine))]
    public class PlayerHitManager : MonoBehaviour {
        [SerializeField]
        private DamageHitUI _damagePrefab;

        [SerializeField]
        private float _damageYOffset = 1.5f;

        [SerializeField]
        private float _recoilForce = 100f;

        private KnockbackFX _knockbackFX;

        private PlayerStateMachine _stateMachine;

        private void Awake() {
            _stateMachine = GetComponent<PlayerStateMachine>();
            _knockbackFX = GetComponentInChildren<KnockbackFX>();
        }

        private void OnEnable() {
            _stateMachine.OnHit += HandleHit;
        }

        private void OnDisable() {
            _stateMachine.OnHit -= HandleHit;
        }

        private void HandleHit(HitData data) {
            if (data.isCritical && data.damage > 0) {
                var finishPosition = data.victim.position + Vector3.up * _damageYOffset;
                var damageUI = Instantiate(_damagePrefab, data.victim.position, Quaternion.identity);
                damageUI.transform.DOMoveY(finishPosition.y, 0.2f);
                damageUI.SetDamageText(data);
            }

            // do some recoil
            _knockbackFX.DoFX(_recoilForce, -data.direction);
        }
    }
}