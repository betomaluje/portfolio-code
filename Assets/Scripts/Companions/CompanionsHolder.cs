using System.Collections.Generic;
using System.Linq;
using Companions.States;
using Cysharp.Threading.Tasks;
using DebugTools;
using UnityEngine;
using static Storage.Player.PlayerMiniModel;

namespace Companions {
    public class CompanionsHolder : MonoBehaviour {
        [SerializeField]
        private float _distanceFromRoomCenter = 1f;

        private Dictionary<string, CompanionStateMachine> _companions;
        private Dictionary<string, List<string>> _equippedCompanions = new();

        public List<CompanionModel> EquippedCompanions => _equippedCompanions.Select(x => {
            var model = new CompanionModel {
                Name = x.Key,
                Colors = x.Value
            };
            return model;
        }).Where(x => x.Colors != null && x.Colors.Count > 0).ToList();

        private void Awake() {
            _companions = Resources.LoadAll<CompanionStateMachine>("Companions").ToDictionary(x => x.name, x => x);
        }

        public async void LoadCompanions(List<CompanionModel> companions) {
            _equippedCompanions.Clear();

            var tasks = new List<UniTask>();

            foreach (var companion in companions) {
                var amount = companion.Colors.Count;

                _equippedCompanions[companion.Name] = companion.Colors;

                for (int i = 0; i < amount; i++) {
                    var instance = SpawnCompanion(_companions[companion.Name], companion.Colors[i]);
                    tasks.Add(CompanionFollowState(instance));
                }
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask CompanionFollowState(CompanionStateMachine companion) {
            await UniTask.Delay(500);
            companion.SetState(typeof(CompanionFollowState));
        }

        private CompanionStateMachine SpawnCompanion(CompanionStateMachine companion, string colorHex) {
            Vector2 playerPosition = transform.position;
            Vector2 nearPlayer = playerPosition + Random.insideUnitCircle * _distanceFromRoomCenter;
            var companionInstance = Instantiate(companion, nearPlayer, Quaternion.identity);
            if (ColorUtility.TryParseHtmlString(colorHex, out Color result)) {
                companionInstance.Tint(result);
            }

            return companionInstance;
        }

        public void EquipCompanion(string name) {
            if (_companions.TryGetValue(name, out CompanionStateMachine companion)) {
                var randomColor = GetRandomColor();

				if (_equippedCompanions.TryGetValue(name, out List<string> currentCompanions)) {
					DebugLog.Log($"Companion {name} already equipped");
					currentCompanions.Add(randomColor);
				}
				else {
					currentCompanions = new List<string> { randomColor };
				}

				_equippedCompanions[companion.name] = currentCompanions;

                SpawnCompanion(companion, randomColor);
            }
        }

        private string GetRandomColor() {
            Color background = new(
                Random.value,
                Random.value,
                Random.value
            );
            var color = ColorUtility.ToHtmlStringRGB(background);
            if (!color.StartsWith("#")) {
                color = $"#{color}";
            }
            return color;
        }
    }
}