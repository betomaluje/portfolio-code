using System.Collections.Generic;
using System.Linq;
using BerserkPixel.Prata.Data;
using BerserkPixel.Prata.Elements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BerserkPixel.Prata.Utilities
{
    public class GraphSaveUtilities
    {
        private PrataGraphView _targetGraphView;
        private DialogContainer _containerCached;

        private List<Edge> Edges => _targetGraphView.edges.ToList();

        private Dictionary<string, PrataNode> Nodes => _targetGraphView.nodes.ToList().Cast<PrataNode>()
            .ToDictionary(node => node.GUID, node => node);

        public GraphSaveUtilities(PrataGraphView graphView)
        {
            _targetGraphView = graphView;
        }

        public void SaveGraph(string filename)
        {
            var paths = AssetDatabase.FindAssets(filename, new string[] { PrataConstants.FOLDER_GRAPH });

            if (paths.Length > 1)
            {
                Debug.LogError($"More than 1 Asset found with the name {filename}");
                return;
            }

            if (paths.Length == 0)
            {
                SaveNewGraph($"{PrataConstants.FOLDER_GRAPH}/{filename}.asset");
            }
            else
            {
                string path = AssetDatabase.GUIDToAssetPath(paths[0]);
                var prevAsset = (DialogContainer)AssetDatabase.LoadAssetAtPath(path, typeof(DialogContainer));
                OverwriteGraph(prevAsset);
            }
        }

        private void SaveNewGraph(string path)
        {
            Debug.Log($"New graph {path}");
            var dialogContainer = ScriptableObject.CreateInstance<DialogContainer>();

            SavePorts(dialogContainer);

            SaveNodes(dialogContainer);

            // creates an Resources folder if there's none
            GenerateFolders();

            AssetDatabase.CreateAsset(dialogContainer, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(dialogContainer);
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = dialogContainer;
            SceneView.FrameLastActiveSceneView();
        }

        private void OverwriteGraph(DialogContainer dialogContainer)
        {
            Debug.Log($"Overwriting {AssetDatabase.GetAssetPath(dialogContainer)}");
            SavePorts(dialogContainer);

            SaveNodes(dialogContainer);

            EditorUtility.SetDirty(dialogContainer);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = dialogContainer;
            SceneView.FrameLastActiveSceneView();
        }

        private void SaveNodes(DialogContainer dialogContainer)
        {
            dialogContainer.DialogNodes.Clear();

            foreach (var prataNode in Nodes.Values.Where(node => node.DialogType != NodeTypes.Start))
            {
                var dialogNode = prataNode.CreateNode();
                dialogContainer.DialogNodes.Add(dialogNode);
            }

            // we need to order the nodes according to the order on the NodeLinkData list
            dialogContainer.DialogNodes = dialogContainer.GetNodeOrder()
                .Where(dialog => dialog.Type != NodeTypes.Start)
                .ToList();
        }

        private void SavePorts(DialogContainer dialogContainer)
        {
            dialogContainer.NodeLinks.Clear();

            foreach (var connectedPort in Edges.Where(x => x.input.node != null))
            {
                var outputNode = connectedPort.output.node as PrataNode;
                var inputNode = connectedPort.input.node as PrataNode;

                if (outputNode != null && inputNode != null)
                {
                    NodeLinkData linkData = new NodeLinkData
                    {
                        BaseNodeGuid = outputNode.GUID,
                        PortName = connectedPort.output.portName,
                        TargetNodeGuid = inputNode.GUID,
                    };

                    dialogContainer.NodeLinks.Add(linkData);
                }
            }

            NodeLinkData startingNode = dialogContainer.NodeLinks.First(nodeLink => nodeLink.PortName.Equals(PrataConstants.FIRST_NODE));

            dialogContainer.NodeLinks = dialogContainer.GetLinkOrder()
                .Where(nodeLink => !nodeLink.PortName.Equals(PrataConstants.FIRST_NODE))
                .ToList();

            dialogContainer.NodeLinks.Insert(0, startingNode);
        }

        public void LoadGraph(DialogContainer dialogContainer)
        {
            _containerCached = dialogContainer;
            if (_containerCached == null)
            {
                EditorUtility.DisplayDialog("File not found", "Target dialog graph does not exist", "Ok");
                return;
            }

            ClearAll();

            RestoreNodes();

            ConnectNodes();
        }

        public void ClearAll()
        {
            _targetGraphView.ClearAll(Nodes, Edges);
        }

        private void RestoreNodes()
        {
            foreach (var nodeData in _containerCached.DialogNodes)
            {
                var tempNode = _targetGraphView.RestoreNode(nodeData);
                _targetGraphView.AddNewElement(tempNode);
            }

            // restoring the starting node GUID
            NodeLinkData startingNode = _containerCached.NodeLinks.First(nodeLink => nodeLink.PortName.Equals(PrataConstants.FIRST_NODE));

            var graphStartingNode = Nodes.Values.FirstOrDefault(node => node.DialogType == NodeTypes.Start);
            graphStartingNode.GUID = startingNode.BaseNodeGuid;
        }

        private void ConnectNodes()
        {
            foreach (var node in Nodes.Values)
            {
                var connections = _containerCached.NodeLinks.Where(
                    x => x.BaseNodeGuid == node.GUID).ToList();

                foreach (var nodeLinkData in connections)
                {
                    var targetNodeGuid = nodeLinkData.TargetNodeGuid;
                    var targetNode = Nodes.Values.FirstOrDefault(x => x.GUID == targetNodeGuid);

                    if (targetNode == null) continue;

                    // we search for the corresponding port name
                    Port outputPort = node.outputContainer.Children()
                        .FirstOrDefault(child => child.Q<Port>().portName.Equals(nodeLinkData.PortName))
                        .Q<Port>();

                    if (outputPort != null)
                    {
                        LinkNodesTogether(outputPort, (Port)targetNode.inputContainer[0]);

                        targetNode.SetPosition(new Rect(
                            _containerCached.DialogNodes.First(x => x.Guid == targetNodeGuid).Position,
                            _targetGraphView.DefaultNodeSize));
                    }
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            _targetGraphView.AddNewElement(tempEdge);
        }

        public static void GenerateFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            if (!AssetDatabase.IsValidFolder(PrataConstants.FOLDER_GRAPH))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Graphs");
            }

            if (!AssetDatabase.IsValidFolder(PrataConstants.FOLDER_INTERACTIONS_COMPLETE))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Interactions");
            }

            if (!AssetDatabase.IsValidFolder(PrataConstants.FOLDER_CHARACTERS_COMPLETE))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Characters");
            }
        }

        public static void CreateFirstCharacter(string newName = "")
        {
            // only if this is the first character we create a dummy one
            if (!DataUtilities.HasAnyCharacter()) return;

            if (!AssetDatabase.IsValidFolder(PrataConstants.FOLDER_CHARACTERS_COMPLETE))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Characters");
            }

            var character = ScriptableObject.CreateInstance<Character>();

            character.characterName = newName;

            AssetDatabase.CreateAsset(character, $"{PrataConstants.FOLDER_CHARACTERS_COMPLETE}/New Character.asset");
            EditorUtility.SetDirty(character);
            AssetDatabase.SaveAssets();
        }
    }
}