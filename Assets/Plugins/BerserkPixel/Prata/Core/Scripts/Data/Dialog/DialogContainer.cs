using System;
using System.Collections.Generic;
using System.Linq;
using BerserkPixel.Prata.Data;
using UnityEngine;

namespace BerserkPixel.Prata
{
    [Serializable]
    public class DialogContainer : ScriptableObject
    {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<DialogNodeData> DialogNodes = new List<DialogNodeData>();

        public IEnumerable<NodeLinkData> GetLinkOrder()
        {
            return NodeLinks
                .OrderBy((nodeLink) => DialogNodes.FindIndex(dialog => dialog.Guid == nodeLink.TargetNodeGuid));
        }

        public IEnumerable<DialogNodeData> GetNodeOrder()
        {
            return DialogNodes
                .OrderBy(dialog => NodeLinks.FindIndex(nodeLink => nodeLink.TargetNodeGuid == dialog.Guid));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            DialogContainer other = (DialogContainer)obj;

            if (NodeLinks.Count != other.NodeLinks.Count)
                return false;

            if (DialogNodes.Count != other.DialogNodes.Count)
                return false;

            return base.Equals(obj);
        }
    }
}