using GamePush;

namespace GamePush.Native
{
    public static class NativeAuth
    {
        public static void Login()
        {
            if (!NativeCore.Auth.HasIntegratedAuth)
            {
                GP_Logger.Info("AUTH", "Auth is not connected for this platform");
                NativeMainThread.Run(() => GP_Player.NotifyNativeLogin(false));
                return;
            }
            if (!NativeAuthSdks.Current.IsAvailable)
            {
                NativeAuthSdks.LogMissingOnce();
                NativeMainThread.Run(() => GP_Player.NotifyNativeLogin(false));
                return;
            }
            NativeAuthSdks.Current.Login();
        }

        public static void Logout()
        {
            if (!NativeCore.Auth.IsLogoutAvailable)
            {
                NativeMainThread.Run(() => GP_Player.NotifyNativeLogout(false));
                return;
            }
            NativeAuthSdks.Current.Logout();
        }
    }
}
