using System;
using System.Linq;
using BerserkPixel.Health;
using BerserkTools.Health.UI;
using Cysharp.Threading.Tasks;
using Extensions;
using Player.Input;
using UnityEngine;

namespace Player.Components {
    public class PlayerAim {
        private bool _isInControl;
        private bool _shouldShowVisuals;

        public bool IsInControl => _isInControl || _shouldChargeWeapon;
        public bool IsCurrentlyAttacking { get; private set; }
        public bool ShouldShowVisuals {
            private get => _shouldShowVisuals;
            set {
                _shouldShowVisuals = value;
                if (_aimVisuals != null) {
                    _aimVisuals.gameObject.SetActive(value);
                }
            }
        }

        private readonly Transform _attackPoint;
        private readonly PlayerBattleInput _playerBattleInput;
        private readonly Action<Vector2, float> OnAttack;
        private readonly Transform _originalAttackParent;
        private readonly Transform _aimVisuals;

        private Transform _weaponTransform;
        private Vector2 _lastInput = Vector2.zero;

        private Vector2 _lastPlayerAim = Vector2.zero;

        // Auto assist settings
        private bool _autoAssist = true;
        private readonly float _assistRadius = 4.8f;

        private LayerMask _targetMask = -1;

        // Charging
        private bool _shouldChargeWeapon;
        private float _currentChargeTime = 0f;
        private float _weaponChargeTime;
        private ProgressbarBehaviour _chargeBar;

        public PlayerAim(
            Action<Vector2, float> attackAction,
            PlayerBattleInput playerBattleInput,
            Transform attackPoint,
            Transform aimVisuals,
            bool autoAssist = true) {

            _playerBattleInput = playerBattleInput;
            OnAttack = attackAction;
            _attackPoint = attackPoint;
            _autoAssist = autoAssist;
            _aimVisuals = aimVisuals;

            _originalAttackParent = attackPoint.parent;

            _playerBattleInput.SouthButtonEvent += HandleAttackPressed;

            PreferencesStorage.OnPreferenceChanged += HandlePreferenceChanged;
        }

        ~PlayerAim() {
            Dispose();
        }

        private void HandlePreferenceChanged(string eventKey, bool active) {
            if (eventKey.Equals(PreferencesStorage.EVENT_AIM)) {
                _autoAssist = active;
            }
        }

        public void SetTargetMask(LayerMask targetMask) {
            _targetMask = targetMask;
        }

        /// <summary>
        /// Sets the weapon to charge or not. Called whenever we change weapon in WeaponManager;
        /// </summary>
        /// <param name="shouldCharge"></param>
        public void SetShouldChargeWeapon(bool shouldCharge, float weaponChargeTime) {
            _currentChargeTime = 0;
            _weaponChargeTime = weaponChargeTime;
            _shouldChargeWeapon = shouldCharge;
        }

        public void SetChargeBar(ProgressbarBehaviour chargeBar) {
            _chargeBar = chargeBar;
        }

        public Vector2 GetClosestEnemyDirection() {
            if (!_autoAssist || _targetMask == -1) {
                return Vector2.zero;
            }

            var hits = Physics2D.OverlapCircleAll(_attackPoint.position, _assistRadius, _targetMask);
            if (hits.Length == 0) {
                return Vector2.zero;
            }

            // we need to know which one is the closest to where we are aiming
            var closest = hits
                    .Where(hit => hit.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead)
                    .OrderBy(hit => Vector2.Distance(_attackPoint.position, hit.transform.position))
                    .FirstOrDefault();

            if (closest != null) {
                return (closest.transform.position - _attackPoint.position).normalized;
            }
            else {
                return Vector2.zero;
            }
        }

        private async void HandleAttackPressed(bool isPressed) {
            if (_shouldChargeWeapon) {
                if (isPressed && !IsCurrentlyAttacking) {
                    IsCurrentlyAttacking = true;
                }
                else if (!isPressed && IsCurrentlyAttacking) {
                    float percentage = Mathf.Clamp01(_currentChargeTime / _weaponChargeTime);

                    IsCurrentlyAttacking = false;

                    OnAttack?.Invoke(_lastInput, percentage);

                    _currentChargeTime = 0f;
                    _chargeBar.ResetBar();

                    await Cooldown();
                }
            }
            else {
                if (_isInControl && isPressed && !IsCurrentlyAttacking) {
                    IsCurrentlyAttacking = true;
                }

                if (_isInControl && !isPressed) {
                    OnAttack?.Invoke(_lastInput, 1f);
                    await Cooldown();
                }
            }
        }

        private async UniTask Cooldown() {
            await UniTask.Delay(TimeSpan.FromSeconds(.25f));
            IsCurrentlyAttacking = false;
        }

        public void Tick(float deltaTime, Vector2 aimDirection) {
            RotateAimVisuals(deltaTime);
            RotateWeaponTransform(deltaTime);
            UpdateChargeTime(deltaTime);
            SetInput(aimDirection);
        }

        private void RotateAimVisuals(float deltaTime) {
            if (!ShouldShowVisuals) {
                return;
            }

            var lastRotation = TransformExt.GetRotation(_lastPlayerAim);
            var rotationSpeed = 12f;
            rotationSpeed = Mathf.Clamp01(deltaTime * rotationSpeed);
            _aimVisuals.rotation = Quaternion.Lerp(_aimVisuals.rotation, lastRotation, rotationSpeed);

            if (_shouldChargeWeapon) {
                // for fliping the aim visuals
                float lastPlayerX = Mathf.Sign(_lastPlayerAim.x);

                var aimScale = _aimVisuals.localScale;

                if (lastPlayerX == aimScale.x) return;

                aimScale.x = lastPlayerX;
                _aimVisuals.localScale = aimScale;
            }
        }

        private void RotateWeaponTransform(float deltaTime) {
            if (_weaponTransform != null) {
                var lastRotation = TransformExt.GetRotation(_lastPlayerAim);
                var rotationSpeed = 12f;
                rotationSpeed = Mathf.Clamp01(deltaTime * rotationSpeed);
                _weaponTransform.rotation = Quaternion.Lerp(_weaponTransform.rotation, lastRotation, rotationSpeed);
            }
        }

        private async void UpdateChargeTime(float deltaTime) {
            if (IsCurrentlyAttacking && _shouldChargeWeapon) {
                _currentChargeTime += deltaTime;

                _chargeBar.ChangePercentage(_currentChargeTime / _weaponChargeTime);

                if (_currentChargeTime >= _weaponChargeTime) {
                    _currentChargeTime = 0f;
                    _chargeBar.ResetBar();
                    OnAttack?.Invoke(_lastInput, 1f);
                    await Cooldown();
                }
            }
        }

        private void SetInput(Vector2 aimDirection) {
            if (aimDirection == Vector2.zero) {
                return;
            }

            _lastPlayerAim = aimDirection;

            _attackPoint.RotateTo(aimDirection);
            _lastInput = _attackPoint.right;

            if (_weaponTransform != null) {
                float lastPlayerX = Mathf.Sign(aimDirection.x);

                var weaponScale = _weaponTransform.localScale;

                if (lastPlayerX == weaponScale.x) return;

                weaponScale.x = lastPlayerX;
                _weaponTransform.localScale = weaponScale;
            }
        }

        public void GainControl(Transform weaponTransform) {
            _isInControl = true;
            _weaponTransform = weaponTransform;
            _attackPoint.parent = weaponTransform;
        }

        public void LoseControl() {
            _isInControl = false;
            _weaponTransform = null;
            _attackPoint.parent = _originalAttackParent;
        }

        public void Dispose() {
            _isInControl = false;
            _targetMask = -1;
            _playerBattleInput.SouthButtonEvent -= HandleAttackPressed;
            PreferencesStorage.OnPreferenceChanged -= HandlePreferenceChanged;
        }

        private void OnDestroy() {
            Dispose();
        }

        public void OnDrawGizmos() {
            if (!_autoAssist || _targetMask == -1) {
                return;
            }
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_attackPoint.position, _assistRadius);
        }
    }
}