using System;
using UnityEngine;
using GamePush;

namespace GamePush.Native
{
    public interface INativeAuthSdk
    {
        bool IsAvailable { get; }
        void Login();
        void Logout();
    }

    public static class NativeAuthSdks
    {
        static bool _loggedMissing;

        public static INativeAuthSdk Current { get; private set; } = NativeUnavailableAuthSdk.Instance;

        public static void Initialize()
        {
#if GP_BUILD_AUTH && UNITY_ANDROID && !UNITY_EDITOR
            Current = NativeWebViewAuthSdk.Instance;
            NativeAuthWebView.EnsureCallback();
#else
            Current = NativeUnavailableAuthSdk.Instance;
#endif
        }

        public static void LogMissingOnce()
        {
            if (_loggedMissing)
                return;
            _loggedMissing = true;
            GP_Logger.Warn("AUTH",
                "This APK was built without the auth WebView. Enable auth for the store in GamePush, Save in Tools/GamePush, then rebuild.");
        }
    }

    sealed class NativeUnavailableAuthSdk : INativeAuthSdk
    {
        public static readonly NativeUnavailableAuthSdk Instance = new NativeUnavailableAuthSdk();
        public bool IsAvailable => false;
        public void Login()
        {
            NativeAuthSdks.LogMissingOnce();
            NativeMainThread.Run(() => GP_Player.NotifyNativeLogin(false));
        }
        public void Logout()
        {
            NativeMainThread.Run(() => GP_Player.NotifyNativeLogout(false));
        }
    }

    sealed class NativeWebViewAuthSdk : INativeAuthSdk
    {
        public static readonly NativeWebViewAuthSdk Instance = new NativeWebViewAuthSdk();
        public bool IsAvailable => true;

        public void Login()
        {
            if (!TryBuildRequest(out var url, out var prefix, out var param, out var tokenType, out var redirectUri))
            {
                NativeMainThread.Run(() => GP_Player.NotifyNativeLogin(false));
                return;
            }

            NativeAuthWebView.Open(url, prefix, param, async result =>
            {
                if (string.IsNullOrEmpty(result) || result == "cancel")
                {
                    NativeMainThread.Run(() => GP_Player.NotifyNativeLogin(false));
                    return;
                }
                try
                {
                    var ok = await NativePlayer.LoginWithOauth(result, tokenType, redirectUri);
                    if (!ok)
                        NativeMainThread.Run(() => GP_Player.NotifyNativeLogin(false));
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("[GamePush Native] Login failed: " + exception.Message);
                    NativeMainThread.Run(() => GP_Player.NotifyNativeLogin(false));
                }
            });
        }

        public void Logout()
        {
            _ = LogoutAsync();
        }

        static async System.Threading.Tasks.Task LogoutAsync()
        {
            try
            {
                var ok = await NativePlayer.LogoutRemote();
                if (!ok)
                    NativeMainThread.Run(() => GP_Player.NotifyNativeLogout(false));
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[GamePush Native] Logout failed: " + exception.Message);
                NativeMainThread.Run(() => GP_Player.NotifyNativeLogout(false));
            }
        }

        static bool TryBuildRequest(out string url, out string prefix, out string param, out string tokenType,
            out string redirectUri)
        {
            url = "";
            prefix = "";
            param = "code";
            tokenType = "ExchangeToken";
            redirectUri = "";
            var auth = NativeCore.Auth;
            var service = auth.ActiveService ?? "";
            if (service == NativeAuthConfig.Google)
            {
                if (string.IsNullOrEmpty(auth.GoogleClientId))
                    return false;
                redirectUri = auth.GoogleRedirectUri;
                prefix = redirectUri;
                url = "https://accounts.google.com/o/oauth2/v2/auth?client_id=" + Uri.EscapeDataString(auth.GoogleClientId) +
                      "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                      "&response_type=code&scope=email%20profile&access_type=offline&state=STATE";
                return true;
            }
            if (service == NativeAuthConfig.Yandex)
            {
                if (string.IsNullOrEmpty(auth.YandexClientId))
                    return false;
                redirectUri = auth.YandexRedirectUri;
                prefix = redirectUri;
                url = "https://oauth.yandex.ru/authorize?response_type=code&client_id=" +
                      Uri.EscapeDataString(auth.YandexClientId) +
                      "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                      "&scope=" + Uri.EscapeDataString("login:info") +
                      "&state=random_state_string";
                return true;
            }
            if (service == NativeAuthConfig.Xsolla)
            {
                if (string.IsNullOrEmpty(auth.XsollaLoginProjectId))
                    return false;
                tokenType = "";
                redirectUri = "";
                param = "token";
                prefix = auth.XsollaRedirectUri;
                url = "https://login-widget.xsolla.com/latest/?projectId=" +
                      Uri.EscapeDataString(auth.XsollaLoginProjectId) +
                      "&locale=en&callbackUrl=" + Uri.EscapeDataString(auth.XsollaRedirectUri);
                return true;
            }
            return false;
        }
    }
}
