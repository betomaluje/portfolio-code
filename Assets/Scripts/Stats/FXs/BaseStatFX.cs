using System;
using System.Collections.Generic;
using BerserkPixel.Health;
using UnityEngine;

namespace Stats.FXs {
    public abstract class BaseStatFX : MonoBehaviour, IStatFX {
        [SerializeField]
        private Material _material;

        [SerializeField]
        private Vector3 _offset = Vector3.zero;

        [SerializeField]
        private Transform[] _particlesPrefabs;

        [SerializeField]
        private ParticleSystem[] _particleSystems;

        protected SpriteRenderer[] _spriteRenderers;
        private readonly Dictionary<SpriteRenderer, Material> _originalMaterials = new();
        private readonly List<Transform> _instantiatedParticles = new();

        public abstract StatType StatType { get; }

        private CharacterHealth _characterHealth;

        public virtual void Awake() {
            _characterHealth = transform.parent.GetComponent<CharacterHealth>();
        }

        private void OnValidate() {
            transform.localPosition = Vector3.zero;
        }

        private void OnEnable() {
            _characterHealth.OnDie += HandleDeath;
        }

        private void OnDisable() {
            _characterHealth.OnDie -= HandleDeath;
        }

        private void HandleDeath() {
            gameObject.SetActive(false);
        }

        public virtual void Setup(SpriteRenderer[] spriteRenderers) {
            _spriteRenderers = spriteRenderers;

            foreach (var spriteRenderer in spriteRenderers) {
                _originalMaterials.Add(spriteRenderer, spriteRenderer.material);
            }
        }

        private void OnDestroy() {
            foreach (ParticleSystem particleSystem in _particleSystems) {
                particleSystem.Stop();
            }
        }

        public virtual void DoFX(StatType type, float amount) {
            foreach (var spriteRenderer in _spriteRenderers) {
                spriteRenderer.material = _material;
            }

            if (_particleSystems.Length > 0) {
                foreach (var particleSystem in _particleSystems) {
                    particleSystem.Play();
                }
            }

            if (_particlesPrefabs.Length > 0) {
                foreach (var particlePrefab in _particlesPrefabs) {
                    var a = Instantiate(particlePrefab, transform.position, Quaternion.identity, transform);
                    a.localPosition = _offset;
                    _instantiatedParticles.Add(a);
                }
            }
        }

        public virtual void ResetFX(StatType type, float amount) {
            foreach (var spriteRenderer in _spriteRenderers) {
                if (_originalMaterials.TryGetValue(spriteRenderer, out var material)) {
                    spriteRenderer.material = material;
                }
            }

            if (_instantiatedParticles.Count > 0) {
                foreach (var instantiatedParticle in _instantiatedParticles) {
                    Destroy(instantiatedParticle.gameObject);
                }
                _instantiatedParticles.Clear();
            }
        }
    }
}