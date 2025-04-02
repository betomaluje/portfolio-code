using DG.Tweening;
using UnityEngine;
using Weapons;

namespace Player {
    [RequireComponent(typeof(PlayerMaskController))]
    public class PlayerSpritesMask : MonoBehaviour {
        [SerializeField]
        private Transform _rendererParent;

        [SerializeField]
        private float _animDuration = .25f;

        [Header("Weapon Modifiers")]
        [SerializeField]
        private WeaponManager _weaponManager;

        private Vector2 _originalScale;

        private void Awake() => _originalScale = transform.localScale;

        private void OnValidate() {
            if (_rendererParent == null) {
                _rendererParent = transform;
            }
        }

        private void OnEnable() => _weaponManager.OnWeaponChanged += ChangeWeaponSize;

        private void OnDisable() => _weaponManager.OnWeaponChanged -= ChangeWeaponSize;

        private void OnDestroy() => DOTween.KillAll();

        private void ChangeWeaponSize(int weaponIndex) {
            var currentWeapon = _weaponManager.Weapon;
            if (currentWeapon == null) {
                return;
            }

            transform.DOScale(currentWeapon.MaskWeaponSize, _animDuration);
        }
    }
}