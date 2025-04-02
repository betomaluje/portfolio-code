using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon {
    [CreateAssetMenu(fileName = "DungeonConfig", menuName = "Dungeon/DungeonConfig", order = 0)]
    public class DungeonConfig : ScriptableObject {
        public TileBase RuleTile;
        public Color MapTint = Color.white;
    }
}