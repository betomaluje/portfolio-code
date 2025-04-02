using Utils;

namespace Dungeon {
    public class DungeonObjectPlacer : DungeonCallback {
        private readonly RandomObjectPlacer _randomObjectPlacer;

		public DungeonObjectPlacer(RandomObjectPlacer randomObjectPlacer) {
			_randomObjectPlacer = randomObjectPlacer;
		}

		override public void OnMapLoaded() {
            _randomObjectPlacer.PlaceRandomObjects();
        }
    }
}