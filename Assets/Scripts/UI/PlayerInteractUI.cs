using Detection;
using DG.Tweening;
using Interactable;
using Player.Input;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class PlayerInteractUI : MonoBehaviour {
        [BoxGroup("Action")]
        [SerializeField]
        private RectTransform _textTransform;

        [BoxGroup("Action")]
        [SerializeField]
        private TextMeshProUGUI _interactionText;

        [BoxGroup("Action")]
        [SerializeField]
        private float _hiddentTextY = -200f;

        [BoxGroup("Description")]
        [SerializeField]
        private RectTransform _descriptionContainer;

        [BoxGroup("Description")]
        [SerializeField]
        private TextMeshProUGUI _descriptionText;

        [BoxGroup("Description")]
        [SerializeField]
        private Image _itemImage;

        [BoxGroup("Description")]
        [SerializeField]
        private Image _itemTagColorImage;

        [BoxGroup("Description")]
        [SerializeField]
        private float _hiddentTextX = 200f;

        private float _originalY, _originalX;

        private InteractionDetection _playerInteractionDetector;
        private DisablePanelComponent _disablePanelComponent;

        private void Awake() {
            _disablePanelComponent = new DisablePanelComponent(GetComponent<RectTransform>());
            var playerInput = FindFirstObjectByType<PlayerBattleInput>();
            if (playerInput != null) {
                _playerInteractionDetector = playerInput.GetComponentInChildren<InteractionDetection>();
            }
        }

        private void Start() {
            if (_playerInteractionDetector == null) {
                _disablePanelComponent.Hide();
                return;
            }

            var currentAnchorPosition = _textTransform.anchoredPosition;
            _originalY = currentAnchorPosition.y;
            currentAnchorPosition.y = _hiddentTextY;
            _textTransform.anchoredPosition = currentAnchorPosition;

            var currentAnchorPositionDesc = _descriptionContainer.anchoredPosition;
            _originalX = currentAnchorPositionDesc.x;
            currentAnchorPositionDesc.x = _hiddentTextX;
            _descriptionContainer.anchoredPosition = currentAnchorPositionDesc;

            _itemImage.preserveAspect = true;
        }

        private void OnEnable() {
            if (_playerInteractionDetector != null) {
                _playerInteractionDetector.OnInteractionDetected.AddListener(Show);
                _playerInteractionDetector.OnInteractionLost.AddListener(HideAction);
                _playerInteractionDetector.OnInteractionLost.AddListener(HideDescription);
            }
        }

        private void OnDisable() {
            if (_playerInteractionDetector != null) {
                _playerInteractionDetector.OnInteractionDetected.RemoveListener(Show);
                _playerInteractionDetector.OnInteractionLost.RemoveListener(HideAction);
                _playerInteractionDetector.OnInteractionLost.RemoveListener(HideDescription);
            }
        }

        private void Show(IInteractTag tag) {
            string message = tag.ObjectName;
            InteractAction action = tag.Action;

            if (action == InteractAction.None) {
                return;
            }

            if (tag is ItemInteractTag) {
                // we are selecting something. We need to show some description
                _interactionText.text = $"{action}";

                _descriptionText.text = message;

                if (_descriptionContainer.anchoredPosition.x != _originalX) {
                    _descriptionContainer.DOAnchorPosX(_originalX, .5f);
                }

                if (_textTransform.anchoredPosition.y != _originalY) {
                    _textTransform.DOAnchorPosY(_originalY, .5f);
                }

                if (tag.ItemIcon != null) {
                    _itemImage.DOFade(1, .25f);
                    _itemImage.sprite = tag.ItemIcon;
                }
                else {
                    _itemImage.sprite = null;
                    _itemImage.DOFade(0, .25f);
                }

                if (_itemTagColorImage != null) {
                    _itemTagColorImage.color = tag.TagColor;
                }
            }
            else {
                HideDescription();

                _interactionText.text = $"{action} {message}";

                if (_textTransform.anchoredPosition.y == _originalY)
                    return;

                _textTransform.DOAnchorPosY(_originalY, .5f);
            }
        }

        private void HideAction() {
            _textTransform.DOAnchorPosY(_hiddentTextY, .5f);
        }

        private void HideDescription() {
            if (_descriptionContainer.anchoredPosition.x != _hiddentTextX) {
                _descriptionContainer.DOAnchorPosX(_hiddentTextX, .5f);
            }
        }
    }
}