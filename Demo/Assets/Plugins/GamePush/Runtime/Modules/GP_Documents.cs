using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.ConsoleController;
using GamePush.Native;
using GamePush.Overlays;

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


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Documents_Open();
        #endif
        public static void Open(Action onDocumentsOpen = null, Action onDocumentsClose = null)
        {
            _onDocumentsOpen = onDocumentsOpen;
            _onDocumentsClose = onDocumentsClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Documents_Open();
#else
            if (GamePushHost.UseNativeCore && GP_Overlays.Open(GP_OverlayKind.Document, new GP_DocumentArgs()))
                return;
            if (GP_Play2Web.Call("DocumentsOpen"))
                return;
            ConsoleLog("OPEN");
#endif
        }

        /// <summary>Opens a specific document type, e.g. PLAYER_PRIVACY_POLICY or PLAYER_TERMS_OF_USE.</summary>
        public static void Open(string type, string format = "TXT", Action onDocumentsOpen = null, Action onDocumentsClose = null)
        {
            _onDocumentsOpen = onDocumentsOpen;
            _onDocumentsClose = onDocumentsClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Documents_Open();
#else
            if (GamePushHost.UseNativeCore &&
                GP_Overlays.Open(GP_OverlayKind.Document, new GP_DocumentArgs { type = type, format = format }))
                return;
            if (GP_Play2Web.Call("DocumentsOpen"))
                return;
            ConsoleLog("OPEN: " + type);
#endif
        }

        internal static void NativeFireOpen() { OnDocumentsOpen?.Invoke(); _onDocumentsOpen?.Invoke(); }
        internal static void NativeFireClose() { OnDocumentsClose?.Invoke(); _onDocumentsClose?.Invoke(); }
        internal static void NativeFireFetch(string content) { OnFetchSuccess?.Invoke(content); _onFetchSuccess?.Invoke(content); }
        internal static void NativeFireFetchError() { OnFetchError?.Invoke(); _onFetchError?.Invoke(); }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Documents_Fetch();
        #endif
        public static void Fetch(Action<string> onFetchSuccess = null, Action onFetchError = null)
        {
            _onFetchSuccess = onFetchSuccess;
            _onFetchError = onFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Documents_Fetch();
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeDocuments.Fetch();
                return;
            }
            ConsoleLog("FETCH");
#endif
        }


        private void CallOnDocumentsOpen() { OnDocumentsOpen?.Invoke(); _onDocumentsOpen?.Invoke(); }
        private void CallOnDocumentsClose() { OnDocumentsClose?.Invoke(); _onDocumentsClose?.Invoke(); }


        private void CallOnDocumentsFetchSuccess(string data) { OnFetchSuccess?.Invoke(data); _onFetchSuccess?.Invoke(data); }
        private void CallOnDocumentsFetchError() { OnFetchError?.Invoke(); _onFetchError?.Invoke(); }
    }
}