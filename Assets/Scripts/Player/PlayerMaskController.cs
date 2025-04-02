using System;
using System.Collections.Generic;
using BerserkPixel.Utils;
using Extensions;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Player {
    public class PlayerMaskController : MonoBehaviour {
        [SerializeField]
        private SpriteMask _spriteMask;

        [SerializeField]
        private LayerMask _targetLayer;

        [Header("Animation")]
        [SerializeField]
        private float _minAlpha = 0.5f;

        [SerializeField]
        private float _animDuration = .25f;

        public Action OnMaskStart = delegate { };
        public Action OnMaskEnd = delegate { };

        private const string fadeProperty = "_Alpha";

        private readonly int Anim_Mask = Animator.StringToHash("Mask_Idle");
        private readonly int Anim_Nothing = Animator.StringToHash("Mask_Nothing");
        private Animator _anim;

        private readonly HashSet<SpriteRenderer> _collisionedRenderers = new();

        private void Awake() {
            _anim = GetComponent<Animator>();
        }

        private async void RestoreAlpha(Collider2D other) {
            var spriteRenderers = other.GetComponentsInChildren<SpriteRenderer>();

            if (spriteRenderers == null || spriteRenderers.Length == 0) return;

            var tasks = new List<UniTask>();
            var token = this.GetCancellationTokenOnDestroy();

            foreach (var sr in spriteRenderers) {
                tasks.Add(DoFade(sr, 1f, token));
            }

            try {
                await UniTask.WhenAny(tasks);
            }
            catch (OperationCanceledException) {
                return;
            }
        }

        private async void DoAlpha(Collider2D other) {
            var spriteRenderers = other.GetComponentsInChildren<SpriteRenderer>();

            if (spriteRenderers == null || spriteRenderers.Length == 0) return;

            var tasks = new List<UniTask>();

            var token = this.GetCancellationTokenOnDestroy();

            foreach (var sr in spriteRenderers) {
                var alpha = sr.color.a;
                if (_minAlpha < alpha) {
                    tasks.Add(DoFade(sr, _minAlpha, token));
                }
            }

            try {
                await UniTask.WhenAny(tasks);
            }
            catch (OperationCanceledException) {
                return;
            }
        }

        private async UniTask DoFade(SpriteRenderer spriteRenderer, float to, CancellationToken token) {
            if (!spriteRenderer.material.HasProperty(fadeProperty)) {
                return;
            }
            
            var preAmount = spriteRenderer.material.GetFloat(fadeProperty);
            var elapsed = 0f;
            while (elapsed < _animDuration) {
                token.ThrowIfCancellationRequested();
                await UniTask.Yield(cancellationToken: token);

                elapsed += Time.deltaTime;
                var percentage = Mathf.Lerp(preAmount, to, elapsed / _animDuration);

                spriteRenderer.material.SetFloat(fadeProperty, percentage);
            }

            if (spriteRenderer != null) {
                spriteRenderer.material.SetFloat(fadeProperty, to);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.isTrigger || !_targetLayer.LayerMatchesObject(other)) return;

            if (other.gameObject.FindInChildren<SpriteRenderer>(out var renderer) && !_collisionedRenderers.Contains(renderer)) {
                _collisionedRenderers.Add(renderer);
                _anim.Play(Anim_Mask);
                DoAlpha(other);
                OnMaskStart?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (!other.isTrigger || !_targetLayer.LayerMatchesObject(other)) return;

            if (other.gameObject.FindInChildren<SpriteRenderer>(out var renderer) && _collisionedRenderers.Contains(renderer)) {
                if (gameObject.activeInHierarchy) {
                    _anim.Play(Anim_Nothing);
                }
                
                _collisionedRenderers.Clear();
                RestoreAlpha(other);
                OnMaskEnd?.Invoke();
            }
        }
    }
}