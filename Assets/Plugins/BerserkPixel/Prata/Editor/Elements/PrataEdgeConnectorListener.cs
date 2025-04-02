using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BerserkPixel.Prata.Elements
{
    public class PrataEdgeConnectorListener : IEdgeConnectorListener
    {
        private PrataGraphView _graphView;

        public PrataEdgeConnectorListener(PrataGraphView graphView)
        {
            _graphView = graphView;
        }

        public void OnDrop(GraphView graphView, Edge edge) { }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            _graphView.OpenSearchWindow(position);   
        }
    }
}