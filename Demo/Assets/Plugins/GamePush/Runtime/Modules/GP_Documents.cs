using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.ConsoleController;
using GamePush.Data;

namespace GamePush
{
    public class GP_Documents : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Documents);

        public static event UnityAction OnDocumentsOpen;
        public static event UnityAction OnDocumentsClose;

        public static event UnityAction<string> OnFetchSuccess;
        public static event UnityAction OnFetchError;

        private static event Action<string> _onFetchSuccess;
        private static event Action _onFetchError;

        private static event Action _onDocumentsOpen;
        private static event Action _onDocumentsClose;

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Documents_Open();
        [DllImport("__Internal")]
        private static extern void GP_Documents_Fetch();
#endif

        private void OnEnable()
        {
            CoreSDK.Documents.OnDocumentsOpen += CallOnDocumentsOpen;
            CoreSDK.Documents.OnDocumentsClose += CallOnDocumentsClose;
            CoreSDK.Documents.OnFetchSuccess += (DocumentData data) => CallOnDocumentsFetchSuccess(data.content);
            CoreSDK.Documents.OnFetchError += CallOnDocumentsFetchError;
        }

        public static void Open(Action onDocumentsOpen = null, Action onDocumentsClose = null)
        {
            _onDocumentsOpen = onDocumentsOpen;
            _onDocumentsClose = onDocumentsClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Documents_Open();
#else
            CoreSDK.Documents.Open();
#endif
        }

        public static void Fetch(Action<string> onFetchSuccess = null, Action onFetchError = null)
        {
            _onFetchSuccess = onFetchSuccess;
            _onFetchError = onFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Documents_Fetch();
#else
            CoreSDK.Documents.Fetch();
#endif
        }


        private void CallOnDocumentsOpen() { OnDocumentsOpen?.Invoke(); _onDocumentsOpen?.Invoke(); }
        private void CallOnDocumentsClose() { OnDocumentsClose?.Invoke(); _onDocumentsClose?.Invoke(); }

        private void CallOnDocumentsFetchSuccess(string data) { OnFetchSuccess?.Invoke(data); _onFetchSuccess?.Invoke(data); }
        private void CallOnDocumentsFetchError() { OnFetchError?.Invoke(); _onFetchError?.Invoke(); }
    }
}