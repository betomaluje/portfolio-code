using System;
using BerserkPixel.Health;
using UnityEngine;

namespace Enemies.Components {
    public class AllyHealthActionDecorator : MonoBehaviour, IAllyActionDecorator {
        [SerializeField]
        private int _healthToGive = 10;

        [SerializeField]
        private Transform _actionParticles;

        public Action<Transform, Transform> OnPerformed { get; set; }

        private IHealth _health;

        public bool ConditionsMet(Transform target, AllyActionsTypes allowedTypes) =>
        allowedTypes.HasFlag(AllyActionsTypes.Heal) &&
            target.TryGetComponent(out _health) &&
            _health.CanGiveHealth();

        public void DoAction(Transform actor, Transform target) {
            _health.GiveHealth(_healthToGive);

            if (_actionParticles != null) {
                Instantiate(_actionParticles, target.position, Quaternion.identity);
            }

            OnPerformed?.Invoke(actor, target);
        }
    }
}