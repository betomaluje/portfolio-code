using UnityEngine;

namespace Dungeon {
    public class DungeonGap : DungeonCallback {
        private float _gapChance = .3f;

        private Vector2 _gapSizeMin = new(3, 3);

        private Vector2 _gapSizeMax = new(5, 5);

        private readonly GameObject _gameObject;

        private DungeonGap(GameObject gameObject) {
            _gameObject = gameObject;
        }

        public override void OnMapStarts() {
            if (Random.value <= _gapChance) {
                var collider = _gameObject.AddComponent<BoxCollider2D>();
                var bounds = collider.size;
                bounds.x = Random.Range(_gapSizeMin.x, _gapSizeMax.x);
                bounds.y = Random.Range(_gapSizeMin.y, _gapSizeMax.y);
                collider.size = bounds;
            }
        }

        public override void OnMapLoaded() {
            base.OnMapLoaded();
            if (_gameObject != null && _gameObject.TryGetComponent<BoxCollider2D>(out var collider)) {
                Object.Destroy(collider);
            }
        }

        public class Builder {
            private float gapChance;
            private Vector2 gapSizeMin = new Vector2(3, 3);
            private Vector2 gapSizeMax = new Vector2(5, 5);

            public Builder WithChance(float gapChance) {
                this.gapChance = gapChance;
                return this;
            }

            public Builder WithMinGapSize(Vector2 gapSizeMin) {
                this.gapSizeMin = gapSizeMin;
                return this;
            }

            public Builder WithMaxGapSize(Vector2 gapSizeMax) {
                this.gapSizeMax = gapSizeMax;
                return this;
            }

            public DungeonGap Build(GameObject gameObject) {
                var gap = new DungeonGap(gameObject) {
                    _gapChance = gapChance,
                    _gapSizeMin = gapSizeMin,
                    _gapSizeMax = gapSizeMax
                };
                return gap;
            }
        }
    }
}