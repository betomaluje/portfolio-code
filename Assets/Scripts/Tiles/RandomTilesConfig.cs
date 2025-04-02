using System.Globalization;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Utils;
using UnityEngine;
using Random = System.Random;

namespace Tiles {
    [CreateAssetMenu(menuName = "Aurora/Tiles/Random Config")]
    public class RandomTilesConfig : ScriptableObject {
        [SerializeField]
        private TileRandom[] _sprites;

        [InspectorButton(nameof(GenerateSeed))]
        public string seed;

        private Random _random = new();

        private WeightedList<Sprite> _weightedList;

        private void OnValidate() {
            if (_sprites != null && _sprites.Length > 0) {
                CreateList();
            }
        }

        private void GenerateSeed() {
            var random = (float) (Time.time / new Random().NextDouble());
            seed = random.ToString(CultureInfo.CurrentCulture);
            CreateList();
        }

        public Sprite GetRandomSprite() {
            if (_weightedList != null && _weightedList.Count > 0) {
                return _weightedList.Next();
            }

            return _sprites[0].Sprite;
        }

        private void CreateList() {
            _random = new Random(seed.GetHashCode());
            _weightedList = new WeightedList<Sprite>(_random);
            foreach (var sprite in _sprites) {
                _weightedList.Add(sprite.Sprite, sprite.Weight);
            }
        }
    }
}