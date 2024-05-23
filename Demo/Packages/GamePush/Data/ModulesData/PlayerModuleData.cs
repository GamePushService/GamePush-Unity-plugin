
namespace GamePush
{
    [System.Serializable]
    public class PlayerFetchFieldsData
    {
        public string name;
        public string key;
        public string type;
        public string defaultValue; // string | bool | number
        public bool important;
        public Variants[] variants;
    }

    [System.Serializable]
    public class Variants
    {
        public string value; // string | number
        public string name;
    }
}
