using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BerserkPixel.Health.FX {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterHealth))]
    public class HealthFXController : MonoBehaviour {
        private CharacterHealth _characterHealth;
        private List<IFX> _allFxs;

        private void Awake() {
            _characterHealth = GetComponent<CharacterHealth>();
            _allFxs = GetComponentsInChildren<IFX>().ToList();
        }

        private void OnEnable() {
            _characterHealth.OnDamagePerformed += PerformFXs;
            _characterHealth.OnDie += RemoveAliveFXs;
        }

        private void OnDisable() {
            _characterHealth.OnDamagePerformed -= PerformFXs;
            _characterHealth.OnDie -= RemoveAliveFXs;
        }

        private void RemoveAliveFXs() {
            _allFxs = _allFxs.Where(fx => fx.LifetimeFX != FXLifetime.OnlyAlive).ToList();
        }

        private void PerformFXs(HitData hitData) {
            foreach (var fx in _allFxs) {
                switch (fx.GetFXType()) {
                    case FXType.Always:
                        fx.DoFX(hitData);
                        break;
                    case FXType.OnlyImmune when _characterHealth.IsImmune:
                    case FXType.OnlyNotImmune when !_characterHealth.IsImmune:
                        fx.DoFX(hitData);
                        break;
                }
            }
        }
    }
}