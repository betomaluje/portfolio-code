using Camera;
using Modifiers.Skills;
using UnityEngine;
using Utils;

namespace Player {
    public class PlayerPowerupFX : MonoBehaviour {
        [SerializeField]
        private Transform _powerupFx;

        [SerializeField]
        private CharacterSkills _playerSkills;

        [SerializeField]
        private CircleCollider2D _collider;

        [SerializeField]
        private float _finalRadius = 3f;

        [SerializeField]
        private float _growthSpeed = 5f;

        [SerializeField]
        private float _duration = 1f;

        private bool _isActive;
        private CountdownTimer _timer;
        private float _originalradius;

        private void Awake() {
            if (_powerupFx != null) {
                _powerupFx.gameObject.SetActive(false);
                _powerupFx.parent = null;
            }

            _originalradius = _collider.radius;

            _timer = new CountdownTimer(_duration);
        }

        private void ResetCollider() {
            _collider.enabled = false;
            _collider.radius = _originalradius;
            _isActive = false;
        }

        private void ActivateCollider() {
            _collider.enabled = true;
            _isActive = true;
        }

        private void OnEnable() {
            if (_playerSkills != null) {
                _playerSkills.OnSkillActivated += HandlePowerupActivated;
            }

            _timer.OnTimerStart += ActivateCollider;
            _timer.OnTimerStop += ResetCollider;
        }

        private void OnDisable() {
            if (_playerSkills != null) {
                _playerSkills.OnSkillActivated -= HandlePowerupActivated;
            }

            _timer.OnTimerStart -= ActivateCollider;
            _timer.OnTimerStop -= ResetCollider;
        }

        private void Update() {
            if (_isActive) {
                _collider.radius = Mathf.Lerp(_collider.radius, _finalRadius, _growthSpeed * Time.deltaTime);
                _timer.Tick(Time.deltaTime);
            }
        }

        private void HandlePowerupActivated(SkillConfig config) {
            if (_powerupFx != null) {
                _powerupFx.position = transform.position;
                _powerupFx.gameObject.SetActive(true);
                CinemachineCameraShake.Instance.ShakeCamera(transform, 8f, .5f, true);
                CameraShockwave.Instance.DoShockwave(transform.position);
                _timer.Start();
            }
        }
    }
}