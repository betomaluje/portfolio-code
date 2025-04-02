using System;
using System.Collections;
using Camera;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sounds;
using UnityEngine;
using UnityEngine.U2D;

namespace Dungeon {
    [RequireComponent(typeof(SpriteShapeController))]
    public class RoomWall : MonoBehaviour {
        private const string fadeProperty = "_FadeAmount";

        [SerializeField]
        private SpriteShape _spriteShapeProfile;

        [SerializeField]
        private float _duration = 1f;

        [SerializeField]
        private Material _wallMaterial;

        [SerializeField]
        private PolygonCollider2D _edgeCollider;

        private SpriteShapeController _spriteShapeController;
        private Coroutine _coroutine;
        private Room _room;

        private void Awake() {
            _spriteShapeController = GetComponent<SpriteShapeController>();
            _spriteShapeController.spriteShape = _spriteShapeProfile;

            var materials = _spriteShapeController.spriteShapeRenderer.materials;
            if (materials.Length > 1) {
                _wallMaterial = materials[1];
            }
        }

        private void OnValidate() {
            if (_edgeCollider == null) {
                _edgeCollider = GetComponent<PolygonCollider2D>();
            }
        }

        public async void SetupCorners(Room room) {
            _room = room;
            var spline = _spriteShapeController.spline;

            spline.Clear();

            for (int i = 0; i < room.Corners.Length; i++) {
                var corner = room.Corners[i];
                spline.InsertPointAt(i, corner);
                spline.SetTangentMode(i, ShapeTangentMode.Broken);
            }

            spline.isOpenEnded = false;

            await UniTask.Yield();

            _spriteShapeController.RefreshSpriteShape();
            _spriteShapeController.BakeCollider();

            GenerateWallCollider(room);
        }

        private void GenerateWallCollider(Room room) {
            Vector2[] corners = room.GenerateCorners();
            int segmentCount = corners.Length;

            // Define collider points as the perimeter of the wall
            Vector2[] colliderPoints = new Vector2[segmentCount];
            for (int i = 0; i < segmentCount; i++) {
                colliderPoints[i] = corners[i];
            }

            _edgeCollider.points = colliderPoints;
        }

        private void OnEnable() {
            CinemachineCameraShake.Instance.ShakeCamera(transform, 8f, 1f);
            SoundManager.instance.Play("dungeon_loaded");
            FadeIn();
        }

        private void OnDestroy() {
            if (_coroutine != null) {
                StopCoroutine(_coroutine);
            }
        }

        private void FadeIn() {
            // 1 to 0
            if (_coroutine != null) {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(Fade(1, 0, () => _edgeCollider.enabled = true));
        }

        public void FadeOut() {
            // 0 to 1
            if (_coroutine != null) {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(Fade(0, 1, () => {
                _edgeCollider.enabled = false;
                _spriteShapeController.spriteShapeRenderer.enabled = false;
            }));
        }

        private IEnumerator Fade(float from, float to, Action callback = null) {
            var preAmount = from;
            var endAmount = to;
            var elapsed = 0f;
            while (elapsed < _duration) {
                elapsed += Time.deltaTime;
                var percentage = Mathf.Lerp(preAmount, endAmount, elapsed / _duration);
                _wallMaterial.SetFloat(fadeProperty, percentage);
                yield return null;
            }

            _wallMaterial.SetFloat(fadeProperty, endAmount);

            callback?.Invoke();
        }

#if UNITY_EDITOR
        [Button]
        private void UpdateWall() {
            if (_room != null) {
                SetupCorners(_room);
            }
        }
#endif
    }
}