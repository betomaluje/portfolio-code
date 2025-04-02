using System.Collections.Generic;
using Base;
using Modifiers.Skills;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons {
    [InlineEditor]
    public abstract class Weapon : ScriptableObject {
        [Tooltip("The name of this weapon.")]
        public string Name;

        [Tooltip("The damage dealt by this weapon.")]
        [SerializeField]
        [Min(0)]
        private int Damage;

        [Tooltip("The force to push the target when attacking")]
        [Min(0f)]
        public float KnockbackForce;

        [Tooltip("The cooldown between attacks.")]
        [Min(0f)]
        public float AttackCooldown;

        [Tooltip("The animation to play when attacking.")]
        public string AttackAnimation = "Attack";

        [Tooltip("The name of the prefab to enable for the weapon animation")]
        public string PrefabNameToEnable = "";

        [Tooltip("What type of animation should be for this type of weapon")]
        public AttackType AttackType = AttackType.Sword;

        [Tooltip("The range that this weapon has")]
        [Min(0f)]
        public float Range = 1.0f;

        [Tooltip("The size of the weapon mask to change o the player's SpriteMask")]
        [Min(0f)]
        public float MaskWeaponSize = 1.8f; // 3.2 - 1.8

        public List<WeaponModifier> Modifiers { get; private set; }

        protected float _nextFireTime;

        public bool IsCoolingDown() => Time.time < _nextFireTime;
        protected virtual void StartCooldown() => _nextFireTime = Time.time + AttackCooldown;

        public abstract void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position);

        public int GetDamage() => Mathf.CeilToInt(Damage * _strengthInfluence);

        public float GetKnockback() => _strengthInfluence != 1 ? (KnockbackForce + (KnockbackForce / 3f)) : KnockbackForce;

        private float _strengthInfluence = 1;

        private void OnEnable() {
            _strengthInfluence = 1;
            _nextFireTime = 0;
            Modifiers = new();
        }

        public virtual void SetDamageInfluence(float strength) {
            _strengthInfluence = strength;
        }

        public virtual void ResetStrengthInfluence() {
            _strengthInfluence = 1;
        }

        public void EquipModifiers(WeaponModifier[] configs) {
            Modifiers ??= new();
            Modifiers.AddRange(configs);
        }

        public void ResetWeapon() {
            _strengthInfluence = 1;
            Modifiers = null;
            _nextFireTime = 0;
        }
    }

}