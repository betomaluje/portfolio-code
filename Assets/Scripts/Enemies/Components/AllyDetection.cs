using System;
using BerserkPixel.StateMachine;
using Cysharp.Threading.Tasks;
using Enemies.States;
using UnityEngine;
using Utils;

namespace Enemies.Components {
    /// <summary>
    /// The AllyDetection class is a MonoBehaviour that detects and handles allies. 
    /// It requires a `StateMachine<EnemyStateMachine>` component to be attached to the same game object.
    /// </summary>
    [RequireComponent(typeof(StateMachine<EnemyStateMachine>))]
    public class AllyDetection : Detection.Detection {
        [SerializeField]
        [Tooltip("The allowed ally detections. Determines what actions are allowed with allies.")]
        private AllyActionsTypes _allowedAllyDetections = AllyActionsTypes.None;

        [SerializeField]
        [Range(0, 20)]
        private float _approachTargetSpeed = 16f;

        [SerializeField]
        [Min(0.1f)]
        private float _minDistanceToTarget = 1f;

        [SerializeField]
        private string _animationTriggerName = "";

        private EnemyStateMachine _stateMachine;
        private IAllyActionDecorator[] _allAllyDecorator;
        private IAllyActionDecorator _chosenAllyDecorator;
        private bool _hasBeenReached;

        private void Awake() {
            _stateMachine = GetComponent<EnemyStateMachine>();
            _hasBeenReached = false;

            _allAllyDecorator = GetComponentsInChildren<IAllyActionDecorator>();
        }

        /// <summary>
        /// Called from Editor UnityEvent on TargetDetection script.
        /// It checks if the ally Transform has any of the allowed decorator actions.
        /// Basically it changes to a state that moves towards the ally. When target is reached it forwards the call to OnTargetReached
        /// </summary>
        public void HandleAllyDetected(Transform ally) {
            if (_stateMachine.CurrentState == typeof(EnemyAllyCheckState)) {
                return;
            }

            foreach (var allyDecorator in _allAllyDecorator) {
                if (allyDecorator.ConditionsMet(ally, _allowedAllyDetections)) {
                    _chosenAllyDecorator = allyDecorator;

                    var allyState = new EnemyAllyCheckState.Builder()
                        .WithTarget(ally)
                        .WithMinDistance(_minDistanceToTarget)
                        .WithSpeed(_approachTargetSpeed)
                        .Build();

                    _stateMachine.SetState(allyState);
                    break;
                }
            }
        }

        public async void OnTargetReached(Transform ally) {
            if (_chosenAllyDecorator == null || ally == null || _hasBeenReached) {
                return;
            }

            _hasBeenReached = true;
            _stateMachine.Movement.Stop();

            // play animation
            if (string.IsNullOrWhiteSpace(_animationTriggerName)) {
                _chosenAllyDecorator.DoAction(transform, ally);
            }
            else {
                var length = _stateMachine.Animations.GetAnimationLength(_animationTriggerName);
                _stateMachine.Animations.Play(_animationTriggerName);
                await UniTask.Delay(TimeSpan.FromSeconds(length));
                _chosenAllyDecorator.DoAction(transform, ally);
                Destroy(ally.gameObject);
            }
        }

        protected override void Detect() {
            var hit = _collider.DetectWithAngle(_targetMask, _detectionAngle, true);
            if (hit.transform != null) {
                _hasBeenReached = false;
                HandleAllyDetected(hit.transform);
            }
            else {
                // nothing, check if it has detected something before
                _hasDetected = false;
            }
        }
    }
}