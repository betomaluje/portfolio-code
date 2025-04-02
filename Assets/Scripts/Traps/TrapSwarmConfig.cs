using Base.Swarm;
using UnityEngine;

namespace Traps {
    [CreateAssetMenu(menuName = "Aurora/Level/Trap Swarm Config")]
    public class TrapSwarmConfig : SwarmConfig<Transform> {
        [Min(0)]
        public int AmountOfWaves = 1;
    }
}