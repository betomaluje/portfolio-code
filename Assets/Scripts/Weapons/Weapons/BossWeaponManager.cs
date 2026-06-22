using System.Collections.Generic;
using System.Linq;
using Base;
using Enemies;
using UnityEngine;

namespace Weapons {
    [RequireComponent(typeof(EnemyStateMachine))]
    public class BossWeaponManager : MonoBehaviour, IWeaponManager {
        [SerializeField]
        private AnimationConfig _bossSpecialAttacks;

        [SerializeField]
        private List<Weapon> weapons = new();

        [SerializeField]
        private Transform spawnPoint;

        private int _currentWeaponIndex = 0;
        private CharacterAnimations _animations;

        private Vector2 _direction;

        private EnemyStateMachine _enemyStateMachine;

        public Weapon Weapon => weapons[_currentWeaponIndex];

        public int TotalWeapons => weapons.Count;

        public bool CanAttack() => Weapon != null && !Weapon.IsCoolingDown();

        public CharacterAnimations Animations => _animations;

        private void Start() {
            _enemyStateMachine = GetComponent<EnemyStateMachine>();

            if (_bossSpecialAttacks == null) {
                var animationHolder = GetComponent<ICharacterHolder>();
                _animations = animationHolder.Animations;
            }
            else {
                Animator animator = GetComponent<Animator>();
                _animations = new CharacterAnimations(animator, _bossSpecialAttacks);
            }

            UpdateWeaponStats();
        }

        private void OnValidate() {
            if (spawnPoint == null) {
                spawnPoint = transform;
            }
        }

        public void Attack() => Attack(_direction);

        public void Attack(Vector2 direction, float chargePower = 1f) {
            if (weapons.Any()) {
                Weapon.Attack(_animations, direction, spawnPoint.position);
            }
        }

        public void ChangeWeapon(int index) {
            if (index < 0 || !weapons.Any() || weapons.Count <= 1 || index >= weapons.Count) {
                return;
            }

            if (index != _currentWeaponIndex) {
                _currentWeaponIndex = index;
                UpdateWeaponStats();
            }
        }

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

        public void ChangeWeaponByRange(float targetDistance) {
            if (weapons.Any() && weapons.Count > 1) {
                const float rangeBuffer = 0.05f;

                // Step 1: Try to find weapons that can reach the target
                var weaponsInRange = weapons
                    .Select((w, index) => new { Weapon = w, Index = index })
                    .Where(w => w.Weapon.Range + rangeBuffer >= targetDistance)
                    .OrderBy(w => w.Weapon.Range); // longest viable range first                    

                if (weaponsInRange.Any()) {
                    // Choose the weapon with the shortest range that can reach the target
                    var bestFit = weaponsInRange.SimpleShuffle().First();
                    if (_currentWeaponIndex != bestFit.Index) {
                        _currentWeaponIndex = bestFit.Index;
                        UpdateWeaponStats();
                    }
                }
                else {
                    // Step 2: No weapon in range — fallback to random-based selection
                    int selectedIndex = _currentWeaponIndex;
                    
                    var (selector, descending) = GetWeaponOrder();
                    var query = weapons.Select((w, i) => new { Weapon = w, Index = i });
                    selectedIndex = descending
                        ? query.OrderByDescending(w => selector(w.Weapon)).First().Index
                        : query.OrderBy(w => selector(w.Weapon)).First().Index;

                    if (_currentWeaponIndex != selectedIndex) {
                        _currentWeaponIndex = selectedIndex;
                        UpdateWeaponStats();
                    }
                }
            }
        }

        private (System.Func<Weapon, float> selector, bool descending) GetWeaponOrder() {
            float random = Random.value;
            if (random <= 0.33f) {
                // Cooldown (ascending - lower is better)
                return (w => w.AttackCooldown, false);
            }
            else if (random <= 0.66f) {
                // Damage (descending - higher is better)
                return (w => w.GetDamage(), true);
            }
            else {
                // Knockback (descending - higher is better)
                return (w => w.GetKnockback(), true);
            }
        }

        private void UpdateWeaponStats() {
            if (_enemyStateMachine == null) {
                return;
            }
            var weapon = weapons[_currentWeaponIndex];

            if (weapon is BaseShootingWeapon shootingWeapon) {
                shootingWeapon.SetShootingPoint(spawnPoint);
            }

            var size = Vector2.one;
            var offset = Vector2.zero;

            if (weapon is IWeaponCollider weaponCollider) {
                size = weaponCollider.AttackSize;
                offset = weaponCollider.AttackOffset;
            }
            var collider = _enemyStateMachine.AttackCollider;
            collider.size = size;
            collider.offset = offset;
        }

        public void OverrideWeapons(List<Weapon> newWeapons, int lastSelectedWeapon) {
            weapons = newWeapons;
            _currentWeaponIndex = lastSelectedWeapon;
            UpdateWeaponStats();
        }

        public void Equip(Weapon weapon) {
            if (weapons.Contains(weapon)) {
                return;
            }

            weapons.Add(weapon);
        }

        public void EquipAll(IList<Weapon> newWeapons) {
            if (newWeapons == null || newWeapons.Count == 0) {
                return;
            }

            foreach (var weapon in newWeapons) {
                Equip(weapon);
            }

            _currentWeaponIndex = 0;
            UpdateWeaponStats();
        }

        public void Clear() => weapons.Clear();

        public void SetStrengthInfluence(float strength) => Weapon?.SetDamageInfluence(strength);

        public void ResetStrengthInfluence() => Weapon?.ResetStrengthInfluence();

        private void OnDestroy() {
            foreach (var weapon in weapons) {
                weapon.ResetWeapon();
            }
        }
    }
}