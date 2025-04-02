namespace Enemies.Components {
    /*
    None = 0,   // or define values via bit shifts:
    Head = 1,   // 1 << 0
    Torso = 2,  // 1 << 1
    Arms = 4,   // 1 << 2
    Hands = 8,  // 1 << 3
    */
    public enum AllyActionsTypes {
        None = 0,   // or define values via bit shifts:
        Revive = 1 << 0,
        Powerup = 1 << 1,
        Multiply = 1 << 2,
        Heal = 1 << 3,
        AddWeapon = 1 << 4,
    }

    public static class AllyActionsTypesExt {
        public static bool HasFlags(this AllyActionsTypes self, AllyActionsTypes flag) {
            return (self & flag) == flag;
        }
    }
}