using UnityEditor;

namespace BerserkPixel.Prata
{
    [CustomEditor(typeof(SimpleInteraction))]
    public class SimpleInteractionEditor : BaseInteractionEditor<SimpleInteraction>
    {
        protected override string GetPropertyName() => "interaction";

        protected override string GetToolboxText() => "This is a simple interaction. It will be available until the player finishes the conversation";

        protected override string GetIconName() => "node_simple_icon";
    }
}