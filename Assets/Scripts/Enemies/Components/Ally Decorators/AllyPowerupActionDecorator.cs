using System;
using BerserkPixel.Health;
using Extensions;
using Modifiers.Powerups;
using UnityEngine;

namespace Enemies.Components {
    public class AllyPowerupActionDecorator : MonoBehaviour, IAllyActionDecorator {
        [SerializeField]
        private PowerupConfig[] _powerupConfigs;

        [SerializeField]
        private Color _newColor = default;

        [SerializeField]
        private Transform _actionParticles;

        [SerializeField]
        private Vector3 _particlesRotation;

        public Action<Transform, Transform> OnPerformed { get; set; }

        private CharacterPowerup _characterPowerup;

        public bool ConditionsMet(Transform target, AllyActionsTypes allowedTypes) =>
            allowedTypes.HasFlags(AllyActionsTypes.Powerup) && target.TryGetComponent(out _characterPowerup) &&
                _powerupConfigs != null && _powerupConfigs.Length > 0 &&
                target.TryGetComponent<CharacterHealth>(out var health) &&
                !health.IsDead;

        public void DoAction(Transform actor, Transform target) {
            foreach (var powerupConfig in _powerupConfigs) {
                _characterPowerup.DoPowerup(powerupConfig, target);
            }

            if (_newColor != default) {
                var renderers = target.GetComponentsInChildren<SpriteRenderer>();
                renderers?.Tint(_newColor);
            }

            OnPerformed?.Invoke(actor, target);

            if (_actionParticles != null) {
                var particles = Instantiate(_actionParticles, target.position, Quaternion.Euler(_particlesRotation));
                Destroy(particles.gameObject, 2f);
            }
        }
    }
}