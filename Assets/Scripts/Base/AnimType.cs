using System;

namespace Base
{
    [Serializable]
    public class AnimType
    {
        public string name;
        public string value;

        public AnimType(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }
}