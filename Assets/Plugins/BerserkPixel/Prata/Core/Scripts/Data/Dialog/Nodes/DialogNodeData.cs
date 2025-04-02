using System;
using System.Collections.Generic;
using UnityEngine;

namespace BerserkPixel.Prata.Data
{
    [Serializable]
    public class DialogNodeData
    {
        public string Guid;
        public DialogContent Content;
        public Vector2 Position;
        public NodeTypes Type;
        public List<string> Choices;

        public DialogNodeData(
            string guid,
            Vector2 position,
            NodeTypes type,
            string character,
            ActorsEmotions actorEmotion,
            string text,
            List<string> choices
        )
        {
            Guid = guid;
            Position = position;
            Type = type;

            if (!string.IsNullOrEmpty(character))
            {
                var content = new DialogContent();
                content.Fill(
                    character,
                    actorEmotion,
                    text
                );
                Content = content;
            }

            if (choices != null && choices.Count > 0)
            {
                Choices = choices;
            }
        }

        internal DialogNodeData()
        {

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            DialogNodeData other = (DialogNodeData)obj;

            if (Choices.Count != other.Choices.Count)
                return false;

            return Guid == other.Guid &&
                Content.Equals(other.Content) &&
                Position == other.Position &&
                Type == other.Type;
        }
    }

    public static class DialogNodeDataExt
    {
        public static DialogNodeData copyWithNewPosition(this DialogNodeData original, Vector2 position)
        {
            var newNodeData = new DialogNodeData();
            newNodeData.Content = original.Content;
            newNodeData.Position = position;
            newNodeData.Type = original.Type;
            newNodeData.Choices = original.Choices;
            newNodeData.Guid = Guid.NewGuid().ToString();

            return newNodeData;
        }
    }
}