using BerserkPixel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemies.States {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Dead")]
    internal class EnemyDeadState : State<EnemyStateMachine> {
        [SerializeField]
        private bool _canBeRevived = true;

        [ShowIf("_canBeRevived")]
        [SerializeField]
        private EnemyStateMachine _skeletonPrefab;

        [ShowIf("_canBeRevived")]
        [SerializeField]
        private Transform _reviveFx;

        public override void Enter(EnemyStateMachine parent) {
            base.Enter(parent);
            parent.MakeBodyDynamic();
            parent.Animations.PlayDead();
            parent.Movement.Stop();

            if (!_canBeRevived) {
                parent.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }

        public override void ChangeState() => _machine.SetState(CreateInstance<EnemyEmptyState>());
    }
}