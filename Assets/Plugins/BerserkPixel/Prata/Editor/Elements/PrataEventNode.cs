using BerserkPixel.Prata.Data;
using BerserkPixel.Prata.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace BerserkPixel.Prata.Elements {
    public class PrataEventNode : PrataNode {

        public UnityEvent Test;
        
        public override void Init(PrataGraphView graphView, Vector2 position, NodeTypes type) {
            base.Init(graphView, position, NodeTypes.Event);
            Choices.Add("Next Dialog");
        }

        public override void Draw() {
            base.Draw();
            foreach (var choice in Choices)
            {
                var portChoice = this.CreatePort(choice, edgeConnectorListener: _edgeListener);

                outputContainer.Add(portChoice);
            }
            
            var customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("prata-node_custom-data-container");

            var foldout = PrataElementsUtilities.CreateFoldout("Event");

            // this needs to be a UnityEvent OR pass a custom scriptable object as an eventbus
            var eventField = PrataElementsUtilities.CreateObjectField<ScriptableObject>("Dialog Event");
            eventField.RegisterCallback<ChangeEvent<Object>>((evt) =>
            {
                if (evt.newValue == null)
                {
                    return;
                }

                if (eventField.value is ScriptableObject scriptableObject)
                {
                    Debug.Log($"{scriptableObject.name} is here");
                }
            });

            eventField.style.maxWidth = 240;
            
            foldout.Add(eventField);

            customDataContainer.Add(foldout);
            extensionContainer.Add(customDataContainer);
        }
        
    }
}