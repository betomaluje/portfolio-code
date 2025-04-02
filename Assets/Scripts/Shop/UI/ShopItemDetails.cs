using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop {
    public class ShopItemDetails : MonoBehaviour {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private TextMeshProUGUI _itemName;

        [SerializeField]
        private TextMeshProUGUI _itemDescription;

        [Header("Animation")]
        [SerializeField]
        private float _scale = 1.2f;

        [SerializeField]
        private float _duration = 0.2f;

        public void SetItemDescription(IShopItem shopItemUI) {
            _image.sprite = shopItemUI.Icon;
            _itemName.text = shopItemUI.Name;
            _itemDescription.text = shopItemUI.Stats();
            var sequence = DOTween.Sequence();
            sequence.Append(_image.rectTransform.DOScale(1, 0));
            sequence.Append(_image.rectTransform.DOScale(_scale, _duration).SetEase(Ease.OutBack));
            sequence.SetUpdate(true);
            sequence.Play();
        }
    }
}