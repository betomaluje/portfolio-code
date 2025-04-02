using BerserkTools.Health.UI;
using Extensions;
using Interactable;
using Scene_Management;
using Sirenix.OdinInspector;
using Sounds;
using UnityEngine;

namespace Portal {
    public class Portal : MonoBehaviour, IInteract {
        [SerializeField]
        [InlineEditor]
        [Required]
        private PortalConfig _config;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        private bool _isActive = true;

        private bool _hasInteracted;

        private InteractTag _interactTag;
        private ProgressBar _progressBar;
        private ParticleSystem _portalParticles;

        private void Awake() {
            _progressBar = GetComponentInChildren<ProgressBar>();
            _portalParticles = GetComponentInChildren<ParticleSystem>();
            this.FindInChildren(out _interactTag);
        }

        private void Start() {
            UpdatePortalStatus();

            UpdatePortalUI();
        }

        private void UpdatePortalStatus() => _isActive = _config.IsThisPortalAvailable();

        private void UpdatePortalUI() {
            var material = _spriteRenderer.material;

            var realmName = _config.RealmName;

            if (realmName.Contains(" ")) {
                realmName = realmName.Split(" ")[0];
            }

            _interactTag.ObjectName = $"portal to {realmName}";

            if (_isActive) {
                material.SetFloat("_GreyscaleBlend", 0f);
                material.SetFloat("_GhostBlend", 0f);

                if (_progressBar) {
                    _progressBar.ChangePercentage(_config.GetProgress());
                }
            }
            else {
                material.SetFloat("_GreyscaleBlend", 1f);
                material.SetFloat("_GhostBlend", 1f);

                ParticleSystem.EmissionModule emission = _portalParticles.emission;
                emission.rateOverTime = 2f;
                ParticleSystem.MainModule mainModule = _portalParticles.main;
                mainModule.startSpeed = -.5f;

                if (_progressBar) {
                    _progressBar.gameObject.SetActive(false);
                }

                if (_interactTag) {
                    _interactTag.gameObject.SetActive(false);
                }
            }
        }

        public void DoInteract() {
            if (!_isActive) {
                return;
            }

            if (_hasInteracted) {
                return;
            }

            _hasInteracted = true;

            // only when it's the intro dungeon portal, we set the first time.
            if (string.IsNullOrEmpty(_config.RealmName) && _config.TargetScene.SceneName == "VillageScene") {
                PreferencesStorage preferencesStorage = new();
                preferencesStorage.SetFirstTime(false);
            }

            string targetScene = _config.GetTargetScene();

            SoundManager.instance.Play("build_place");
            SceneLoader.Instance.LoadScene(targetScene);
        }

        public void CancelInteraction() { }
    }
}