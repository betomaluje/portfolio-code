using System.Collections.Generic;
using Base;
using Modifiers.Skills;
using Sirenix.OdinInspector;
using UnityEngine;
using BerserkPixel.Utils.ServiceLocator;
using Sounds;

namespace Weapons {
    [InlineEditor]
    public abstract class Weapon : ScriptableObject {
        [Tooltip("The unique string identifier for this weapon, useful for localization.")]
        public string ID;

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

        public virtual int GetDamage() => Mathf.CeilToInt(Damage * _strengthInfluence);

        public virtual float GetKnockback() => _strengthInfluence != 1 ? (KnockbackForce + (KnockbackForce / 3f)) : KnockbackForce;

        public virtual bool ShouldMoveAttackCollider() => AttackType != AttackType.Gun && AttackType != AttackType.Fullscreen && AttackType != AttackType.Throwable;

        private float _strengthInfluence = 1;

        private void OnEnable() {
            _strengthInfluence = 1;
            _nextFireTime = 0;
            Modifiers = new();
        }

        protected virtual void OnValidate() {
            var type = GetType();
            var requiredBullet = (RequiredBulletAttribute)System.Attribute.GetCustomAttribute(type, typeof(RequiredBulletAttribute));
            
            if (requiredBullet != null) {
                // Check common field names: BulletPrefab (Shooting) or ObjectPrefab (Spawning)
                string[] fieldNames = { "BulletPrefab", "ObjectPrefab" };
                object prefabInstance = null;
                
                foreach (var fieldName in fieldNames) {
                    var field = type.GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy);
                    if (field != null) {
                        prefabInstance = field.GetValue(this);
                        break;
                    }
                }

                if (prefabInstance != null) {
                    GameObject bulletObj = null;
                    if (prefabInstance is GameObject go) bulletObj = go;
                    else if (prefabInstance is Component comp) bulletObj = comp.gameObject;

                    if (bulletObj != null && bulletObj.GetComponent(requiredBullet.BulletType) == null) {
                        Debug.LogWarning($"[{name}] Warning: Requires a Prefab containing a '{requiredBullet.BulletType.Name}' component.");
                    }
                }
            }
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

        protected void PlayImpactSound(Vector2 position, string soundName) {
            NonPersistentServiceLocator.Get<SoundManager>().PlayWithPitchOnSpot(soundName, position);
        }
    }

}