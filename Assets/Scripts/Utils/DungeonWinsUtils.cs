namespace Dungeon {
    public static class DungeonWinsUtils {
        private static readonly string PRFS_DUNGEON_WINS = "_Wins";
        private static readonly string PRFS_DUNGEON_LOSSES = "_Losses";

        public static string GetWinsPrefsName(string sceneName) {
            if (string.IsNullOrEmpty(sceneName)) {
                return PRFS_DUNGEON_WINS;
            }
            return CleanSceneName(sceneName) + PRFS_DUNGEON_WINS;
        }

        public static string GetLossesPrefsName(string sceneName) {
            if (string.IsNullOrEmpty(sceneName)) {
                return PRFS_DUNGEON_WINS;
            }
            return CleanSceneName(sceneName) + PRFS_DUNGEON_LOSSES;
        }

        private static string CleanSceneName(string sceneName) => sceneName.Replace(" Scene", "").Replace(" ", "-").ToLower();
    }
}