using Cysharp.Threading.Tasks;
using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Dungeon.Factory;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Dungeon {
    public class DungeonMap : MonoBehaviour {
        #region Algorithm Configuration

        [BoxGroup("Algorithm")]
        [SerializeField]
        private DungeonShape _dungeonShape = DungeonShape.Circular;

        [BoxGroup("Algorithm/Core")]
        [SerializeField]
        [Min(1)]
        private int _tileSize = 4;

        [BoxGroup("Algorithm/Core")]
        [SerializeField]
        [Min(1)]
        private int _radius = 8;

        [BoxGroup("Algorithm/Core")]
        [SerializeField]
        [Min(1)]
        private int _numberOfPoints = 8;

        [ShowIf("@_dungeonShape == DungeonShape.Linear")]
        [BoxGroup("Algorithm/Specific")]
        [SerializeField]
        [Min(0.01f)]
        private float _stepSize = 2f;

        [ShowIf("@_dungeonShape == DungeonShape.Linear")]
        [BoxGroup("Algorithm/Specific")]
        [SerializeField]
        [Range(0, 1f)]
        private float _scattering = .1f;

        [ShowIf("@_dungeonShape == DungeonShape.Rectangle")]
        [BoxGroup("Algorithm/Specific")]
        [SerializeField]
        [Min(1)]
        private int _rectangleSize = 40;

        [ShowIf("@_dungeonShape == DungeonShape.Spiral")]
        [BoxGroup("Algorithm/Specific")]
        [SerializeField]
        [Min(1)]
        private float _spiralTurns = 30f;

        [ShowIf("@_dungeonShape == DungeonShape.Donut")]
        [BoxGroup("Algorithm/Specific")]
        [SerializeField]
        [Min(1)]
        private int _donutInnerRadius = 30;

        [ShowIf("@_dungeonShape == DungeonShape.Infinity")]
        [BoxGroup("Algorithm/Specific")]
        [SerializeField]
        [Min(0.01f)]
        private float _infinityScale = 1f;

        [ShowIf("@_dungeonShape == DungeonShape.Infinity")]
        [BoxGroup("Algorithm/Specific")]
        [SerializeField]
        [Min(0.01f)]
        private float _infinityAspectRatio = 1f;

        #endregion

        #region Room Configuration

        [BoxGroup("Room Config")]
        [SerializeField]
        private RoomFactory _roomFactory;

        [BoxGroup("Room Config")]
        [SerializeField]
        [Min(1)]
        private int _maxIterations = 100;

        [BoxGroup("Room Config")]
        [SerializeField]
        [Min(1)]
        private int _maxAmountOfRooms = 5;

        [BoxGroup("Room Config")]
        [Tooltip("This is the distance of the fake spring. The lower the distance the fastest they will go to the center")]
        [SerializeField]
        [Min(0.1f)]
        private float _attractionToCenterForce = 2f;

        #endregion

        #region Events

        [FoldoutGroup("Events")]
        public UnityEvent OnMapStart;

        [FoldoutGroup("Events")]
        public UnityEvent<IList<Room>, IList<Transform>> OnAllRoomsGenerated;

        [FoldoutGroup("Events")]
        public UnityEvent<IList<Room>> OnRoomsSelected;

        [FoldoutGroup("Events")]
        public UnityEvent<List<Room>> OnRoomsToCenter;

        [FoldoutGroup("Events")]
        public UnityEvent<int> OnMapLoaded;

        #endregion

        [BoxGroup("Debug")]
        [SerializeField]
        private bool _isDebugging = true;

        public List<Room> SelectedRooms { get; private set; }

        private DungeonGenerator _dungeonGenerator;

        private Dictionary<int, Rigidbody2D> _allClonedBodies;
        private Dictionary<int, GameObject> _allTempRooms;

        private GameObject _container;
        private Rigidbody2D _mainRigidBody;

        private bool _isBusy;
        private readonly List<Vector2> _debugGeneratedPoints = new();

        private void Start() {
            Generate();
        }

        private void OnValidate() {
            if (_maxIterations <= 0) {
                _maxIterations = 1;
            }

            if (_numberOfPoints < _maxAmountOfRooms) {
                _maxAmountOfRooms = _numberOfPoints;
            }
        }

        [ShowIf("@_isDebugging == true")]
        [ButtonGroup("Debug/Buttons", order: 1)]
        private async void Generate() {
            if (_isBusy) {
                return;
            }
            _isBusy = true;

            _dungeonGenerator ??= new DungeonGenerator(_tileSize, "Beto".GetHashCode());

            OnMapStart.Invoke();

            DestroyContainer();
            _container = new GameObject("DungeonContainer");
            _container.transform.SetParent(gameObject.transform);

            _mainRigidBody = gameObject.AddComponent<Rigidbody2D>();
            _mainRigidBody.bodyType = RigidbodyType2D.Kinematic;
            _mainRigidBody.freezeRotation = true;

            var points = CreateSparsePoints();

            var allRooms = _roomFactory.GenerateRooms(points);

            _allTempRooms = new Dictionary<int, GameObject>(allRooms.Length);
            _allClonedBodies = new Dictionary<int, Rigidbody2D>(allRooms.Length);

            for (int i = 0; i < allRooms.Length; i++) {
                (_allTempRooms[i], _allClonedBodies[i]) = CreateCloneRigidBodies(allRooms[i]);
            }

            var allTransforms = _allTempRooms.Select(x => x.Value.transform).ToList();

            OnAllRoomsGenerated?.Invoke(allRooms.ToList(), allTransforms);

            // we need to run the physics engine to check for collisions on a room            
            await BasicSimulation.Simulate(_maxIterations);

            // select elligeble rooms
            var eligibleRooms = _roomFactory.SelectMainRooms(_maxAmountOfRooms);

            OnRoomsSelected?.Invoke(eligibleRooms);

            // we rewrite the _allClonedBodies dictionary to hold only the eligible rooms
            _allClonedBodies = _allClonedBodies.Where(x => eligibleRooms.Select(x => x.Index).Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);

            // now we push them to the center
            foreach (var room in eligibleRooms) {
                if (_allClonedBodies.TryGetValue(room.Index, out Rigidbody2D rb)) {
                    if (!rb.gameObject.TryGetComponent(out SpringJoint2D _)) {
                        var spring = rb.gameObject.AddComponent<SpringJoint2D>();
                        spring.connectedBody = _mainRigidBody;
                        spring.frequency = 8f;
                        spring.dampingRatio = 1f;
                        spring.distance = _attractionToCenterForce;
                    }
                }
            }

            // we need to run the physics engine to check for collisions on a room            
            await BasicSimulation.Simulate(_maxIterations);

            // we need to update the center for each room
            foreach (var room in eligibleRooms) {
                if (_allClonedBodies.TryGetValue(room.Index, out Rigidbody2D rb)) {
                    rb.bodyType = RigidbodyType2D.Static;
                    await UniTask.Yield();
                    room.UpdatePosition(rb.transform.position);
                }
            }

            OnRoomsToCenter?.Invoke(eligibleRooms);

            SelectedRooms = eligibleRooms;

            _mainRigidBody.DestroyNow();

            OnMapLoaded?.Invoke(SelectedRooms.Count);

            _isBusy = false;
        }

        /// <summary>
        /// Creates temporary game objects and also clones of it so we can manipulate the clones later with Physics.
        /// </summary>
        /// <param name="room">The Room object to create a GameObject from</param>
        /// <returns>A Tuple in the form of (Original, Clone)</returns>
        private (GameObject, Rigidbody2D) CreateCloneRigidBodies(Room room) {
            var roomGameObject = new GameObject(room.ToString());
            var roomTransform = roomGameObject.transform;

            roomTransform.position = (Vector2)room.Center;
            roomTransform.SetParent(_container.transform);
            roomTransform.localScale = new Vector3(room.Width, room.Height, 1f);

            var rb = roomGameObject.AddComponent<Rigidbody2D>();
            var collision = rb.gameObject.AddComponent<BoxCollider2D>();

            rb.simulated = false;
            collision.enabled = false;

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            var clone = Instantiate(rb, rb.position, Quaternion.identity);
            clone.transform.SetParent(_container.transform);
            clone.simulated = true;
            clone.GetComponent<Collider2D>().enabled = true;

            return (roomGameObject, clone);
        }

        private void OnDrawGizmosSelected() {
            if (!_isDebugging) {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _radius);

            if (_debugGeneratedPoints.Count == 0) {
                return;
            }

            var smallerRadius = _radius / _debugGeneratedPoints.Count;
            foreach (var point in _debugGeneratedPoints) {
                Gizmos.DrawWireSphere(point, smallerRadius);
            }
        }

        [ShowIf("@_isDebugging == true")]
        [ButtonGroup("Debug/Buttons", order: 0)]
        private List<Vector2> CreateSparsePoints() {
            var points = new List<Vector2>();
            _dungeonGenerator ??= new DungeonGenerator(_tileSize, "Beto".GetHashCode());
            _debugGeneratedPoints.Clear();

            // Temporary set to ensure unique points
            HashSet<Vector2> uniquePoints = new(points);
            Vector2 previousPoint = transform.position;
            previousPoint.x -= _radius / 2f;
            previousPoint.y -= _radius / 2f;
            var directionSign = RandomUtils.NextDirection();

            int maxAttempts = _numberOfPoints * 10; // Limits attempts to avoid infinite loops with difficult shapes
            int attempts = 0;
            int i = 0;

            while (uniquePoints.Count < _numberOfPoints && attempts < maxAttempts) {
                var point = _dungeonShape switch {
                    DungeonShape.Circular => _dungeonGenerator.GetRandomPointInCircle(_radius),
                    DungeonShape.Ellipses => _dungeonGenerator.GetRandomPointInEllipse(Mathf.CeilToInt(_radius * 1.25f), _radius),
                    DungeonShape.Square => _dungeonGenerator.GetRandomPointInSquare(_radius),
                    DungeonShape.Rectangle => _dungeonGenerator.GetRandomPointInRectangle(_radius, _rectangleSize),
                    DungeonShape.Linear => _dungeonGenerator.GetRandomPointOnLine(previousPoint, previousPoint + (_scattering * _stepSize * i * directionSign * UnityEngine.Random.insideUnitCircle)),
                    DungeonShape.Hexagonal => _dungeonGenerator.GetRandomPointInHexagon(_radius),
                    DungeonShape.Spiral => _dungeonGenerator.GetRandomPointInSpiral(_radius, _spiralTurns),
                    DungeonShape.Donut => _dungeonGenerator.GetRandomPointInRing(_donutInnerRadius, _radius),
                    DungeonShape.Infinity => _dungeonGenerator.GetRandomPointInLemniscate(_radius, _infinityScale, _infinityAspectRatio),
                    DungeonShape.Cross => _dungeonGenerator.GetRandomPointInCross(_radius),
                    _ => throw new InvalidEnumArgumentException($"Not a valid dungeon shape {nameof(_dungeonShape)}"),
                };

                if (uniquePoints.Add(point)) {
                    points.Add(point);
                    previousPoint = point;
                }

                i++;

                attempts++;
            }

            // Log if unique points could not fill the requested amount
            if (uniquePoints.Count < _numberOfPoints) {
                DebugTools.DebugLog.Log($"Could only generate {uniquePoints.Count}/{_numberOfPoints} unique points.");
            }

            _debugGeneratedPoints.AddRange(points);
            return points;
        }

        [ShowIf("@_isDebugging == true")]
        [ButtonGroup("Debug/Buttons", order: 2)]
        private void DestroyContainer() {
            if (_container == null) {
                return;
            }

            _container.DestroyNow();
        }

        private void OnDestroy() {
            DestroyContainer();
        }
    }
}