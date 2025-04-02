using System;
using UnityEngine;

namespace BerserkPixel.Prata
{
    public class DialogManager : MonoBehaviour
    {
        private static DialogManager _instance;
        public static DialogManager Instance => _instance;

        [SerializeField] private DialogRenderer dialogRenderer;

        /// <summary>
        /// Subscribe to this actions to listen and act according to this different events
        ///
        /// For example on another script you can do:
        ///
        /// private void Start()
        /// {
        ///     DialogManager.Instance.OnDialogStart += HandleDialogStart;
        ///     DialogManager.Instance.OnDialogEnds += HandleDialogEnd;
        ///     DialogManager.Instance.OnDialogCancelled += HandleDialogEnd;
        /// }
        ///
        /// private void OnDisable()
        /// {
        ///     DialogManager.Instance.OnDialogStart -= HandleDialogStart;
        ///     DialogManager.Instance.OnDialogEnds -= HandleDialogEnd;
        ///     DialogManager.Instance.OnDialogCancelled -= HandleDialogEnd;
        /// }
        ///
        /// private void HandleDialogStart()
        /// {
        ///     // Disable player's movement
        /// }
        ///
        /// private void HandleDialogEnd()
        /// {
        ///     // Enable player's movement
        /// }
        ///  
        /// </summary>
        public Action<Interaction> OnDialogStart = delegate { };
        public Action<Interaction> OnDialogEnds = delegate { };
        public Action OnDialogCancelled = delegate { };
        public Action<Dialog> OnDialogChanged = delegate { };
        public Action<Interaction, string> OnChoiceMade = delegate { };

        private bool _isInConversation;
        private Interaction _lastInteraction;

        private Dialog _currentDialog = null;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        /// <summary>
        /// Starts or continues a conversation.
        /// </summary>
        /// <param name="interaction">The Interaction to use</param>
        public void Talk(Interaction interaction)
        {
            if (interaction.HasAnyDialogLeft())
            {
                if (!_isInConversation)
                {
                    _isInConversation = true;
                    OnDialogStart?.Invoke(interaction);
                }

                if (_lastInteraction != interaction)
                {
                    _lastInteraction = interaction;
                }

                ShowDialog(_lastInteraction.GetDialog());
            }
            else
            {
                HideDialog();
            }
        }

        public bool CurrentDialogHasChoices() {
            return _currentDialog != null && _currentDialog.HasChoices();
        }

        public void HideDialog()
        {
            dialogRenderer?.Hide();

            if (_lastInteraction)
            {
                if (_lastInteraction.HasAnyDialogLeft())
                {
                    OnDialogCancelled?.Invoke();
                }
                else
                {
                    OnDialogEnds?.Invoke(_lastInteraction);
                }

                _lastInteraction = null;
            }

            _isInConversation = false;
        }

        private void ShowDialog(Dialog dialog)
        {
            OnDialogChanged?.Invoke(dialog);

            _currentDialog = dialog;

            dialogRenderer?.Show();
            dialogRenderer?.Render(dialog);
        }

        public void MakeChoice(string dialogGuid, string choice)
        {
            if (_lastInteraction == null) return;
            
            OnChoiceMade?.Invoke(_lastInteraction, choice);

            var dialog = _lastInteraction.GetNextDialogFromChoice(dialogGuid, choice);

            OnDialogChanged?.Invoke(dialog);

            _currentDialog = dialog;

            if (dialog == null)
            {
                HideDialog();
                return;
            }

            dialogRenderer?.Render(dialog);
        }
    }
}