using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using Utils;

namespace UI {
    public class MoneyAnimationsUI : Singleton<MoneyAnimationsUI> {
        [FoldoutGroup("General")]
        [SerializeField]
        private CoinUI _coinPrefab;

        [FoldoutGroup("General")]
        [SerializeField]
        private float _animDuration;

        [FoldoutGroup("General")]
        [SerializeField]
        private int _coinSpread = 100;

        [FoldoutGroup("General")]
        [SerializeField]
        private Ease _easeType;

        [FoldoutGroup("General")]
        [SerializeField]
        private int _coinAmount = 30;

        [FoldoutGroup("General")]
        [SerializeField]
        private float _coinPerDelay = .1f;

        [FoldoutGroup("General")]
        [SerializeField]
        private float _totalDelay = .5f;

        [FoldoutGroup("Transforms")]
        [SerializeField]
        private Transform _defaultFromTransform;

        [FoldoutGroup("Transforms")]
        [SerializeField]
        private Transform _defaultToTransform;

        private ObjectPool<CoinUI> _coinPool;

        private void Start() {
            _coinPool = new ObjectPool<CoinUI>(
                () => Instantiate(_coinPrefab, transform),
                coin => coin.gameObject.SetActive(true),
                coin => coin.gameObject.SetActive(false),
                coin => Destroy(coin.gameObject),
                false,
                _coinAmount,
                _coinAmount * 2
            );
        }

        public void ShowCoins(int totalCoins, Transform from) {
            Vector3 screenPos = UnityEngine.Camera.main.WorldToScreenPoint(from.position);
            Vector2 screenPos2D = new(screenPos.x, screenPos.y);

            _coinPerDelay = _totalDelay / totalCoins;
            for (int i = 0; i < totalCoins; i++) {
                var targetDelay = i * _coinPerDelay;
                ShowCoin(screenPos2D, transform, targetDelay);
            }

            // Invoke(nameof(OnAnimationDone), _totalDelay * _animDuration);
        }

        private void ShowCoin(Vector3 from, Transform to, float delay) {
            var coinObject = _coinPool.Get();
            coinObject.transform.SetParent(transform);
            Vector3 offset = Random.insideUnitCircle * _coinSpread;
            var startPos = offset + from;
            coinObject.transform.position = startPos;

            coinObject.transform.localScale = Vector3.zero;

            coinObject.transform.DOScale(Vector3.one, delay);
            coinObject.transform.DORotateQuaternion(Quaternion.Euler(0, Random.Range(0, 360), 0), _animDuration / 2f).SetLoops(2, LoopType.Yoyo);

            coinObject.transform.DOMove(to.position, _animDuration).SetEase(_easeType).SetDelay(delay).OnComplete(() => {
                _coinPool.Release(coinObject);
            });
        }

        private void OnAnimationDone() {

        }
    }
}