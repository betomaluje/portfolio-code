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
        private readonly Dictionary<Renderer, Material> _originalMaterials = new();
        private readonly Dictionary<Renderer, Material> _flashMaterials = new();

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
            CreateAndBackupMaterials();
        }

        private void CreateAndBackupMaterials() {
            foreach (var renderer in rend) {
                if (renderer == null)
                    continue;

                // backup materials to originals
                _originalMaterials[renderer] = renderer.material;

                // we create a new material to avoid modifying the original material
                var newFlashMat = new Material(flashMaterial);
                _flashMaterials[renderer] = newFlashMat;
            }
        }

        private void CleanupFlashMaterials() {
            foreach (var mat in _flashMaterials.Values) {
                if (mat != null) {
                    if (Application.isPlaying) {
                        Destroy(mat);
                    }
                    else {
                        DestroyImmediate(mat);
                    }
                }
            }
            _flashMaterials.Clear();
        }

        private void OnDestroy() {
            CleanupFlashMaterials();
            SetOriginalMaterials();
        }

        public void DoFX(HitData hitData) {
            // If the flashRoutine is not null, then it is currently running.
            FlashRoutine();
        }

        private void SwapMaterials(bool isFlashing) {
            foreach (var renderer in rend) {
                if (renderer == null)
                    continue;

                renderer.material = isFlashing ? _flashMaterials[renderer] : _originalMaterials[renderer];
            }
        }

        private async void FlashRoutine() {
            var durationPerFlash = duration / numberOfFlashes;
            // we divide by 2 since we need to turn to flash and back to original with a pause
            int waitingTime = (int)(durationPerFlash / 2 * 1000);

            SwapMaterials(true);

            try {
                for (var i = 0; i < numberOfFlashes; i++) {
                    // Enable flash
                    foreach (var mat in _flashMaterials.Values) {
                        if (mat.HasProperty(_property))
                            mat.SetFloat(_property, 1);
                    }

                    await UniTask.Delay(waitingTime, cancellationToken: cancellationToken);

                    // Disable flash
                    foreach (var mat in _flashMaterials.Values) {
                        if (mat.HasProperty(_property))
                            mat.SetFloat(_property, 0);
                    }

                    // so we show the original material for the same amount of time
                    await UniTask.Delay(waitingTime, cancellationToken: cancellationToken);
                }
            }
            catch (OperationCanceledException) { }
            finally {
                try {
                    SwapMaterials(false);
                }
                catch (MissingReferenceException) { }
            }
        }

        private void SetOriginalMaterials() {
            if (rend == null || rend.Length == 0)
                return;

            foreach (var renderer in rend) {
                if (renderer == null)
                    continue;

                if (_originalMaterials.TryGetValue(renderer, out var material)) {
                    renderer.material = material;
                }
            }
        }
    }
}