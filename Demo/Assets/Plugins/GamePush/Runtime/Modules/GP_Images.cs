using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities;
using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Images : MonoBehaviour
    {
        public static event UnityAction<List<ImageData>> OnImagesFetchSuccess;
        public static event UnityAction<string> OnImagesFetchError;

        public static event UnityAction<bool> OnImagesCanLoadMore;

        private static event Action<List<ImageData>> _onImagesFetchSuccess;
        private static event Action<string> _onImagesFetchError;



        [DllImport("__Internal")]
        private static extern void GP_Images_Choose();
        public static void Choose()
        {

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Images_Choose();
#else
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("Images: ", "Choose");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Images_Upload(string tags);
        public static void Upload(string[] tags = null)
        {

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Images_Upload(JsonUtility.ToJson(tags));
#else
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("Images: ", "Upload");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Images_UploadUrl(string url, string tags);
        public static void UploadUrl(string url, string[] tags = null)
        {

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Images_Upload(url, JsonUtility.ToJson(tags));
#else
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("Images: ", "Upload");
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
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("Images: ", "FETCH");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Images_FetchMore(string filter);
        public static void FetchMore(ImagesFetchFilter filter = null)
        {

#if !UNITY_EDITOR && UNITY_WEBGL
            if (filter == null)
                GP_Images_FetchMore(JsonUtility.ToJson(new ImagesFetchFilter()));
            else
                GP_Images_FetchMore(JsonUtility.ToJson(filter));
#else
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("Images: ", "FETCH");
#endif
        }



        private void CallImagesFetchSuccess(string result)
        {
            List<ImageData> images = GP_JSON.GetList<ImageData>(result);

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
            Console.Log("Result: " + result);
        }

        private void CallImagesUploadError(string error)
        {
            Console.Log("Error: " + error);
        }

        private void CallImagesChooseFile(string result)
        {
            Console.Log("Result: " + result);
        }

        private void CallImagesChooseFileError(string error)
        {
            Console.Log("Error: " + error);
        }

        private void CallImagesResize(string result)
        {

        }

        private void CallImagesResizeError(string error)
        {

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
        public int width;
        public int height;
        public bool cutBySize;
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
