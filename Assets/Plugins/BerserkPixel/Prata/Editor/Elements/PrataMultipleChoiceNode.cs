using System;
using BerserkPixel.Prata.Data;
using BerserkPixel.Prata.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Label = System.Reflection.Emit.Label;

namespace BerserkPixel.Prata.Elements
{
    public class PrataMultipleChoiceNode : PrataNode
    {
        public override void Init(PrataGraphView graphView, Vector2 position, NodeTypes type)
        {
            base.Init(graphView, position, NodeTypes.MultipleChoice);
            Choices.Add("New Choice 0");
            Choices.Add("New Choice 1");
        }

        public override void Draw()
        {
            base.Draw();
            
            var addChoiceButton = PrataElementsUtilities.CreateButton("Add Choice", () =>
            {
                var outputPortCount = outputContainer.Query("connector").ToList().Count;
                CreateChoicePort($"New Choice {outputPortCount}");
                Choices.Add($"New Choice {outputPortCount}");
            });

            addChoiceButton.AddToClassList("prata-node_button");

            mainContainer.Insert(1, addChoiceButton);

            foreach (var choice in Choices) 
                CreateChoicePort(choice);
        }

        private void CreateChoicePort(string textTitle)
        {
            var portChoice = this.CreatePort(textTitle, edgeConnectorListener: _edgeListener);
            var portLabel = portChoice.Q<UnityEngine.UIElements.Label>();
            portLabel.style.visibility = Visibility.Hidden; 
            portLabel.AddToClassList("hidden");

            var deleteChoiceButton = PrataElementsUtilities.CreateButton("X", () => RemovePort(portChoice));

            deleteChoiceButton.AddToClassList("prata-node_button");

            var choiceTextField = PrataElementsUtilities.CreateTextField(textTitle, (evt) =>
            {
                var prev = evt.previousValue;
                var index = Choices.FindIndex(choice => choice == prev);
                Choices[index] = evt.newValue;
                portChoice.portName = evt.newValue;
            });

            choiceTextField.AddClasses(
                "prata-node_textfield",
                "prata-node_choice-textfield",
                "prata-node_textfield_hidden"
            );
            choiceTextField.style.width = 60;

            portChoice.Add(choiceTextField);
            portChoice.Add(deleteChoiceButton);

            outputContainer.Add(portChoice);
        }

        private void RemovePort(Port port)
        {
            _graphView.RemovePort(this, port);
        }
    }
}