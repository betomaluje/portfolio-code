using System;
using BerserkPixel.Health;
using Bezier;
using UnityEngine;
using Utils;

namespace Weapons {
    [RequireComponent(typeof(BezierRenderer))]
    public class Tentacle : MonoBehaviour {
        [SerializeField]
        private float _timeBetween = .5f;

        [SerializeField]
        private float _maxDistance = 5f;

        private BezierRenderer _bezierRenderer;
        private readonly Vector2[] _points = new Vector2[3];

        private HitData _hitData;
        private CharacterHealth _ownerHealth;
        private CharacterHealth _targetHealth;
        private NotifyingCountdownTimer _timer;
        private Action<GameObject> OnObjectDestroyed;

        private void Awake() {
            _bezierRenderer = GetComponent<BezierRenderer>();
            _timer = new NotifyingCountdownTimer(60, _timeBetween);
        }

        public void Setup(HitData hitData, Action<GameObject> onObjectDestroyed) {
            _hitData = hitData;
            OnObjectDestroyed = onObjectDestroyed;

            _ownerHealth = _hitData.attacker.GetComponent<CharacterHealth>();
            _targetHealth = _hitData.victim.GetComponent<CharacterHealth>();

            _bezierRenderer.Begin();

            _timer.OnInterval += OnInterval;
            _timer.Start();
        }

        private void OnInterval() {
            if (_targetHealth != null) {

                if (_targetHealth.IsDead) {
                    // here we need to tell DetectAndSpawn that this gameObject is no longer needed
                    OnObjectDestroyed?.Invoke(gameObject);
                    Destroy(gameObject);
                }

                _targetHealth.PerformDamage(_hitData);
            }

            if (_ownerHealth != null && _ownerHealth.CanGiveHealth()) {
                // maybe give half of the damage to the attacker?
                _ownerHealth.GiveHealth(_hitData.damage / 2);
            }
        }

        private void OnDestroy() {
            _timer.Stop();
            _timer.OnInterval -= OnInterval;
        }

        private void Update() {
            _timer.Tick(Time.deltaTime);
        }

        private void LateUpdate() {
            if (_hitData == null || _hitData.attacker == null || _hitData.victim == null) {
                return;
            }

            Vector2 origin = _hitData.attacker.position;

            Vector2 midPoint = Vector2.Lerp(origin, _hitData.victim.position, 0.5f);

            Vector2 endPoint = _hitData.victim.position;

            float distance = Vector3.Distance(origin, endPoint);
            if (distance > _maxDistance) {
                _bezierRenderer.Finish();
                OnObjectDestroyed?.Invoke(gameObject);
                Destroy(gameObject);
            }
            else {
                _points[0] = origin;
                _points[1] = midPoint;
                _points[2] = endPoint;

                _bezierRenderer.UpdatePositions(_points);
            }

        }
    }
}