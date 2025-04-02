using System.Collections.Generic;
using BerserkPixel.Health;
using Extensions;
using UnityEngine;

namespace Modifiers.Skills {
    [CreateAssetMenu(menuName = "Aurora/Skills/Immunity Skill")]
    public class ImmunitySkill : SkillConfig {
        [SerializeField]
        private Material _immunityMaterial;

        private CharacterHealth _characterHealth;
        private CircleCollider2D _characterCollider;

        private Dictionary<SpriteRenderer, Material> _renderers;

        private bool _conditionsOnActivate;

        override public void Setup(Transform owner) {
            base.Setup(owner);
            _renderers = new Dictionary<SpriteRenderer, Material>();
            if (owner.gameObject.FindAllInChildren<SpriteRenderer>(out var renderers)) {
                foreach (var renderer in renderers) {
                    _renderers.Add(renderer, renderer.material);
                }
            }
        }

        public override void Activate(Transform target) {
            base.Activate(target);
            if (_conditionsOnActivate = CheckConditions()) {
                if (target.TryGetComponent(out _characterHealth)) {
                    _characterHealth.SetImmune();
                }

                if (target.TryGetComponent(out _characterCollider)) {
                    _characterCollider.enabled = false;
                }

                foreach (var kvp in _renderers) {
                    kvp.Key.material = _immunityMaterial;
                }
            }
        }

        public override void Deactivate() {
            base.Deactivate();
            if (_conditionsOnActivate) {
                if (_characterHealth != null) {
                    _characterHealth.ResetImmune();
                }

                if (_characterCollider != null) {
                    _characterCollider.enabled = true;
                }

                foreach (var kvp in _renderers) {
                    kvp.Key.material = kvp.Value;
                }
            }
        }
    }
}