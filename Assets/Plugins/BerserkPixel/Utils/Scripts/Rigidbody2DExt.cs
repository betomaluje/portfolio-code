using System.Collections;
using UnityEngine;

namespace BerserkPixel.Utils {
    public static class Rigidbody2DExt {
        public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition,
            float explosionRadius, float upliftModifier) {
            var dir = body.transform.position - explosionPosition;
            var wearoff = 1 - dir.magnitude / explosionRadius;
            var baseForce = dir.normalized * (wearoff <= 0f ? 0f : explosionForce) * wearoff;
            body.AddForce(baseForce);

            var upliftWearoff = 1 - upliftModifier / explosionRadius;
            Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
            body.AddForce(upliftForce);
        }

        public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 emitterPosition,
            float upwardsModifier = 0.0F, ForceMode2D mode = ForceMode2D.Impulse) {
            var explosionDir = (rb.position - emitterPosition).normalized;
            var explosionDistance = explosionDir.magnitude;

            // Normalize without computing magnitude again
            if (upwardsModifier == 0 && explosionDistance > 0) {
                explosionDir /= explosionDistance;
            }
            else {
                // From Rigidbody.AddExplosionForce doc:
                // If you pass a non-zero value for the upwardsModifier parameter, the direction
                // will be modified by subtracting that value from the Y component of the centre point.
                explosionDir.y += upwardsModifier;
                explosionDir.Normalize();
            }

            rb.AddForce(Mathf.Lerp(0, explosionForce, 1 - explosionDistance) * explosionDir, mode);
        }

        public static void AddKnockBack(this Rigidbody2D rb, MonoBehaviour caller, float knockBackForce,
            Vector2 emitterPosition, float resetTimer) {
            var difference = rb.position - emitterPosition;
            difference = difference.normalized * knockBackForce;
            rb.AddForce(difference, ForceMode2D.Impulse);
            caller.StartCoroutine(KnockBackReset(rb, resetTimer, false));
        }

        public static void AddRockKnockBack(this Rigidbody2D rb, MonoBehaviour caller, float knockBackForce,
            Vector2 emitterPosition, float resetTimer) {
            rb.bodyType = RigidbodyType2D.Dynamic;
            var difference = rb.position - emitterPosition;
            difference = difference.normalized * knockBackForce;
            rb.AddForce(difference, ForceMode2D.Impulse);
            caller.StartCoroutine(KnockBackReset(rb, resetTimer, true));
        }

        private static IEnumerator KnockBackReset(Rigidbody2D rb, float resetTimer, bool resetKinematic) {
            yield return new WaitForSeconds(resetTimer);
            if (rb != null) {
                rb.linearVelocity = Vector2.zero;
                if (resetKinematic) {
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }
            }
        }
    }
}