using Level;

namespace Dungeon {
    public class DungeonPostProcessing : DungeonCallback {
        public override void OnMapStarts() {
            PostProcessingManager.Instance.SetProfile("MapGeneration");
        }

        public override void OnMapLoaded() {
            PostProcessingManager.Instance.Reset();
        }
    }
}