using Extensions;
using Modifiers.Powerups;
using Modifiers.Skills;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Interactable {
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class ItemInteractTag : MonoBehaviour, IInteractTag {
        [SerializeField]
        private InteractAction _interactAction = InteractAction.Select;

        [SerializeField]
        private string _objectName;

        [TextArea(0, 3)]
        [SerializeField]
        private string _description;

        public InteractAction Action => _interactAction;

        public Sprite ItemIcon {
            get {
                var spriteChild = transform.parent.FindChildren("Sprite");
                if (spriteChild != null && spriteChild.TryGetComponent<SpriteRenderer>(out var spriteRenderer)) {
                    return spriteRenderer.sprite;
                }
                return null;
            }
        }

        private CapsuleCollider2D _collider;

        public string ObjectName {
            get {
                if (!string.IsNullOrEmpty(_description)) {
                    return _description;
                }
                else if (string.IsNullOrEmpty(_objectName)) {
                    return transform.parent != null ? transform.parent.name.Replace("(Clone)", "") : transform.name.Replace("(Clone)", "");
                }
                else {
                    return _objectName;
                }
            }
        }

        public Color TagColor => _tagColor;
        private Color _tagColor = Color.white;

        private void Start() {
            UpdateItemDetails();
        }

        private void OnValidate() {
            UpdateItemDetails();
        }

        [Button("Update Item Details")]
        public void UpdateItemDetailsButton() {
            _description = "";
            _objectName = "";
            UpdateItemDetails();
        }

        private void UpdateItemDetails() {
            UpdateSkillModifier();
            UpdatePowerupModifier();
            UpdateWeaponModifier();
        }

        private void UpdateSkillModifier() {
            if (transform.parent.TryGetComponent<SkillHolder>(out var skillHolder)) {
                var skill = skillHolder.FirstItem;
                if (skill != null) {
                    if (string.IsNullOrEmpty(_description))
                        _description = skill.GetDescription();

                    if (string.IsNullOrEmpty(_objectName))
                        _objectName = skill.Name;

                    if (skill.Icon != null) {
                        var spriteChild = transform.parent.FindChildren("Sprite");
                        if (spriteChild != null && spriteChild.TryGetComponent<SpriteRenderer>(out var spriteRenderer)) {
                            spriteRenderer.sprite = skill.Icon;
                        }
                    }

                    _tagColor = skill.GetTagColor();
                }
            }
        }

        private void UpdatePowerupModifier() {
            if (transform.parent.TryGetComponent<PowerupHolder>(out var powerupHolder)) {
                var powerup = powerupHolder.FirstItem;
                if (powerup != null) {
                    if (string.IsNullOrEmpty(_description))
                        _description = powerup.GetDescription();

                    if (string.IsNullOrEmpty(_objectName))
                        _objectName = powerup.Name;

                    if (powerup.Icon != null) {
                        var spriteChild = transform.parent.FindChildren("Sprite");
                        if (spriteChild != null && spriteChild.TryGetComponent<SpriteRenderer>(out var spriteRenderer)) {
                            spriteRenderer.sprite = powerup.Icon;
                        }
                    }

                    _tagColor = powerup.GetTagColor();
                }
            }
        }

        private void UpdateWeaponModifier() {
            if (transform.parent.TryGetComponent<WeaponModifierHolder>(out var weaponHolder)) {
                var weaponModifier = weaponHolder.FirstItem;
                if (weaponModifier != null) {
                    if (string.IsNullOrEmpty(_description))
                        _description = weaponModifier.GetDescription();

                    if (string.IsNullOrEmpty(_objectName))
                        _objectName = weaponModifier.Name;

                    _tagColor = weaponModifier.GetTagColor();
                }
            }
        }

        private void Reset() {
            gameObject.layer = LayerMask.NameToLayer("Interactable");
            EnsureCollider();
            UpdateItemDetails();
        }

        private void EnsureCollider() {
            if (!TryGetComponent(out _collider)) {
                _collider = gameObject.AddComponent<CapsuleCollider2D>();
                _collider.offset = new Vector2(0, -0.2f);
                _collider.size = new Vector2(1.8f, 2.5f);
                _collider.isTrigger = true;
            }
            else {
                _collider.offset = new Vector2(0, -0.2f);
                _collider.size = new Vector2(1.8f, 2.5f);
                _collider.isTrigger = true;
            }

            if (TryGetComponent<CircleCollider2D>(out var circleCollider)) {
                DestroyImmediate(circleCollider);
            }
        }
    }
}