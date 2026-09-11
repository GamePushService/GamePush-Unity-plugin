#if UNITY_EDITOR
using System.Collections.Concurrent;

namespace GamePush
{
    public static class GP_Play2WebBus
    {
        static readonly ConcurrentQueue<string> ToBrowser = new ConcurrentQueue<string>();
        static readonly ConcurrentQueue<string> ToUnity = new ConcurrentQueue<string>();

        public static void SendToBrowser(string json)
        {
            if (!string.IsNullOrEmpty(json))
                ToBrowser.Enqueue(json);
        }

        public static void SendToUnity(string json)
        {
            if (!string.IsNullOrEmpty(json))
                ToUnity.Enqueue(json);
        }

        public static bool TryTakeToBrowser(out string json) => ToBrowser.TryDequeue(out json);

        public static bool TryTakeToUnity(out string json) => ToUnity.TryDequeue(out json);

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            while (ToBrowser.TryDequeue(out _)) { }
            while (ToUnity.TryDequeue(out _)) { }
        }
    }
}
#endif
