using System;
using BerserkPixel.Utils;
using Extensions;
using Interactable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Loot {
    public class Chest : MonoBehaviour, ILoot, IInteract {
        [SerializeField]
        private LootConfig _lootConfig;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private Transform _openParticles;

        [SerializeField]
        private float _xRotation = -24f;

        private int Open_Anim = Animator.StringToHash("Chest_Open");
        private WeightedList<Transform> _randomObjects;
        private bool _hasBeenOpened;

        private void Awake() {
            System.Random random = new(DateTime.Now.TimeOfDay.Minutes);
            _randomObjects = new(_lootConfig.LootObjects, random);
        }

        public void Unlock() {
            _animator.CrossFade(Open_Anim, 0.1f);
            if (TryGetComponent<InteractTag>(out var tag)) {
                Destroy(tag);
            }

            if (TryGetComponent<CircleCollider2D>(out var collider)) {
                Destroy(collider);
            }
        }

        /// <summary>
        /// Called from the Open Animation in the Chest object. Instantiates the loot
        /// </summary>
        public void InstantiateLoot() {
            _audioSource.Play();
            for (var i = 0; i < _lootConfig.LootAmount; i++) {
                Instantiate(_randomObjects.Next(), transform.position.GetRandomPosition(_lootConfig.LootSpawnRadius), Quaternion.identity);
            }

            if (_openParticles != null) {
                Instantiate(_openParticles, transform.position, Quaternion.Euler(_xRotation, 0f, 0f));
            }
        }

        private void OnValidate() {
            if (_animator == null) {
                _animator = GetComponent<Animator>();
            }

            if (_audioSource == null) {
                _audioSource = GetComponent<AudioSource>();
            }
        }

        public void DoInteract() {
            if (!_hasBeenOpened) {
                _hasBeenOpened = true;
                Unlock();
            }
        }

        public void CancelInteraction() {
            if (_hasBeenOpened) {
                _hasBeenOpened = false;
            }
        }

#if UNITY_EDITOR
        [Button]
        public void ResetChest() {
            _hasBeenOpened = false;
            _animator.Play("Chest_Idle");
        }
#endif
    }
}