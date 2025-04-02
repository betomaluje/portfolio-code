using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EditorTool.Storage;
using Extensions;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using Random = System.Random;

namespace EditorTool {
    /// <summary>
    /// Battle grid for the editor
    /// </summary>
    public class BattleGrid : Singleton<BattleGrid> {
        [SerializeField]
        private int _width = 26;

        [SerializeField]
        private int _height = 16;

        [SerializeField]
        private int _gridSize = 1;

        [SerializeField]
        private Vector2Int _gridOffset = new(0, 0);

        [SerializeField]
        private Transform _objectContainer;

        [SerializeField]
        private Transform _mockObjectContainer;

        [Header("FX")]
        [SerializeField]
        private float _animDuration = .2f;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField]
        private bool _debug = false;

        [SerializeField]
        private Color _debugColor = Color.grey;
#endif
        [Space]
        [Header("Events")]
        [Space]
        [Tooltip("Called whenever an object has been placed on the grid")]
        public UnityEvent<GameObject> OnObjectPlaced;

        public UnityEvent OnGridLoaded;

        public Dictionary<Vector3Int, Tool> PlacedTools { get; private set; }
        public Dictionary<Vector3Int, GameObject> PlacedObjects { get; private set; }

        private Vector3 _centerPosition;
        private HashSet<Vector3Int> _points;
        private IGridStorage _storage;

        private HashSet<Tool> _availableTools = new();

        protected override void Awake() {
            base.Awake();
            _storage = GetComponent<CloudGridStorage>();
            _points = EvaluateGridPoints();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (!_debug) {
                return;
            }

            Gizmos.color = _debugColor;

            var size = Vector2.one * _gridSize;
            var points = EvaluateGridPoints();
            foreach (var point in points) {
                Gizmos.DrawWireCube(point, size);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_centerPosition, .4f);
            Handles.Label(_centerPosition, "Center");
        }
#endif

        public void SaveGrid() {
            _storage.SaveGrid();
        }

        /// <summary>
        /// Loads a grid and places Mock Objects on it.
        /// </summary>
        public void LoadMockGrid() {
            if (_availableTools.Count == 0) {
                _availableTools = Resources.LoadAll<Tool>("Tools").ToHashSet();
            }
            _storage.LoadGrid(loadedObjects => {
                foreach (var tool in loadedObjects) {
                    var newTool = _availableTools.FirstOrDefault(t => t.name == tool.Name);
                    var position = new Vector3(tool.X, tool.Y);
                    PlaceMockObject(position, newTool);
                }
                OnGridLoaded?.Invoke();
            });
        }

        /// <summary>
        /// Loads a grid and places playable Objects on it.
        /// </summary>
        public void LoadGrid() {
            if (_availableTools.Count == 0) {
                _availableTools = Resources.LoadAll<Tool>("Tools").ToHashSet();
            }

            _storage.LoadGrid(loadedObjects => {
                foreach (var tool in loadedObjects) {
                    var newTool = _availableTools.FirstOrDefault(t => t.name == tool.Name);
                    var position = new Vector3(tool.X, tool.Y);
                    PlaceEditorObject(position, newTool);
                }
                OnGridLoaded?.Invoke();
            });
        }

        public void LoadOtherPlayerGrid(string playerId) {
            if (_availableTools.Count == 0) {
                _availableTools = Resources.LoadAll<Tool>("Tools").ToHashSet();
            }

            _storage.LoadPlayerGrid(loadedObjects => {
                // TODO: do something if there is an error loading the objects. Check if it's empty
                foreach (var tool in loadedObjects) {
                    var newTool = _availableTools.FirstOrDefault(t => t.name == tool.Name);
                    var position = new Vector3(tool.X, tool.Y);
                    PlaceObject(position, newTool);
                }

                OnGridLoaded?.Invoke();
            }, playerId);
        }

        public Vector3Int GetRandomPoint() {
            if (_points == null || !_points.Any()) {
                _points = EvaluateGridPoints().SimpleShuffle().ToHashSet();
            }

            var random = new Random();
            return _points.ElementAt(random.Next(_points.Count));
        }

        public ImmutableList<Vector3Int> GetRandomPoints(int amount) {
            if (_points == null || !_points.Any()) {
                _points = EvaluateGridPoints().SimpleShuffle().ToHashSet();
            }

            return _points.Take(amount).ToImmutableList();
        }

        /// <summary>
        /// Removes a position from the saved list of placed objects.
        /// </summary>
        /// <param name="position"></param>
        public Tool RemoveObject(Vector3Int position) {
            if (PlacedObjects.TryGetValue(position, out var prefab) && PlacedTools.TryGetValue(position, out var tool)) {
                PlacedObjects.Remove(position);
                PlacedTools.Remove(position);
                Destroy(prefab);
                return tool;
            }
            else {
                // we try using Physics2D.OverlapCircle to see if the object is placed on the grid
                var colliders = Physics2D.OverlapCircleAll((Vector3)position, _gridSize + .2f);
                if (colliders != null && colliders.Any()) {
                    foreach (var collider in colliders) {
                        var estimatedPosition = collider.transform.position.ToInt();
                        var selected = PlacedObjects.First(x => x.Key == estimatedPosition).Value;
                        if (selected) {

                            if (PlacedTools.TryGetValue(estimatedPosition, out var tool2)) {
                                PlacedObjects.Remove(estimatedPosition);
                                PlacedTools.Remove(estimatedPosition);
                                Destroy(selected);
                                return tool2;
                            }
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Removes all objects from the grid.
        /// </summary>
        public void RemoveAllObjects() {
            _objectContainer?.DestroyChildrenAsync();
        }

        public void HideAllMockObjects() {
            _mockObjectContainer.gameObject.SetActive(false);
        }

        public void ShowAllMockObjects() {
            _mockObjectContainer.gameObject.SetActive(true);
        }

        /// <summary>
        /// Places a mock object on the grid.
        /// </summary>
        /// <param name="point">Where to put the mock object</param>
        /// <param name="tool">The Tool object we are using</param>
        public void PlaceMockObject(Vector3 point, Tool tool) {
            var prefab = Instantiate(tool.MockPrefab, point, Quaternion.identity);

            PlacedObjects ??= new Dictionary<Vector3Int, GameObject>();
            var intPoint = new Vector3Int((int)point.x, (int)point.y);
            if (PlacedObjects.TryAdd(intPoint, prefab.gameObject)) {
                Place(point, prefab, tool, _mockObjectContainer);
            }
        }

        /// <summary>
        /// Places a playable object on the grid.
        /// </summary>
        /// <param name="point">Where to put the mock object</param>
        /// <param name="tool">The Tool object we are using</param>
        public void PlaceObject(Vector3 point, Tool tool) {
            var prefab = Instantiate(tool.Prefab, point, Quaternion.identity);

            Place(point, prefab, tool, _objectContainer);
        }

        /// <summary>
        /// Places a playable object on the grid.
        /// </summary>
        /// <param name="point">Where to put the mock object</param>
        /// <param name="tool">The Tool object we are using</param>
        public void PlaceEditorObject(Vector3 point, Tool tool) {
            var prefab = Instantiate(tool.EditorPrefab, point, Quaternion.identity);

            OnObjectPlaced?.Invoke(prefab.gameObject);

            Place(point, prefab, tool, _objectContainer);
        }

        /// <summary>
        /// Places a prefab on the grid.
        /// </summary>
        /// <param name="point">Where to put the prefab</param>
        /// <param name="prefab">The gameObject to place</param>
        /// <param name="tool">What tool is it from</param>
        /// <param name="parent">The parent transform to use</param>
        private void Place(Vector3 point, Transform prefab, Tool tool, Transform parent) {
            AnimatePrefab(prefab, parent);

            PlacedTools ??= new Dictionary<Vector3Int, Tool>();
            var intPoint = new Vector3Int((int)point.x, (int)point.y);
            if (PlacedTools.TryAdd(intPoint, tool)) {
                OnObjectPlaced?.Invoke(prefab.gameObject);
            }
        }

        /// <summary>
        /// Animates a prefab to the grid and parents it to a container.
        /// </summary>
        /// <param name="prefab">The gameObject to animate</param>
        /// <param name="parent">The parent transform to use</param>
        private void AnimatePrefab(Transform prefab, Transform parent) {
            var sequence = DOTween.Sequence();
            sequence.Append(prefab.DOScale(Vector2.zero, 0));
            sequence.Append(prefab.DOScale(Vector2.one, _animDuration));
            sequence.OnComplete(() => {
                prefab.SetParent(parent);
            });
            sequence.Play();
        }

        /// <summary>
        /// Checks if a position is available for a new object.
        /// </summary>
        /// <param name="point">Where the desired point would be</param>
        /// <returns>True if there's nothing there already, False otherwise</returns>
        public bool CanPlaceObject(Vector3 point) {
            return PlacedObjects == null || !PlacedObjects.ContainsKey(new Vector3Int((int)point.x, (int)point.y));
        }

        private HashSet<Vector3Int> EvaluateGridPoints() {
            var offsetX = _gridOffset.x;
            var offsetY = _gridOffset.y;

            var points = new HashSet<Vector3Int>();

            _centerPosition = new Vector3(_width / 2f - .5f + offsetX, _height / 2f - .5f + offsetY, 0);

            for (var x = 0; x < _width; x++) {
                for (var y = 0; y < _height; y++) {
                    points.Add(new Vector3Int(x + offsetX, y + offsetY, 0));
                }
            }

            return points;
        }
    }
}