using Player;
using UnityEngine;
using Utils;

namespace Weapons {
    public class AutoDetachBullet : MonoBehaviour {
        [SerializeField]
        private int _damage = 5;

        [SerializeField]
        private float _lifetime = 3f;

        [SerializeField]
        private float _bulletDelay = .8f;

        [SerializeField]
        private BaseBullet _toDetach;

        private CountdownTimer _timer;
        private PlayerStateMachine _player;
        private Vector2 _direction;

        private void Awake() {
            _player = FindFirstObjectByType<PlayerStateMachine>();

            _timer = new CountdownTimer(_bulletDelay);
            _timer.OnTimerStop += FireBullet;
        }

        private void Start() {
            if (_toDetach != null) {
                _toDetach.transform.parent = null;
                _toDetach.SetDamage(_damage);
            }

            if (gameObject.activeInHierarchy) {
                Destroy(gameObject, _lifetime);
            }

            _timer.Start();
        }

        private void Update() {
            _timer?.Tick(Time.deltaTime);
        }

        private void FireBullet() {
            if (_player != null) {
                _direction = Vector2.right * _player.Movement.LastX;
            }

            _toDetach.Fire(_direction);
        }

        private void OnDestroy() {
            _timer.OnTimerStop -= FireBullet;
        }
    }
}