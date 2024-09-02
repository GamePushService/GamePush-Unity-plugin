
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
    public class PlatformFetchVariables
    {
        public string clientParams;
    }
}
