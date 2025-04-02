using BerserkPixel.Prata;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Scripts.Player
{
    public class NewPlayerDialogInteractor : MonoBehaviour
    {
        [SerializeField] private NewPlayerInput _playerInput;

        public GameObject currentSelection;

        private Interaction interaction;

        private void OnEnable()
        {
            _playerInput.interactEvent += HandleInteract;
            _playerInput.talkEvent += HandleTalk;
        }

        private void OnDisable()
        {
            _playerInput.interactEvent -= HandleInteract;
            _playerInput.talkEvent -= HandleTalk;
        }

        public void ReadyForInteraction(Interaction newInteraction)
        {
            interaction = newInteraction;
        }

        private void HandleTalk()
        {
            if (interaction != null)
            {
                if (interaction.HasAnyDialogLeft())
                {
                    _playerInput.EnableDialogueInput();
                }

                DialogManager.Instance.Talk(interaction);
            }
        }

        private void HandleInteract()
        {
            if (interaction != null)
            {
                if (!interaction.HasAnyDialogLeft())
                {
                    _playerInput.EnableGameplayInput();
                    EventSystem.current.SetSelectedGameObject(null);
                }

                // if the current dialog has choices, we need to fake a click button so everything
                // we set up on the TestInputDialogRenderer works (specially the dynamic click listener for a choice)
                if (DialogManager.Instance.CurrentDialogHasChoices())
                {                    
                    var currentSelection = EventSystem.current.currentSelectedGameObject;
                    if (currentSelection != null)
                    {
                        var button = currentSelection.GetComponent<Button>();
                        button.onClick.Invoke();
                    }
                }
                else
                {
                    DialogManager.Instance.Talk(interaction);
                }
            }
        }
    }
}