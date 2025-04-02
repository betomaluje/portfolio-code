using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BerserkPixel.Health.FX {
    public class FlashFX : MonoBehaviour, IFX {
        [Tooltip("Material to switch to during the flash.")]
        [SerializeField]
        private Material flashMaterial;

        [SerializeField]
        private Renderer[] rend;

        [Tooltip("Duration of the flash.")]
        [SerializeField]
        private float duration = .2f;

        [SerializeField]
        private int numberOfFlashes = 2;

        private readonly int _property = Shader.PropertyToID("_HitEffectBlend");

        // The material that was in use, when the script started.
        // private Material[] _originalMaterial;
        private readonly Dictionary<Renderer, Material> _originalMaterials = new();

        public FXType GetFXType() => FXType.OnlyNotImmune;

        public FXLifetime LifetimeFX => FXLifetime.Always;

        private CancellationToken cancellationToken;

        private void OnValidate() {
            if (rend == null || rend.Length == 0) {
                var spriteObject = transform.parent.Find("Sprite");
                if (spriteObject != null) {
                    rend = new Renderer[1];
                    rend[0] = spriteObject.GetComponent<SpriteRenderer>();
                }
            }
        }

        private void Awake() {
            cancellationToken = this.GetCancellationTokenOnDestroy();
        }

        private void Start() {
            // _originalMaterial = new Material[rend.Length];
            UpdateOriginalMaterials();
        }

        private void UpdateOriginalMaterials() {
            // for (var i = 0; i < rend.Length; i++) {
            //     _originalMaterial[i] = rend[i].material;
            // }
            foreach (var renderer in rend) {
                if (renderer == null)
                    continue;

                if (_originalMaterials.ContainsKey(renderer)) {
                    _originalMaterials[renderer] = renderer.material;
                }
                else {
                    _originalMaterials.Add(renderer, renderer.material);
                }
            }
        }

        private void OnDestroy() {
            SetOriginalMaterials();
        }

        public void DoFX(HitData hitData) {
            // If the flashRoutine is not null, then it is currently running.
            FlashRoutine();
        }

        private async void FlashRoutine() {
            var durationPerFlash = duration / numberOfFlashes;
            // we divide by 2 since we need to turn to flash and back to original with a pause
            int waitingTime = (int)(durationPerFlash / 2 * 1000);

            UpdateOriginalMaterials();
            SetFlashMaterials();

            for (var i = 0; i < numberOfFlashes; i++) {
                try {
                    // Swap to the flashMaterial.
                    flashMaterial.SetFloat(_property, 1);

                    await UniTask.Delay(waitingTime, cancellationToken: cancellationToken);

                    // After the pause, swap back to the original material.
                    flashMaterial.SetFloat(_property, 0);

                    // so we show the original material for the same amount of time
                    await UniTask.Delay(waitingTime, cancellationToken: cancellationToken);
                }
                catch (OperationCanceledException) { }
            }

            try {
                SetOriginalMaterials();
            }
            catch (MissingReferenceException) { }
        }

        private void SetOriginalMaterials() {
            if (rend == null || rend.Length == 0)
                return;

            // for (var i = 0; i < rend.Length; i++) {
            //     rend[i].material = _originalMaterial[i];
            // }
            foreach (var renderer in rend) {
                if (renderer == null)
                    continue;

                if (_originalMaterials.TryGetValue(renderer, out var material)) {
                    renderer.material = material;
                }
            }
        }

        private void SetFlashMaterials() {
            if (rend == null || rend.Length == 0)
                return;

            for (var i = 0; i < rend.Length; i++) {
                rend[i].material = flashMaterial;
            }
        }
    }
}