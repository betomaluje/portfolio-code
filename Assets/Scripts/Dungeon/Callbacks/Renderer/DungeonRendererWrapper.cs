using UnityEngine.Tilemaps;

namespace Dungeon.Renderer {
    [System.Serializable]
    public class DungeonRendererWrapper {
        public Tilemap tilemap;
        public DungeonConfig config;
        public RenderType type = RenderType.Normal;
        public bool isActive = true;

        public enum RenderType {
            Normal,
            Walls
        }
    }
}