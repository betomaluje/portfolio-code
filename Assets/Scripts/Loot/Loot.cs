using System;
using BerserkPixel.Health;
using BerserkPixel.Utils;
using Extensions;
using UnityEngine;

namespace Loot {
    [RequireComponent(typeof(CharacterHealth))]
    public class Loot : MonoBehaviour, ILoot {
        [SerializeField]
        private LootConfig _lootConfig;

        public Action OnUnlock = delegate { };

        private CharacterHealth _health;

        private WeightedList<Transform> _randomObjects;

        private void Awake() {
            _health = GetComponent<CharacterHealth>();
            var chance = _lootConfig.LootChance >= UnityEngine.Random.value;
            if (!chance) {
                Destroy(this);
            }

            System.Random random = new(DateTime.Now.TimeOfDay.Minutes);
            _randomObjects = new(_lootConfig.LootObjects, random);
        }

        private void OnEnable() {
            _health.OnDie += Unlock;
        }

        private void OnDisable() {
            _health.OnDie -= Unlock;
        }

        public void Unlock() {
            OnUnlock?.Invoke();

            for (var i = 0; i < _lootConfig.LootAmount; i++) {
                Instantiate(_randomObjects.Next(), transform.position.GetRandomPosition(_lootConfig.LootSpawnRadius), Quaternion.identity);
            }
        }
    }
}