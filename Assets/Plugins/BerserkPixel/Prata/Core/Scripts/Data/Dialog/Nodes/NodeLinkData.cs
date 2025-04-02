using System;

namespace BerserkPixel.Prata.Data
{
    [Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string PortName;
        public string TargetNodeGuid;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            NodeLinkData other = (NodeLinkData)obj;

            return BaseNodeGuid == other.BaseNodeGuid &&
                PortName == other.PortName &&
                TargetNodeGuid == other.TargetNodeGuid;
        }
    }
}