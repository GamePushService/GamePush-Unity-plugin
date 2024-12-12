using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace GamePush
{
    public class GP_Socials : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Socials);

        public static event UnityAction<bool> OnShare;
        public static event UnityAction<bool> OnPost;
        public static event UnityAction<bool> OnInvite;
        public static event UnityAction<bool> OnJoinCommunity;

        private void OnEnable()
        {
            CoreSDK.socials.OnShare += (bool success) => OnShare?.Invoke(success);
            CoreSDK.socials.OnPost += (bool success) => OnPost?.Invoke(success);
            CoreSDK.socials.OnInvite += (bool success) => OnInvite?.Invoke(success);
            CoreSDK.socials.OnJoinCommunity += (bool success) => OnJoinCommunity?.Invoke(success);
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Socials_Share(string text, string url, string image);
        [DllImport("__Internal")]
        private static extern void GP_Socials_Post(string text, string url, string image);
        [DllImport("__Internal")]
        private static extern void GP_Socials_Invite(string text, string url, string image);
        [DllImport("__Internal")]
        private static extern void GP_Socials_JoinCommunity();
        [DllImport("__Internal")]
        private static extern string GP_Socials_CommunityLink();
        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsShare();
        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsNativeShare();
        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsNativePosts();
        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsNativeInvite();
        [DllImport("__Internal")]
        private static extern string GP_Socials_CanJoinCommunity();
        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsNativeCommunityJoin();
        [DllImport("__Internal")]
        private static extern string GP_Socials_MakeShareLink(string content);
        [DllImport("__Internal")]
        private static extern int GP_Socials_GetSharePlayerID();
        [DllImport("__Internal")]
        private static extern string GP_Socials_GetShareContent();
#endif


        public static void Share(string text = "", string url = "", string image = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Socials_Share(text, url, image);
#else
            CoreSDK.socials.Share(text, url, image);
#endif
        }

        public static void Post(string text = "", string url = "", string image = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Socials_Post(text, url, image);
#else
            CoreSDK.socials.Post(text, url, image);
#endif
        }

        public static void Invite(string text = "", string url = "", string image = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Socials_Invite(text, url, image);
#else
            CoreSDK.socials.Invite(text, url, image);
#endif
        }

        public static void JoinCommunity()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Socials_JoinCommunity();
#else
            CoreSDK.socials.JoinCommunity();
#endif
        }

        public static string CommunityLink()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_CommunityLink();
#else
            return CoreSDK.socials.CommunityLink();
#endif
        }

        public static bool IsSupportsShare()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsShare() == "true";
#elif !UNITY_EDITOR
            return CoreSDK.socials.IsSupportsShare();
#else
            ConsoleLog("SOCIALS: IS SUPPORTS SHARE: " + GP_Settings.instance.GetPlatformSettings().IsSupportsShare);
            return GP_Settings.instance.GetPlatformSettings().IsSupportsShare;
#endif
        }

        public static bool IsSupportsNativeShare()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativeShare() == "true";
#elif !UNITY_EDITOR
            return CoreSDK.socials.IsSupportsNativeShare();
#else
            return GP_Settings.instance.GetPlatformSettings().IsSupportsNativeShare;
#endif
        }

        public static bool IsSupportsNativePosts()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativePosts() == "true";
#elif !UNITY_EDITOR
            return CoreSDK.socials.IsSupportsNativePosts();
#else
            return GP_Settings.instance.GetPlatformSettings().IsSupportsNativePosts;
#endif
        }

        public static bool IsSupportsNativeInvite()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativeInvite() == "true";
#elif !UNITY_EDITOR
            return CoreSDK.socials.IsSupportsNativeInvite();
#else
            return GP_Settings.instance.GetPlatformSettings().IsSupportsNativeInvite;
#endif
        }

        public static bool CanJoinCommunity()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_CanJoinCommunity() == "true";
#elif !UNITY_EDITOR
            return CoreSDK.socials.CanJoinCommunity();
#else
            return GP_Settings.instance.GetPlatformSettings().CanJoinCommunity;
#endif
        }

        public static bool IsSupportsNativeCommunityJoin()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativeCommunityJoin() == "true";
#elif !UNITY_EDITOR
            return CoreSDK.socials.CanJoinCommunity();
#else
            return GP_Settings.instance.GetPlatformSettings().IsSupportsNativeCommunityJoin;
#endif
        }

        public static string MakeShareLink(string content = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_MakeShareLink(content);
#else
            return CoreSDK.socials.MakeShareLink(content);
#endif
        }

        public static int GetSharePlayerID()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_GetSharePlayerID();
#else
            return CoreSDK.socials.GetSharePlayerID();
#endif
        }

        public static string GetShareContent()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_GetShareContent();
#else
            return CoreSDK.socials.GetShareContent();
#endif
        }


        private void CallSocialsShare(string success) => OnShare?.Invoke(success == "true");
        private void CallSocialsPost(string success) => OnPost?.Invoke(success == "true");
        private void CallSocialsInvite(string success) => OnInvite?.Invoke(success == "true");
        private void CallSocialsJoinCommunity(string success) => OnJoinCommunity?.Invoke(success == "true");
    }
}