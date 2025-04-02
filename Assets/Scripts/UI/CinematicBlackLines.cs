using DG.Tweening;
using UnityEngine;

namespace UI {
    public class CinematicBlackLines : MonoBehaviour {
        [SerializeField]
        private RectTransform _topLine;

        [SerializeField]
        private RectTransform _bottomLine;

        [SerializeField]
        [Range(0f, 1f)]
        private float _finalScale = 1f;

        [SerializeField]
        private float _animDuration = 1f;

        private void Awake() {
            _topLine.localScale = GetYScale(0f);
            _bottomLine.localScale = GetYScale(0f);
        }

        private Vector3 GetYScale(float yScale) {
            return new Vector3(1f, yScale, 1f);
        }

        public void DoAppear() {
            var finalScale = GetYScale(_finalScale);
            _topLine.DOScale(finalScale, _animDuration);
            _bottomLine.DOScale(finalScale, _animDuration);
        }

        public void DoDisappear() {
            _topLine.DOScale(GetYScale(0f), _animDuration);
            _bottomLine.DOScale(GetYScale(0f), _animDuration);
        }
    }
}