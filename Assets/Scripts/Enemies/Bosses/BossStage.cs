using System.Collections.Generic;
using BerserkPixel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemies.Bosses {
    [CreateAssetMenu(menuName = "Aurora/Enemy/Boss/Stage")]
    [InlineEditor]
    public class BossStage : ScriptableObject {
        public Color HealthBarColor = Color.black;
        public float PercentageToActivate;
        public List<State<EnemyStateMachine>> States;
        public List<Weapons.Weapon> Weapons;
    }
}