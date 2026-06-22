using System.Collections.Generic;
using System.Linq;
using Base;
using Companions;
using UnityEngine;

namespace Weapons {
    public class CompanionWeaponManager : MonoBehaviour, IWeaponManager {
        [SerializeField]
        private List<Weapon> weapons = new();

        [SerializeField]
        private Transform spawnPoint;

        public Weapon Weapon => weapons[_currentWeaponIndex];

        public CharacterAnimations Animations => null;

        public int TotalWeapons => weapons.Count;

        private CompanionStateMachine _companionStateMachine;
        private int _currentWeaponIndex = 0;

        private void Awake() {
            _companionStateMachine = GetComponent<CompanionStateMachine>();
        }

        private void Start() => UpdateWeaponStats();

        private void OnValidate() {
            if (spawnPoint == null) {
                spawnPoint = transform;
            }
        }

        public void Attack() => Attack(transform.right);

        public void Attack(Vector2 direction, float chargePower = 1f) {
            if (weapons.Any()) {
                weapons[_currentWeaponIndex].Attack(_companionStateMachine.Animations, direction, spawnPoint.position);
            }
        }

        public bool CanAttack() => Weapon != null && !Weapon.IsCoolingDown();

        public void ChangeNextWeapon() {
            if (weapons.Any()) {
                _currentWeaponIndex = (_currentWeaponIndex + 1) % weapons.Count;
                UpdateWeaponStats();
            }
        }

        public void ChangePreviousWeapon() {
            if (weapons.Any()) {
                _currentWeaponIndex--;
                if (_currentWeaponIndex < 0) {
                    _currentWeaponIndex = weapons.Count - 1;
                }

                UpdateWeaponStats();
            }
        }

        public void ChangeWeapon(int index) {
            if (index < 0 || !weapons.Any() || index >= weapons.Count) {
                return;
            }

            if (index != _currentWeaponIndex) {
                _currentWeaponIndex = index;
                UpdateWeaponStats();
            }
        }

        public void ChangeWeaponByRange(float targetDistance) {
            if (weapons.Any() && weapons.Count > 1) {
                // is there a weapon in range?
                var weaponsInRange = weapons.Where(weapon => weapon.Range >= targetDistance).ToList();
                if (!weaponsInRange.Any()) {
                    // No weapon in range. 
                    // Select any of the Gun attacktype first
                    var anyGunWeapon = weapons.Where(weapon => weapon.AttackType == AttackType.Gun || weapon.AttackType == AttackType.Throwable).FirstOrDefault();
                    if (anyGunWeapon != null) {
                        var gunIndex = weapons.IndexOf(anyGunWeapon);
                        if (gunIndex != _currentWeaponIndex) {
                            _currentWeaponIndex = gunIndex;
                            UpdateWeaponStats();
                        }
                    }
                    else {
                        // if no Gun type, return Max range
                        var maxRangeWeaponIndex = weapons.FindIndex(weapon => weapon.Range == weapons.Max(weapon => weapon.Range));

                        if (maxRangeWeaponIndex != _currentWeaponIndex) {
                            // Weapon changed from {_currentWeaponIndex} to {maxRangeWeaponIndex}
                            _currentWeaponIndex = maxRangeWeaponIndex;
                            UpdateWeaponStats();
                        }
                    }
                }
                else {
                    // we need to choose between conditions like:
                    // damage, knockback force, cooldown time, etc
                    var random = Random.value;

                    var selectedIndex = _currentWeaponIndex;

                    if (random <= 0.33f) {
                        // cooldown time
                        var lessCooldownWeaponIndex = weapons.FindIndex(weapon => weapon.AttackCooldown == weapons.Min(weapon => weapon.AttackCooldown));
                        selectedIndex = lessCooldownWeaponIndex;
                    }
                    else if (random > 0.33f && random <= 0.66f) {
                        // damage
                        var maxDamageWeaponIndex = weapons.FindIndex(weapon => weapon.GetDamage() == weapons.Max(weapon => weapon.GetDamage()));
                        selectedIndex = maxDamageWeaponIndex;
                    }
                    else {
                        // knockback force
                        var maxKnockbackWeaponIndex = weapons.FindIndex(weapon => weapon.GetKnockback() == weapons.Max(weapon => weapon.GetKnockback()));
                        selectedIndex = maxKnockbackWeaponIndex;
                    }

                    if (selectedIndex != _currentWeaponIndex) {
                        // Weapon changed from {_currentWeaponIndex} to {selectedIndex}
                        _currentWeaponIndex = selectedIndex;
                        UpdateWeaponStats();
                    }
                }
            }
        }

        public void Clear() => weapons.Clear();

        public void OverrideWeapons(List<Weapon> weapons, int lastSelectedWeapon) { }

        public void Equip(Weapon weapon) { }

        public void EquipAll(IList<Weapon> weapons) { }

        private void UpdateWeaponStats() {
            if (_companionStateMachine == null) {
                return;
            }
            var weapon = weapons[_currentWeaponIndex];

            var size = Vector2.one;
            var offset = Vector2.zero;

            if (weapon is IWeaponCollider weaponCollider) {
                size = weaponCollider.AttackSize;
                offset = weaponCollider.AttackOffset;
            }
            var collider = _companionStateMachine.AttackCollider;
            collider.size = size;
            collider.offset = offset;
        }

        public void SetStrengthInfluence(float strength) => Weapon?.SetDamageInfluence(strength);

        public void ResetStrengthInfluence() => Weapon?.ResetStrengthInfluence();

        private void OnDestroy() {
            foreach (var weapon in weapons) {
                weapon.ResetWeapon();
            }
        }
    }
}