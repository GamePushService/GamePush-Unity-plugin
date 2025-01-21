using UnityEngine;

namespace GamePush
{
    [System.Serializable]
    public class GamesCollectionsData
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public GamesData[] games;
    }

    [System.Serializable]
    public class GamesData
    {
        public int id;
        public string name;
        public string description;
        public string icon;
        public string url;
    }
}


