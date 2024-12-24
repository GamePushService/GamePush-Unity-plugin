
namespace GamePush
{
    [System.Serializable]
    public class FetchGameVariable
    {
        public string key;
        public object value;
        public string type;
    }

    [System.Serializable]
    public class GameVariableConfigData
    {
        public string key;
        public string value;
        public string type;

        public GameVariableConfigData(string key, string value, string type)
        {
            this.key = key;
            this.value = value;
            this.type = type;
        }
    }

    [System.Serializable]
    public class PlatformFetchVariables
    {
        public string clientParams;
    }
}
