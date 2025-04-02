using System.Collections;
using Extensions;
using Player;
using Player.Input;
using Sounds;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Buildings {
    public class GhostBuilding : MonoBehaviour {
        [SerializeField]
        private SpriteRenderer _image;

        [SerializeField]
        private float _timeToMove = .1f;

        [SerializeField]
        private BuildingBlueprint _blueprintPrefab;

        [Header("Collision")]
        [SerializeField]
        private BoxCollider2D _collider;

        [SerializeField]
        private Vector2 _margin;

        private Building _building;
        private bool _isMoving;

        private PlayerBuildingInput _playerBuildingInput;
        private PlayerBattleInput _playerInput;
        private PlayerMoneyManager _playerMoneyManager;
        private InputAction _playerMovementInput;
        private Rigidbody2D _rb;
        private Vector2 _input;

        private void Awake() {
            _playerBuildingInput = GetComponent<PlayerBuildingInput>();
            _playerInput = FindFirstObjectByType<PlayerBattleInput>();
            _playerMoneyManager = FindFirstObjectByType<PlayerMoneyManager>();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start() {
            _playerMovementInput = _playerBuildingInput.BuildingActions.Movement;
        }

        private void Update() {
            _input = _playerMovementInput.ReadValue<Vector2>().normalized;
        }

        private void FixedUpdate() {
            StartCoroutine(Move(_input));
        }

        private void OnEnable() {
            _playerBuildingInput.PlaceBuilding += HandlePlace;
        }

        private void OnDisable() {
            _playerBuildingInput.PlaceBuilding -= HandlePlace;
        }

        private void HandlePlace() {
            if (_blueprintPrefab != null) {
                var blueprint = Instantiate(_blueprintPrefab, transform.position, Quaternion.identity);
                blueprint.Setup(_building, _collider);
            }

            // remove player money
            _playerMoneyManager.TakeAmount(_building.Cost);
            SoundManager.instance.Play("build_place");

            _playerInput.BattleActions.Enable();
            Destroy(gameObject);
        }

        public void Setup(Building building) {
            _building = building;
            _image.sprite = building.Sprite;

            // update colliders bounds
            _image.UpdateColliderSize(_collider, _margin);
        }

        private IEnumerator Move(Vector2 direction) {
            if (_isMoving) {
                yield break;
            }

            _isMoving = true;

            var elapsedTime = 0f;
            var originPos = (Vector2)transform.position;
            var endPos = originPos + direction;

            while (elapsedTime < _timeToMove) {
                _rb.MovePosition(Vector2.Lerp(originPos, endPos, elapsedTime / _timeToMove));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _rb.MovePosition(endPos);

            _isMoving = false;
        }
    }
}