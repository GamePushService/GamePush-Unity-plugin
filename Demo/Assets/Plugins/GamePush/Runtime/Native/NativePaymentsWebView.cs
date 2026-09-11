using System;
using System.Threading.Tasks;
using GamePush;
using UnityEngine;

namespace GamePush.Native
{
    public static class NativePaymentsWebView
    {
        static TaskCompletionSource<string> _pending;

        public static void EnsureCallback()
        {
            var host = NativeMainThread.Instance;
            if (host.GetComponent<NativePaymentsCallback>() == null)
                host.gameObject.AddComponent<NativePaymentsCallback>();
        }

        public static Task<string> OpenAsync(string url, string donePrefix)
        {
            return Start(url, null, donePrefix);
        }

        public static Task<string> OpenHtmlAsync(string html, string donePrefix)
        {
            return Start("", html, donePrefix);
        }

        static Task<string> Start(string url, string html, string donePrefix)
        {
            var previous = _pending;
            previous?.TrySetResult("cancel");
            var source = new TaskCompletionSource<string>();
            _pending = source;
            EnsureCallback();
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using var intent = new AndroidJavaObject("android.content.Intent", activity,
                    new AndroidJavaClass("com.gamepush.auth.GpPaymentActivity"));
                intent.Call<AndroidJavaObject>("putExtra", "url", url ?? "");
                intent.Call<AndroidJavaObject>("putExtra", "prefix", donePrefix ?? "");
                if (!string.IsNullOrEmpty(html))
                    intent.Call<AndroidJavaObject>("putExtra", "html", html);
                activity.Call("startActivity", intent);
            }
            catch (Exception exception)
            {
                GP_Logger.Error("PAYMENTS", "WebView failed: " + exception.Message);
                HandleResult("cancel");
            }
#else
            HandleResult("cancel");
#endif
            return source.Task;
        }

        public static void Close()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var cls = new AndroidJavaClass("com.gamepush.auth.GpPaymentActivity");
                cls.CallStatic("closeFromUnity");
            }
            catch (Exception exception)
            {
                GP_Logger.Error("PAYMENTS", "Close WebView failed: " + exception.Message);
                HandleResult("done");
            }
#else
            HandleResult("done");
#endif
        }

        public static void HandleResult(string payload)
        {
            var result = string.IsNullOrEmpty(payload) ? "cancel" : payload;
            GP_Logger.Info("PAYMENTS", "WebView callback=" + result);
            var source = _pending;
            _pending = null;
            source?.TrySetResult(result);
        }
    }

    sealed class NativePaymentsCallback : MonoBehaviour
    {
        public void OnGpPaymentResult(string payload)
        {
            NativePaymentsWebView.HandleResult(payload);
        }
    }
}
