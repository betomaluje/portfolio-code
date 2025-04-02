using UnityEditor;

namespace BerserkPixel.Prata
{
    [CustomEditor(typeof(SequenceInteraction))]
    public class SequenceInteractionEditor : BaseInteractionEditor<SequenceInteraction>
    {
        protected override string GetPropertyName() => "interactions";

        protected override string GetToolboxText() => "Each interaction will be displayed one after the other, depending on the order below";

        protected override string GetIconName() => "node_sequence_icon";
    }
}