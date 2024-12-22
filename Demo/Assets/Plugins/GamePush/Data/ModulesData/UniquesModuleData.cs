using System;

namespace GamePush
{
    [System.Serializable]
    public class UniquesData
    {
        public string tag;
        public string value;

        public UniquesData(string tag = "", string value = "")
        {
            this.tag = tag;
            this.value = value;
        }
    }
}