using BerserkPixel.StateMachine;
using UnityEngine;

namespace Enemies.States {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Play Animation")]
    public class EnemyPlayAnimationState : State<EnemyAnimationStateMachine> {
        [SerializeField]
        private string _animationName;

        public override void Enter(EnemyAnimationStateMachine parent) {
            base.Enter(parent);
            parent.Animations.Play(_animationName);
        }

        public override void ChangeState() {

        }
    }
}