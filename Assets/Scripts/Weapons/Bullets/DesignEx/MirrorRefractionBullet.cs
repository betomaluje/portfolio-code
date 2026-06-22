using UnityEngine;
using BerserkPixel.Health;

namespace Weapons.DesignEx {
    /// <summary>
    /// A visual 'ghost' of an attack strike. 
    /// It doesn't move but deals damage once in its forward zone using HitData.
    /// </summary>
    public class MirrorRefractionBullet : BaseBullet {
        
        [Header("Mirror Ghost Config")]
        [Tooltip("The duration the ghost lingers visually.")]
        [SerializeField] private float _lingerTime = 0.8f;
        
        [Tooltip("The width of the mirror's strike zone.")]
        [SerializeField] private float _strikeRadius = 2.0f;

        private bool _hasStruck = false;
        private SpriteRenderer _display;
        private Color _initialColor;

        private void Start() {
            _display = GetComponentInChildren<SpriteRenderer>();
            if (_display != null) {
                _initialColor = _display.color;
            }
            
            // Mirror strikes instantly across its path
            ExecuteMirrorSlash();
            
            Destroy(gameObject, _lingerTime);
        }

        private void ExecuteMirrorSlash() {
            if (_hasStruck) return;
            _hasStruck = true;

            // Simple overlap circle for the ghost's strike zone
            Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, _strikeRadius, _targetMask);
            
            foreach (var col in results) {
                if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var mirrorHit = new HitDataBuilder()
                        .WithWeapon(_weapon)
                        .WithDamage(Mathf.CeilToInt(GetDamage() * 0.75f)) // Mirror images a bit weaker
                        .WithCriticalHitChance(0.2f)
                        .WithDirection(transform.right)
                        .Build(_owner, col.transform);
                        
                    health.PerformDamage(mirrorHit);
                }
            }

            PlayImpactSound(transform.position, "mirror_slash_echo");
            SpawnCollisionParticles(transform.position);
        }

        private void Update() {
            // Visual fade-out
            if (_display != null) {
                float timeRatio = (Time.time % _lingerTime) / _lingerTime;
                Color nextCol = _initialColor;
                nextCol.a = Mathf.Lerp(_initialColor.a, 0f, timeRatio);
                _display.color = nextCol;
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _strikeRadius);
        }
    }
}
