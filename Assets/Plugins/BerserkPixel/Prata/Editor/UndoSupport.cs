using System.Collections.Generic;
using BerserkPixel.Prata;
using BerserkPixel.Prata.Data;
using BerserkPixel.Prata.Elements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class UndoSupport
{
    private PrataGraphView _graphView;

    private PrataStack<GraphElement> _elementsAdded;
    private List<DialogNodeData> _nodesToCopy;

    public UndoSupport(PrataGraphView graphView)
    {
        _graphView = graphView;
        _elementsAdded = new PrataStack<GraphElement>();
        _nodesToCopy = new List<DialogNodeData>();

        RegisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        Undo.undoRedoPerformed += OnUndoRedo;
        _graphView.serializeGraphElements += CopyOperation;
        _graphView.unserializeAndPaste += PasteOperation;
        _graphView.graphViewChanged = OnGraphChange;
    }

    private void OnUndoRedo()
    {
        if (_elementsAdded.Count > 0)
        {
            var lastElement = _elementsAdded.Pop();
            _graphView.RemoveElement(lastElement);
            _elementsAdded.Remove(lastElement);
        }
    }

    private GraphViewChange OnGraphChange(GraphViewChange change)
    {
        if (change.edgesToCreate != null)
        {
            foreach (Edge edge in change.edgesToCreate)
            {
                if (!_elementsAdded.Contains(edge))
                {
                    _elementsAdded.Push(edge);
                }
            }
        }

        if (change.elementsToRemove != null)
        {
            foreach (GraphElement item in change.elementsToRemove)
            {
                if (_elementsAdded.Contains(item))
                {
                    _elementsAdded.Remove(item);
                }
            }
        }

        return change;
    }

    private string CopyOperation(IEnumerable<GraphElement> elements)
    {
        _nodesToCopy.Clear();
        foreach (GraphElement element in elements)
        {
            if (element is PrataNode nodeView)
            {
                var dialogNodeData = nodeView.CreateNode();
                _nodesToCopy.Add(dialogNodeData);
            }
        }
        return "Copy Nodes";
    }

    private void PasteOperation(string operationName, string data)
    {
        // we deselect everything in the editor
        _graphView.ClearSelection();

        foreach (DialogNodeData originalNode in _nodesToCopy)
        {
            var copy = originalNode.copyWithNewPosition(_graphView.GetLocalMousePosition());
            var pastedNode = _graphView.RestoreNode(copy);
            _graphView.AddElement(pastedNode);
            _elementsAdded.Push(pastedNode);
            // we select the latest node in the editor
            _graphView.AddToSelection(pastedNode);
        }
    }

    public void AddElement(GraphElement element)
    {
        _elementsAdded.Push(element);
    }

    public void Reset()
    {
        _elementsAdded.Clear();
    }
}
