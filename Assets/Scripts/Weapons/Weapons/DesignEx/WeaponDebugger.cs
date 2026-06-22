using UnityEngine;
using Base;
using Weapons;

namespace Weapons.DesignEx {
    /// <summary>
    /// A simple diagnostic component to test new weapon behaviors in the editor.
    /// Attach this to a GameObject in your scene to simulate weapon firing.
    /// </summary>
    public class WeaponDebugger : MonoBehaviour {
        [Header("Testing Config")]
        [Tooltip("The weapon ScriptableObject to test.")]
        public Weapon weaponToTest;

        [Tooltip("Artificial charge power (0 to 1) for ICharge weapons.")]
        [Range(0f, 1f)] public float testChargePower = 1f;

        [Header("Scene References (Optional)")]
        [Tooltip("Where projectiles will spawn from.")]
        public Transform testSpawnPoint;

        [Tooltip("Animation controller to pass to the weapon.")]
        public CharacterAnimations testAnimations;

        private void Update() {
            // Press [Space] to fire the weapon in the direction the object is facing
            if (Input.GetKeyDown(KeyCode.Space)) {
                FireTest();
            }
        }

        /// <summary>
        /// Simulates a weapon attack with the configured debug parameters.
        /// </summary>
        [ContextMenu("Fire Test")]
        public void FireTest() {
            if (weaponToTest == null) {
                Debug.LogWarning("WeaponDebugger: No weapon assigned to test!");
                return;
            }

            Vector2 direction = transform.right;
            Vector3 position = testSpawnPoint != null ? testSpawnPoint.position : transform.position;

            // Handle charge if applicable
            if (weaponToTest is ICharge chargeWeapon) {
                chargeWeapon.Charge = testChargePower;
                Debug.Log($"WeaponDebugger: Firing {weaponToTest.Name} with Charge: {testChargePower}");
            } else {
                Debug.Log($"WeaponDebugger: Firing {weaponToTest.Name}");
            }

            // Execute attack
            weaponToTest.Attack(testAnimations, direction, position);
        }
    }
}
