using System;

namespace BerserkPixel.Prata.Data
{
    [Serializable]
    public class DialogContent
    {
        public string characterID;
        public ActorsEmotions emotion;
        public string DialogText = "New Dialog";

        public void Fill(
            string character,
            ActorsEmotions actorEmotion,
            string text
        )
        {
            characterID = character;
            emotion = actorEmotion;
            DialogText = text;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            DialogContent other = (DialogContent)obj;

            return characterID == other.characterID &&
                emotion == other.emotion &&
                DialogText == other.DialogText;
        }
    }
}