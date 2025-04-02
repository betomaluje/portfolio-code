using UnityEngine;

namespace BerserkPixel.Health.FX {
    public class BlockFX : MonoBehaviour, IFX {
        [SerializeField]
        private ParticleSystem _blockParticles;

        public FXType GetFXType() => FXType.OnlyImmune;

        public FXLifetime LifetimeFX => FXLifetime.Always;

        public void DoFX(HitData hitData) {
            _blockParticles?.Play();
        }
    }
}