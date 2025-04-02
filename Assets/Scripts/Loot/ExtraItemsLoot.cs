using DG.Tweening;
using Extensions;
using UnityEngine;

namespace Loot {
    [RequireComponent(typeof(Loot))]
    public class ExtraItemsLoot : MonoBehaviour {
        [SerializeField]
        private GameObject[] _extraItems;

        [SerializeField]
        private float _lootSpawnRadius = 1f;

        private Loot _loot;

        private void Awake() {
            _loot = GetComponent<Loot>();
        }

        private void OnEnable() {
            _loot.OnUnlock += UnlockExtraItems;
        }

        private void OnDisable() {
            _loot.OnUnlock -= UnlockExtraItems;
        }

        private void UnlockExtraItems() {
            foreach (var item in _extraItems) {
                var i = Instantiate(item, transform.position, Quaternion.identity);
                var extraDistance = transform.position.GetRandomPosition(_lootSpawnRadius);
                extraDistance.y = Mathf.Abs(extraDistance.y);
                i.transform.DOJump(extraDistance, jumpPower: 2f, numJumps: 1, duration: .4f);
            }
        }
    }
}