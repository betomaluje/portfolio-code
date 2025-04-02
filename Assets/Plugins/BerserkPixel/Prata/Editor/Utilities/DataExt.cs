using BerserkPixel.Prata.Data;
using BerserkPixel.Prata.Elements;
public static class DataExt
{
    public static DialogNodeData CreateNode(this PrataNode prataNode)
    {
        return new DialogNodeData(
            prataNode.GUID,
            prataNode.GetPosition().position,
            prataNode.DialogType,
            prataNode.Content.characterID,
            prataNode.Content.emotion,
            prataNode.Content.DialogText,
            prataNode.Choices
        );
    }
}
