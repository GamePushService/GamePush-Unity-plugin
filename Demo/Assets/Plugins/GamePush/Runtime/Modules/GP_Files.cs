using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities.Console;
using GP_Utilities;

namespace GamePush
{
    public class GP_Files : MonoBehaviour
    {
        public static event UnityAction<FileData> OnUploadSuccess;
        public static event UnityAction OnUploadError;

        public static event UnityAction<FileData> OnUploadUrlSuccess;
        public static event UnityAction OnUploadUrlError;

        public static event UnityAction<FileData> OnUploadContentSuccess;
        public static event UnityAction OnUploadContentError;

        public static event UnityAction<string> OnChooseFile;
        public static event UnityAction OnChooseFileError;

        public static event UnityAction<string> OnLoadContent;
        public static event UnityAction OnLoadContentError;

        public static event UnityAction<List<FileData>, bool> OnFetchSuccess;
        public static event UnityAction OnFetchError;

        public static event UnityAction<List<FileData>, bool> OnFetchMoreSuccess;
        public static event UnityAction OnFetchMoreError;


        private static event Action<FileData> _onUpload;
        private static event Action _onUploadError;

        private static event Action<FileData> _onUploadUrl;
        private static event Action _onUploadUrlError;

        private static event Action<FileData> _onUploadContent;
        private static event Action _onUploadContentError;

        private static event Action<string> _onLoadContent;
        private static event Action _onLoadContentError;

        private static event Action<string> _onFileChoose;
        private static event Action _onFileChooseError;

        private static event Action<List<FileData>, bool> _onFetch;
        private static event Action _onFetchError;

        private static event Action<List<FileData>, bool> _onFetchMore;
        private static event Action _onFetchMoreError;




        [DllImport("__Internal")]
        private static extern void GP_Files_Upload(string tags);
        public static void Upload(string tags, Action<FileData> onUpload = null, Action onUploadError = null)
        {
            _onUpload = onUpload;
            _onUploadError = onUploadError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Files_Upload(tags);
#else
            if (GP_ConsoleController.Instance.FilesConsoleLogs)
                Console.Log("FILES: UPLOAD: ", tags);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Files_UploadUrl(string url, string filename, string tags);
        public static void UploadUrl(string url, string filename = "", string tags = "", Action<FileData> onUploadUrl = null, Action onUploadUrlError = null)
        {
            _onUploadUrl = onUploadUrl;
            _onUploadUrlError = onUploadUrlError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Files_UploadUrl(url, filename, tags);
#else
            if (GP_ConsoleController.Instance.FilesConsoleLogs)
                Console.Log("FILES: UPLOAD URL: ", url + " " + filename + " " + tags);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Files_UploadContent(string content, string filename, string tags);
        public static void UploadContent(string content, string filename, string tags, Action<FileData> onUploadContent = null, Action onUploadContentError = null)
        {
            _onUploadContent = onUploadContent;
            _onUploadContentError = onUploadContentError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Files_UploadContent(content, filename, tags);
#else
            if (GP_ConsoleController.Instance.FilesConsoleLogs)
                Console.Log("FILES: UPLOAD CONTENT: ", content + " " + filename + " " + tags);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Files_LoadContent(string url);
        public static void LoadContent(string url, Action<string> onLoadContent = null, Action onLoadContentError = null)
        {
            _onLoadContent = onLoadContent;
            _onLoadContentError = onLoadContentError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Files_LoadContent(url);
#else
            if (GP_ConsoleController.Instance.FilesConsoleLogs)
                Console.Log("FILES: ", "LOAD CONTENT: " + url);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Files_ChooseFile(string type);
        public static void ChooseFile(string type, Action<string> onFileChoose = null, Action onFileChooseError = null)
        {
            _onFileChoose = onFileChoose;
            _onFileChooseError = onFileChooseError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Files_ChooseFile(type);
#else
            if (GP_ConsoleController.Instance.FilesConsoleLogs)
                Console.Log("FILES: ", "CHOOSE FILE: " + type);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Files_Fetch(string filter);
        public static void Fetch(FilesFetchFilter filter = null, Action<List<FileData>, bool> onFetch = null, Action onFetchError = null)
        {
            _onFetch = onFetch;
            _onFetchError = onFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
            if (filter == null)
                GP_Files_Fetch(JsonUtility.ToJson(new FilesFetchFilter()));
            else
                GP_Files_Fetch(JsonUtility.ToJson(filter));
#else
            if (GP_ConsoleController.Instance.FilesConsoleLogs)
                Console.Log("FILES: ", "FETCH");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Files_FetchMore(string filter);
        public static void FetchMore(FilesFetchMoreFilter filter = null, Action<List<FileData>, bool> onFetchMore = null, Action onFetchMoreError = null)
        {
            _onFetchMore = onFetchMore;
            _onFetchMoreError = onFetchMoreError;

#if !UNITY_EDITOR && UNITY_WEBGL
            if (filter == null)
                GP_Files_FetchMore(JsonUtility.ToJson(new FilesFetchFilter()));
            else
                GP_Files_FetchMore(JsonUtility.ToJson(filter));
#else
            if (GP_ConsoleController.Instance.FilesConsoleLogs)
                Console.Log("FILES: ", "FETCH MORE");
#endif
        }



        private void CallFilesUploadSuccess(string data) { OnUploadSuccess?.Invoke(JsonUtility.FromJson<FileData>(data)); _onUpload?.Invoke(JsonUtility.FromJson<FileData>(data)); }
        private void CallFilesUploadError() { OnUploadError?.Invoke(); _onUploadError?.Invoke(); }


        private void CallFilesUploadUrlSuccess(string data) { OnUploadUrlSuccess?.Invoke(JsonUtility.FromJson<FileData>(data)); _onUploadUrl?.Invoke(JsonUtility.FromJson<FileData>(data)); }
        private void CallFilesUploadUrlError() { OnUploadUrlError?.Invoke(); _onUploadUrlError?.Invoke(); }


        private void CallFilesUploadContentSuccess(string data) { OnUploadContentSuccess?.Invoke(JsonUtility.FromJson<FileData>(data)); _onUploadContent?.Invoke(JsonUtility.FromJson<FileData>(data)); }
        private void CallFilesUploadContentError() { OnUploadContentError?.Invoke(); _onUploadContentError?.Invoke(); }


        private void CallFilesLoadContentSuccess(string text) { OnLoadContent?.Invoke(text); _onLoadContent?.Invoke(text); }
        private void CallFilesLoadContentError() { OnLoadContentError?.Invoke(); _onLoadContentError?.Invoke(); }


        private void CallFilesChooseFile(string tempUrl) { OnChooseFile?.Invoke(tempUrl); _onFileChoose?.Invoke(tempUrl); }
        private void CallFilesChooseFileError() { OnChooseFileError?.Invoke(); _onFileChooseError?.Invoke(); }


        private bool _canLoadMoreFetch;
        private void CallFilesFetchCanLoadMore(string value) => _canLoadMoreFetch = value == "true";
        private void CallFilesFetchError() { OnFetchError?.Invoke(); _onFetchError?.Invoke(); }
        private void CallFilesFetchSuccess(string data)
        {
            var fetchData = GP_JSON.GetList<FileData>(data);
            OnFetchSuccess?.Invoke(fetchData, _canLoadMoreFetch);
            _onFetch?.Invoke(fetchData, _canLoadMoreFetch);
        }

        private bool _canLoadMoreFetchMore;
        private void CallFilesFetchMoreCanLoadMore(string value) => _canLoadMoreFetchMore = value == "true";
        private void CallFilesFetchMoreError() { OnFetchMoreError?.Invoke(); _onFetchMoreError?.Invoke(); }
        private void CallFilesFetchMoreSuccess(string data)
        {
            var fetchData = GP_JSON.GetList<FileData>(data);
            OnFetchMoreSuccess?.Invoke(fetchData, _canLoadMoreFetchMore);
            _onFetchMore?.Invoke(fetchData, _canLoadMoreFetchMore);
        }

    }

    [System.Serializable]
    public class FileData
    {
        public string id;
        public int playerId;
        public string name;
        public string src;
        public float size;
        public string[] tags;
    }

    [System.Serializable]
    public class FilesFetchFilter
    {
        public string[] tags;
        public int playerId;
        public int limit = 10;
        public int offset = 0;
    }

    [System.Serializable]
    public class FilesFetchMoreFilter
    {
        public string[] tags;
        public int playerId;
        public int limit = 10;
    }
}