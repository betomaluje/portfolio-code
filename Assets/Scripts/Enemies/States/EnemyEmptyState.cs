using BerserkPixel.StateMachine;
using Extensions;
using UnityEngine;

namespace Enemies.States {
  internal class EnemyEmptyState : State<EnemyStateMachine> {
    public override void Enter(EnemyStateMachine parent) {
      base.Enter(parent);
      var renderers = parent.GetComponentsInChildren<SpriteRenderer>();
      renderers?.Tint(Color.white);
    }

    public override void ChangeState() { }
  }
}