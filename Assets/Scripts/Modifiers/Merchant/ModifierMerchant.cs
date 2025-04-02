using System;
using System.Collections.Generic;
using Camera;
using Cysharp.Threading.Tasks;
using Dungeon;
using Interactable;
using Modifiers.Powerups;
using Modifiers.Skills;
using Player;
using Sounds;
using UI;
using UnityEngine;
using Weapons;

namespace Modifiers.Merchant {
    public class ModifierMerchant : MonoBehaviour, IInteract {
        private const float InteractionTimeWindow = 0.2f;

        [SerializeField]
        private Transform _spriteTransform;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private bool _shouldDisappear = true;

        private readonly int ANIM_DONE = Animator.StringToHash("ModifierMerchant-Done");

        private PlayerChooseModifier _playerChooseModifier;
        private Transform _playerTransform;

        private Animator _animator;
        private bool _playerHasChosenModifier;
        private float _lastX = 1;
        private float _localScaleX;
        private FirstRoomSetup _lastFirstRoomSetup;

        private float _lastInteractionTime;

        private void Awake() {
            _playerChooseModifier = GetComponentInChildren<PlayerChooseModifier>();
            _playerTransform = FindFirstObjectByType<PlayerStateMachine>().transform;
            _animator = GetComponentInChildren<Animator>();
            _localScaleX = _spriteTransform.localScale.x;
        }

        public void CancelInteraction() { }

        public void DoInteract() {
            if (_playerHasChosenModifier) {
                return;
            }

            float currentTime = Time.time;
            if (currentTime - _lastInteractionTime > InteractionTimeWindow) {
                // open the UI and show trade options
                _playerChooseModifier.ToggleUI();
            }

            _lastInteractionTime = currentTime;

        }

        private void LateUpdate() {
            var direction = (_playerTransform.position - transform.position).normalized;

            FlipSprite(direction);
        }

        private void FlipSprite(Vector2 direction) {
            // any input? or if we are moving only vertically
            if (direction.sqrMagnitude <= .1f || direction.x == 0f) {
                return;
            }

            var sign = Mathf.Sign(direction.x);

            // check if we are already facing in that direction 
            if (Mathf.Sign(_spriteTransform.localScale.x) == _lastX && sign == _lastX) {
                return;
            }

            _lastX = sign;

            SetScale();
        }

        private void SetScale() {
            var localScale = _spriteTransform.localScale;
            localScale.x = _localScaleX * _lastX;
            localScale.z = 1f;
            _spriteTransform.localScale = localScale;
        }

        public void SetupFirstRoom(FirstRoomSetup firstRoomSetup) {
            _lastFirstRoomSetup = firstRoomSetup;

            var weaponModifiers = firstRoomSetup.WeaponModifiers;
            SpawnItems(weaponModifiers.Items, weaponModifiers.Amount);

            // Spawn skills
            var skills = firstRoomSetup.Skills;
            SpawnItems(skills.Items, skills.Amount);

            // Spawn powerups
            var powerups = firstRoomSetup.Powerups;
            SpawnItems(powerups.Items, powerups.Amount);
        }

        private void SpawnItems<T>(IList<T> items, int amount) where T : IModifier {
            if (items == null || items.Count == 0) {
                return;
            }

            amount = Mathf.Clamp(amount, 0, items.Count);

            var shuffledList = items.SimpleShuffle();

            for (int i = 0; i < amount; i++) {
                _playerChooseModifier.AddModifier(shuffledList[i]);
            }
        }

        public async void Disappear(IModifier modifier) {
            AddToPlayer(modifier);

            _audioSource.Play();

            if (_shouldDisappear) {
                _playerHasChosenModifier = true;
                await UniTask.Delay(TimeSpan.FromMilliseconds(750));

                _animator.Play(ANIM_DONE);
                CameraShockwave.Instance.DoShockwave(_playerTransform.position);
                CinemachineCameraShake.Instance.ShakeCamera(transform, 12f, 2f, true);

                float length = GetAnimationLength(ANIM_DONE);

                Destroy(gameObject, length);
            }
            else {
                CameraShockwave.Instance.DoShockwave(_playerTransform.position);
                CinemachineCameraShake.Instance.ShakeCamera(transform, 12f, 2f, true);

                RefillModifiers();
            }
        }

        public void RefillModifiers() {
            if (_lastFirstRoomSetup != null) {
                _playerChooseModifier.ResetModifiers();
                SetupFirstRoom(_lastFirstRoomSetup);
            }
        }

        private void AddToPlayer(IModifier modifier) {
            if (_playerTransform == null) {
                return;
            }

            if (modifier is PowerupConfig powerup && _playerTransform.TryGetComponent<CharacterPowerup>(out var characterPowerup)) {
                if (characterPowerup.DoPowerup(powerup, _playerTransform)) {
                    SoundManager.instance.Play("coin");

                    CinemachineCameraHighlight.Instance.HighlightInBounds(_playerTransform);
                }
            }

            if (modifier is SkillConfig skill && _playerTransform.TryGetComponent<CharacterSkills>(out var characterSkills)) {
                if (characterSkills.EquipSkill(skill, _playerTransform)) {
                    SoundManager.instance.Play("coin");
                    CinemachineCameraHighlight.Instance.HighlightInBounds(_playerTransform);
                }
            }

            if (modifier is WeaponModifier weapon && _playerTransform.TryGetComponent<WeaponManager>(out var weaponManager)) {
                weaponManager.EquipModifiers(new WeaponModifier[1] { weapon });
                SoundManager.instance.Play("coin");
                CinemachineCameraHighlight.Instance.HighlightInBounds(_playerTransform);
            }
        }

        private float GetAnimationLength(int toPlay) {
            float defaultTime = .5f;

            foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips) {
                if (Animator.StringToHash(clip.name) == toPlay) {
                    return clip.length;
                }
            }

            return defaultTime;
        }
    }
}