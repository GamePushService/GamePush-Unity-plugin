using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using GamePush.Tools;

namespace GamePush
{
    public class GP_App : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.App);

        public static event UnityAction<int> OnReviewResult;
        public static event UnityAction<string> OnReviewClose;
        public static event UnityAction<bool> OnAddShortcut;

        private static event Action<int> _onReviewResult;
        private static event Action<string> _onReviewClose;
        private static event Action<bool> _onAddShortcut;

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string GP_App_Title();

        [DllImport("__Internal")]
        private static extern string GP_App_Description();

        [DllImport("__Internal")]
        private static extern string GP_App_Image();

        [DllImport("__Internal")]
        private static extern string GP_App_Url();

        [DllImport("__Internal")]
        private static extern string GP_App_ReviewRequest();

        [DllImport("__Internal")]
        private static extern string GP_App_IsAlreadyReviewed();

        [DllImport("__Internal")]
        private static extern string GP_App_CanReview();

        [DllImport("__Internal")]
        private static extern string GP_App_AddShortcut();

        [DllImport("__Internal")]
        private static extern string GP_App_CanAddShortcut();
#endif

        private void OnEnable()
        {
            CoreSDK.App.OnAddShortcut += CallAddShortcutBool;

            CoreSDK.App.OnReviewResult += CallReviewResult;
            CoreSDK.App.OnReviewClose += CallReviewClose;
        }

        private void OnDisable()
        {
            CoreSDK.App.OnAddShortcut -= CallAddShortcutBool;

            CoreSDK.App.OnReviewResult -= CallReviewResult;
            CoreSDK.App.OnReviewClose -= CallReviewClose;
        }

        public static string Title()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_Title();
#else
            return CoreSDK.App.Title();
#endif
        }


        public static string Description()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_Description();
#else
            return CoreSDK.App.Description();
#endif
        }

        public async static void GetImage(Image image)
        {
            string cover = "";

#if !UNITY_EDITOR && UNITY_WEBGL
            cover = GP_App_Image();
#else
            cover = CoreSDK.App.ProjectIcon();
#endif

            if (cover == null || cover == "") return;
            await UtilityImage.DownloadImageAsync(cover, image);
        }

        public static string ImageUrl()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_Image();
#else
            return CoreSDK.App.ProjectIcon();
#endif
        }

        public static string Url()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_Url();
#else
            return CoreSDK.App.AppLink();
#endif
        }

        public static void ReviewRequest(Action<int> onReviewResult = null, Action<string> onReviewClose = null)
        {
            _onReviewResult = onReviewResult;
            _onReviewClose = onReviewClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_App_ReviewRequest();
#else
            CoreSDK.App.ReviewRequest();
#endif
        }

        public static bool IsAlreadyReviewed()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_IsAlreadyReviewed() == "true";
#else
            //bool result = GP_Settings.instance.GetPlatformSettings().IsAlreadyReviewed;
            bool result = CoreSDK.App.IsAlreadyReviewed();
            return result;
#endif
        }

        public static bool CanReview()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_CanReview() == "true";
#else
            //bool result = GP_Settings.instance.GetPlatformSettings().CanReview;
            bool result = CoreSDK.App.CanReview();
            return result;
#endif
        }

        public static void AddShortcut(Action<bool> onAddShortcut = null)
        {
            _onAddShortcut = onAddShortcut;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_App_AddShortcut();
#else
            CoreSDK.App.AddShortcut();
#endif
        }

        public static bool CanAddShortcut()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_App_CanAddShortcut() == "true";
#else
            return CoreSDK.App.CanAddShortcut();
#endif
        }

        private void CallAddShortcutBool(bool success)
        {
            OnAddShortcut?.Invoke(success);
            _onAddShortcut?.Invoke(success);
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
