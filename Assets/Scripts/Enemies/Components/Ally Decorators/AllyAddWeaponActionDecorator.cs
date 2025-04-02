using System;
using BerserkPixel.Health;
using UnityEngine;
using Weapons;

namespace Enemies.Components {
    public class AllyAddWeaponActionDecorator : MonoBehaviour, IAllyActionDecorator {
        [SerializeField]
        private Weapon _weapon;

        [SerializeField]
        private Transform _actionParticles;

        public Action<Transform, Transform> OnPerformed { get; set; }

        private EnemyWeaponManager _allyWeaponManager;

        public bool ConditionsMet(Transform target, AllyActionsTypes allowedTypes) =>
        allowedTypes.HasFlag(AllyActionsTypes.AddWeapon) &&
            _weapon != null &&
            target.TryGetComponent(out _allyWeaponManager) &&
            target.TryGetComponent<CharacterHealth>(out var health) &&
            !health.IsDead;

        public void DoAction(Transform actor, Transform target) {
            if (_actionParticles != null) {
                Instantiate(_actionParticles, target.position, Quaternion.identity);
            }

            _allyWeaponManager.Equip(_weapon);

            OnPerformed?.Invoke(actor, target);
        }
    }
}