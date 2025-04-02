using DG.Tweening;
using UnityEngine;

namespace Consumables {
    public class ConsumableHolder : MonoBehaviour {
        [SerializeField]
        private ConsumableSO _consumable;

        private void Start() {
            transform.DOShakeRotation(.5f, new Vector3(0, 0, 10), 10).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutBack);
        }

        private void OnDestroy() {
            DOTween.Kill(transform);
        }

        public void Consume(Transform target) {
            _consumable.Consume(transform, target);
        }
    }
}