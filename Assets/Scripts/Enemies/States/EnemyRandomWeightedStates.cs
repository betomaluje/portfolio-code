using System;
using BerserkPixel.StateMachine;
using BerserkPixel.Utils;
using UnityEngine;

namespace Enemies.States {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Weighted Random")]
    public class EnemyRandomWeightedStates : State<EnemyStateMachine> {
        [SerializeField]
        private WeightedListItem<State<EnemyStateMachine>>[] _states;

        [SerializeField]
        [Min(0f)]
        private float _delay = 0f;

        [SerializeField]
        private int _seed = -1;

        private WeightedList<State<EnemyStateMachine>> _randomObjects;

        private State<EnemyStateMachine> _chosenState;

        private float _elapsedTime;

        public override void Enter(EnemyStateMachine parent) {
            base.Enter(parent);
            if (_randomObjects == null) {
                if (_seed == -1) {
                    _seed = DateTime.Now.TimeOfDay.Seconds;
                }
                System.Random random = new(_seed);
                _randomObjects = new(_states, random);
            }

            _chosenState = _randomObjects.Next();
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime >= _delay && _chosenState != null) {
                _machine.SetState(_chosenState);
            }
        }
    }
}