using UnityEngine;

namespace GamePush
{
    /// <summary>
    /// Desktop applies a fullscreen switch a frame or two late, and the player can also toggle it
    /// with Alt+Enter behind the SDK's back. Polling the flag keeps GP_Fullscreen's events honest.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GP_FullscreenWatcher : MonoBehaviour
    {
        static GP_FullscreenWatcher _instance;

        bool _last;

        internal static void Ensure()
        {
            if (_instance != null)
                return;
            var host = new GameObject("GamePushFullscreenWatcher");
            DontDestroyOnLoad(host);
            host.hideFlags = HideFlags.HideInHierarchy;
            _instance = host.AddComponent<GP_FullscreenWatcher>();
        }

        void Awake()
        {
            _instance = this;
            _last = Screen.fullScreen;
        }

        void Update()
        {
            var current = Screen.fullScreen;
            if (current == _last)
                return;
            _last = current;
            GP_Fullscreen.NativeFireChanged(current);
        }
    }
}
