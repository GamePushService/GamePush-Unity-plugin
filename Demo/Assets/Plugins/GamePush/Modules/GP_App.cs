using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

using GP_Utilities;
using GP_Utilities.Console;

namespace GamePush
{
    public class GP_App : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern string GP_App_Title();
        public static string Title()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_Title();
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("APP: TITLE: ", "-> NULL");
            return null;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_App_Description();
        public static string Description()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_Description();
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("APP: DESCRIPTION: ", "-> NULL");
            return null;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_App_Image();
        public async static void GetImage(Image image) => await GP_Utility.DownloadImageAsync(GP_App_Image(), image);

        public static string ImageUrl()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_Image();
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("APP: IMAGE URL: ", "-> URL");
            return "URL";
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_App_Url();
        public static string Url()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_Url();
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("APP: URL: ", "-> URL");
            return "URL";
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_App_ReviewRequest();
        public static string ReviewRequest()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_ReviewRequest();
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("APP: ReviewRequest: ", "-> URL");
            return "URL";
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_App_CanReview();
        public static string CanReview()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_CanReview();
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("APP: CanReview: ", "-> URL");
            return "URL";
#endif
        }
    }
}
