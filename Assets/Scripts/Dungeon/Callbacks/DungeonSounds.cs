using System.Collections.Generic;
using Sounds;

namespace Dungeon {
    public class DungeonSounds : DungeonCallback {
        public override void OnMapStarts() {
            SoundManager.instance.Play("dungeon_start");
        }

        public override void OnRoomsSelected(ref IList<Room> selectedRooms) {
            SoundManager.instance.Play("dungeon_loaded");
        }
    }
}