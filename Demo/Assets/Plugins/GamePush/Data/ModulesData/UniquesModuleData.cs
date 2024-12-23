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

    [System.Serializable]
    public class TagValueData
    {
        public string tag;
        public object value;

        public TagValueData(string tag = "", object value = null)
        {
            this.tag = tag;
            this.value = value;
        }
    }

    [System.Serializable]
    public class TagData
    {
        public string tag;

        public TagData(string tag = "")
        {
            this.tag = tag;
        }
    }

}