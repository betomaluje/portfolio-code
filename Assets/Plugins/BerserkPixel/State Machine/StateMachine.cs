using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BerserkPixel.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BerserkPixel.StateMachine {
    public abstract class StateMachine<T> : MonoBehaviour, IStateAnimationTrigger where T : MonoBehaviour {
        [SerializeField]
        protected List<State<T>> _states;

        [BoxGroup("Debug", order: 100)]
        [SerializeField]
        private bool _displayActiveState = false;

        [BoxGroup("Debug", order: 100)]
        [ShowIf("_displayActiveState")]
        [SerializeField]
        private int _maxStatePrints = 3;

        [BoxGroup("Debug", order: 100)]
        [SerializeField]
        [Min(12)]
        [ShowIf("_displayActiveState")]
        private int _fontSize = 20;

        [BoxGroup("Debug", order: 100)]
        [SerializeField]
        [ShowIf("_displayActiveState")]
        private float _marginHorizontal = 0;

        [BoxGroup("Debug", order: 100)]
        [SerializeField]
        [ShowIf("_displayActiveState")]
        private float _marginVertical = 100;

        [BoxGroup("Debug", order: 100)]
        [SerializeField]
        [ShowIf("_displayActiveState")]
        [Tooltip("Padding for the version label. X is horizontal padding, Y is vertical padding.")]
        private Vector2Int _padding = new(10, 6);

        public Type CurrentState => _activeState != null ? _activeState.GetType() : default;

        private State<T> _activeState;

        private T _parent;

        private Queue<string> _debugStates;

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
            if (!_states.Contains(newState)) {
                _states.Add(newState);
            }
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

        private void OnGUI() {
            if (!_displayActiveState) {
                return;
            }

            StringBuilder stringBuilder = new();
            foreach (var state in _debugStates) {
                var content = !string.IsNullOrWhiteSpace(state) ? state.ToString() : "(no active state)";
                stringBuilder.AppendLine(content);
            }

            GUIStyle boxStyle = new(GUI.skin.box);
            boxStyle.normal.textColor = Color.black;
            boxStyle.hover.textColor = Color.black;
            boxStyle.fontSize = _fontSize;

            var boxContent = new GUIContent($"{stringBuilder}");
            var size = boxStyle.CalcSize(boxContent);
            var position = new Rect(_marginHorizontal, Screen.height - _marginVertical, size.x + _padding.x, size.y + _padding.y);

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