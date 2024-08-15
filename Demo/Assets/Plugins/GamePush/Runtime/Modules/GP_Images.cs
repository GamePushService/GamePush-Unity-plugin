using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Utilities;

namespace GamePush
{
    public class GP_Images : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Images);

        public static event UnityAction<List<ImageData>> OnImagesFetchSuccess;
        public static event UnityAction<string> OnImagesFetchError;
        public static event UnityAction<bool> OnImagesCanLoadMore;

        public static event UnityAction<ImageData> OnImagesUploadSuccess;
        public static event UnityAction<string> OnImagesUploadError;

        public static event UnityAction<ImageData> OnImagesUploadUrlSuccess;
        public static event UnityAction<string> OnImagesUploadUrlError;

        public static event UnityAction<string> OnImagesChooseFile;
        public static event UnityAction<string> OnImagesChooseError;

        public static event UnityAction<string> OnImagesResize;
        public static event UnityAction<string> OnImagesResizeError;

        private static event Action<List<ImageData>> _onImagesFetchSuccess;
        private static event Action<string> _onImagesFetchError;

        public static event Action<ImageData> _onImagesUploadSuccess;
        public static event Action<string> _onImagesUploadError;

        public static event Action<ImageData> _onImagesUploadUrlSuccess;
        public static event Action<string> _onImagesUploadUrlError;

        public static event Action<string> _onImagesChooseFile;
        public static event Action<string> _onImagesChooseError;

        public static event Action<string> _onImagesResize;
        public static event Action<string> _onImagesResizeError;

        [DllImport("__Internal")]
        private static extern void GP_Images_Choose();
        public static void Choose(Action<string> onImagesChooseFile = null, Action<string> onImagesChooseError = null)
        {
            _onImagesChooseFile = onImagesChooseFile;
            _onImagesChooseError = onImagesChooseError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Images_Choose();
#else

            ConsoleLog("Choose");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Images_Upload(string tags);
        public static void Upload(string[] tags = null, Action<ImageData> onImagesUploadSuccess = null, Action<string> onImagesUploadError = null)
        {
            _onImagesUploadSuccess = onImagesUploadSuccess;
            _onImagesUploadError = onImagesUploadError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Images_Upload(string.Join(",", tags));
#else

            ConsoleLog("Upload");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Images_UploadUrl(string url, string tags);
        public static void UploadUrl(string url, string[] tags = null, Action<ImageData> onImagesUploadUrlSuccess = null, Action<string> onImagesUploadUrlError = null)
        {
            _onImagesUploadUrlSuccess = onImagesUploadUrlSuccess;
            _onImagesUploadUrlError = onImagesUploadUrlError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Images_UploadUrl(url, string.Join(",", tags));
#else

            ConsoleLog("Upload");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Images_Fetch(string filter);
        public static void Fetch(ImagesFetchFilter filter = null, Action<List<ImageData>> onImagesFetchSuccess = null, Action<string> onImagesFetchError = null)
        {
            _onImagesFetchSuccess = onImagesFetchSuccess;
            _onImagesFetchError = onImagesFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
            if (filter == null)
                GP_Images_Fetch(JsonUtility.ToJson(new ImagesFetchFilter()));
            else
                GP_Images_Fetch(JsonUtility.ToJson(filter));
#else

            ConsoleLog("FETCH");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Images_FetchMore(string filter);
        public static void FetchMore(ImagesFetchFilter filter = null, Action<List<ImageData>> onImagesFetchSuccess = null, Action<string> onImagesFetchError = null)
        {
            _onImagesFetchSuccess = onImagesFetchSuccess;
            _onImagesFetchError = onImagesFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
            if (filter == null)
                GP_Images_FetchMore(JsonUtility.ToJson(new ImagesFetchFilter()));
            else
                GP_Images_FetchMore(JsonUtility.ToJson(filter));
#else

            ConsoleLog("FETCH MORE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Images_Resize(string filter);
        public static void Resize(ImageResizeData resizeData = null, Action<string> onImagesResize = null, Action<string> onImagesResizeError = null)
        {
            _onImagesResize = onImagesResize;
            _onImagesResizeError = onImagesResize;

#if !UNITY_EDITOR && UNITY_WEBGL
            if (resizeData == null)
                GP_Images_Resize(JsonUtility.ToJson(new ImageResizeData()));
            else
                GP_Images_Resize(JsonUtility.ToJson(resizeData));
#else

            ConsoleLog("RESIZE");
#endif
        }

        public static string FormatToPng(string url) => FormatUrl(url, ".png");

        public static string FormatUrl(string url, string format)
        {
            string formatUrl = url.Replace(".webp", format);
            return formatUrl;
        }


        private void CallImagesFetchSuccess(string result)
        {
            List<ImageData> images = UtilityJSON.GetList<ImageData>(result);

            _onImagesFetchSuccess?.Invoke(images);
            OnImagesFetchSuccess?.Invoke(images);
        }

        private void CallImagesFetchCanLoadMore(string canLoadMore)
        {
            bool canLoad = canLoadMore == "true";
            OnImagesCanLoadMore?.Invoke(canLoad);
        }

        private void CallImagesFetchError(string error)
        {
            _onImagesFetchError?.Invoke(error);
            OnImagesFetchError?.Invoke(error);
        }

        private void CallImagesUploadSuccess(string result)
        {
            ImageData imageData = UtilityJSON.Get<ImageData>(result);
            _onImagesUploadSuccess?.Invoke(imageData);
            OnImagesUploadSuccess?.Invoke(imageData);
        }

        private void CallImagesUploadError(string error)
        {
            _onImagesUploadError?.Invoke(error);
            OnImagesUploadError?.Invoke(error);
        }

        private void CallImagesUploadUrlSuccess(string result)
        {
            ImageData imageData = UtilityJSON.Get<ImageData>(result);
            _onImagesUploadUrlSuccess?.Invoke(imageData);
            OnImagesUploadUrlSuccess?.Invoke(imageData);
        }

        private void CallImagesUploadUrlError(string error)
        {
            _onImagesUploadUrlError?.Invoke(error);
            OnImagesUploadUrlError?.Invoke(error);
        }

        private void CallImagesChooseFile(string result)
        {
            _onImagesChooseFile?.Invoke(result);
            OnImagesChooseFile?.Invoke(result);
        }

        private void CallImagesChooseFileError(string error)
        {
            _onImagesChooseError?.Invoke(error);
            OnImagesChooseError?.Invoke(error);
        }

        private void CallImagesResize(string result)
        {
            _onImagesResize?.Invoke(result);
            OnImagesResize?.Invoke(result);
        }

        private void CallImagesResizeError(string error)
        {
            _onImagesResizeError?.Invoke(error);
            OnImagesResizeError?.Invoke(error);
        }

    }

    [System.Serializable]
    public class ImageData
    {
        public string id;
        public int playerId;
        public string src;
        public string[] tags;
        public int width;
        public int height;
    }

    [System.Serializable]
    public class ImageResizeData
    {
        public string url;
        public int width = 256;
        public int height = 256;
        public bool cutBySize = true;
    }

    [System.Serializable]
    public class ImagesFetchFilter
    {
        public string[] tags;
        public int playerId;
        public int limit = 10;
        public int offset = 0;
    }
}
