using UI;

namespace Dungeon {
    public class DungeonUILoader : DungeonCallback {
        private readonly InGameUILoader _inGameUILoader;

        public DungeonUILoader(InGameUILoader inGameUILoader) {
            _inGameUILoader = inGameUILoader;
        }

        public override void OnMapLoaded() {
			base.OnMapLoaded();
            _inGameUILoader.LoadUI();
		}
	}
}