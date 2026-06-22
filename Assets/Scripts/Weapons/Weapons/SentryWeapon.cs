using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A weapon that deploys an automated Sentry Turret.
    /// </summary>
    [RequiredBullet(typeof(SentryTurret))]
    [CreateAssetMenu(fileName = "SentryWeapon", menuName = "Aurora/Weapons/Sentry Weapon")]
    public class SentryWeapon : SpawnObjectWeapon {
        // Inherits most behavior from SpawnObjectWeapon, 
        // which instantiates the ObjectPrefab (set to a SentryTurret in the editor).

    }
}
