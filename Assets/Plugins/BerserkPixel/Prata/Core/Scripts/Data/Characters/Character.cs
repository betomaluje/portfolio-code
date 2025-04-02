using System;
using System.Collections.Generic;
using UnityEngine;

namespace BerserkPixel.Prata
{
    [CreateAssetMenu(fileName = "Character", menuName = "Prata/New Character", order = 0)]
    public class Character : ScriptableObject
    {
        [HideInInspector]
        public string id = Guid.NewGuid().ToString();

        public string characterName;

        public List<Faces> faces;

        public bool isPlayer = false;

        public Sprite GetFaceForEmotion(ActorsEmotions emotion) =>
            faces.Find(face => face.emotion == emotion)?.face;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Character other = (Character)obj;

            if (faces.Count != other.faces.Count)
                return false;

            return id == other.id &&
                characterName.Equals(other.characterName);
        }
    }
}