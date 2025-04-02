using System;
using System.Collections.Generic;
using System.Linq;
using Battle;
using Camera;
using Cysharp.Threading.Tasks;
using Dungeon.FogOfWar;
using Dungeon.Renderer;
using Extensions;
using Sirenix.OdinInspector;
using Tiles;
using UI;
using UnityEngine;
using Utils;

namespace Dungeon {
    [RequireComponent(typeof(RandomObjectPlacer), typeof(DungeonGrassPlacer))]
    [RequireComponent(typeof(DungeonRoomEnemySpawners))]
    public class DungeonCallbackReceiver : Singleton<DungeonCallbackReceiver> {
        [SerializeField]
        private DungeonMap _dungeonMap;

        [Header("Room Config")]
        [SerializeField]
        [Required]
        private Transform _roomContainer;

        [SerializeField]
        private FirstRoomSetup _firstRoomSetup;

        [Space]
        [Header("Renderers")]
        [SerializeField]
        private DungeonRendererWrapper[] _renderers;

        [Header("Gap Config")]
        [SerializeField]
        [Range(0f, 1f)]
        private float _gapChance = .3f;
        [SerializeField]
        private Vector2 _gapSizeMin = new(3, 3);
        [SerializeField]
        private Vector2 _gapSizeMax = new(5, 5);

        private readonly IList<MyCallback> _callbacks = new List<MyCallback>();

        private IList<Vector3Int> _allPositions = new List<Vector3Int>();

        private DungeonPositionsHolder _positionHolder;

        protected override void Awake() {
            var dungeonRoomBehaviours = new DungeonRoomBehaviours(_roomContainer, _firstRoomSetup);
            _positionHolder = DungeonPositionsHolder.Instance;
            _positionHolder.Setup(dungeonRoomBehaviours);

            var dungeonSounds = new DungeonSounds();
            var dungeonGap = new DungeonGap.Builder()
                .WithChance(_gapChance)
                .WithMinGapSize(_gapSizeMin)
                .WithMaxGapSize(_gapSizeMax)
                .Build(gameObject);

            var cinematicDungeon = FindFirstObjectByType<CinematicDungeon>();
            var dungeonCameraZoom = new DungeonCameraZoom(FindFirstObjectByType<CinemachineCameraZoom>(), transform);
            var dungeonObjectPlacer = new DungeonObjectPlacer(GetComponent<RandomObjectPlacer>());
            var dungeonTilemaps = new DungeonTilemaps(this.FindAllInParent<TilemapBackground>(), FindFirstObjectByType<TilemapBorder>());
            var dungeonGrassPlacer = GetComponent<DungeonGrassPlacer>();
            var dungeonSelectionPainter = GetComponent<DungeonSelectionPainter>();
            var dungeonFogOfWar = GetComponent<DungeonFogOfWar>();

            var dungeonRigidBodyRemover = new DungeonRigidBodyRemover();
            var dungeonEnemySwarms = GetComponent<DungeonRoomEnemySpawners>();
            var dungeonUILoader = new DungeonUILoader(FindFirstObjectByType<InGameUILoader>());

            _callbacks.Add(new MyCallback(dungeonRoomBehaviours));
            _callbacks.Add(new MyCallback(dungeonRigidBodyRemover));
            _callbacks.Add(new MyCallback(dungeonSelectionPainter, dungeonSelectionPainter.FadeDuration)); // 12 rooms times each from DungeonSelectionPainter
            _callbacks.Add(new MyCallback(dungeonFogOfWar));

            _callbacks.Add(new MyCallback(cinematicDungeon));
            _callbacks.Add(new MyCallback(new DungeonPostProcessing()));
            _callbacks.Add(new MyCallback(dungeonSounds));
            _callbacks.Add(new MyCallback(dungeonGap));
            _callbacks.Add(new MyCallback(dungeonTilemaps));
            _callbacks.Add(new MyCallback(dungeonObjectPlacer));
            _callbacks.Add(new MyCallback(dungeonGrassPlacer));
            _callbacks.Add(new MyCallback(dungeonEnemySwarms));
            _callbacks.Add(new MyCallback(dungeonUILoader));
            _callbacks.Add(new MyCallback(dungeonCameraZoom));

            if (TryGetComponent(out TopLayerGenerator topLayerGenerator) && topLayerGenerator.enabled) {
                _callbacks.Add(new MyCallback(topLayerGenerator));
            }

            if (_renderers.Where(render => render.isActive).Count() == 0) {
                _renderers[0].isActive = true;
            }

            var availableRenderers = _renderers.Where(renderer => renderer != null && renderer.tilemap != null && renderer.isActive);

            if (availableRenderers.Count() == 0) {
                throw new NullReferenceException("No available renderers");
            }

            foreach (var renderer in availableRenderers) {
                _callbacks.Add(new MyCallback(RendererFactory.CreateRenderer(renderer)));
            }

            base.Awake();
        }

        private void Start() {
            _roomContainer.SetParent(null);
        }

        private void OnValidate() {
            if (_dungeonMap == null) {
                _dungeonMap = FindFirstObjectByType<DungeonMap>();
            }
        }

        private void OnEnable() {
            _dungeonMap.OnMapStart.AddListener(OnMapStart);
            _dungeonMap.OnAllRoomsGenerated.AddListener(OnAllRoomsGenerated);
            _dungeonMap.OnRoomsSelected.AddListener(OnRoomsSelected);
            _dungeonMap.OnRoomsToCenter.AddListener(OnRoomsToCenter);
            _dungeonMap.OnMapLoaded.AddListener(OnMapLoaded);
        }

        private void OnDisable() {
            _dungeonMap.OnMapStart.RemoveListener(OnMapStart);
            _dungeonMap.OnAllRoomsGenerated.RemoveListener(OnAllRoomsGenerated);
            _dungeonMap.OnRoomsSelected.RemoveListener(OnRoomsSelected);
            _dungeonMap.OnRoomsToCenter.RemoveListener(OnRoomsToCenter);
            _dungeonMap.OnMapLoaded.RemoveListener(OnMapLoaded);
        }

        #region IDungeonCallback
        private void OnMapStart() {
            _allPositions.Clear();
            _positionHolder.ClearPositions();
            foreach (var callback in _callbacks) {
                callback.Callback?.OnMapStarts();
            }
        }

        private void OnAllRoomsGenerated(IList<Room> allRooms, IList<Transform> roomTransforms) {
            foreach (var callback in _callbacks) {
                callback.Callback?.OnAllRoomsGenerated(ref allRooms, ref roomTransforms);
            }
        }

        private void OnRoomsSelected(IList<Room> selectedRooms) {
            foreach (var callback in _callbacks) {
                callback.Callback?.OnRoomsSelected(ref selectedRooms);
            }
        }

        private void OnRoomsToCenter(IList<Room> rooms) {
            _positionHolder.FillAllPositions(rooms);
            _allPositions = _positionHolder.AllPositions;

            foreach (var callback in _callbacks) {
                callback.Callback?.OnRoomsToCenter(ref rooms, ref _allPositions);
            }
        }

        private async void OnMapLoaded(int totalRooms) {
            foreach (var callback in _callbacks) {
                callback.Callback?.OnMapLoaded();
                if (callback.WaitTimePerRoom > 0f) {
                    await UniTask.Delay(TimeSpan.FromSeconds(callback.WaitTimePerRoom * totalRooms));
                }
            }

            // TODO: Move this to it's own script
            var battleManager = FindFirstObjectByType<BattleManager>(FindObjectsInactive.Include);

            if (battleManager != null && !battleManager.gameObject.activeInHierarchy) {
                battleManager.gameObject.SetActive(true);
            }
        }
        #endregion
    }

    [Serializable]
    public struct MyCallback {
        public IDungeonCallback Callback;
        public float WaitTimePerRoom;

        public MyCallback(IDungeonCallback callback, float loadedWaitTime) {
            Callback = callback;
            WaitTimePerRoom = loadedWaitTime;
            if (WaitTimePerRoom < 0f) {
                WaitTimePerRoom = 0f;
            }
        }

        public MyCallback(IDungeonCallback callback) : this(callback, 0f) { }
    }
}