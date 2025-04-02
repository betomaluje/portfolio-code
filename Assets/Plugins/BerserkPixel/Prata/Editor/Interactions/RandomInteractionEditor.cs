using UnityEditor;

namespace BerserkPixel.Prata
{
    [CustomEditor(typeof(RandomInteraction))]
    public class RandomInteractionEditor : BaseInteractionEditor<RandomInteraction>
    {
        protected override string GetPropertyName() => "interactions";

        protected override string GetToolboxText() => "Each interaction will be displayed randomly";

        protected override string GetIconName() => "node_random_icon";
    }
}