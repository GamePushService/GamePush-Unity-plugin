using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.ConsoleController;

namespace GamePush
{
    public class GP_Documents : GP_Module
    {
        private void OnValidate() => SetModuleName(ModuleName.Documents);

        public static event UnityAction OnDocumentsOpen;
        public static event UnityAction OnDocumentsClose;

        public static event UnityAction<string> OnFetchSuccess;
        public static event UnityAction OnFetchError;

        private static event Action<string> _onFetchSuccess;
        private static event Action _onFetchError;

        private static event Action _onDocumentsOpen;
        private static event Action _onDocumentsClose;


        [DllImport("__Internal")]
        private static extern void GP_Documents_Open();
        public static void Open(Action onDocumentsOpen = null, Action onDocumentsClose = null)
        {
            _onDocumentsOpen = onDocumentsOpen;
            _onDocumentsClose = onDocumentsClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Documents_Open();
#else

            ConsoleLog("OPEN");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Documents_Fetch();
        public static void Fetch(Action<string> onFetchSuccess = null, Action onFetchError = null)
        {
            _onFetchSuccess = onFetchSuccess;
            _onFetchError = onFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Documents_Fetch();
#else

            ConsoleLog("FETCH");
#endif
        }


        private void CallOnDocumentsOpen() { OnDocumentsOpen?.Invoke(); _onDocumentsOpen?.Invoke(); }
        private void CallOnDocumentsClose() { OnDocumentsClose?.Invoke(); _onDocumentsClose?.Invoke(); }


        private void CallOnDocumentsFetchSuccess(string data) { OnFetchSuccess?.Invoke(data); _onFetchSuccess?.Invoke(data); }
        private void CallOnDocumentsFetchError() { OnFetchError?.Invoke(); _onFetchError?.Invoke(); }
    }
}