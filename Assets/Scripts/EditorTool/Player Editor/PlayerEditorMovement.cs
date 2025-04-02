using Extensions;
using Player.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EditorTool.PlayerEditor {
    public class PlayerEditorMovement : MonoBehaviour {
        [Header("Movement")]
        [SerializeField]
        private Transform _playerTool;

        [SerializeField]
        private float _checkRadius = .3f;

        [SerializeField]
        private LayerMask _whatStopsMovement;

        [SerializeField]
        private float _timeToMove = 10f;

        private PlayerEditorInput _playerInput;
        private InputAction _playerMovementInput;

        private void Awake() {
            _playerInput = GetComponent<PlayerEditorInput>();
            _playerTool.parent = null;
        }

        private void Start() {
            _playerMovementInput = _playerInput.BuildingActions.Movement;
        }

        private void Update() {
            transform.position = Vector3.MoveTowards(transform.position, _playerTool.position, _timeToMove * Time.deltaTime);

            var input = _playerMovementInput.ReadValue<Vector2>().normalized;

            if (!(Vector3.Distance(transform.position, _playerTool.position) <= float.Epsilon) || input.sqrMagnitude < .1f) {
                return;
            }

            var hit = Physics2D.OverlapBox(
                _playerTool.position + (Vector3)input,
                Vector3.one * _checkRadius,
                0f,
                _whatStopsMovement
            );

            if (!hit) {
                _playerTool.position += input.ToInt();
            }
        }

        private void OnDrawGizmosSelected() {
            if (_playerTool == null || _playerMovementInput == null) {
                return;
            }
            Gizmos.color = Color.red;
            var input = _playerMovementInput.ReadValue<Vector2>().normalized;
            Gizmos.DrawWireCube(_playerTool.position + input.ToInt(), Vector3.one * _checkRadius);
        }
    }
}