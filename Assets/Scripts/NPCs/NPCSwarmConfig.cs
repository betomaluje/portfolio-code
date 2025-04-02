using Base.Swarm;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace NPCs {
    [CreateAssetMenu(menuName = "Aurora/Level/NPC Swarm Config")]
    public class NPCSwarmConfig : SwarmConfig<NPCStateMachine> {
        public SpriteLibraryAsset[] SpriteLibraryAsset;
    }
}