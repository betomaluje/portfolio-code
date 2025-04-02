using System;
using System.Collections.Generic;
using Camera;
using Level;
using Pooling.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Weapons {
    [RequireComponent(typeof(AudioSource))]
    public class FullscreenBossWeapon : MonoBehaviour {
        [SerializeField]
        private Transform _parentContainer;

        [SerializeField]
        private Transform _leftSide;

        [SerializeField]
        private Transform _rightSide;

        public static Action<BossFullscreenConfig> Shoot;

        private AudioSource _audioSource;
        private CountdownTimer _timer;
        private List<Vector3> _allPositions;
        private Transform _player;

        private BossFullscreenConfig _fullscreenWeaponConfig;

        private BulletPoolManager _bulletPoolManager;

        private Dictionary<string, GameObject> _bulletContainers = new();

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            transform.parent = null;

            Shoot = SetupWeaponConfig;
        }

        private void PerformShooting() {
            _audioSource.Play();
            if (!string.IsNullOrEmpty(_fullscreenWeaponConfig.PostProcessingProfile)) {
                PostProcessingManager.Instance.SetProfile(_fullscreenWeaponConfig.PostProcessingProfile);
            }
            CinemachineCameraShake.Instance.ShakeCamera(transform, _fullscreenWeaponConfig.ScreenShakeMagnitude, _fullscreenWeaponConfig.ScreenShakeDuration, true);

            string prefabName = _fullscreenWeaponConfig.ProjectilePrefab.name;

            foreach (var position in _allPositions) {

                var bullet = _bulletPoolManager.GetAndSetup(_fullscreenWeaponConfig, position, _bulletContainers[prefabName]);

                var direction = (_player.position - position).normalized;

                if (_fullscreenWeaponConfig.Direction == FullscreenDirection.Horizontal) {
                    direction.y = 0;
                }
                else if (_fullscreenWeaponConfig.Direction == FullscreenDirection.Vertical) {
                    direction.x = 0;
                }

                bullet.Fire(direction);
            }
        }

        private void SetupWeaponConfig(BossFullscreenConfig fullscreenWeaponConfig) {
            if (_bulletPoolManager == null) {
                _bulletPoolManager = new BulletPoolManager(fullscreenWeaponConfig.ProjectilePrefab, 10);

                string prefabName = fullscreenWeaponConfig.ProjectilePrefab.name;

                if (!_bulletContainers.ContainsKey(prefabName)) {
                    var container = new GameObject(prefabName);
                    container.transform.parent = null;
                    _bulletContainers[prefabName] = container;
                }
            }

            if (_fullscreenWeaponConfig == null || _fullscreenWeaponConfig.GetHashCode() != fullscreenWeaponConfig.GetHashCode()) {
                _fullscreenWeaponConfig = fullscreenWeaponConfig;

                _timer = new CountdownTimer(fullscreenWeaponConfig.TimeToActivate);
                _timer.OnTimerStop += PerformShooting;
            }

            GetCurrentPositions();

            _timer.Reset();
            _timer.Start();
        }

        private void GetCurrentPositions() {
            if (_parentContainer != null) {
                // rotate transform if necessary
                if (_fullscreenWeaponConfig.Direction == FullscreenDirection.Horizontal) {
                    _parentContainer.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (_fullscreenWeaponConfig.Direction == FullscreenDirection.Vertical) {
                    _parentContainer.rotation = Quaternion.Euler(0, 0, 90);
                }
            }

            _allPositions = GetPositions(_fullscreenWeaponConfig.ProjectileCount, _fullscreenWeaponConfig.TotalRange, _leftSide);
            _allPositions.AddRange(GetPositions(_fullscreenWeaponConfig.ProjectileCount, _fullscreenWeaponConfig.TotalRange, _rightSide));
        }

        private List<Vector3> GetPositions(int amount, float totalRange, Transform initialTransform) {
            if (amount <= 0) return default;

            // Calculate the spacing based on the total range and the number of projectiles
            float spacing = totalRange / (amount - 1); // Subtract 1 to ensure edge-aligned spacing

            // Starting y-position, centering the projectiles
            float startYPosition = initialTransform.position.y - totalRange / 2;
            float startXPosition = initialTransform.position.x - totalRange / 2;

            List<Vector3> spawnPositions = new();

            for (int i = 0; i < amount; i++) {
                Vector3 spawnPosition = Vector3.zero;

                if (_fullscreenWeaponConfig.Direction == FullscreenDirection.Horizontal) {
                    float yPosition = startYPosition + i * spacing;

                    spawnPosition = new(
                        initialTransform.position.x,
                        yPosition,
                        0
                    );
                }
                else if (_fullscreenWeaponConfig.Direction == FullscreenDirection.Vertical) {
                    float xPosition = startXPosition + i * spacing;

                    spawnPosition = new(
                        xPosition,
                        initialTransform.position.y,
                        0
                    );
                }

                spawnPositions.Add(spawnPosition);
            }

            return spawnPositions;
        }

        private void Update() {
            if (_timer == null) {
                return;
            }

            _timer.Tick(Time.deltaTime);
        }

        private void OnDestroy() {
            if (_timer != null) {
                _timer.OnTimerStop -= PerformShooting;
            }
        }
        #region Debug
        private void OnDrawGizmosSelected() {
            if (_leftSide == null || _rightSide == null || _fullscreenWeaponConfig == null) return;

            var allPositions = GetPositions(_fullscreenWeaponConfig.ProjectileCount, _fullscreenWeaponConfig.TotalRange, _leftSide);
            allPositions.AddRange(GetPositions(_fullscreenWeaponConfig.ProjectileCount, _fullscreenWeaponConfig.TotalRange, _rightSide));

            Gizmos.color = Color.green;

            foreach (var position in allPositions) {
                Gizmos.DrawWireSphere(position, 0.2f);
            }
        }

        [Button]
        private void DebugMovement(Transform target, BossFullscreenConfig fullscreenWeaponConfig) {
            _fullscreenWeaponConfig = fullscreenWeaponConfig;
            GetCurrentPositions();

            foreach (var position in _allPositions) {
                var direction = (target.position - position).normalized;

                if (_fullscreenWeaponConfig.Direction == FullscreenDirection.Horizontal) {
                    direction.y = 0;
                }
                else if (_fullscreenWeaponConfig.Direction == FullscreenDirection.Vertical) {
                    direction.x = 0;
                }

                Debug.DrawLine(position, position + direction * 10, Color.green, 2f);
            }
        }

        #endregion
    }
}