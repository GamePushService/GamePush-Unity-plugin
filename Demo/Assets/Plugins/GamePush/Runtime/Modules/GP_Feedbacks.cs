using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Utilities;
using GamePush.Native;

namespace GamePush
{
    public class GP_Feedbacks : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Feedbacks);

        public static event UnityAction<FeedbackData> OnSend;
        public static event UnityAction<string> OnSendError;
        public static event UnityAction OnOpenList;
        public static event UnityAction<string> OnOpenListError;
        public static event UnityAction OnCloseList;
        public static event UnityAction OnOpenFeedback;
        public static event UnityAction<string> OnOpenFeedbackError;
        public static event UnityAction OnCloseFeedback;
        public static event UnityAction<FeedbackData[], bool> OnFetch;
        public static event UnityAction<string> OnFetchError;
        public static event UnityAction<FeedbackData[], bool> OnFetchMore;
        public static event UnityAction<string> OnFetchMoreError;
        public static event UnityAction<FeedbackMessageData> OnSendMessage;
        public static event UnityAction<string> OnSendMessageError;
        public static event UnityAction<FeedbackMessageData> OnFeedbackMessage;
        public static event UnityAction<FeedbackData> OnFeedbackCreated;
        public static event UnityAction<FeedbackData> OnFeedbackStatusUpdated;
        public static event UnityAction<FeedbackData> OnFeedbackPlatformStatusUpdated;

        private static event Action<FeedbackData> _onSend;
        private static event Action<string> _onSendError;
        private static event Action _onOpenList;
        private static event Action<string> _onOpenListError;
        private static event Action _onOpenFeedback;
        private static event Action<string> _onOpenFeedbackError;
        private static event Action<FeedbackData[], bool> _onFetch;
        private static event Action<string> _onFetchError;
        private static event Action<FeedbackData[], bool> _onFetchMore;
        private static event Action<string> _onFetchMoreError;
        private static event Action<FeedbackMessageData> _onSendMessage;
        private static event Action<string> _onSendMessageError;

        public static bool CanLoadMore { get; private set; }

        #region Native

        internal static void NativeFireSend(FeedbackData data)
        {
            OnSend?.Invoke(data);
            _onSend?.Invoke(data);
        }

        internal static void NativeFireSendError(string error)
        {
            OnSendError?.Invoke(error);
            _onSendError?.Invoke(error);
        }

        internal static void NativeFireFetch(Native.NativeFeedbacksPage page)
        {
            CanLoadMore = page.canLoadMore;
            OnFetch?.Invoke(page.items, page.canLoadMore);
            _onFetch?.Invoke(page.items, page.canLoadMore);
        }

        internal static void NativeFireFetchError(string error)
        {
            OnFetchError?.Invoke(error);
            _onFetchError?.Invoke(error);
        }

        internal static void NativeFireFetchMore(Native.NativeFeedbacksPage page)
        {
            CanLoadMore = page.canLoadMore;
            OnFetchMore?.Invoke(page.items, page.canLoadMore);
            _onFetchMore?.Invoke(page.items, page.canLoadMore);
        }

        internal static void NativeFireFetchMoreError(string error)
        {
            OnFetchMoreError?.Invoke(error);
            _onFetchMoreError?.Invoke(error);
        }

        internal static void NativeFireSendMessage(FeedbackMessageData data)
        {
            OnSendMessage?.Invoke(data);
            _onSendMessage?.Invoke(data);
        }

        internal static void NativeFireSendMessageError(string error)
        {
            OnSendMessageError?.Invoke(error);
            _onSendMessageError?.Invoke(error);
        }

        internal static void NativeFireOpenList()
        {
            OnOpenList?.Invoke();
            _onOpenList?.Invoke();
        }

        internal static void NativeFireCloseList() => OnCloseList?.Invoke();

        internal static void NativeFireOpenFeedback()
        {
            OnOpenFeedback?.Invoke();
            _onOpenFeedback?.Invoke();
        }

        internal static void NativeFireOpenFeedbackError(string error)
        {
            OnOpenFeedbackError?.Invoke(error);
            _onOpenFeedbackError?.Invoke(error);
        }

        internal static void NativeFireCloseFeedback() => OnCloseFeedback?.Invoke();

        #endregion

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Feedbacks_Send(string payload);
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Feedbacks_Open(string type, string status);
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Feedbacks_OpenFeedback(string feedbackId);
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Feedbacks_Fetch(string payload);
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Feedbacks_FetchMore(string payload);
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Feedbacks_SendMessage(string payload);
        #endif

        public static void Send(FeedbackData data, Action<FeedbackData> onSend = null, Action<string> onSendError = null)
        {
            _onSend = onSend;
            _onSendError = onSendError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Feedbacks_Send(JsonUtility.ToJson(data ?? new FeedbackData()));
#else
            if (GamePushHost.UseNativeCore)
            {
                Native.NativeFeedbacks.Send(data);
                return;
            }
            ConsoleLog("SEND");
            FeedbackData stub = CreateEditorStub(data);
            OnSend?.Invoke(stub);
            _onSend?.Invoke(stub);
#endif
        }

        public static void Open(Action onOpen = null, Action<string> onError = null)
        {
            Open(null, null, onOpen, onError);
        }

        public static void Open(string type, string status, Action onOpen = null, Action<string> onError = null)
        {
            _onOpenList = onOpen;
            _onOpenListError = onError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Feedbacks_Open(type ?? "", status ?? "");
            GP_WebGLInput.Release();
#else
            if (GamePushHost.UseNativeCore && Overlays.GP_Overlays.Open(Overlays.GP_OverlayKind.Feedbacks,
                    new Overlays.GP_FeedbacksArgs { type = type ?? "", status = status ?? "" }))
                return;
            if (GP_Play2Web.Call("FeedbacksOpen", type ?? "", status ?? ""))
                return;
            ConsoleLog("OPEN");
            OnOpenList?.Invoke();
            _onOpenList?.Invoke();
#endif
        }

        public static void OpenFeedback(string feedbackId, Action onOpen = null, Action<string> onError = null)
        {
            _onOpenFeedback = onOpen;
            _onOpenFeedbackError = onError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Feedbacks_OpenFeedback(feedbackId ?? "");
            GP_WebGLInput.Release();
#else
            if (GamePushHost.UseNativeCore && Overlays.GP_Overlays.Open(Overlays.GP_OverlayKind.Feedbacks,
                    new Overlays.GP_FeedbacksArgs { feedbackId = feedbackId ?? "" }))
                return;
            if (GP_Play2Web.Call("FeedbacksOpenFeedback", feedbackId ?? ""))
                return;
            ConsoleLog("OPEN FEEDBACK: " + feedbackId);
            OnOpenFeedback?.Invoke();
            _onOpenFeedback?.Invoke();
#endif
        }

        public static void Fetch(Action<FeedbackData[]> onSuccess = null, Action<string> onError = null)
        {
            Fetch(null, null, 20, onSuccess, onError);
        }

        public static void Fetch(string type, string status, Action<FeedbackData[]> onSuccess = null, Action<string> onError = null)
        {
            Fetch(type, status, 20, onSuccess, onError);
        }

        public static void Fetch(string type, string status, int limit, Action<FeedbackData[]> onSuccess = null, Action<string> onError = null)
        {
            _onFetch = onSuccess == null ? null : (items, _) => onSuccess(items);
            _onFetchError = onError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Feedbacks_Fetch(ToFilterJson(type, status, limit));
#else
            if (GamePushHost.UseNativeCore)
            {
                Native.NativeFeedbacks.Fetch(type, status, limit);
                return;
            }
            ConsoleLog("FETCH");
            FeedbackData[] empty = Array.Empty<FeedbackData>();
            CanLoadMore = false;
            OnFetch?.Invoke(empty, false);
            _onFetch?.Invoke(empty, false);
#endif
        }

        public static void FetchMore(int limit = 20, string type = null, string status = null, Action<FeedbackData[]> onSuccess = null, Action<string> onError = null)
        {
            _onFetchMore = onSuccess == null ? null : (items, _) => onSuccess(items);
            _onFetchMoreError = onError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Feedbacks_FetchMore(ToFilterJson(type, status, limit));
#else
            if (GamePushHost.UseNativeCore)
            {
                Native.NativeFeedbacks.FetchMore(type, status, limit);
                return;
            }
            ConsoleLog("FETCH MORE");
            FeedbackData[] empty = Array.Empty<FeedbackData>();
            CanLoadMore = false;
            OnFetchMore?.Invoke(empty, false);
            _onFetchMore?.Invoke(empty, false);
#endif
        }

        public static void SendMessage(FeedbackMessageData data, Action<FeedbackMessageData> onSend = null, Action<string> onError = null)
        {
            _onSendMessage = onSend;
            _onSendMessageError = onError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Feedbacks_SendMessage(JsonUtility.ToJson(data ?? new FeedbackMessageData()));
#else
            if (GamePushHost.UseNativeCore)
            {
                Native.NativeFeedbacks.SendMessage(data);
                return;
            }
            ConsoleLog("SEND MESSAGE");
            FeedbackMessageData stub = data ?? new FeedbackMessageData();
            if (string.IsNullOrEmpty(stub.id))
                stub.id = "0";
            OnSendMessage?.Invoke(stub);
            _onSendMessage?.Invoke(stub);
#endif
        }

        private void CallFeedbacksSend(string data)
        {
            FeedbackData feedback = ParseFeedback(data);
            OnSend?.Invoke(feedback);
            _onSend?.Invoke(feedback);
        }

        private void CallFeedbacksSendError(string error)
        {
            OnSendError?.Invoke(error);
            _onSendError?.Invoke(error);
        }

        private void CallFeedbacksOpenList()
        {
            OnOpenList?.Invoke();
            _onOpenList?.Invoke();
        }

        private void CallFeedbacksOpenListError(string error)
        {
            GP_WebGLInput.Restore();
            OnOpenListError?.Invoke(error);
            _onOpenListError?.Invoke(error);
        }

        private void CallFeedbacksCloseList()
        {
            GP_WebGLInput.Restore();
            OnCloseList?.Invoke();
        }

        private void CallFeedbacksOpenFeedback()
        {
            OnOpenFeedback?.Invoke();
            _onOpenFeedback?.Invoke();
        }

        private void CallFeedbacksOpenFeedbackError(string error)
        {
            GP_WebGLInput.Restore();
            OnOpenFeedbackError?.Invoke(error);
            _onOpenFeedbackError?.Invoke(error);
        }

        private void CallFeedbacksCloseFeedback()
        {
            GP_WebGLInput.Restore();
            OnCloseFeedback?.Invoke();
        }

        private void CallFeedbacksFetchSuccess(string data)
        {
            FeedbacksFetchResult result = ParseFetch(data);
            CanLoadMore = result.canLoadMore;
            FeedbackData[] items = result.items ?? Array.Empty<FeedbackData>();
            OnFetch?.Invoke(items, result.canLoadMore);
            _onFetch?.Invoke(items, result.canLoadMore);
        }

        private void CallFeedbacksFetchError(string error)
        {
            OnFetchError?.Invoke(error);
            _onFetchError?.Invoke(error);
        }

        private void CallFeedbacksFetchMoreSuccess(string data)
        {
            FeedbacksFetchResult result = ParseFetch(data);
            CanLoadMore = result.canLoadMore;
            FeedbackData[] items = result.items ?? Array.Empty<FeedbackData>();
            OnFetchMore?.Invoke(items, result.canLoadMore);
            _onFetchMore?.Invoke(items, result.canLoadMore);
        }

        private void CallFeedbacksFetchMoreError(string error)
        {
            OnFetchMoreError?.Invoke(error);
            _onFetchMoreError?.Invoke(error);
        }

        private void CallFeedbacksSendMessage(string data)
        {
            FeedbackMessageData message = ParseMessage(data);
            OnSendMessage?.Invoke(message);
            _onSendMessage?.Invoke(message);
        }

        private void CallFeedbacksSendMessageError(string error)
        {
            OnSendMessageError?.Invoke(error);
            _onSendMessageError?.Invoke(error);
        }

        private void CallFeedbacksMessageEvent(string data)
        {
            OnFeedbackMessage?.Invoke(ParseMessage(data));
        }

        private void CallFeedbacksCreatedEvent(string data)
        {
            OnFeedbackCreated?.Invoke(ParseFeedback(data));
        }

        private void CallFeedbacksStatusUpdatedEvent(string data)
        {
            OnFeedbackStatusUpdated?.Invoke(ParseFeedback(data));
        }

        private void CallFeedbacksPlatformStatusUpdatedEvent(string data)
        {
            OnFeedbackPlatformStatusUpdated?.Invoke(ParseFeedback(data));
        }

        private static FeedbackData ParseFeedback(string data)
        {
            if (string.IsNullOrEmpty(data))
                return new FeedbackData();
            return JsonUtility.FromJson<FeedbackData>(data) ?? new FeedbackData();
        }

        private static FeedbackMessageData ParseMessage(string data)
        {
            if (string.IsNullOrEmpty(data))
                return new FeedbackMessageData();
            return JsonUtility.FromJson<FeedbackMessageData>(data) ?? new FeedbackMessageData();
        }

        private static FeedbacksFetchResult ParseFetch(string data)
        {
            if (string.IsNullOrEmpty(data))
                return new FeedbacksFetchResult();
            return JsonUtility.FromJson<FeedbacksFetchResult>(data) ?? new FeedbacksFetchResult();
        }

        private static string ToFilterJson(string type, string status, int limit)
        {
            return JsonUtility.ToJson(new FeedbacksFetchFilter
            {
                type = type ?? "",
                status = status ?? "",
                limit = limit > 0 ? limit : 20
            });
        }

        private static FeedbackData CreateEditorStub(FeedbackData data)
        {
            FeedbackData stub = data ?? new FeedbackData();
            if (string.IsNullOrEmpty(stub.id))
                stub.id = "0";
            if (string.IsNullOrEmpty(stub.status))
                stub.status = "NEW";
            return stub;
        }
    }

    [Serializable]
    public class FeedbackData
    {
        public string id;
        public string type;
        public string text;
        public string status;
        public string[] files;
        public FeedbackMessageData[] messages;
        public int playerId;
        public int projectId;
        public int platformId;
        public string createdAt;
        public string updatedAt;
    }

    [Serializable]
    public class FeedbackMessageData
    {
        public string id;
        public string text;
        public string[] files;
        public string[] attachments;
        public string author;
        public string feedbackId;
        public string createdAt;
    }

    [Serializable]
    public class FeedbacksFetchResult
    {
        public FeedbackData[] items;
        public bool canLoadMore;
    }

    [Serializable]
    public class FeedbacksFetchFilter
    {
        public string type;
        public string status;
        public int limit = 20;
    }
}
