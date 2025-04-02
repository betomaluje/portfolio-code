using BerserkPixel.Prata;
using Player.Input;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Dialog {
    public class CustomDialogRenderer : DialogRenderer {
        [SerializeField]
        private GameObject container;

        [SerializeField]
        private TextMeshProUGUI authorText;

        [SerializeField]
        private TextMeshProUGUI dialogText;

        [SerializeField]
        private Transform choicesContainer;

        [SerializeField]
        private GameObject choiceButtonPrefab;

        private PlayerUIInput _playerInput;

        private void Awake() {
            GetPlayer();
        }

        private void GetPlayer() {
            _playerInput = GetComponent<PlayerUIInput>();
        }

        public override void Show() {
            container.SetActive(true);
            _playerInput.UIActions.Enable();
        }

        public override void Render(BerserkPixel.Prata.Dialog dialog) {
            dialogText.text = dialog.text;
            authorText.text = dialog.character.characterName;

            if (dialog.choices.Count > 1) {
                RemoveChoices();
                foreach (var choice in dialog.choices) {
                    var choiceButton = Instantiate(choiceButtonPrefab, choicesContainer);
                    choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice;
                    choiceButton.GetComponent<Button>().onClick.AddListener(() => {
                        DialogManager.Instance.MakeChoice(dialog.guid, choice);
                    });
                }

                EventSystem.current.SetSelectedGameObject(choicesContainer.GetChild(0).gameObject);

                choicesContainer.gameObject.SetActive(true);
            }
            else {
                choicesContainer.gameObject.SetActive(false);
            }
        }

        public override void Hide() {
            RemoveChoices();
            container.SetActive(false);
            _playerInput.UIActions.Disable();
        }

        private void RemoveChoices() {
            if (choicesContainer.childCount <= 0) {
                return;
            }

            foreach (Transform child in choicesContainer) {
                Destroy(child.gameObject);
            }
        }
    }
}