using System;
using UnityEngine;

namespace GamePush.Native
{
    public static class NativeAuthWebView
    {
        static Action<string> _pending;

        public static void EnsureCallback()
        {
            var host = NativeMainThread.Instance;
            if (host.GetComponent<NativeAuthCallback>() == null)
                host.gameObject.AddComponent<NativeAuthCallback>();
        }

        public static void Open(string url, string redirectPrefix, string queryParam, Action<string> done)
        {
            _pending = done;
            EnsureCallback();
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using var intent = new AndroidJavaObject("android.content.Intent", activity,
                    new AndroidJavaClass("com.gamepush.auth.GpAuthActivity"));
                intent.Call<AndroidJavaObject>("putExtra", "url", url ?? "");
                intent.Call<AndroidJavaObject>("putExtra", "prefix", redirectPrefix ?? "");
                intent.Call<AndroidJavaObject>("putExtra", "param", string.IsNullOrEmpty(queryParam) ? "code" : queryParam);
                activity.Call("startActivity", intent);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[GamePush Native] Auth WebView failed: " + exception.Message);
                HandleResult("cancel");
            }
#else
            HandleResult("cancel");
#endif
        }

        public static void HandleResult(string payload)
        {
            var callback = _pending;
            _pending = null;
            callback?.Invoke(payload ?? "cancel");
        }
    }

    sealed class NativeAuthCallback : MonoBehaviour
    {
        public void OnGpAuthResult(string payload)
        {
            NativeAuthWebView.HandleResult(payload);
        }
    }
}
