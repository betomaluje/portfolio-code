using UnityEngine;

namespace BerserkPixel.Health.FX {
    public class EnemyBlockFX : MonoBehaviour, IFX {
        [SerializeField]
        private ParticleSystem _blockParticles;

        public FXType GetFXType() => FXType.OnlyImmune;

        public FXLifetime LifetimeFX => FXLifetime.OnlyAlive;

        public void DoFX(HitData hitData) {
            _blockParticles?.Play();
        }
    }
}