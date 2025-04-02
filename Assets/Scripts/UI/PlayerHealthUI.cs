using BerserkPixel.Health;
using Player.Input;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(TextHealthBar))]
    public class PlayerHealthUI : MonoBehaviour {
        private void Awake() {
            var player = FindFirstObjectByType<PlayerBattleInput>();
            var textHealthBar = GetComponent<TextHealthBar>();
            if (player != null && player.TryGetComponent<CharacterHealth>(out var health)) {
                textHealthBar.SetCharacterHealth(health);
                textHealthBar.HandleHealth(health.CurrentHealth, health.MaxHealth);
            }
        }
    }
}