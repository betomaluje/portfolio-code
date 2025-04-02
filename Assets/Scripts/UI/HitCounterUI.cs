using System.Collections;
using DG.Tweening;
using Player;
using TMPro;
using UnityEngine;

namespace UI {
    public class HitCounterUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI _comboText;

        [SerializeField]
        private float _hiddentTextX = 120f;

        private PlayerStateMachine _player;
        private RectTransform _textTransform;

        private float _originalX;
        private Coroutine _currentCoroutine;
        private readonly WaitForSeconds _timeToHide = new(2f);
        private DisablePanelComponent _disablePanelComponent;

        private void Start() {
            _player = FindFirstObjectByType<PlayerStateMachine>();
            _textTransform = _comboText.transform.parent.GetComponent<RectTransform>();
            _disablePanelComponent = new DisablePanelComponent(GetComponent<RectTransform>());

            var currentAnchorPosition = _textTransform.anchoredPosition;
            _originalX = currentAnchorPosition.x;
            currentAnchorPosition.x = _hiddentTextX;
            _textTransform.anchoredPosition = currentAnchorPosition;

            if (_player != null) {
                _player.HitComboCounter.OnHitAdded += OnHitAdded;
            }
            else {
                _disablePanelComponent.Hide();
            }
        }

        private void OnDestroy() {
            if (_player != null && _player.HitComboCounter != null) {
                _player.HitComboCounter.OnHitAdded -= OnHitAdded;
            }

            if (_currentCoroutine != null) {
                StopCoroutine(_currentCoroutine);
            }
        }

        private void OnHitAdded(int currentHits) {
            if (currentHits >= 3) {
                _comboText.text = $"x{currentHits}";
                _textTransform.DOAnchorPosX(_originalX, .5f);

                // autohide
                if (_currentCoroutine != null) {
                    StopCoroutine(_currentCoroutine);
                }
                _currentCoroutine = StartCoroutine(Hide());
            }
        }

        private IEnumerator Hide() {
            yield return _timeToHide;

            _textTransform.DOAnchorPosX(_hiddentTextX, 1f).OnComplete(() => _comboText.text = "");
        }
    }
}