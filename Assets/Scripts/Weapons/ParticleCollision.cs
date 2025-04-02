using System.Collections.Generic;
using BerserkPixel.Health;
using BerserkPixel.Utils;
using UnityEngine;

namespace Weapons {
    public class ParticleCollision : MonoBehaviour {
        [SerializeField]
        private LayerMask _targetMask;

        [SerializeField]
        [Tooltip("Weapon that is being used to calculate damage")]
        private Weapon _weapon;

        private ParticleSystem _part;
        private readonly List<ParticleCollisionEvent> _collisionEvents = new();

        private void Start() {
            _part = GetComponent<ParticleSystem>();
        }

        private void OnParticleCollision(GameObject other) {
            if (_targetMask.LayerMatchesObject(other)) {
                var numCollisionEvents = _part.GetCollisionEvents(other, _collisionEvents);
                for (var i = 0; i < numCollisionEvents; i++) {
                    var collision = _collisionEvents[i];
                    var dir = (collision.intersection - transform.position).normalized;
                    var hitData = new HitDataBuilder()
                        .WithWeapon(_weapon)
                        .WithDirection(dir)
                        .Build(transform, other.transform);
                    hitData.PerformDamage(other);
                }
            }
        }
    }
}