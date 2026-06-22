using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BerserkPixel.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BerserkPixel.StateMachine {
    public abstract class StateMachine<T> : MonoBehaviour, IStateAnimationTrigger where T : MonoBehaviour {
        [Searchable]
        [SerializeField]
        protected List<State<T>> _states;

        [BoxGroup("Debug", order: 100)]
        [SerializeField]
        protected bool _displayActiveState = false;

        [BoxGroup("Debug", order: 100)]
        [ShowIf("_displayActiveState")]
        [SerializeField]
        protected int _maxStatePrints = 1;

        [BoxGroup("Debug", order: 100)]
        [SerializeField]
        [Min(12)]
        [ShowIf("_displayActiveState")]
        protected int _fontSize = 14;

        [BoxGroup("Debug", order: 100)]
        [SerializeField]
        [ShowIf("_displayActiveState")]
        protected float _offsetY = .5f;

        [BoxGroup("Debug", order: 100)]
        [SerializeField]
        [ShowIf("_displayActiveState")]
        [Tooltip("Padding for the version label. X is horizontal padding, Y is vertical padding.")]
        protected Vector2Int _padding = new(10, 0);

        public Type CurrentState => _activeState != null ? _activeState.GetType() : default;
        public State<T> ActiveState => _activeState;

        protected State<T> _activeState;

        private T _parent;

        protected Queue<string> _debugStates;

        protected virtual void Awake() {
            _parent = GetComponent<T>();
            _debugStates = new Queue<string>(_maxStatePrints);
        }

        protected virtual void Start() {
            if (_states.Count <= 0) {
                return;
            }

            SetState(_states[0]);
        }

        protected virtual void Update() {
            _activeState?.Tick(Time.deltaTime);
            _activeState?.ChangeState();
        }

        protected virtual void FixedUpdate() {
            _activeState?.FixedTick(Time.fixedDeltaTime);
        }

        protected virtual void OnValidate() {
            DopeArrayEditor(_states);
        }

        /// <summary>
        ///     Clones all the States. Useful when there are multiple instances of the same state machine in the scene.
        ///     IMPORTANT! Call this on Awake();
        /// </summary>
        protected void CloneStates() {
            var list = new List<State<T>>(_states.Count);
            foreach (var state in _states) {
                list.Add(state.Clone());
            }

            _states = list;
        }

        private static void DopeArrayEditor<U>(List<U> states) {
            if (states == null || states.Count <= 0) {
                return;
            }

            var prop = typeof(T).GetField("name");
            if (prop == null) {
                return;
            }

            foreach (var n in states) {
                prop.SetValue(n, n.ToString());
            }
        }

        public void SetState(State<T> newStateType) {
            _activeState?.Exit();
            _activeState = newStateType;
            _activeState?.Enter(_parent);

            if (!_displayActiveState) {
                return;
            }

            if (_debugStates.Count >= _maxStatePrints) {
                _debugStates.Dequeue();
            }

            var content = _activeState != null ? (string.IsNullOrWhiteSpace(_activeState.name) ? _activeState.ToString() : _activeState.name) : "(no active state)";
            _debugStates.Enqueue(content);
        }

        public void SetState(Type newStateType) {
            var possibleStates = _states.Where(s => s.GetType() == newStateType).ToList();
            var totalStates = possibleStates.Count;
            if (totalStates > 1) {
                // select a random one
                SetState(possibleStates[UnityEngine.Random.Range(0, totalStates)]);
            }
            else {
                var newState = possibleStates.FirstOrDefault();
                if (newState) {
                    SetState(newState);
                }
            }
        }

        public void SetStates(List<State<T>> newStates) {
            _states?.Clear();
            _states = newStates;
        }

        public State<T> GetState(Type newStateType) {
            return _states.FirstOrDefault(s => s.GetType() == newStateType);
        }

        public void AddState(State<T> newState) {
            var newList = _states != null ? new List<State<T>>(_states) : new List<State<T>>();
            
            if (!newList.Contains(newState)) {
                newList.Add(newState);
                _states = newList;
            }
        }

        public void ClearStates() {
            if (_activeState != null) {
                _activeState.Exit();
                _activeState = null;
            }
            _states.Clear();
        }

        public bool HasState(Type newStateType) => _states.Any(s => s.GetType() == newStateType);

        /// <summary>
        ///     Can be called from the Animation Timeline. This will propagate the AnimationTriggerType
        ///     to the current active state.
        /// </summary>
        /// <param name="triggerType"></param>
        public void SetAnimationTriggerEvent(AnimationTriggerType triggerType) {
            _activeState?.AnimationTriggerEvent(triggerType);
        }

        protected virtual void OnGUI() {
            if (!_displayActiveState) {
                return;
            }

            StringBuilder stringBuilder = new();
            foreach (var state in _debugStates) {
                var content = !string.IsNullOrWhiteSpace(state) ? state.ToString() : "(no active state)";
                stringBuilder.AppendLine(content);
            }

            GUIStyle boxStyle = new(GUI.skin.box);
            boxStyle.normal.textColor = Color.white;
            boxStyle.hover.textColor = Color.white;
            boxStyle.fontSize = _fontSize;

            var boxContent = new GUIContent($"{stringBuilder}");
            var size = boxStyle.CalcSize(boxContent);

            Vector2 worldPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * _offsetY);

            Rect position = new(
                worldPos.x - size.x / 2f,
                Screen.height - worldPos.y - size.y, // Y-axis flipped and adjusted to sit above
                size.x + _padding.x,
                size.y + _padding.y
            );

            GUI.Box(position, boxContent, boxStyle);
        }

        [BoxGroup("Debug", order: 100)]
        [Button("Clear Debug Log")]
        private void ClearDebug() {
            _debugStates.Clear();
        }

        private void OnDrawGizmos() {
            _activeState?.OnDrawGizmos();
        }

        private void OnDrawGizmosSelected() {
            _activeState?.OnDrawGizmosSelected();
        }
    }
}