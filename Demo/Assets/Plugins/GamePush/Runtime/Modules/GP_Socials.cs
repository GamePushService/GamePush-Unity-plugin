using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace GamePush
{
    public class GP_Socials : GP_Module
    {
        private void OnValidate() => SetModuleName(ModuleName.Socials);

        public static event UnityAction<bool> OnShare;
        public static event UnityAction<bool> OnPost;
        public static event UnityAction<bool> OnInvite;
        public static event UnityAction<bool> OnJoinCommunity;

        [DllImport("__Internal")]
        private static extern void GP_Socials_Share(string text, string url, string image);
        public static void Share(string text = "", string url = "", string image = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Socials_Share(text, url, image);
#else
            ConsoleLog("SHARE");
            OnShare?.Invoke(true);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Socials_Post(string text, string url, string image);
        public static void Post(string text = "", string url = "", string image = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Socials_Post(text, url, image);
#else
            ConsoleLog("POST");
            OnPost?.Invoke(true);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Socials_Invite(string text, string url, string image);
        public static void Invite(string text = "", string url = "", string image = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Socials_Invite(text, url, image);
#else

            ConsoleLog("INVITE");
            OnInvite?.Invoke(true);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Socials_JoinCommunity();
        public static void JoinCommunity()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Socials_JoinCommunity();
#else

            ConsoleLog("JOIN COMMUNITY");
            OnJoinCommunity?.Invoke(true);
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Socials_CommunityLink();
        public static string CommunityLink()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_CommunityLink();
#else

            ConsoleLog("COMMUNITY LINK");
            return "GP_LINK";
#endif
        }




        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsShare();
        public static bool IsSupportsShare()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsShare() == "true";
#else

            Console.Log("SOCIALS: IS SUPPORTS SHARE: ", "TRUE");
            return GP_Settings.instance.GetPlatformSettings().IsSupportsShare;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsNativeShare();
        public static bool IsSupportsNativeShare()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativeShare() == "true";
#else

            Console.Log("SOCIALS: IS SUPPORTS NATIVE SHARE: ", "TRUE");
            return GP_Settings.instance.GetPlatformSettings().IsSupportsNativeShare;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsNativePosts();
        public static bool IsSupportsNativePosts()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativePosts() == "true";
#else

            Console.Log("SOCIALS: IS SUPPORTS NATIVE POSTS: ", "TRUE");
            return GP_Settings.instance.GetPlatformSettings().IsSupportsNativePosts;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsNativeInvite();
        public static bool IsSupportsNativeInvite()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativeInvite() == "true";
#else

            Console.Log("SOCIALS: IS SUPPORTS NATIVE INVITE: ", "TRUE");
            return GP_Settings.instance.GetPlatformSettings().IsSupportsNativeInvite;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Socials_CanJoinCommunity();
        public static bool CanJoinCommunity()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_CanJoinCommunity() == "true";
#else

            Console.Log("SOCIALS: CAN JOIN COMMUNITY: ", "TRUE");
            return GP_Settings.instance.GetPlatformSettings().CanJoinCommunity;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsNativeCommunityJoin();
        public static bool IsSupportsNativeCommunityJoin()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativeCommunityJoin() == "true";
#else

            Console.Log("SOCIALS: IS SUPPORTS NATIVE COMMUNITY JOIN: ", "TRUE");
            return GP_Settings.instance.GetPlatformSettings().IsSupportsNativeCommunityJoin;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Socials_MakeShareLink(string content);
        public static string MakeShareLink(string content = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_MakeShareLink(content);
#else

            ConsoleLog("SHARE LINK");
            return "GP_LINK";
#endif
        }

        [DllImport("__Internal")]
        private static extern int GP_Socials_GetSharePlayerID();
        public static int GetSharePlayerID()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_GetSharePlayerID();
#else

            ConsoleLog("SHARE PLAYER ID");
            return GP_Player.GetID();
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Socials_GetShareContent();
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