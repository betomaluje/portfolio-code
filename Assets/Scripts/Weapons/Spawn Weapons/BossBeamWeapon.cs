using Camera;
using Level;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Weapons {
    public class BossBeamWeapon : BaseSpawnWeapon {
        [BoxGroup("Spawn Settings")]
        [SerializeField]
        private float _positionOffset = 1;

        [BoxGroup("Spawn Settings")]
        [SerializeField]
        private float _yOffset = 1f;

        [BoxGroup("Movement Settings")]
        [SerializeField]
        private bool _followPlayer = false;

        [BoxGroup("Movement Settings")]
        [ShowIf("_followPlayer")]
        [SerializeField]
        private float _lifeTime = 3f;

        [BoxGroup("Movement Settings")]
        [SerializeField]
        private float _timeToActivate = 1.0f;

        [BoxGroup("Movement Settings")]
        [SerializeField]
        private float _duration = 1.0f;

        [BoxGroup("Movement Settings")]
        [SerializeField]
        private AnimationCurve _movementCurve;

        private AudioSource _audioSource;

        private float _leftPosition, _rightPosition;
        private float _originalYPosition;
        private CountdownTimer _timer;
        private Transform _player;

        private MovementConfig _movementConfig;
        protected override MovementConfig MovementConfig => _movementConfig;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
            _player = GameObject.FindGameObjectWithTag("Player").transform;

            _timer = new CountdownTimer(_timeToActivate);
        }

        private void OnEnable() {
            _timer.OnTimerStop += StartMoving;
        }

        private void OnDisable() {
            _timer.OnTimerStop -= StartMoving;
        }

        override public void Shoot() {
            ResetPosition();
            _timer.Start();
        }

        private void StartMoving() {
            _audioSource.Play();
            PostProcessingManager.Instance.SetProfile("Beam Boss 2");
            CinemachineCameraShake.Instance.ShakeCamera(transform, 10, _lifeTime, true);
        }

        private void OnDestroy() {
            PostProcessingManager.Instance.Reset();
        }

        private void ResetPosition() {
            var camera = UnityEngine.Camera.main;
            _originalYPosition = _player.position.y + _yOffset;

            _leftPosition = camera.ScreenToWorldPoint(new Vector3(0, 0)).x - _positionOffset;
            _rightPosition = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, 0)).x + _positionOffset;

            if (_followPlayer) {
                _movementConfig = new FollowTargetMovementConfig(
                    transform,
                    _player,
                    _lifeTime,
                    _duration,
                    _movementCurve
                );
            }
            else {
                var targetPosition = new Vector3(_rightPosition, _originalYPosition);

                _movementConfig = new StraightMovementConfig(
                   transform,
                   targetPosition,
                   _duration,
                   _movementCurve
               );
            }

            _movementConfig.SetMovementEndAction(HandleMovementEnd);

            transform.position = new Vector3(_leftPosition, _originalYPosition, 0);
        }

        private void HandleMovementEnd() {
            // TODO: make any FX before destroying
            Destroy(gameObject);
        }

        private void Update() {
            _timer.Tick(Time.deltaTime);

            if (_timer.IsRunning) {
                return;
            }

            _movementConfig.Move(Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckCollision(other)) { }
        }

        [Button]
        private void DebugMovement(Transform target) {
            _player = target;
            ResetPosition();
            _timer = new CountdownTimer(_timeToActivate);
            _timer.Start();
        }
    }
}