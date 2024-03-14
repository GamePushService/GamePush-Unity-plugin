using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Images : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void GP_Images_Fetch(string filter);
        public static void Fetch(ImagesFetchFilter filter = null)
        {

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
        private static extern void GP_Images_Upload();
        public static void Upload()
        {

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Images_Upload();
#else
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("Images: ", "Upload");
#endif
        }

        private void CallImagesFetchSuccess(string result)
        {
            Console.Log("Result: " + result);
        }

        private void CallImagesFetchError(string error)
        {
            Console.Log("Error: " + error);
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
