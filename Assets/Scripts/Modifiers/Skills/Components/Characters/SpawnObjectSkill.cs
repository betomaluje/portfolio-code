using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Player;
using UnityEngine;
using Weapons;

namespace Modifiers.Skills {
    [CreateAssetMenu(menuName = "Aurora/Skills/Spawn Object Skill")]
    public class SpawnObjectSkill : SkillConfig {
        [SerializeField]
        private Transform _spawnTransform;

        [SerializeField]
        private LayerMask _blockMask;

        [SerializeField]
        private int _amount = 5;

        [SerializeField]
        private int _startAngle = 0;

        [SerializeField]
        private int _angle = 30;

        [SerializeField]
        private float _distanceFromOrigin = 2f;

        [SerializeField]
        private float _timeBetweenSpawns = 0.5f;

        [SerializeField]
        private bool _spawnBehindPlayer = false;

        [SerializeField]
        private bool _destroyOnDeactivate = false;

        private Vector2 _origin;
        private List<Transform> _spawnedItems;
        private Transform _owner;

        public override void Setup(Transform owner) {
            base.Setup(owner);
            _owner = owner;
        }

        public override void Activate(Transform target) {
            base.Activate(target);

            if (CheckConditions() && _owner.TryGetComponent<PlayerStateMachine>(out var machine)) {
                _origin = (Vector2)_owner.position;
                var direction = _spawnBehindPlayer ? -machine.Movement.LastX : machine.Movement.LastX;
                SpawnWithAngle(direction);
            }
        }

        private async void SpawnWithAngle(float facingDirection) {
            _spawnedItems = new();

            var initialDirection = facingDirection < 0 ? Vector2.left : Vector2.right;

            // we raycast each X degrees to detect if there is a wall
            var angle = _startAngle;
            var maxAngle = _startAngle + 360;
            int currentAmount = 0;
            while (angle < maxAngle) {
                var direction = Quaternion.Euler(0, 0, angle) * initialDirection;
                var finalPosition = _origin + (Vector2)(direction * _distanceFromOrigin);

                // Debug.DrawLine(_origin, finalPosition, Color.blue, 1f);

                if (IsWalkableCell(finalPosition) && _spawnTransform != null) {
                    var newObject = Instantiate(_spawnTransform, finalPosition, Quaternion.identity);
                    var xScale = Mathf.Sign(facingDirection);
                    var scale = newObject.localScale;
                    scale.x = xScale;
                    newObject.localScale = scale;

                    _spawnedItems.Add(newObject);
                }

                await UniTask.Delay(System.TimeSpan.FromSeconds(_timeBetweenSpawns));

                angle += _angle;

                currentAmount++;
                if (currentAmount >= _amount) {
                    break;
                }
            }
        }

        private bool IsWalkableCell(Vector2 endPoint) {
            var hitOnEdge = Physics2D.OverlapCircle(endPoint, .2f, _blockMask);

            return hitOnEdge == null;
        }

        public override void Deactivate() {
            base.Deactivate();

            if (_destroyOnDeactivate) {
                foreach (var item in _spawnedItems) {
                    if (item != null && item.gameObject != null) {
                        Destroy(item.gameObject);
                    }
                }
            }
            _spawnedItems.Clear();
        }
    }
}