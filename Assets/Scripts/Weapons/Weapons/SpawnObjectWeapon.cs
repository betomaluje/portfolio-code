using Base;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(menuName = "Aurora/Weapons/Spawn Object Weapon")]
    public class SpawnObjectWeapon : Weapon {
        [Tooltip("The prefab to spawn when firing.")]
        public BaseSpawnWeapon ObjectPrefab;

        private BaseSpawnWeapon _spawnedObject;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) {
                return;
            }
            animations?.Play(AttackAnimation);
            HandleSpawnObject(position, direction);
            StartCooldown();
        }

        private void HandleSpawnObject(Vector3 position, Vector2 direction) {
            if (ObjectPrefab == null) {
                return;
            }

            if (_spawnedObject == null) {
                _spawnedObject = Instantiate(ObjectPrefab, position, Quaternion.identity);
            }
            else {
                _spawnedObject.transform.position = position;
                _spawnedObject.Activate();
            }

            _spawnedObject.SetWeapon(this);
            _spawnedObject.SetDirection(direction);
            _spawnedObject.Shoot();
        }
    }
}