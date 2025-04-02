using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extensions;
using Player.Input;
using UnityEngine;
using UnityEngine.VFX;

namespace Dungeon.FogOfWar {
    public class DungeonFogOfWar : MonoBehaviour, IDungeonCallback {
        [SerializeField]
        private VisualEffect _fogOfWarVfx;

        [SerializeField]
        private Vector2 _offset = Vector2.zero;

        private Transform _dungeonContainer;
        private readonly Dictionary<int, Transform> _allRooms = new();
        private List<Room> _selectedRooms = new();
        private PreferencesStorage _preferencesStorage = new();

        private void Start() {
            _fogOfWarVfx.Stop();
        }

        private void OnEnable() {
            PreferencesStorage.OnPreferenceChanged += ToggleFogOfWar;
        }

        private void OnDisable() {
            PreferencesStorage.OnPreferenceChanged -= ToggleFogOfWar;
        }

        private void OnDestroy() {
            PreferencesStorage.OnPreferenceChanged -= ToggleFogOfWar;
        }

        private void ToggleFogOfWar(string eventKey, bool active) {
            if (eventKey.Equals(PreferencesStorage.EVENT_FOG_OF_WAR)) {
                ToggleFogOfWar(active);
            }
        }

        private void ToggleFogOfWar(bool active) {
            _fogOfWarVfx.enabled = active;
            if (active) {
                _fogOfWarVfx.Play();
            }
            else {
                _fogOfWarVfx.Stop();
            }
        }

        public void OnMapStarts() { }

        public void OnAllRoomsGenerated(ref IList<Room> rooms, ref IList<Transform> roomTransforms) {
            _dungeonContainer = FindFirstObjectByType<DungeonMap>().transform.Find("DungeonContainer");

            for (int i = 0; i < roomTransforms.Count; i++) {
                _allRooms[i] = roomTransforms[i];
            }
        }

        public void OnRoomsSelected(ref IList<Room> selectedRooms) {
            _selectedRooms = selectedRooms.ToList();

            var mySet = new HashSet<int>(selectedRooms.Select(x => x.Index));

            var notSelected = _allRooms.Where(x => !mySet.Contains(x.Key))
                .ToDictionary(x => $"Room-{x.Key}", x => x.Value.parent.gameObject)
                .ToArray();

            FadeNotSelected(notSelected);
        }

        private void FadeNotSelected(KeyValuePair<string, GameObject>[] notSelected) {
            foreach (var destroy in notSelected) {
                destroy.Value.SetActive(false);
            }
        }

        public void OnRoomsToCenter(ref IList<Room> rooms, ref IList<Vector3Int> positions) { }

        public void OnMapLoaded() {
            // get the bounds for the selected rooms
            var bounds = CalculateBounds(_selectedRooms.Select(x => x.Center).ToList());

            // assign bounds to vfx
            _fogOfWarVfx.SetVector2("BoxSize", new Vector2(bounds.size.x, bounds.size.y));

            // find the player 
            var player = FindFirstObjectByType<PlayerBattleInput>().gameObject;

            // assign the player to the vfx graph
            var collider = player.GetOrAdd<ColliderFogOfWar>();
            collider.Setup(_fogOfWarVfx);

            ToggleFogOfWar(_preferencesStorage.GetShowFogOfWar());

            _dungeonContainer.gameObject.SetActive(false);
        }

        private Bounds CalculateBounds(List<Vector2Int> points) {
            if (points == null || points.Count == 0) {
                throw new ArgumentException("Points collection cannot be null or empty.");
            }

            // Calculate min and max for x and y
            int minX = points.Min(point => point.x);
            int maxX = points.Max(point => point.x);
            int minY = points.Min(point => point.y);
            int maxY = points.Max(point => point.y);

            // Calculate center of mass (cast to float for precision)
            Vector2 centerOfMass = new(
                (float)points.Average(point => point.x),
                (float)points.Average(point => point.y)
            );

            // Calculate size with offset applied
            Vector2 size = new Vector2(maxX - minX, maxY - minY) + _offset;

            // Create and return Bounds
            return new(centerOfMass, size);
        }
    }
}