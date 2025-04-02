using BerserkPixel.Prata.Data;
using BerserkPixel.Prata.Utilities;
using UnityEngine;

namespace BerserkPixel.Prata.Elements
{
    public class PrataSingleChoiceNode : PrataNode
    {
        public override void Init(PrataGraphView graphView, Vector2 position, NodeTypes type)
        {
            base.Init(graphView, position, NodeTypes.SingleChoice);
            Choices.Add("Next Dialog");
        }

        public override void Draw()
        {
            base.Draw();

            foreach (var choice in Choices)
            {
                var portChoice = this.CreatePort(choice, edgeConnectorListener: _edgeListener);

                outputContainer.Add(portChoice);
            }
        }
    }
}