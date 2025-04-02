using System;
using System.Collections.Generic;
using UnityEngine;

namespace BerserkPixel.Prata
{
    [Serializable]
    public class Dialog
    {
        public string guid;
        public Character character;
        public ActorsEmotions emotion;
        public string text;
        public List<string> choices;

        public Sprite GetImage() => character.GetFaceForEmotion(emotion);

        public bool HasChoices()
        {
            if (choices == null || choices.Count <= 1) return false;

            return choices.Count > 1;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Dialog other = (Dialog)obj;

            if (choices.Count != other.choices.Count)
                return false;

            return guid == other.guid;
        }
    }
}