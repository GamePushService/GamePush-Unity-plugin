using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using GP_Utilities;
using GP_Utilities.Console;

namespace GamePush
{
    public class GP_App : MonoBehaviour
    {
        public static event UnityAction<int> OnReviewResult;
        public static event UnityAction<string> OnReviewClose;
        public static event UnityAction<bool> OnAddShortcut;

        private static event Action<int> _onReviewResult;
        private static event Action<string> _onReviewClose;
        private static event Action<bool> _onAddShortcut;

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
        
        public async static void GetImage(Image image)
        {
            string cover = GP_App_Image();
            if (cover == null || cover == "") return;
            await GP_Utility.DownloadImageAsync(cover, image);
        }

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
        public static void ReviewRequest(Action<int> onReviewResult = null, Action<string> onReviewClose = null)
        {
            _onReviewResult = onReviewResult;
            _onReviewClose = onReviewClose;
            
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_App_ReviewRequest();
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("APP: ReviewRequest");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_App_CanReview();
        public static bool CanReview()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_CanReview() == "true";
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("APP: CanReview: ", "TRUE");
            
            return GP_Settings.instance.GetPlatformSettings().CanReview;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_App_AddShortcut();
        public static void AddShortcut(Action<bool> onAddShortcut = null)
        {
            _onAddShortcut = onAddShortcut;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_App_AddShortcut();
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("APP: AddShortcut");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_App_CanAddShortcut();
        public static bool CanAddShortcut()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_CanAddShortcut() == "true";
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("APP: CanAddShortcut: ", "TRUE");
            return true;
#endif
        }

        private void CallAddShortcut(string success)
        {
            OnAddShortcut?.Invoke(success == "true");
            _onAddShortcut?.Invoke(success == "true");
        }

        private void CallReviewResult(int rating)
        {
            OnReviewResult?.Invoke(rating);
            _onReviewResult?.Invoke(rating);
        }

        private void CallReviewClose(string error)
        {
            OnReviewClose?.Invoke(error);
            _onReviewClose?.Invoke(error);
        }

    }
}
