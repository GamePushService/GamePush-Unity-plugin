using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Socials : MonoBehaviour
    {
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
            if (GP_ConsoleController.Instance.SocialsConsoleLogs)
                Console.Log("SOCIALS: ", "SHARE");
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
            if (GP_ConsoleController.Instance.SocialsConsoleLogs)
                Console.Log("SOCIALS: ", "POST");
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
            if (GP_ConsoleController.Instance.SocialsConsoleLogs)
                Console.Log("SOCIALS: ", "INVITE");
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
            if (GP_ConsoleController.Instance.SocialsConsoleLogs)
                Console.Log("SOCIALS: ", "JOIN COMMUNITY");
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
            if (GP_ConsoleController.Instance.SocialsConsoleLogs)
                Console.Log("SOCIALS: ", "COMMUNITY LINK");
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
            if (GP_ConsoleController.Instance.SocialsConsoleLogs)
                Console.Log("SOCIALS: IS SUPPORTS SHARE: ", "TRUE");
            return true;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsNativeShare();
        public static bool IsSupportsNativeShare()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativeShare() == "true";
#else
            if (GP_ConsoleController.Instance.SocialsConsoleLogs)
                Console.Log("SOCIALS: IS SUPPORTS NATIVE SHARE: ", "TRUE");
            return true;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsNativePosts();
        public static bool IsSupportsNativePosts()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativePosts() == "true";
#else
            if (GP_ConsoleController.Instance.SocialsConsoleLogs)
                Console.Log("SOCIALS: IS SUPPORTS NATIVE POSTS: ", "TRUE");
            return true;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsNativeInvite();
        public static bool IsSupportsNativeInvite()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativeInvite() == "true";
#else
            if (GP_ConsoleController.Instance.SocialsConsoleLogs)
                Console.Log("SOCIALS: IS SUPPORTS NATIVE INVITE: ", "TRUE");
            return true;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Socials_CanJoinCommunity();
        public static bool CanJoinCommunity()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_CanJoinCommunity() == "true";
#else
            if (GP_ConsoleController.Instance.SocialsConsoleLogs)
                Console.Log("SOCIALS: CAN JOIN COMMUNITY: ", "TRUE");
            return true;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Socials_IsSupportsNativeCommunityJoin();
        public static bool IsSupportsNativeCommunityJoin()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Socials_IsSupportsNativeCommunityJoin() == "true";
#else
            if (GP_ConsoleController.Instance.SocialsConsoleLogs)
                Console.Log("SOCIALS: IS SUPPORTS NATIVE COMMUNITY JOIN: ", "TRUE");
            return true;
#endif
        }


        private void CallSocialsShare(string success) => OnShare?.Invoke(success == "true");
        private void CallSocialsPost(string success) => OnPost?.Invoke(success == "true");
        private void CallSocialsInvite(string success) => OnInvite?.Invoke(success == "true");
        private void CallSocialsJoinCommunity(string success) => OnJoinCommunity?.Invoke(success == "true");
    }
}