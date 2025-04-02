using System.Collections.Generic;
using System.Linq;
using BerserkPixel.Prata.Utilities;
using UnityEngine;

namespace BerserkPixel.Prata
{
    [CreateAssetMenu(fileName = "Interaction", menuName = "Prata/New Interaction", order = 1)]
    public class Interaction : ScriptableObject
    {
        [SerializeField] private DialogContainer graphReference;

        private int TotalNodes => graphReference.DialogNodes.Count;

        private List<Character> allCharacters => DataUtilities.GetAllCharacters();

        private readonly List<Dialog> conversation = new List<Dialog>();

        private Dialog currentDialog;
        private bool hasStarted;
        private bool hasBeenCancelled;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Interaction other = (Interaction)obj;

            if (allCharacters.Count != other.allCharacters.Count)
                return false;

            if (conversation.Count != other.conversation.Count)
                return false;

            return TotalNodes == other.TotalNodes &&
                graphReference.Equals(other.graphReference);
        }

        private void OnEnable()
        {
            if (graphReference == null || graphReference.DialogNodes.Count <= 0 || allCharacters == null || allCharacters.Count <= 0) return;

            var content = graphReference.DialogNodes[0].Content;

            if (content == null) return;

            var lastCharacter = content.characterID;

            Dictionary<string, Character> charactersDictionary = new Dictionary<string, Character>();

            // by default we start facing right
            foreach (var nodeData in graphReference.DialogNodes)
            {
                // this means we have a connection (it's a valid Node)
                var dialog = new Dialog
                {
                    guid = nodeData.Guid
                };

                Character character = null;
                if (charactersDictionary.ContainsKey(nodeData.Content.characterID))
                {
                    character = charactersDictionary[nodeData.Content.characterID];
                }
                else
                {
                    character = allCharacters.Find(character => character.id == nodeData.Content.characterID);
                    charactersDictionary.Add(nodeData.Content.characterID, character);
                }

                var emotion = nodeData.Content.emotion;

                dialog.character = character;
                dialog.emotion = emotion;
                dialog.text = nodeData.Content.DialogText;

                // If the character that's speaking changes, then we toggle it so it faces left
                if (!string.IsNullOrEmpty(lastCharacter) && lastCharacter != character.id)
                {
                    lastCharacter = character.id;
                }

                // we need to only show the choices that actually have a connection on the port
                var usedChoices = graphReference.NodeLinks
                    .Where(node => node.BaseNodeGuid == nodeData.Guid)
                    .Select(node => node.PortName)
                    .Distinct()
                    .ToList();

                dialog.choices = usedChoices;

                conversation.Add(dialog);
            }

            currentDialog = null;
            hasStarted = false;
            hasBeenCancelled = false;
        }

        public void Cancel()
        {
            hasBeenCancelled = true;
        }

        public Dialog GetDialog()
        {
            // check if we had previously another dialog
            if (!hasStarted || currentDialog == null)
            {
                // if it's the first one
                currentDialog = GetFirstDialog();

                hasStarted = true;
                return currentDialog;
            }

            if (hasBeenCancelled)
            {
                hasBeenCancelled = false;
                return currentDialog;
            }

            currentDialog = GetNextDialog();

            return currentDialog;
        }

        private Dialog GetFirstDialog() => conversation[0];

        public Dialog GetLastDialog() => conversation.Last();

        public bool IsLastDialog() => currentDialog == conversation.Last();

        public Dialog GetNextDialogFromChoice(string dialogGuid, string choice)
        {
            currentDialog = GetNextDialogByIdAndChoice(dialogGuid, choice);
            return currentDialog;
        }

        private Dialog GetNextDialog()
        {
            currentDialog = GetNextDialogById(currentDialog.guid);
            return currentDialog;
        }

        private Dialog GetNextDialogById(string guid)
        {
            var linkData = graphReference.NodeLinks
                .Where(link => link.BaseNodeGuid == guid)
                .Random();
            var nodeData = graphReference.DialogNodes.FirstOrDefault(node => node.Guid == linkData?.TargetNodeGuid);
            var next = conversation.FirstOrDefault(dialog => dialog.guid == nodeData?.Guid);

            return next;
        }

        private Dialog GetNextDialogByIdAndChoice(string guid, string choice)
        {
            var linkData = graphReference.NodeLinks
                .FirstOrDefault(link => link.BaseNodeGuid == guid && link.PortName == choice);
            var nodeData = graphReference.DialogNodes.FirstOrDefault(node => node.Guid == linkData?.TargetNodeGuid);
            var next = conversation.FirstOrDefault(dialog => dialog.guid == nodeData?.Guid);

            return next;
        }

        public bool HasAnyDialogLeft()
        {
            if (!hasStarted)
            {
                return true;
            }

            return currentDialog != null &&
                   graphReference.NodeLinks.Any(link => link.BaseNodeGuid == currentDialog.guid);
        }

        public void Reset()
        {
            if (!hasStarted) return;

            hasBeenCancelled = false;
            currentDialog = null;
            hasStarted = false;
        }
    }
}