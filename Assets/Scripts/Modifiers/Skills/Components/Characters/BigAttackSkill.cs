using System.Collections.Generic;
using BerserkPixel.Health;
using Camera;
using DG.Tweening;
using Extensions;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Skills {
    [CreateAssetMenu(menuName = "Aurora/Skills/Big Attack Skill")]
    public class BigAttackSkill : SkillConfig {
        private const string fadeProperty = "_Alpha";

        [BoxGroup("Detection")]
        [SerializeField]
        private LayerMask _targetMask;

        [BoxGroup("Detection")]
        [SerializeField]
        private int _amount = 5;

        [BoxGroup("Detection")]
        [SerializeField]
        private float _range = 5f;

        [BoxGroup("Detection")]
        [SerializeField]
        private float _detectionRadius = 0.5f;

        [BoxGroup("Detection")]
        [SerializeField]
        private bool _bothWays = false;

        [BoxGroup("FX")]
        [SerializeField]
        [Min(0)]
        private float _hitStopInSeconds = .1f;

        [BoxGroup("FX")]
        [SerializeField]
        private float _moveOwnerTime = .2f;

        [BoxGroup("FX")]
        [SerializeField]
        private Material _ghostMaterial;

        [BoxGroup("FX")]
        [SerializeField]
        private string[] _childNamesToReplicate = { "hair", "base" };

        [BoxGroup("Fade In")]
        [SerializeField]
        private float _inGhostInterval;

        [BoxGroup("Fade In")]
        [SerializeField]
        private float _fadeInTime;

        [BoxGroup("Fade Out")]
        [SerializeField]
        private float _outGhostInterval;

        [BoxGroup("Fade Out")]
        [SerializeField]
        private float _fadeOutTime;

        private Transform _ghostsParent;
        private PlayerStateMachine _owner;
        private Transform _ownerSpriteTransform;
        private readonly Dictionary<string, SpriteRenderer> _originalRenderers = new();

        public override void Setup(Transform owner) {
            base.Setup(owner);
            _owner = owner.GetComponent<PlayerStateMachine>();

            CreateGhosts();
        }

        public override void Cleanup() {
            base.Cleanup();
            if (_ghostsParent != null) {
                _ghostsParent.DestroyChildren();
                GameObject.Destroy(_ghostsParent.gameObject);
            }
        }

        private void CreateGhosts() {
            _ghostsParent = new GameObject("Ghosts").transform;
            _ghostsParent.transform.parent = null;
            _ghostsParent.gameObject.SetActive(false);

            _ownerSpriteTransform = _owner.transform.Find("player");

            for (int i = 0; i < _amount; i++) {
                GameObject ghost = new($"Ghost {i}");
                ghost.transform.parent = _ghostsParent;
                ghost.transform.localPosition = Vector2.zero;

                foreach (string childName in _childNamesToReplicate) {
                    var child = _ownerSpriteTransform.Find(childName);

                    if (child == null) {
                        continue;
                    }

                    if (!child.TryGetComponent<SpriteRenderer>(out var originalRenderer)) {
                        continue;
                    }

                    string ghostName = $"{childName} [Ghost]";

                    GameObject ghostChild = new(ghostName);
                    ghostChild.transform.parent = ghost.transform;

                    _originalRenderers[ghostName] = originalRenderer;

                    var ghostRenderer = ghostChild.AddComponent<SpriteRenderer>();
                    ghostRenderer.color = originalRenderer.color;
                    ghostRenderer.sortingLayerID = originalRenderer.sortingLayerID;
                    ghostRenderer.sortingOrder = originalRenderer.sortingOrder;
                    ghostRenderer.material = _ghostMaterial;
                    ghostRenderer.DOFade(0, 0);
                }
            }
        }

        public override void Activate(Transform target) {
            base.Activate(target);

            var direction = _owner.LastDirection;
            var points = _owner.transform.ProjectPoints(_amount, _range, direction, _bothWays);

            CinemachineCameraShake.Instance.ShakeCamera(target, 8, .5f, true);

            DetectEnemies(ref points);

            MoveOwner(ref points);

            ShowGhosts(ref points);
        }

        private void DetectEnemies(ref List<Vector2> points) {
            foreach (var point in points) {
                var hits = Physics2D.OverlapCircleAll(point, _detectionRadius, _targetMask);

                if (hits.Length == 0) {
                    continue;
                }

                foreach (var hit in hits) {
                    var dir = (hit.transform.position - _owner.transform.position).normalized;

                    var hitData = new HitDataBuilder()
                        .WithDamage((int)EndValue)
                        .WithDirection(dir)
                        .WithTimescaleData(_hitStopInSeconds, 0f)
                        .Build(_owner.transform, hit.gameObject.transform);

                    hitData.PerformDamage(hit);
                }
            }
        }

        private void MoveOwner(ref List<Vector2> points) {
            // we move the owner to the last point in the list
            _owner.transform.DOMove(points[^1], _moveOwnerTime);
        }

        private void ShowGhosts(ref List<Vector2> points) {
            _ghostsParent.transform.position = _owner.transform.position;
            _ghostsParent.gameObject.SetActive(true);
            Sequence s = DOTween.Sequence();
            s.SetUpdate(true);

            Vector3 ownerScale = _ownerSpriteTransform.localScale;
            bool flipX = _owner.LastDirection.x < 0;

            for (int i = 0; i < _ghostsParent.childCount; i++) {
                Transform currentGhost = _ghostsParent.GetChild(i);

                var pos = points[i];
                s.AppendCallback(() => {
                    currentGhost.position = pos;

                    var localScale = ownerScale;
                    localScale.x = flipX ? -1 : 1;
                    currentGhost.localScale = localScale;
                });

                foreach (var spriteRenderer in currentGhost.GetComponentsInChildren<SpriteRenderer>()) {
                    // Fade in the ghost, each starting after the interval
                    if (_originalRenderers.TryGetValue(spriteRenderer.name, out var originalRenderer)) {
                        s.AppendCallback(() => spriteRenderer.sprite = originalRenderer.sprite);
                    }

                    spriteRenderer.material.SetFloat(fadeProperty, 0);
                    s.Append(DOVirtual.Float(0, 1, _fadeInTime, (f) => {
                        spriteRenderer.material.SetFloat(fadeProperty, f);
                    }));
                }

                // Add interval between each ghost's appearance
                s.AppendInterval(_inGhostInterval);
            }

            // Once showing sequence completes, set up to hide ghosts
            s.OnComplete(() => {
                HideGhosts();
            });
        }

        private void HideGhosts() {
            Sequence s = DOTween.Sequence();
            s.SetUpdate(true);

            for (int i = 0; i < _ghostsParent.childCount; i++) {
                Transform currentGhost = _ghostsParent.GetChild(i);

                foreach (var spriteRenderer in currentGhost.GetComponentsInChildren<SpriteRenderer>()) {
                    // Fade out the ghost sequentially
                    spriteRenderer.material.SetFloat(fadeProperty, 1);
                    s.Append(DOVirtual.Float(1, 0, _fadeOutTime, (f) => {
                        spriteRenderer.material.SetFloat(fadeProperty, f);
                    }));
                }

                // Add interval between each ghost's disappearance
                s.AppendInterval(_outGhostInterval);
            }

            // Once hiding sequence completes, deactivate the ghosts' parent
            s.OnComplete(() => {
                _ghostsParent.gameObject.SetActive(false);
            });
        }
    }
}