using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using BerserkTools.Health.UI;
using Extensions;
using Modifiers.Skills;
using Player;
using Player.Components;
using Player.Input;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons {
    public class WeaponManager : MonoBehaviour, IWeaponManager {
        [SerializeField]
        private List<Weapon> weapons = new();

        [SerializeField]
        private Transform _weaponHolder;

        [SerializeField]
        private Transform _spawnPoint;

        [Header("Animations")]
        [SerializeField]
        private Animator _weaponAnimator;

        [SerializeField]
        protected AnimationConfig _weaponAnimationConfig;

        [SerializeField]
        private Transform _aimVisuals;

        public PlayerAim PlayerAim { get; private set; }
        public Weapon Weapon => weapons[_currentWeaponIndex];
        public int TotalWeapons => weapons.Count;
        public event Action<int> OnWeaponChanged = delegate { };

        public bool IsCharging => Weapon.AttackType == AttackType.Throwable;

        public List<string> WeaponNames => weapons.Select(weapon => weapon.Name).ToList();

        public bool CanAttack() => Weapon != null && !Weapon.IsCoolingDown();

        public CharacterAnimations Animations => _weaponAnimations;

        private CharacterAnimations _weaponAnimations;
        private PlayerStateMachine _playerStateMachine;
        private PlayerBattleInput _playerBattleInput;
        private int _currentWeaponIndex = 0;
        private GameObject _weaponChild;
        private bool _autoAssist;
        private Dictionary<string, Transform> _weaponTransforms = new();

        private void Awake() {
            _playerBattleInput = GetComponent<PlayerBattleInput>();
            _playerStateMachine = GetComponent<PlayerStateMachine>();
            _weaponAnimations = new CharacterAnimations(_weaponAnimator, _weaponAnimationConfig);

            var preferencesStorage = new PreferencesStorage();
            _autoAssist = preferencesStorage.GetAimAssist();

            PlayerAim = new PlayerAim(Attack, _playerBattleInput, _spawnPoint, _aimVisuals, _autoAssist);
            PlayerAim.SetChargeBar(GetComponentInChildren<ProgressbarBehaviour>());
        }

        private void OnValidate() {
            if (_spawnPoint == null) {
                _spawnPoint = transform;
            }
        }

        private void OnEnable() {
            _playerBattleInput.NextWeaponEvent += ChangeNextWeapon;
            _playerBattleInput.PreviousWeaponEvent += ChangePreviousWeapon;
        }

        private void OnDisable() {
            _playerBattleInput.NextWeaponEvent -= ChangeNextWeapon;
            _playerBattleInput.PreviousWeaponEvent -= ChangePreviousWeapon;
        }

        private void OnDestroy() {
            foreach (var weapon in weapons) {
                weapon.ResetWeapon();
            }
            PlayerAim.Dispose();
        }

        private void Update() {
            var lastInput = _playerStateMachine.PlayerAimDirection;

            if (_autoAssist) {
                var closest = PlayerAim.GetClosestEnemyDirection();
                if (closest != Vector2.zero) {
                    lastInput = closest;
                }
            }

            // we update the rotation of the weapon
            PlayerAim.Tick(Time.deltaTime, lastInput);

            if (PlayerAim.IsCurrentlyAttacking) {
                _playerStateMachine.Movement.FlipSprite(lastInput);
            }
        }

        public void OverrideWeapons(List<Weapon> weapons, int lastSelectedWeapon) {
            this.weapons = weapons;
            _currentWeaponIndex = lastSelectedWeapon;

            foreach (var weapon in weapons) {
                string prefabNameToEnable = weapon.PrefabNameToEnable;
                Transform weaponTransform;

                if (prefabNameToEnable.Contains("/")) {
                    var elements = prefabNameToEnable.Split('/');

                    weaponTransform = _weaponHolder;

                    // Traverse each element in the path
                    foreach (string element in elements) {
                        // Attempt to find the child by name
                        weaponTransform = weaponTransform.Find(element);
                    }
                }
                else {
                    weaponTransform = _weaponHolder.Find(prefabNameToEnable);
                }

                _weaponTransforms[prefabNameToEnable] = weaponTransform;
            }

            DeactivateAllWeapons();
            UpdateWeaponStats();
        }

        // only called from Player's AttackState. Maybe change that
        public void Attack() {
            _weaponHolder.gameObject.SetActive(true);

            // if the weapon requires to be in control, we need to stop the player and aim
            if (PlayerAim.IsInControl) {
                _playerStateMachine.Animations.PlayIdle();
                _playerStateMachine.Movement.Stop();
            }
            else {
                Attack(_spawnPoint.right);
            }
        }

        public void Attack(Vector2 direction, float chargePower = 1f) {
            if (weapons.Any()) {

                if (Weapon is ICharge chargeWeapon) {
                    chargeWeapon.Charge = chargePower;
                }

                // this is for the player's hand
                _weaponAnimations.PlayAttack(Weapon.AttackType);
                // this one triggers the weapon's attack animation
                Weapon.Attack(_weaponAnimations, direction, _spawnPoint.position);
            }
        }

        public void ChangeWeapon(int index) {
            if (index < 0 || !weapons.Any() || index >= weapons.Count) {
                return;
            }

            _currentWeaponIndex = index;
            UpdateWeaponStats();
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

        public bool HasWeapon(Weapon weapon) => weapons.Contains(weapon);

        public void Equip(Weapon weapon) {
            if (HasWeapon(weapon)) {
                var weaponIndex = weapons.IndexOf(weapon);
                ChangeWeapon(weaponIndex);
            }
            else {
                weapons.Add(weapon);
                UpdateWeaponStats();
            }
        }

        public void EquipAll(IList<Weapon> weapons) {
            if (weapons == null || weapons.Count == 0) {
                return;
            }

            foreach (var weapon in weapons) {
                Equip(weapon);
            }

            _currentWeaponIndex = 0;
            UpdateWeaponStats();
        }

        public void EquipModifiers(WeaponModifier[] configs) {
            Weapon?.EquipModifiers(configs);
        }

        public void Clear() {
            weapons.Clear();
        }

        private void SetAimControl(Weapon weapon) {
            if (weapon.AttackType == AttackType.Gun) {
                PlayerAim.GainControl(_weaponChild.transform);
            }
            else {
                PlayerAim.LoseControl();
            }
        }

        private void SetAimCharging(Weapon weapon) {
            bool shouldCharge = weapon.AttackType == AttackType.Throwable;
            PlayerAim.SetShouldChargeWeapon(shouldCharge, weapon is ThrowableWeapon throwableWeapon ? throwableWeapon.ChargeTime : 0f);
        }

        private void SetShouldShowVisuals(Weapon weapon) {
            PlayerAim.ShouldShowVisuals = weapon.AttackType == AttackType.Throwable || weapon.AttackType == AttackType.Gun;
        }

        private void SetOriginalWeapon(Weapon weapon) {
            if (weapon is IThrowable throwable) {
                throwable.SetOriginalWeapon(_weaponChild);
            }
        }

        private void UpdateWeaponCollider(Weapon weapon) {
            var size = Vector2.one;
            var offset = Vector2.zero;

            if (weapon is IWeaponCollider weaponCollider) {
                size = weaponCollider.AttackSize;
                offset = weaponCollider.AttackOffset;
            }
            var collider = _playerStateMachine.AttackCollider;
            collider.size = size;
            collider.offset = offset;
        }

        private void UpdateWeaponStats() {
            var weapon = weapons[_currentWeaponIndex];
            OnWeaponChanged?.Invoke(_currentWeaponIndex);

            UpdateWeaponCollider(weapon);

            var prefabNameToEnable = weapon.PrefabNameToEnable;

            if (!string.IsNullOrEmpty(prefabNameToEnable)) {
                _weaponHolder.gameObject.SetActive(true);

                if (_weaponChild != null && _weaponChild.name != "Hand") {
                    // if we have already another weapon visible
                    _weaponChild.SetActive(false);
                }

                if (prefabNameToEnable.Contains("/")) {
                    ActivateWeapon(prefabNameToEnable);
                }

                if (_weaponTransforms.TryGetValue(prefabNameToEnable, out Transform weaponTransform)) {
                    _weaponChild = weaponTransform.gameObject;
                }

                if (_weaponChild != null && _weaponChild.FindInChildren(out WeaponSprite weaponSprite)) {
                    weaponSprite.SortSprite();
                }

                // fix this. ot doesn't work with Bow and Arrow
                _aimVisuals.parent = _weaponChild?.transform ?? _weaponHolder;

                SetOriginalWeapon(weapon);
                SetAimControl(weapon);
                SetAimCharging(weapon);
                SetShouldShowVisuals(weapon);

                _weaponChild?.SetActive(true);
            }
            else {
                _weaponHolder.gameObject.SetActive(false);
            }
        }

        private void ActivateWeapon(string prefabNameToEnable) {
            // Split path into elements for traversal
            var elements = prefabNameToEnable.Split('/');

            // Start from the weapon holder as the root
            Transform currentTransform = _weaponHolder;

            // Traverse each element in the path
            foreach (string element in elements) {
                // Attempt to find the child by name
                currentTransform = currentTransform.Find(element);

                // If any part of the path is missing, log an error and return
                if (currentTransform != null) {
                    // Activate the current level of the path to ensure full hierarchy activation
                    currentTransform.gameObject.SetActive(true);
                }
            }
        }

        // Helper method to deactivate all weapons recursively
        private void DeactivateAllWeapons() {
            foreach (Transform child in _weaponHolder) {
                if (child.name == "Hand" || child.name == "Aim Indicator" || child.name == _spawnPoint.name) {
                    continue;
                }
                DeactivateChildRecursively(child);
            }
        }

        // Deactivate a child and all its descendants
        private void DeactivateChildRecursively(Transform parent) {
            parent.gameObject.SetActive(false);
            foreach (Transform child in parent) {
                DeactivateChildRecursively(child);
            }
        }

        #region Stat influences
        public void SetStrengthInfluence(float strength) {
            Weapon?.SetDamageInfluence(strength);
        }

        public void ResetStrengthInfluence() {
            Weapon?.ResetStrengthInfluence();
        }

        #endregion

        #region Editor

        private void OnDrawGizmos() {
            PlayerAim?.OnDrawGizmos();
        }

        [Button("Equip All Weapons")]
        private void EquipAllWeapons() {
            var loadedWeapons = Resources.LoadAll<Weapon>("Weapons");

            weapons = loadedWeapons.ToList();
        }

        #endregion
    }
}