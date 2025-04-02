using System;
using System.Collections.Generic;
using System.Linq;
using BerserkPixel.Prata.Data;
using BerserkPixel.Prata.Elements;
using BerserkPixel.Prata.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BerserkPixel.Prata
{
    public class PrataGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
        private PrataSearchWindow _searchWindow;

        private Vector2 _mousePosition = Vector2.zero;

        private UndoSupport _undoSupport;

        public PrataGraphView()
        {
            AddManipulators();
            AddSearchWindow();
            AddGridBackground();
            AddStyles();

            _undoSupport = new UndoSupport(this);

            // Creating start node
            AddElement(GenerateEntryPointNode());
            Focus();
        }

        private PrataNode GenerateEntryPointNode()
        {
            PrataNode node = new PrataNode
            {
                title = "Start",
                DialogType = NodeTypes.Start
            };

            node.SetPosition(new Rect(100, 200, 100, 150));

            var outputPort = node.CreatePort(PrataConstants.FIRST_NODE, edgeConnectorListener: new PrataEdgeConnectorListener(this));
            node.outputContainer.Add(outputPort);

            // we can't remove or move the first node
            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            node.RefreshExpandedState();
            node.RefreshPorts();

            return node;
        }

        #region Override Methods

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port) return;

                if (startPort.node == port.node) return;

                if (startPort.direction == port.direction) return;

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        #endregion

        #region Element Creation

        public PrataNode RestoreNode(DialogNodeData nodeData)
        {
            var nodeType = Type.GetType($"{PrataConstants.NODE_TYPE_BASE}{nodeData.Type}Node");

            if (nodeType == null) return null;

            var node = (PrataNode)Activator.CreateInstance(nodeType);

            node.Init(this, nodeData, nodeData.Type);
            node.Draw();
            node.RefreshExpandedState();
            node.RefreshPorts();

            return node;
        }

        public PrataNode CreateNode(NodeTypes type)
        {
            Vector2 mousePosition = GetLocalMousePosition();

            var nodeType = Type.GetType($"{PrataConstants.NODE_TYPE_BASE}{type}Node");

            if (nodeType == null) return null;

            var node = (PrataNode)Activator.CreateInstance(nodeType);

            node.Init(this, mousePosition, type);
            node.Draw();
            node.RefreshExpandedState();
            node.RefreshPorts();

            return node;
        }

        public Group CreateGroup(string title)
        {
            Vector2 mousePosition = GetLocalMousePosition();

            var group = new Group
            {
                title = title
            };

            group.SetPosition(new Rect(mousePosition, Vector2.zero));

            return group;
        }

        public void AddNewElement(GraphElement element)
        {
            _undoSupport.AddElement(element);
            AddElement(element);
        }

        public void ClearAll(Dictionary<string, PrataNode> Nodes, List<Edge> Edges)
        {
            DeleteElements(Nodes.Values.Where(t => t.DialogType != NodeTypes.Start));
            // remove edges connected to node
            DeleteElements(Edges);
            _undoSupport.Reset();
        }

        #endregion

        #region Manipulators

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContentDragger());

            this.RegisterCallback<MouseMoveEvent>(x => { _mousePosition = GetMousePosition(x.localMousePosition); });
            this.RegisterCallback<MouseDownEvent>(x => { _mousePosition = GetMousePosition(x.localMousePosition); });

            this.AddManipulator(CreateNodeContextualMenu("Add Single Choice Node", NodeTypes.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Multiple Choice Node", NodeTypes.MultipleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Random Choice Node", NodeTypes.RandomChoice));
            this.AddManipulator(CreateGroupContextualMenu());
        }

        private Vector3 GetMousePosition(Vector2 localMousePosition) => viewTransform.matrix.inverse.MultiplyPoint(localMousePosition);

        private IManipulator CreateNodeContextualMenu(string actionTitle, NodeTypes type)
        {
            var contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(
                    actionTitle,
                    actionEvent =>
                    {
                        var newNode = CreateNode(type);
                        AddNewElement(newNode);
                    }
                )
            );

            return contextualMenuManipulator;
        }

        private IManipulator CreateGroupContextualMenu()
        {
            var contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(
                    "Create Group",
                    actionEvent =>
                    {
                        var newGroup = CreateGroup("Dialog Group");
                        AddNewElement(newGroup);
                    }
                )
            );

            return contextualMenuManipulator;
        }

        #endregion

        #region Element Addition

        private void AddSearchWindow()
        {
            if (_searchWindow == null)
            {
                _searchWindow = ScriptableObject.CreateInstance<PrataSearchWindow>();
                _searchWindow.Init(this);
            }

            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        public void OpenSearchWindow(Vector2 position)
        {
            SearchWindow.Open(new SearchWindowContext(position), _searchWindow);
        }

        private void AddStyles()
        {
            this.AddStyleSheets(
                PrataConstants.STYLESHEET_GRAPH,
                PrataConstants.STYLESHEET_NODE
            );
        }

        private void AddGridBackground()
        {
            var gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        #endregion

        #region Utilities

        public Vector2 GetLocalMousePosition() => _mousePosition;

        public void RemovePort(PrataNode node, Port socket)
        {
            if (node.Choices.Count > 1)
            {
                node.RemoveFromChoices(socket.portName);
                var targetEdge = edges.ToList()
                    .Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
                if (targetEdge.Any())
                {
                    var edge = targetEdge.First();
                    edge.input.Disconnect(edge);
                    RemoveElement(targetEdge.First());
                }

                node.outputContainer.Remove(socket);
                node.RefreshPorts();
                node.RefreshExpandedState();
            }
        }

        #endregion
    }
}