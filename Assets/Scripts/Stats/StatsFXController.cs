using System.Collections.Generic;
using System.Linq;
using Stats.FXs;
using UnityEngine;

namespace Stats {
    [RequireComponent(typeof(IStatsModifier))]
    public class StatsFXController : MonoBehaviour {
        [SerializeField]
        private SpriteRenderer[] _spriteRenderers;

        private IStatsModifier _statsModifier;
        private List<BaseStatFX> _allFxs;

        private void Awake() {
            _statsModifier = GetComponent<IStatsModifier>();
            _allFxs = GetComponentsInChildren<BaseStatFX>().ToList();
        }

        private void OnValidate() {
            if (_spriteRenderers == null || _spriteRenderers.Length == 0) {
                var spriteObject = transform.Find("Sprite");
                if (spriteObject != null) {
                    _spriteRenderers = new SpriteRenderer[1];
                    _spriteRenderers[0] = spriteObject.GetComponent<SpriteRenderer>();
                }
            }
        }

        private void Start() {
            foreach (BaseStatFX fx in _allFxs) {
                fx.Setup(_spriteRenderers);
            }
        }

        private void OnEnable() {
            _statsModifier.OnStatModifierAdded += UpdateFX;
            _statsModifier.OnStatModifierReset += ResetFX;
        }

        private void OnDisable() {
            _statsModifier.OnStatModifierAdded -= UpdateFX;
            _statsModifier.OnStatModifierReset -= ResetFX;
        }

        private void UpdateFX(StatType type, float amount) {
            foreach (BaseStatFX fx in _allFxs) {
                if (fx.StatType == type) {
                    fx.DoFX(type, amount);
                }
            }
        }

        private void ResetFX(StatType type, float amount) {
            foreach (BaseStatFX fx in _allFxs) {
                if (fx.StatType == type) {
                    fx.ResetFX(type, amount);
                }
            }
        }
    }
}