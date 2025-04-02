using BerserkPixel.Prata.Data;
using BerserkPixel.Prata.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BerserkPixel.Prata.Elements
{
    public class PrataRandomChoiceNode : PrataNode
    {
        public override void Init(PrataGraphView graphView, Vector2 position, NodeTypes type)
        {
            base.Init(graphView, position, NodeTypes.RandomChoice);
            Choices.Add("Multiple Dialogues");
        }

        public override void Draw()
        {
            base.Draw();

            foreach (var choice in Choices)
            {
                var portChoice = this.CreatePort(choice, capacity: Port.Capacity.Multi, edgeConnectorListener: _edgeListener);

                outputContainer.Add(portChoice);
            }
        }
    }
}