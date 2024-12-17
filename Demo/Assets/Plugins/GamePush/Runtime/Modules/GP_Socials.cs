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
            ConsoleLog("SHARE");
            OnShare?.Invoke(true);
#endif
        }

        public static void Post(string text = "", string url = "", string image = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Socials_Post(text, url, image);
#else
            ConsoleLog("POST");
            OnPost?.Invoke(true);
#endif
        }

        public static void Invite(string text = "", string url = "", string image = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Socials_Invite(text, url, image);
#else

            ConsoleLog("INVITE");
            OnInvite?.Invoke(true);
#endif
        }

        public static void JoinCommunity()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Socials_JoinCommunity();
#else

            ConsoleLog("JOIN COMMUNITY");
            OnJoinCommunity?.Invoke(true);
#endif
        }

        public static string CommunityLink()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_CommunityLink();
#else

            ConsoleLog("COMMUNITY LINK");
            return "GP_LINK";
#endif
        }

        public static bool IsSupportsShare()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsShare() == "true";
#else

            ConsoleLog("SOCIALS: IS SUPPORTS SHARE: " + GP_Settings.instance.GetPlatformSettings().IsSupportsShare);
            return GP_Settings.instance.GetPlatformSettings().IsSupportsShare;
#endif
        }

        public static bool IsSupportsNativeShare()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativeShare() == "true";
#else

            ConsoleLog("SOCIALS: IS SUPPORTS NATIVE SHARE: " + GP_Settings.instance.GetPlatformSettings().IsSupportsNativeShare);
            return GP_Settings.instance.GetPlatformSettings().IsSupportsNativeShare;
#endif
        }

        public static bool IsSupportsNativePosts()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativePosts() == "true";
#else

            ConsoleLog("SOCIALS: IS SUPPORTS NATIVE POSTS: " + GP_Settings.instance.GetPlatformSettings().IsSupportsNativePosts);
            return GP_Settings.instance.GetPlatformSettings().IsSupportsNativePosts;
#endif
        }

        public static bool IsSupportsNativeInvite()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativeInvite() == "true";
#else

            ConsoleLog("SOCIALS: IS SUPPORTS NATIVE INVITE: " + GP_Settings.instance.GetPlatformSettings().IsSupportsNativeInvite);
            return GP_Settings.instance.GetPlatformSettings().IsSupportsNativeInvite;
#endif
        }

        public static bool CanJoinCommunity()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_CanJoinCommunity() == "true";
#else

            ConsoleLog("SOCIALS: CAN JOIN COMMUNITY: " + GP_Settings.instance.GetPlatformSettings().CanJoinCommunity);
            return GP_Settings.instance.GetPlatformSettings().CanJoinCommunity;
#endif
        }

        public static bool IsSupportsNativeCommunityJoin()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativeCommunityJoin() == "true";
#else

            ConsoleLog("SOCIALS: IS SUPPORTS NATIVE COMMUNITY JOIN: " + GP_Settings.instance.GetPlatformSettings().IsSupportsNativeCommunityJoin);
            return GP_Settings.instance.GetPlatformSettings().IsSupportsNativeCommunityJoin;
#endif
        }

        public static string MakeShareLink(string content = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_MakeShareLink(content);
#else

            ConsoleLog("SHARE LINK");
            return "GP_LINK";
#endif
        }

        public static int GetSharePlayerID()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_GetSharePlayerID();
#else

            ConsoleLog("SHARE PLAYER ID");
            return GP_Player.GetID();
#endif
        }

        public static string GetShareContent()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_GetShareContent();
#else

            ConsoleLog("SHARE CONTENT");
            return "GP_LINK";
#endif
        }


        private void CallSocialsShare(string success) => OnShare?.Invoke(success == "true");
        private void CallSocialsPost(string success) => OnPost?.Invoke(success == "true");
        private void CallSocialsInvite(string success) => OnInvite?.Invoke(success == "true");
        private void CallSocialsJoinCommunity(string success) => OnJoinCommunity?.Invoke(success == "true");
    }
}