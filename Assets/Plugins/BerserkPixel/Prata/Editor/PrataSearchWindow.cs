using System.Collections.Generic;
using BerserkPixel.Prata.Data;
using BerserkPixel.Prata.Elements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BerserkPixel.Prata
{
    public class PrataSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private PrataGraphView graphView;
        private Texture2D indentationIcon;

        public void Init(PrataGraphView prataGraphView)
        {
            graphView = prataGraphView;
            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            return new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Dialog Node"), level: 1),
                new SearchTreeEntry(new GUIContent("Single Choice", indentationIcon))
                {
                    level = 2,
                    userData = NodeTypes.SingleChoice
                },
                new SearchTreeEntry(new GUIContent("Multiple Choice", indentationIcon))
                {
                    level = 2,
                    userData = NodeTypes.MultipleChoice
                },
                new SearchTreeEntry(new GUIContent("Random Choice", indentationIcon))
                {
                    level = 2,
                    userData = NodeTypes.RandomChoice
                },
                new SearchTreeEntry(new GUIContent("Event Node", indentationIcon))
                {
                    level = 2,
                    userData = NodeTypes.Event
                },
                new SearchTreeGroupEntry(new GUIContent("Dialog Group"), level: 1),
                new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                {
                    level = 2,
                    userData = new Group()
                }
            };
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            switch (searchTreeEntry.userData)
            {
                case NodeTypes.SingleChoice:
                    var singleNode =
                        (PrataSingleChoiceNode)graphView.CreateNode(NodeTypes.SingleChoice);
                    graphView.AddNewElement(singleNode);
                    return true;
                case NodeTypes.MultipleChoice:
                    var multipleNode =
                        (PrataMultipleChoiceNode)graphView.CreateNode(NodeTypes.MultipleChoice);
                    graphView.AddNewElement(multipleNode);
                    return true;
                case NodeTypes.RandomChoice:
                    var randomNode =
                    (PrataRandomChoiceNode)graphView.CreateNode(NodeTypes.RandomChoice);
                    graphView.AddNewElement(randomNode);
                    return true;
                case NodeTypes.Event:
                    var eventNode = (PrataEventNode) graphView.CreateNode(NodeTypes.Event);
                    graphView.AddNewElement(eventNode);
                    return true;
                case Group _:
                    var group = graphView.CreateGroup("Dialog Group");
                    graphView.AddNewElement(group);
                    return true;
                default:
                    return false;
            }
        }
    }
}