using Base;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(menuName = "Aurora/Weapons/Fullscreen Weapon")]
    public class BossFullscreenConfig : Weapon {
        [Header("Fullscreen Settings")]

        public PoolBullet ProjectilePrefab;
        public float TimeToActivate = 1.0f;
        [Min(1)]
        public int ProjectileCount = 5;
        public float TotalRange = 8f;

        [Header("Post Processing")]
        public string PostProcessingProfile;

        public FullscreenDirection Direction = FullscreenDirection.Horizontal;

        [Header("Screen Shake")]
        public float ScreenShakeDuration = 3f;
        public float ScreenShakeMagnitude = 10f;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (ProjectilePrefab == null) {
                return;
            }

            FullscreenBossWeapon.Shoot?.Invoke(this);
        }
    }

    [System.Serializable]
    public enum FullscreenDirection {
        Horizontal,
        Vertical
    }
}