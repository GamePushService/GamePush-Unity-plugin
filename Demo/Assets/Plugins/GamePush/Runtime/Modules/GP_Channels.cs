using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Tools;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GamePush
{
    public class GP_Channels : GP_Module
    {
        private static readonly Queue<TaskCompletionSource<ChannelStateValueData>> PendingStateValueOperations =
            new Queue<TaskCompletionSource<ChannelStateValueData>>();
        private static readonly Queue<TaskCompletionSource<CreateChannelData>> PendingCreateChannelOperations =
            new Queue<TaskCompletionSource<CreateChannelData>>();
        private static readonly Queue<TaskCompletionSource<UpdateChannelData>> PendingUpdateChannelOperations =
            new Queue<TaskCompletionSource<UpdateChannelData>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingDeleteChannelOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<FetchChannelData>> PendingFetchChannelOperations =
            new Queue<TaskCompletionSource<FetchChannelData>>();
        private static readonly Queue<TaskCompletionSource<FetchChannelData>> PendingFetchPersonalChannelOperations =
            new Queue<TaskCompletionSource<FetchChannelData>>();
        private static readonly Queue<TaskCompletionSource<FetchChannelData>> PendingFetchFeedChannelOperations =
            new Queue<TaskCompletionSource<FetchChannelData>>();
        private static readonly Queue<TaskCompletionSource<FetchChannelsResultData>> PendingFetchChannelsOperations =
            new Queue<TaskCompletionSource<FetchChannelsResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchChannelsResultData>> PendingFetchMoreChannelsOperations =
            new Queue<TaskCompletionSource<FetchChannelsResultData>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingJoinOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingCancelJoinOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingLeaveOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingKickOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingMuteOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingUnmuteOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingSendInviteOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingCancelInviteOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingAcceptInviteOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingRejectInviteOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingAcceptJoinRequestOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingRejectJoinRequestOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<ChannelMessageResultData>> PendingSendMessageOperations =
            new Queue<TaskCompletionSource<ChannelMessageResultData>>();
        private static readonly Queue<TaskCompletionSource<ChannelMessageResultData>> PendingSendPersonalMessageOperations =
            new Queue<TaskCompletionSource<ChannelMessageResultData>>();
        private static readonly Queue<TaskCompletionSource<ChannelMessageResultData>> PendingSendFeedMessageOperations =
            new Queue<TaskCompletionSource<ChannelMessageResultData>>();
        private static readonly Queue<TaskCompletionSource<ChannelMessageResultData>> PendingEditMessageOperations =
            new Queue<TaskCompletionSource<ChannelMessageResultData>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingDeleteMessageOperations =
            new Queue<TaskCompletionSource<bool>>();
        private static readonly Queue<TaskCompletionSource<FetchMembersResultData>> PendingFetchMembersOperations =
            new Queue<TaskCompletionSource<FetchMembersResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchMembersResultData>> PendingFetchMoreMembersOperations =
            new Queue<TaskCompletionSource<FetchMembersResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchInvitesResultData>> PendingFetchInvitesOperations =
            new Queue<TaskCompletionSource<FetchInvitesResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchInvitesResultData>> PendingFetchMoreInvitesOperations =
            new Queue<TaskCompletionSource<FetchInvitesResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchInvitesResultData>> PendingFetchChannelInvitesOperations =
            new Queue<TaskCompletionSource<FetchInvitesResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchInvitesResultData>> PendingFetchMoreChannelInvitesOperations =
            new Queue<TaskCompletionSource<FetchInvitesResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchInvitesResultData>> PendingFetchSentInvitesOperations =
            new Queue<TaskCompletionSource<FetchInvitesResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchInvitesResultData>> PendingFetchMoreSentInvitesOperations =
            new Queue<TaskCompletionSource<FetchInvitesResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchJoinRequestsResultData>> PendingFetchJoinRequestsOperations =
            new Queue<TaskCompletionSource<FetchJoinRequestsResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchJoinRequestsResultData>> PendingFetchMoreJoinRequestsOperations =
            new Queue<TaskCompletionSource<FetchJoinRequestsResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchJoinRequestsResultData>> PendingFetchSentJoinRequestsOperations =
            new Queue<TaskCompletionSource<FetchJoinRequestsResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchJoinRequestsResultData>> PendingFetchMoreSentJoinRequestsOperations =
            new Queue<TaskCompletionSource<FetchJoinRequestsResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchMessagesResultData>> PendingFetchMessagesOperations =
            new Queue<TaskCompletionSource<FetchMessagesResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchMessagesResultData>> PendingFetchMoreMessagesOperations =
            new Queue<TaskCompletionSource<FetchMessagesResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchMessagesResultData>> PendingFetchPersonalMessagesOperations =
            new Queue<TaskCompletionSource<FetchMessagesResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchMessagesResultData>> PendingFetchMorePersonalMessagesOperations =
            new Queue<TaskCompletionSource<FetchMessagesResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchMessagesResultData>> PendingFetchFeedMessagesOperations =
            new Queue<TaskCompletionSource<FetchMessagesResultData>>();
        private static readonly Queue<TaskCompletionSource<FetchMessagesResultData>> PendingFetchMoreFeedMessagesOperations =
            new Queue<TaskCompletionSource<FetchMessagesResultData>>();

        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Channels);

        private static GP_Data CreateDataOrNull(string data)
        {
            if (string.IsNullOrEmpty(data) || data == "undefined" || data == "null")
                return null;

            return new GP_Data(data);
        }

        private static string NormalizeValue(string value) =>
            string.IsNullOrEmpty(value) || value == "undefined" || value == "null"
                ? null
                : value;

        private static string ExtractErrorMessage(string data)
        {
            if (string.IsNullOrEmpty(data) || data == "undefined" || data == "null")
                return "unknown_error";

            try
            {
                JObject payload = JObject.Parse(data);
                return payload.Value<string>("message") ?? data;
            }
            catch
            {
                return data;
            }
        }

        private static string JoinTags(string[] tags) =>
            tags == null || tags.Length == 0
                ? string.Empty
                : string.Join(",", tags);

        private static void SetWebGLKeyboardCapture(bool isEnabled)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            Type webGLInputType = typeof(Application).Assembly.GetType("UnityEngine.WebGLInput");
            if (webGLInputType == null)
                return;

            System.Reflection.PropertyInfo captureProperty =
                webGLInputType.GetProperty("captureAllKeyboardInput");

            if (captureProperty != null && captureProperty.CanWrite)
                captureProperty.SetValue(null, isEnabled);
#endif
        }

        #region Actions

        internal static event UnityAction<CreateChannelData> OnCreateChannel;
        internal static event UnityAction OnCreateChannelError;

        internal static event UnityAction<UpdateChannelData> OnUpdateChannel;
        internal static event UnityAction OnUpdateChannelError;

        internal static event UnityAction OnDeleteChannelSuccess;
        internal static event UnityAction<int> OnDeleteChannelEvent;
        internal static event UnityAction OnDeleteChannelError;

        internal static event UnityAction<FetchChannelData> OnFetchChannel;
        internal static event UnityAction OnFetchChannelError;

        internal static event UnityAction<FetchChannelData> OnFetchPersonalChannel;
        internal static event UnityAction OnFetchPersonalChannelError;

        internal static event UnityAction<FetchChannelData> OnFetchFeedChannel;
        internal static event UnityAction OnFetchFeedChannelError;

        internal static event UnityAction<List<FetchChannelData>, bool> OnFetchChannels;
        internal static event UnityAction OnFetchChannelsError;

        internal static event UnityAction<List<FetchChannelData>, bool> OnFetchMoreChannels;
        internal static event UnityAction OnFetchMoreChannelsError;

        internal static event UnityAction OnJoinSuccess;
        internal static event UnityAction<GP_Data> OnJoinEvent;
        internal static event UnityAction OnJoinError;

        internal static event UnityAction<GP_Data> OnJoinRequest;

        internal static event UnityAction OnCancelJoinSuccess;
        internal static event UnityAction<CancelJoinData> OnCancelJoinEvent;
        internal static event UnityAction OnCancelJoinError;

        internal static event UnityAction OnLeaveSuccess;
        internal static event UnityAction<MemberLeaveData> OnLeaveEvent;
        internal static event UnityAction OnLeaveError;

        internal static event UnityAction OnKick;
        internal static event UnityAction OnKickError;

        internal static event UnityAction<ChannelStateValueData> OnSetValue;
        internal static event UnityAction OnSetValueError;
        internal static event UnityAction<ChannelStateValueData> OnChangeValue;

        internal static event UnityAction<GP_Data, bool> OnFetchMembers;
        internal static event UnityAction OnFetchMembersError;

        internal static event UnityAction<GP_Data, bool> OnFetchMoreMembers;
        internal static event UnityAction OnFetchMoreMembersError;

        internal static event UnityAction OnMuteSuccess;
        internal static event UnityAction<MuteData> OnMuteEvent;
        internal static event UnityAction OnMuteError;

        internal static event UnityAction OnUnmuteSuccess;
        internal static event UnityAction<UnmuteData> OnUnmuteEvent;
        internal static event UnityAction OnUnmuteError;

        internal static event UnityAction OnSendInvite;
        internal static event UnityAction OnSendInviteError;

        internal static event UnityAction<InviteData> OnInvite;

        internal static event UnityAction OnCancelInviteSuccess;
        internal static event UnityAction<CancelInviteData> OnCancelInviteEvent;
        internal static event UnityAction OnCancelInviteError;

        internal static event UnityAction OnAcceptInvite;
        internal static event UnityAction OnAcceptInviteError;

        internal static event UnityAction OnRejectInviteSuccess;
        internal static event UnityAction<RejectInviteData> OnRejectInviteEvent;
        internal static event UnityAction OnRejectInviteError;

        internal static event UnityAction<GP_Data, bool> OnFetchInvites;
        internal static event UnityAction OnFetchInvitesError;

        internal static event UnityAction<GP_Data, bool> OnFetchMoreInvites;
        internal static event UnityAction OnFetchMoreInvitesError;

        internal static event UnityAction<GP_Data, bool> OnFetchChannelInvites;
        internal static event UnityAction OnFetchChannelInvitesError;

        internal static event UnityAction<GP_Data, bool> OnFetchMoreChannelInvites;
        internal static event UnityAction OnFetchMoreChannelInvitesError;

        internal static event UnityAction<GP_Data, bool> OnFetchSentInvites;
        internal static event UnityAction OnFetchSentInvitesError;

        internal static event UnityAction<GP_Data, bool> OnFetchMoreSentInvites;
        internal static event UnityAction OnFetchMoreSentInvitesError;

        internal static event UnityAction OnAcceptJoinRequest;
        internal static event UnityAction OnAcceptJoinRequestError;

        internal static event UnityAction OnRejectJoinRequestSuccess;
        internal static event UnityAction<RejectJoinRequestData> OnRejectJoinRequestEvent;
        internal static event UnityAction OnRejectJoinRequestError;

        internal static event UnityAction<GP_Data, bool> OnFetchJoinRequests;
        internal static event UnityAction OnFetchJoinRequestsError;

        internal static event UnityAction<GP_Data, bool> OnFetchMoreJoinRequests;
        internal static event UnityAction OnFetchMoreJoinRequestsError;

        internal static event UnityAction<List<JoinRequestsData>, bool> OnFetchSentJoinRequests;
        internal static event UnityAction OnFetchSentJoinRequestsError;

        internal static event UnityAction<List<JoinRequestsData>, bool> OnFetchMoreSentJoinRequests;
        internal static event UnityAction OnFetchMoreSentJoinRequestsError;

        internal static event UnityAction<GP_Data> OnSendMessage;
        internal static event UnityAction OnSendMessageError;

        internal static event UnityAction<GP_Data> OnMessage;

        internal static event UnityAction<GP_Data> OnEditMessageSuccess;
        internal static event UnityAction<MessageData> OnEditMessageEvent;
        internal static event UnityAction OnEditMessageError;

        internal static event UnityAction OnDeleteMessageSuccess;
        internal static event UnityAction<MessageData> OnDeleteMessageEvent;
        internal static event UnityAction OnDeleteMessageError;


        internal static event UnityAction<GP_Data, bool> OnFetchMessages;
        internal static event UnityAction OnFetchMessagesError;

        internal static event UnityAction<GP_Data, bool> OnFetchPersonalMessages;
        internal static event UnityAction OnFetchPersonalMessagesError;

        internal static event UnityAction<GP_Data, bool> OnFetchFeedMessages;
        internal static event UnityAction OnFetchFeedMessagesError;

        internal static event UnityAction<GP_Data, bool> OnFetchMoreMessages;
        internal static event UnityAction OnFetchMoreMessagesError;

        internal static event UnityAction<GP_Data, bool> OnFetchMorePersonalMessages;
        internal static event UnityAction OnFetchMorePersonalMessagesError;

        internal static event UnityAction<GP_Data, bool> OnFetchMoreFeedMessages;
        internal static event UnityAction OnFetchMoreFeedMessagesError;

        internal static event UnityAction OnOpenChat;
        internal static event UnityAction OnOpenChatError;
        internal static event UnityAction OnCloseChat;

        private static event Action _onOpenChat;
        private static event Action _onOpenChatError;
        private static event Action _onCloseChat;

        private static readonly HashSet<string> _dataEventNames = new HashSet<string>
        {
            "event",
            "createChannel",
            "error:createChannel",
            "updateChannel",
            "error:updateChannel",
            "event:updateChannel",
            "event:deleteChannel",
            "error:deleteChannel",
            "event:connect",
            "fetchChannel",
            "error:fetchChannel",
            "fetchPersonalChannel",
            "error:fetchPersonalChannel",
            "fetchFeedChannel",
            "error:fetchFeedChannel",
            "fetchChannels",
            "error:fetchChannels",
            "fetchMoreChannels",
            "error:fetchMoreChannels",
            "error:openChat",
            "error:openFeed",
            "event:join",
            "error:join",
            "event:joinRequest",
            "event:cancelJoin",
            "error:cancelJoin",
            "event:leave",
            "error:leave",
            "error:kick",
            "setValue",
            "error:setValue",
            "event:changeValue",
            "fetchMembers",
            "error:fetchMembers",
            "fetchMoreMembers",
            "error:fetchMoreMembers",
            "event:mute",
            "error:mute",
            "event:unmute",
            "error:unmute",
            "error:sendInvite",
            "event:invite",
            "event:cancelInvite",
            "error:cancelInvite",
            "event:acceptInvite",
            "error:acceptInvite",
            "event:rejectInvite",
            "error:rejectInvite",
            "fetchInvites",
            "error:fetchInvites",
            "fetchMoreInvites",
            "error:fetchMoreInvites",
            "fetchChannelInvites",
            "error:fetchChannelInvites",
            "fetchMoreChannelInvites",
            "error:fetchMoreChannelInvites",
            "fetchSentInvites",
            "error:fetchSentInvites",
            "fetchMoreSentInvites",
            "error:fetchMoreSentInvites",
            "error:acceptJoinRequest",
            "event:rejectJoinRequest",
            "error:rejectJoinRequest",
            "fetchJoinRequests",
            "error:fetchJoinRequests",
            "fetchMoreJoinRequests",
            "error:fetchMoreJoinRequests",
            "fetchSentJoinRequests",
            "error:fetchSentJoinRequests",
            "fetchMoreSentJoinRequests",
            "error:fetchMoreSentJoinRequests",
            "sendMessage",
            "error:sendMessage",
            "event:message",
            "editMessage",
            "event:editMessage",
            "error:editMessage",
            "event:deleteMessage",
            "error:deleteMessage",
            "fetchMessages",
            "error:fetchMessages",
            "fetchPersonalMessages",
            "error:fetchPersonalMessages",
            "fetchFeedMessages",
            "error:fetchFeedMessages",
            "fetchMoreMessages",
            "error:fetchMoreMessages",
            "fetchMorePersonalMessages",
            "error:fetchMorePersonalMessages",
            "fetchMoreFeedMessages",
            "error:fetchMoreFeedMessages"
        };

        private static readonly HashSet<string> _simpleEventNames = new HashSet<string>
        {
            "deleteChannel",
            "openChat",
            "closeChat",
            "join",
            "cancelJoin",
            "leave",
            "kick",
            "mute",
            "unmute",
            "sendInvite",
            "cancelInvite",
            "acceptInvite",
            "rejectInvite",
            "acceptJoinRequest",
            "rejectJoinRequest",
            "deleteMessage"
        };

        private static readonly Dictionary<string, UnityAction<GP_Data>> _dataEventCallbacks =
            new Dictionary<string, UnityAction<GP_Data>>();

        private static readonly Dictionary<string, UnityAction> _simpleEventCallbacks =
            new Dictionary<string, UnityAction>();

        private static readonly Dictionary<string, Dictionary<Delegate, UnityAction<GP_Data>>> _typedEventCallbackWrappers =
            new Dictionary<string, Dictionary<Delegate, UnityAction<GP_Data>>>();

        private static void AddDataEventCallback(string eventName, UnityAction<GP_Data> callback)
        {
            string normalizedEventName = NormalizePublicEventName(eventName);

            if (callback == null ||
                (!_dataEventNames.Contains(normalizedEventName) && !_simpleEventNames.Contains(normalizedEventName)))
                return;

            _dataEventCallbacks.TryGetValue(normalizedEventName, out UnityAction<GP_Data> current);
            _dataEventCallbacks[normalizedEventName] = current + callback;
        }

        private static void RemoveDataEventCallback(string eventName, UnityAction<GP_Data> callback)
        {
            string normalizedEventName = NormalizePublicEventName(eventName);

            if (callback == null ||
                (!_dataEventNames.Contains(normalizedEventName) && !_simpleEventNames.Contains(normalizedEventName)))
                return;

            if (!_dataEventCallbacks.TryGetValue(normalizedEventName, out UnityAction<GP_Data> current))
                return;

            current -= callback;

            if (current == null)
                _dataEventCallbacks.Remove(normalizedEventName);
            else
                _dataEventCallbacks[normalizedEventName] = current;
        }

        private static void AddTypedEventCallback<T>(string eventName, UnityAction<T> callback)
        {
            string normalizedEventName = NormalizePublicEventName(eventName);

            if (callback == null ||
                (!_dataEventNames.Contains(normalizedEventName) && !_simpleEventNames.Contains(normalizedEventName)))
                return;

            if (!_typedEventCallbackWrappers.TryGetValue(normalizedEventName, out Dictionary<Delegate, UnityAction<GP_Data>> wrappers))
            {
                wrappers = new Dictionary<Delegate, UnityAction<GP_Data>>();
                _typedEventCallbackWrappers[normalizedEventName] = wrappers;
            }

            if (wrappers.ContainsKey(callback))
                return;

            UnityAction<GP_Data> wrapper = payload => callback.Invoke(ConvertEventPayload<T>(payload));
            wrappers[callback] = wrapper;
            AddDataEventCallback(normalizedEventName, wrapper);
        }

        private static void RemoveTypedEventCallback<T>(string eventName, UnityAction<T> callback)
        {
            string normalizedEventName = NormalizePublicEventName(eventName);

            if (callback == null || !_typedEventCallbackWrappers.TryGetValue(normalizedEventName, out Dictionary<Delegate, UnityAction<GP_Data>> wrappers))
                return;

            if (!wrappers.TryGetValue(callback, out UnityAction<GP_Data> wrapper))
                return;

            RemoveDataEventCallback(normalizedEventName, wrapper);
            wrappers.Remove(callback);

            if (wrappers.Count == 0)
                _typedEventCallbackWrappers.Remove(normalizedEventName);
        }

        private static T ConvertEventPayload<T>(GP_Data payload)
        {
            if (payload == null)
                return default;

            if (typeof(T) == typeof(GP_Data))
                return (T)(object)payload;

            if (typeof(T) == typeof(string))
                return (T)(object)payload.Data;

            try
            {
                return payload.Get<T>();
            }
            catch
            {
                return default;
            }
        }

        private static string NormalizePublicEventName(string eventName) =>
            eventName == "message" ? "event:message" : eventName;

        private static void InvokeDataEvent(string eventName, GP_Data payload)
        {
            if (_dataEventCallbacks.TryGetValue(eventName, out UnityAction<GP_Data> callback))
                callback?.Invoke(payload);

            if (eventName != "event" && IsRealtimeEventName(eventName) &&
                _dataEventCallbacks.TryGetValue("event", out UnityAction<GP_Data> eventCallback))
            {
                eventCallback?.Invoke(CreateRealtimeEventPayload(eventName, payload));
            }
        }

        private static bool IsRealtimeEventName(string eventName) =>
            !string.IsNullOrEmpty(eventName) && eventName.StartsWith("event:");

        private static GP_Data CreateRealtimeEventPayload(string eventName, GP_Data payload)
        {
            string dataJson = payload?.Data;
            if (string.IsNullOrEmpty(dataJson) || dataJson == "undefined")
                dataJson = "null";

            string envelope =
                "{\"type\":\"" + EscapeJsonString(eventName) + "\",\"data\":" + dataJson + "}";

            return new GP_Data(envelope);
        }

        private static string EscapeJsonString(string value) =>
            (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");

        private static void AddSimpleEventCallback(string eventName, UnityAction callback)
        {
            if (callback == null || !_simpleEventNames.Contains(eventName))
                return;

            _simpleEventCallbacks.TryGetValue(eventName, out UnityAction current);
            _simpleEventCallbacks[eventName] = current + callback;
        }

        private static void RemoveSimpleEventCallback(string eventName, UnityAction callback)
        {
            if (callback == null || !_simpleEventNames.Contains(eventName))
                return;

            if (!_simpleEventCallbacks.TryGetValue(eventName, out UnityAction current))
                return;

            current -= callback;

            if (current == null)
                _simpleEventCallbacks.Remove(eventName);
            else
                _simpleEventCallbacks[eventName] = current;
        }

        private static void InvokeSimpleEvent(string eventName)
        {
            if (_simpleEventCallbacks.TryGetValue(eventName, out UnityAction callback))
                callback?.Invoke();

            if (_dataEventCallbacks.TryGetValue(eventName, out UnityAction<GP_Data> dataCallback))
                dataCallback?.Invoke(null);
        }

        private static GP_Data CreatePagedResultData(string itemsData, bool canLoadMore)
        {
            string normalizedItems =
                string.IsNullOrEmpty(itemsData) || itemsData == "null" || itemsData == "undefined"
                    ? "[]"
                    : itemsData;

            return new GP_Data("{\"items\":" + normalizedItems + ",\"canLoadMore\":" +
                               (canLoadMore ? "true" : "false") + "}");
        }

        private static List<T> DeserializeListOrEmpty<T>(string data)
        {
            string normalized = NormalizeValue(data);
            if (string.IsNullOrEmpty(normalized))
                return new List<T>();

            try
            {
                return JsonConvert.DeserializeObject<List<T>>(normalized) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        private static Task RunVoidOperation(Queue<TaskCompletionSource<bool>> queue, Action operation) =>
            RunTypedOperation(queue, operation);

        private static void CompleteVoidOperationSuccess(Queue<TaskCompletionSource<bool>> queue)
        {
            if (queue.Count == 0)
                return;

            queue.Dequeue().TrySetResult(true);
        }

        private static void CompleteVoidOperationError(Queue<TaskCompletionSource<bool>> queue, string data)
        {
            if (queue.Count == 0)
                return;

            queue.Dequeue().TrySetException(new InvalidOperationException(ExtractErrorMessage(data)));
        }

        private static TResult CreateTypedPagedResult<TResult, TItem>(string data, bool canLoadMore)
            where TResult : PagedItemsResultData<TItem>, new()
        {
            return new TResult
            {
                items = DeserializeListOrEmpty<TItem>(data),
                canLoadMore = canLoadMore
            };
        }

        #endregion

#if !UNITY_EDITOR && UNITY_WEBGL
        #region DllImport

        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenChat(int channel_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenChatWithTags(int channel_ID, string tags);
        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenPersonalChat(int player_ID, string tags);
        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenFeed(int player_ID, string tags);
        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenChatOverlay(string channel, string messages, string tags);
        [DllImport("__Internal")]
        private static extern string GP_Channels_ProcessTags(string tags, int player_ID);
        [DllImport("__Internal")]
        private static extern string GP_Channels_CanBeOnline();
        [DllImport("__Internal")]
        private static extern string GP_Channels_IsMainChatEnabled();
        [DllImport("__Internal")]
        private static extern int GP_Channels_MainChatId();
        [DllImport("__Internal")]
        private static extern void GP_Channels_Join(int channel_ID, string password);
        [DllImport("__Internal")]
        private static extern void GP_Channels_CancelJoin(int channel_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_Leave(int channel_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_Kick(int channel_ID, int player_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_Mute_Seconds(int channel_ID, int player_ID, int seconds);
        [DllImport("__Internal")]
        private static extern void GP_Channels_Mute_UnmuteAt(int channel_ID, int player_ID, string unmuteAt);
        [DllImport("__Internal")]
        private static extern void GP_Channels_UnMute(int channel_ID, int player_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_SendInvite(int channel_ID, int player_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_CancelInvite(int channel_ID, int player_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_AcceptInvite(int channel_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_RejectInvite(int channel_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchInvites(int limit, int offset);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreInvites(int limit);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchChannelInvites(int channel_ID, int limit, int offset);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreChannelInvites(int channel_ID, int limit);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchSentInvites(int limit, int offset);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreSentInvites(int limit);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchPersonalChannel(int player_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchFeedChannel(int player_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_AcceptJoinRequest(int channel_ID, int player_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_RejectJoinRequest(int channel_ID, int player_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchJoinRequests(int channel_ID, int limit, int offset);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreJoinRequests(int channel_ID, int limit);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchSentJoinRequests(int limit, int offset);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreSentJoinRequests(int limit);
        [DllImport("__Internal")]
        private static extern void GP_Channels_SendMessage(int channel_ID, string text, string tags);
        [DllImport("__Internal")]
        private static extern void GP_Channels_SendPersonalMessage(int player_ID, string text, string tags);
        [DllImport("__Internal")]
        private static extern void GP_Channels_SendFeedMessage(int player_ID, string text, string tags);
        [DllImport("__Internal")]
        private static extern void GP_Channels_EditMessage(string message_ID, string text);
        [DllImport("__Internal")]
        private static extern void GP_Channels_DeleteMessage(string message_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMessages(int channel_ID, string tags, int limit, int offset);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchPersonalMessages(int player_ID, string tags, int limit, int offset);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchFeedMessages(int player_ID, int author_ID, string tags, int limit, int offset);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreMessages(int channel_ID, string tags, int limit);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMorePersonalMessages(int player_ID, string tags, int limit);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreFeedMessages(int player_ID, int author_ID, string tags, int limit);
        [DllImport("__Internal")]
        private static extern void GP_Channels_DeleteChannel(int channel_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchChannel(int channel_ID);
        [DllImport("__Internal")]
        private static extern string GP_Channels_GetLocalChannelState(int channel_ID);
        [DllImport("__Internal")]
        private static extern string GP_Channels_GetChannelField(int channel_ID, string key);
        [DllImport("__Internal")]
        private static extern string GP_Channels_GetChannelValue(int channel_ID, string key);
        [DllImport("__Internal")]
        private static extern void GP_Channels_SetValueString(int channel_ID, string key, string value);
        [DllImport("__Internal")]
        private static extern void GP_Channels_SetValueNumber(int channel_ID, string key, float value);
        [DllImport("__Internal")]
        private static extern void GP_Channels_SetValueBool(int channel_ID, string key, bool value);
        [DllImport("__Internal")]
        private static extern void GP_Channels_SetValueJson(string query);
        [DllImport("__Internal")]
        private static extern void GP_Channels_AddValue(int channel_ID, string key, float value);
        [DllImport("__Internal")]
        private static extern void GP_Channels_AddValueJson(string query);
        [DllImport("__Internal")]
        private static extern void GP_Channels_CreateChannel(string filter);
        [DllImport("__Internal")]
        private static extern void GP_Channels_UpdateChannel(string filter);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchChannels(string filter);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreChannels(string filter);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMembers(string filter);
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreMembers(string filter);

        #endregion
#endif

        #region Methods
        internal static void OpenChat(int channel_ID, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_OpenChat(channel_ID);
#else

            ConsoleLog("OPEN CHAT: CHANNEL ID: " + channel_ID);
            _onOpenChat?.Invoke();
#endif
        }

        internal static void OpenChat(string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_OpenChatWithTags(-10, tags);
#else

            ConsoleLog("OPEN CHAT");
            _onOpenChat?.Invoke();
#endif
        }

        internal static void OpenChat(int channel_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_OpenChatWithTags(channel_ID, tags);
#else

            ConsoleLog("OPEN CHAT: CHANNEL ID: " + channel_ID);
            _onOpenChat?.Invoke();
#endif
        }

        internal static void OpenChat(Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_OpenChat(-10);
            SetWebGLKeyboardCapture(false);
#else

            ConsoleLog("OPEN CHAT");
#endif
        }

        internal static void OpenPersonalChat(int player_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_OpenPersonalChat(player_ID, tags);
#else

            ConsoleLog("OPEN PERSONAL CHAT: PLAYER ID: " + player_ID);
            _onOpenChat?.Invoke();
#endif
        }

        internal static void OpenPersonalChat(int player_ID, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            OpenPersonalChat(player_ID, "", onOpen, onClose, onOpenError);
        }

        internal static void OpenFeed(int player_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_OpenFeed(player_ID, tags);
#else

            ConsoleLog("OPEN FEED: PLAYER ID: " + player_ID);
            _onOpenChat?.Invoke();
#endif
        }

        internal static void OpenFeed(int player_ID, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            OpenFeed(player_ID, "", onOpen, onClose, onOpenError);
        }

        private static void OpenChatOverlay(GP_Data channel, GP_Data messages, string tags = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_OpenChatOverlay(channel.Data, messages.Data, tags);
#else

            ConsoleLog("OPEN CHAT OVERLAY");
#endif
        }

        private static GP_Data ProcessTags(string tags, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return CreateDataOrNull(GP_Channels_ProcessTags(tags, player_ID));
#else

            ConsoleLog("PROCESS TAGS");
            return null;
#endif
        }

        internal static bool IsMainChatEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Channels_IsMainChatEnabled() == "true";
#else

            Console.Log("IS MAIN CHAT ENABLED: TRUE");
            return true;
#endif
        }

        internal static int MainChatId()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Channels_MainChatId();
#else

            Console.Log("MAIN CHAT ID: 0");
            return 0;
#endif
        }

        internal static bool CanBeOnline()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Channels_CanBeOnline() == "true";
#else

            ConsoleLog("CAN BE ONLINE: TRUE");
            return true;
#endif
        }

        internal static void Join(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_Join(channel_ID, "");
#else

            ConsoleLog("JOIN");
#endif
        }

        internal static void Join(int channel_ID, string password)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_Join(channel_ID, password);
#else

            ConsoleLog("JOIN");
#endif
        }

        internal static void CancelJoin(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_CancelJoin(channel_ID);
#else

            ConsoleLog("CANCEL JOIN");
#endif
        }

        internal static void Leave(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_Leave(channel_ID);
#else

            ConsoleLog("LEAVE");
#endif
        }

        internal static void Kick(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_Kick(channel_ID, player_ID);
#else

            ConsoleLog("KICK");
#endif
        }

        internal static void Mute(int channel_ID, int player_ID, int seconds)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_Mute_Seconds(channel_ID, player_ID, seconds);
#else

            ConsoleLog("MUTE");
#endif
        }

        internal static void Mute(int channel_ID, int player_ID, string unmuteAT)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_Mute_UnmuteAt(channel_ID, player_ID, unmuteAT);
#else

            ConsoleLog("MUTE");
#endif
        }

        internal static void UnMute(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_UnMute(channel_ID, player_ID);
#else

            ConsoleLog("UNMUTE");
#endif
        }

        internal static void SendInvite(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_SendInvite(channel_ID, player_ID);
#else

            ConsoleLog("SEND INVITE");
#endif
        }

        internal static void CancelInvite(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_CancelInvite(channel_ID, player_ID);
#else

            ConsoleLog("CANCEL INVITE");
#endif
        }

        internal static void AcceptInvite(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_AcceptInvite(channel_ID);
#else

            ConsoleLog("ACCEPT INVITE");
#endif
        }

        internal static void RejectInvite(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_RejectInvite(channel_ID);
#else

            ConsoleLog("REJECT INVITE");
#endif
        }

        internal static void FetchInvites(int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchInvites(limit, offset);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH INVITES");
#endif
        }

        internal static void FetchMoreInvites(int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreInvites(limit);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE INVITES");
#endif
        }

        internal static void FetchChannelInvites(int channel_ID, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchChannelInvites(channel_ID, limit, offset);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH CHANNEL INVITES");
#endif
        }

        internal static void FetchMoreChannelInvites(int channel_ID, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreChannelInvites(channel_ID, limit);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE CHANNEL INVITES");
#endif
        }

        internal static void FetchSentInvites(int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchSentInvites(limit, offset);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH SENT INVITES");
#endif
        }

        internal static void FetchMoreSentInvites(int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreSentInvites(limit);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE SENT INVITES");
#endif
        }

        internal static void FetchPersonalChannel(int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchPersonalChannel(player_ID);
#else

            ConsoleLog("FETCH PERSONAL CHANNEL");
#endif
        }

        internal static void FetchFeedChannel(int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchFeedChannel(player_ID);
#else

            ConsoleLog("FETCH FEED CHANNEL");
#endif
        }

        internal static void AcceptJoinRequest(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_AcceptJoinRequest(channel_ID, player_ID);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("ACCEPT JOIN REQUEST");
#endif
        }

        internal static void RejectJoinRequest(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_RejectJoinRequest(channel_ID, player_ID);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("REJECT JOIN REQUEST");
#endif
        }

        internal static void FetchJoinRequests(int channel_ID, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchJoinRequests(channel_ID, limit, offset);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH JOIN REQUESTS");
#endif
        }

        internal static void FetchMoreJoinRequests(int channel_ID, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreJoinRequests(channel_ID, limit);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE JOIN REQUESTS");
#endif
        }

        internal static void FetchSentJoinRequests(int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchSentJoinRequests(limit, offset);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH SENT JOIN REQUESTS");
#endif
        }

        internal static void FetchMoreSentJoinRequests(int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreSentJoinRequests(limit);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE SENT JOIN REQUESTS");
#endif
        }

        internal static void SendMessage(int channel_ID, string text, string tags = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_SendMessage(channel_ID, text, tags);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("SEND MESSAGE");
#endif
        }

        internal static void SendPersonalMessage(int player_ID, string text, string tags = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_SendPersonalMessage(player_ID, text, tags);
#else

            ConsoleLog("SEND PERSONAL MESSAGE");
#endif
        }

        internal static void SendFeedMessage(int player_ID, string text, string tags = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_SendFeedMessage(player_ID, text, tags);
#else

            ConsoleLog("SEND FEED MESSAGE");
#endif
        }

        internal static void EditMessage(string message_ID, string text)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_EditMessage(message_ID, text);
#else

            ConsoleLog("EDIT MESSAGE");
#endif
        }

        internal static void DeleteMessage(string message_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_DeleteMessage(message_ID);
#else

            ConsoleLog("DELETE MESSAGE");
#endif
        }

        internal static void FetchMessages(int channel_ID, string tags = "", int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMessages(channel_ID, tags, limit, offset);
#else

            ConsoleLog("FETCH MESSAGES");
#endif
        }

        internal static void FetchPersonalMessages(int player_ID, string tags = "", int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchPersonalMessages(player_ID, tags, limit, offset);
#else

            ConsoleLog("FETCH PERSONAL MESSAGES");
#endif
        }

        internal static void FetchFeedMessages(int player_ID, string tags = "", int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchFeedMessages(player_ID, 0, tags, limit, offset);
#else

            ConsoleLog("FETCH FEED MESSAGES");
#endif
        }

        internal static void FetchFeedMessages(int player_ID, int author_ID, string tags = "", int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchFeedMessages(player_ID, author_ID, tags, limit, offset);
#else

            ConsoleLog("FETCH FEED MESSAGES");
#endif
        }

        internal static void FetchMoreMessages(int channel_ID, string tags = "", int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreMessages(channel_ID, tags, limit);
#else

            ConsoleLog("FETCH MORE MESSAGES");
#endif
        }

        internal static void FetchMorePersonalMessages(int player_ID, string tags = "", int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMorePersonalMessages(player_ID, tags, limit);
#else

            ConsoleLog("FETCH MORE PERSONAL MESSAGES");
#endif
        }

        internal static void FetchMoreFeedMessages(int player_ID, string tags = "", int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreFeedMessages(player_ID, 0, tags, limit);
#else

            ConsoleLog("FETCH MORE FEED MESSAGES");
#endif
        }

        internal static void FetchMoreFeedMessages(int player_ID, int author_ID, string tags = "", int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreFeedMessages(player_ID, author_ID, tags, limit);
#else

            ConsoleLog("FETCH MORE FEED MESSAGES");
#endif
        }

        internal static void DeleteChannel(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_DeleteChannel(channel_ID);
#else

            ConsoleLog("DELETE CHANNEL");
#endif
        }

        internal static void FetchChannel(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchChannel(channel_ID);
#else

            ConsoleLog("FETCH CHANNEL");
#endif
        }

        private static T DeserializeJsonOrDefault<T>(string data) where T : class
        {
            if (string.IsNullOrEmpty(data) || data == "null" || data == "undefined")
                return null;

            return JsonConvert.DeserializeObject<T>(data);
        }

        internal static ChannelLocalStateData GetLocalChannelState(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return DeserializeJsonOrDefault<ChannelLocalStateData>(GP_Channels_GetLocalChannelState(channel_ID));
#else

            ConsoleLog("GET LOCAL CHANNEL STATE");
            return null;
#endif
        }

        internal static ChannelModelFieldData GetChannelField(int channel_ID, string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return DeserializeJsonOrDefault<ChannelModelFieldData>(GP_Channels_GetChannelField(channel_ID, key));
#else

            ConsoleLog("GET CHANNEL FIELD");
            return null;
#endif
        }

        private static object ParseChannelValue(string data)
        {
            if (data == "undefined")
                return GP_Undefined.Value;

            if (string.IsNullOrEmpty(data) || data == "null")
                return null;

            try
            {
                JObject payload = JObject.Parse(data);

                if (payload.Value<bool?>("isUndefined") == true)
                    return GP_Undefined.Value;

                JToken valueToken = payload["value"];

                if (valueToken == null || valueToken.Type == JTokenType.Null)
                    return null;

                switch (valueToken.Type)
                {
                    case JTokenType.Integer:
                        return valueToken.Value<long>();
                    case JTokenType.Float:
                        return valueToken.Value<double>();
                    case JTokenType.Boolean:
                        return valueToken.Value<bool>();
                    case JTokenType.String:
                        return valueToken.Value<string>();
                    default:
                        return valueToken.ToString(Formatting.None);
                }
            }
            catch
            {
                return data;
            }
        }

        internal static object GetChannelValue(int channel_ID, string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return ParseChannelValue(GP_Channels_GetChannelValue(channel_ID, key));
#else

            ConsoleLog("GET CHANNEL VALUE");
            return null;
#endif
        }

        internal static void SetValue(int channel_ID, string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_SetValueString(channel_ID, key, value);
#else

            ConsoleLog("SET CHANNEL VALUE");
#endif
        }

        internal static void SetValue(int channel_ID, string key, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_SetValueNumber(channel_ID, key, value);
#else

            ConsoleLog("SET CHANNEL VALUE");
#endif
        }

        internal static void SetValue(int channel_ID, string key, float value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_SetValueNumber(channel_ID, key, value);
#else

            ConsoleLog("SET CHANNEL VALUE");
#endif
        }

        internal static void SetValue(int channel_ID, string key, bool value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_SetValueBool(channel_ID, key, value);
#else

            ConsoleLog("SET CHANNEL VALUE");
#endif
        }

        internal static void AddValue(int channel_ID, string key, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_AddValue(channel_ID, key, value);
#else

            ConsoleLog("ADD CHANNEL VALUE");
#endif
        }

        internal static void AddValue(int channel_ID, string key, float value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_AddValue(channel_ID, key, value);
#else

            ConsoleLog("ADD CHANNEL VALUE");
#endif
        }

        internal static void CreateChannel(CreateChannelFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_CreateChannel(
                JsonConvert.SerializeObject(
                    filter,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }));
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("CREATE CHANNEL");
#endif
        }

        internal static void UpdateChannel(UpdateChannelFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_UpdateChannel(
                JsonConvert.SerializeObject(
                    filter,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }));
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("UPDATE CHANNEL");
#endif
        }

        internal static void FetchChannels(FetchChannelsFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchChannels(JsonUtility.ToJson(filter));
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH CHANNELS");
#endif
        }

        internal static void FetchMoreChannels(FetchMoreChannelsFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreChannels(JsonUtility.ToJson(filter));
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE CHANNELS");
#endif
        }

        internal static void FetchMembers(FetchMembersFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMembers(JsonUtility.ToJson(filter));
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MEMBERS");
#endif
        }

        internal static void FetchMoreMembers(FetchMoreMembersFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreMembers(JsonUtility.ToJson(filter));
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE MEMBERS");
#endif
        }

        public static bool canBeOnline => CanBeOnline();
        public static bool isMainChatEnabled => IsMainChatEnabled();
        public static int mainChatId => MainChatId();

        internal static void openChat(int channel_ID, Action onOpen = null, Action onClose = null, Action onOpenError = null) => OpenChat(channel_ID, onOpen, onClose, onOpenError);
        internal static void openChat(string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null) => OpenChat(tags, onOpen, onClose, onOpenError);
        internal static void openChat(int channel_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null) => OpenChat(channel_ID, tags, onOpen, onClose, onOpenError);
        internal static void openChat(Action onOpen = null, Action onClose = null, Action onOpenError = null) => OpenChat(onOpen, onClose, onOpenError);
        public static void openChat(OpenChatQuery query, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            if (query == null)
            {
                OpenChat(onOpen, onClose, onOpenError);
                return;
            }

            if (query.id > 0)
                OpenChat(query.id, JoinTags(query.tags), onOpen, onClose, onOpenError);
            else
                OpenChat(JoinTags(query.tags), onOpen, onClose, onOpenError);
        }
        internal static void openPersonalChat(int player_ID, string tags = "", Action onOpen = null, Action onClose = null, Action onOpenError = null) => OpenPersonalChat(player_ID, tags, onOpen, onClose, onOpenError);
        public static void openPersonalChat(PlayerChatQuery query, Action onOpen = null, Action onClose = null, Action onOpenError = null) =>
            OpenPersonalChat(query.playerId, JoinTags(query.tags), onOpen, onClose, onOpenError);
        internal static void openFeed(int player_ID, string tags = "", Action onOpen = null, Action onClose = null, Action onOpenError = null) => OpenFeed(player_ID, tags, onOpen, onClose, onOpenError);
        public static void openFeed(PlayerChatQuery query, Action onOpen = null, Action onClose = null, Action onOpenError = null) =>
            OpenFeed(query.playerId, JoinTags(query.tags), onOpen, onClose, onOpenError);
        internal static void join(int channel_ID) => Join(channel_ID);
        internal static void join(int channel_ID, string password) => Join(channel_ID, password);
        public static Task join(JoinChannelQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingJoinOperations, () => Join(query.channelId, query.password));
#else
            Join(query.channelId, query.password);
            return Task.CompletedTask;
#endif
        }
        internal static void cancelJoin(int channel_ID) => CancelJoin(channel_ID);
        public static Task cancelJoin(ChannelQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingCancelJoinOperations, () => CancelJoin(query.channelId));
#else
            CancelJoin(query.channelId);
            return Task.CompletedTask;
#endif
        }
        internal static void leave(int channel_ID) => Leave(channel_ID);
        public static Task leave(ChannelQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingLeaveOperations, () => Leave(query.channelId));
#else
            Leave(query.channelId);
            return Task.CompletedTask;
#endif
        }
        internal static void kick(int channel_ID, int player_ID) => Kick(channel_ID, player_ID);
        public static Task kick(ChannelPlayerQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingKickOperations, () => Kick(query.channelId, query.playerId));
#else
            Kick(query.channelId, query.playerId);
            return Task.CompletedTask;
#endif
        }
        internal static void mute(int channel_ID, int player_ID, int seconds) => Mute(channel_ID, player_ID, seconds);
        internal static void mute(int channel_ID, int player_ID, string unmuteAT) => Mute(channel_ID, player_ID, unmuteAT);
        public static Task mute(MutePlayerQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingMuteOperations, () =>
            {
                if (!string.IsNullOrEmpty(query.unmuteAt))
                    Mute(query.channelId, query.playerId, query.unmuteAt);
                else
                    Mute(query.channelId, query.playerId, query.seconds);
            });
#else
            if (!string.IsNullOrEmpty(query.unmuteAt))
                Mute(query.channelId, query.playerId, query.unmuteAt);
            else
                Mute(query.channelId, query.playerId, query.seconds);
            return Task.CompletedTask;
#endif
        }
        internal static void unmute(int channel_ID, int player_ID) => UnMute(channel_ID, player_ID);
        public static Task unmute(ChannelPlayerQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingUnmuteOperations, () => UnMute(query.channelId, query.playerId));
#else
            UnMute(query.channelId, query.playerId);
            return Task.CompletedTask;
#endif
        }
        internal static void sendInvite(int channel_ID, int player_ID) => SendInvite(channel_ID, player_ID);
        public static Task sendInvite(ChannelPlayerQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingSendInviteOperations, () => SendInvite(query.channelId, query.playerId));
#else
            SendInvite(query.channelId, query.playerId);
            return Task.CompletedTask;
#endif
        }
        internal static void cancelInvite(int channel_ID, int player_ID) => CancelInvite(channel_ID, player_ID);
        public static Task cancelInvite(ChannelPlayerQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingCancelInviteOperations, () => CancelInvite(query.channelId, query.playerId));
#else
            CancelInvite(query.channelId, query.playerId);
            return Task.CompletedTask;
#endif
        }
        internal static void acceptInvite(int channel_ID) => AcceptInvite(channel_ID);
        public static Task acceptInvite(ChannelQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingAcceptInviteOperations, () => AcceptInvite(query.channelId));
#else
            AcceptInvite(query.channelId);
            return Task.CompletedTask;
#endif
        }
        internal static void rejectInvite(int channel_ID) => RejectInvite(channel_ID);
        public static Task rejectInvite(ChannelQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingRejectInviteOperations, () => RejectInvite(query.channelId));
#else
            RejectInvite(query.channelId);
            return Task.CompletedTask;
#endif
        }
        internal static void fetchInvites(int limit = 50, int offset = 0) => FetchInvites(limit, offset);
        public static Task<FetchInvitesResultData> fetchInvites(PagingQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchInvitesOperations, () => FetchInvites(query.limit, query.offset));
#else
            FetchInvites(query.limit, query.offset);
            return Task.FromResult<FetchInvitesResultData>(null);
#endif
        }
        internal static void fetchMoreInvites(int limit = 50) => FetchMoreInvites(limit);
        public static Task<FetchInvitesResultData> fetchMoreInvites(LimitQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchMoreInvitesOperations, () => FetchMoreInvites(query.limit));
#else
            FetchMoreInvites(query.limit);
            return Task.FromResult<FetchInvitesResultData>(null);
#endif
        }
        internal static void fetchChannelInvites(int channel_ID, int limit = 50, int offset = 0) => FetchChannelInvites(channel_ID, limit, offset);
        public static Task<FetchInvitesResultData> fetchChannelInvites(ChannelPagingQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchChannelInvitesOperations, () => FetchChannelInvites(query.channelId, query.limit, query.offset));
#else
            FetchChannelInvites(query.channelId, query.limit, query.offset);
            return Task.FromResult<FetchInvitesResultData>(null);
#endif
        }
        internal static void fetchMoreChannelInvites(int channel_ID, int limit = 50) => FetchMoreChannelInvites(channel_ID, limit);
        public static Task<FetchInvitesResultData> fetchMoreChannelInvites(ChannelLimitQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchMoreChannelInvitesOperations, () => FetchMoreChannelInvites(query.channelId, query.limit));
#else
            FetchMoreChannelInvites(query.channelId, query.limit);
            return Task.FromResult<FetchInvitesResultData>(null);
#endif
        }
        internal static void fetchSentInvites(int channel_ID, int limit = 50, int offset = 0) => FetchSentInvites(limit, offset);
        public static Task<FetchInvitesResultData> fetchSentInvites(PagingQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchSentInvitesOperations, () => FetchSentInvites(query.limit, query.offset));
#else
            FetchSentInvites(query.limit, query.offset);
            return Task.FromResult<FetchInvitesResultData>(null);
#endif
        }
        internal static void fetchMoreSentInvites(int channel_ID, int limit = 50) => FetchMoreSentInvites(limit);
        public static Task<FetchInvitesResultData> fetchMoreSentInvites(LimitQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchMoreSentInvitesOperations, () => FetchMoreSentInvites(query.limit));
#else
            FetchMoreSentInvites(query.limit);
            return Task.FromResult<FetchInvitesResultData>(null);
#endif
        }
        internal static void fetchPersonalChannel(int player_ID) => FetchPersonalChannel(player_ID);
        public static Task<FetchChannelData> fetchPersonalChannel(PlayerQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchPersonalChannelOperations, () => FetchPersonalChannel(query.playerId));
#else
            FetchPersonalChannel(query.playerId);
            return Task.FromResult<FetchChannelData>(null);
#endif
        }
        internal static void fetchFeedChannel(int player_ID) => FetchFeedChannel(player_ID);
        public static Task<FetchChannelData> fetchFeedChannel(PlayerQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchFeedChannelOperations, () => FetchFeedChannel(query.playerId));
#else
            FetchFeedChannel(query.playerId);
            return Task.FromResult<FetchChannelData>(null);
#endif
        }
        internal static void acceptJoinRequest(int channel_ID, int player_ID) => AcceptJoinRequest(channel_ID, player_ID);
        public static Task acceptJoinRequest(ChannelPlayerQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingAcceptJoinRequestOperations, () => AcceptJoinRequest(query.channelId, query.playerId));
#else
            AcceptJoinRequest(query.channelId, query.playerId);
            return Task.CompletedTask;
#endif
        }
        internal static void rejectJoinRequest(int channel_ID, int player_ID) => RejectJoinRequest(channel_ID, player_ID);
        public static Task rejectJoinRequest(ChannelPlayerQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingRejectJoinRequestOperations, () => RejectJoinRequest(query.channelId, query.playerId));
#else
            RejectJoinRequest(query.channelId, query.playerId);
            return Task.CompletedTask;
#endif
        }
        internal static void fetchJoinRequests(int channel_ID, int limit = 50, int offset = 0) => FetchJoinRequests(channel_ID, limit, offset);
        public static Task<FetchJoinRequestsResultData> fetchJoinRequests(ChannelPagingQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchJoinRequestsOperations, () => FetchJoinRequests(query.channelId, query.limit, query.offset));
#else
            FetchJoinRequests(query.channelId, query.limit, query.offset);
            return Task.FromResult<FetchJoinRequestsResultData>(null);
#endif
        }
        internal static void fetchMoreJoinRequests(int channel_ID, int limit = 50) => FetchMoreJoinRequests(channel_ID, limit);
        public static Task<FetchJoinRequestsResultData> fetchMoreJoinRequests(ChannelLimitQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchMoreJoinRequestsOperations, () => FetchMoreJoinRequests(query.channelId, query.limit));
#else
            FetchMoreJoinRequests(query.channelId, query.limit);
            return Task.FromResult<FetchJoinRequestsResultData>(null);
#endif
        }
        internal static void fetchSentJoinRequests(int limit = 50, int offset = 0) => FetchSentJoinRequests(limit, offset);
        public static Task<FetchJoinRequestsResultData> fetchSentJoinRequests(PagingQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchSentJoinRequestsOperations, () => FetchSentJoinRequests(query.limit, query.offset));
#else
            FetchSentJoinRequests(query.limit, query.offset);
            return Task.FromResult<FetchJoinRequestsResultData>(null);
#endif
        }
        internal static void fetchMoreSentJoinRequests(int limit = 50) => FetchMoreSentJoinRequests(limit);
        public static Task<FetchJoinRequestsResultData> fetchMoreSentJoinRequests(LimitQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchMoreSentJoinRequestsOperations, () => FetchMoreSentJoinRequests(query.limit));
#else
            FetchMoreSentJoinRequests(query.limit);
            return Task.FromResult<FetchJoinRequestsResultData>(null);
#endif
        }
        internal static void sendMessage(int channel_ID, string text, string tags = "") => SendMessage(channel_ID, text, tags);
        public static Task<ChannelMessageResultData> sendMessage(SendChannelMessageQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingSendMessageOperations, () => SendMessage(query.channelId, query.text, JoinTags(query.tags)));
#else
            SendMessage(query.channelId, query.text, JoinTags(query.tags));
            return Task.FromResult<ChannelMessageResultData>(null);
#endif
        }
        internal static void sendPersonalMessage(int player_ID, string text, string tags = "") => SendPersonalMessage(player_ID, text, tags);
        public static Task<ChannelMessageResultData> sendPersonalMessage(SendPlayerMessageQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingSendPersonalMessageOperations, () => SendPersonalMessage(query.playerId, query.text, JoinTags(query.tags)));
#else
            SendPersonalMessage(query.playerId, query.text, JoinTags(query.tags));
            return Task.FromResult<ChannelMessageResultData>(null);
#endif
        }
        internal static void sendFeedMessage(int player_ID, string text, string tags = "") => SendFeedMessage(player_ID, text, tags);
        public static Task<ChannelMessageResultData> sendFeedMessage(SendPlayerMessageQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingSendFeedMessageOperations, () => SendFeedMessage(query.playerId, query.text, JoinTags(query.tags)));
#else
            SendFeedMessage(query.playerId, query.text, JoinTags(query.tags));
            return Task.FromResult<ChannelMessageResultData>(null);
#endif
        }
        internal static void editMessage(string message_ID, string text) => EditMessage(message_ID, text);
        public static Task<ChannelMessageResultData> editMessage(EditMessageQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingEditMessageOperations, () => EditMessage(query.messageId, query.text));
#else
            EditMessage(query.messageId, query.text);
            return Task.FromResult<ChannelMessageResultData>(null);
#endif
        }
        internal static void deleteMessage(string message_ID) => DeleteMessage(message_ID);
        public static Task deleteMessage(MessageQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingDeleteMessageOperations, () => DeleteMessage(query.messageId));
#else
            DeleteMessage(query.messageId);
            return Task.CompletedTask;
#endif
        }
        internal static void fetchMessages(int channel_ID, string tags = "", int limit = 50, int offset = 0) => FetchMessages(channel_ID, tags, limit, offset);
        public static Task<FetchMessagesResultData> fetchMessages(FetchMessagesQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchMessagesOperations, () => FetchMessages(query.channelId, JoinTags(query.tags), query.limit, query.offset));
#else
            FetchMessages(query.channelId, JoinTags(query.tags), query.limit, query.offset);
            return Task.FromResult<FetchMessagesResultData>(null);
#endif
        }
        internal static void fetchPersonalMessages(int player_ID, string tags = "", int limit = 50, int offset = 0) => FetchPersonalMessages(player_ID, tags, limit, offset);
        public static Task<FetchMessagesResultData> fetchPersonalMessages(FetchPlayerMessagesQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchPersonalMessagesOperations, () => FetchPersonalMessages(query.playerId, JoinTags(query.tags), query.limit, query.offset));
#else
            FetchPersonalMessages(query.playerId, JoinTags(query.tags), query.limit, query.offset);
            return Task.FromResult<FetchMessagesResultData>(null);
#endif
        }
        internal static void fetchFeedMessages(int player_ID, string tags = "", int limit = 50, int offset = 0) => FetchFeedMessages(player_ID, tags, limit, offset);
        public static Task<FetchMessagesResultData> fetchFeedMessages(FetchFeedMessagesQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchFeedMessagesOperations, () => FetchFeedMessages(query.playerId, query.authorId, JoinTags(query.tags), query.limit, query.offset));
#else
            FetchFeedMessages(query.playerId, query.authorId, JoinTags(query.tags), query.limit, query.offset);
            return Task.FromResult<FetchMessagesResultData>(null);
#endif
        }
        internal static void fetchMoreMessages(int channel_ID, string tags = "", int limit = 50) => FetchMoreMessages(channel_ID, tags, limit);
        public static Task<FetchMessagesResultData> fetchMoreMessages(FetchMoreMessagesQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchMoreMessagesOperations, () => FetchMoreMessages(query.channelId, JoinTags(query.tags), query.limit));
#else
            FetchMoreMessages(query.channelId, JoinTags(query.tags), query.limit);
            return Task.FromResult<FetchMessagesResultData>(null);
#endif
        }
        internal static void fetchMorePersonalMessages(int player_ID, string tags = "", int limit = 50) => FetchMorePersonalMessages(player_ID, tags, limit);
        public static Task<FetchMessagesResultData> fetchMorePersonalMessages(FetchMorePlayerMessagesQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchMorePersonalMessagesOperations, () => FetchMorePersonalMessages(query.playerId, JoinTags(query.tags), query.limit));
#else
            FetchMorePersonalMessages(query.playerId, JoinTags(query.tags), query.limit);
            return Task.FromResult<FetchMessagesResultData>(null);
#endif
        }
        internal static void fetchMoreFeedMessages(int player_ID, string tags = "", int limit = 50) => FetchMoreFeedMessages(player_ID, tags, limit);
        public static Task<FetchMessagesResultData> fetchMoreFeedMessages(FetchMoreFeedMessagesQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchMoreFeedMessagesOperations, () => FetchMoreFeedMessages(query.playerId, query.authorId, JoinTags(query.tags), query.limit));
#else
            FetchMoreFeedMessages(query.playerId, query.authorId, JoinTags(query.tags), query.limit);
            return Task.FromResult<FetchMessagesResultData>(null);
#endif
        }
        internal static void deleteChannel(int channel_ID) => DeleteChannel(channel_ID);
        public static Task deleteChannel(ChannelQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingDeleteChannelOperations, () => DeleteChannel(query.channelId));
#else
            DeleteChannel(query.channelId);
            return Task.CompletedTask;
#endif
        }
        internal static void fetchChannel(int channel_ID) => FetchChannel(channel_ID);
        public static Task<FetchChannelData> fetchChannel(ChannelQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchChannelOperations, () => FetchChannel(query.channelId));
#else
            FetchChannel(query.channelId);
            return Task.FromResult<FetchChannelData>(null);
#endif
        }
        internal static ChannelLocalStateData getLocalChannelState(int channel_ID) => GetLocalChannelState(channel_ID);
        public static ChannelLocalStateData getLocalChannelState(ChannelQuery query) => GetLocalChannelState(query.channelId);
        internal static ChannelModelFieldData getChannelField(int channel_ID, string key) => GetChannelField(channel_ID, key);
        public static ChannelModelFieldData getChannelField(ChannelKeyQuery query) => GetChannelField(query.channelId, query.key);
        internal static object getChannelValue(int channel_ID, string key) => GetChannelValue(channel_ID, key);
        public static object getChannelValue(ChannelKeyQuery query) => GetChannelValue(query.channelId, query.key);
        internal static void setValue(int channel_ID, string key, string value) => SetValue(channel_ID, key, value);
        internal static void setValue(int channel_ID, string key, int value) => SetValue(channel_ID, key, value);
        internal static void setValue(int channel_ID, string key, float value) => SetValue(channel_ID, key, value);
        internal static void setValue(int channel_ID, string key, bool value) => SetValue(channel_ID, key, value);
        private static string SerializeChannelStateValueQuery(ChannelStateValueQuery query) =>
            JsonConvert.SerializeObject(
                query,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

        private static Task<T> RunTypedOperation<T>(
            Queue<TaskCompletionSource<T>> queue,
            Action operation)
        {
            if (operation == null)
                return Task.FromResult<T>(default);

            TaskCompletionSource<T> completion = new TaskCompletionSource<T>();
            queue.Enqueue(completion);

            try
            {
                operation();
            }
            catch (Exception exception)
            {
                if (queue.Count > 0)
                    queue.Dequeue();

                completion.SetException(exception);
            }

            return completion.Task;
        }

        private static void CompleteTypedOperationSuccess<T>(
            Queue<TaskCompletionSource<T>> queue,
            T payload)
        {
            if (queue.Count == 0)
                return;

            queue.Dequeue().TrySetResult(payload);
        }

        private static void CompleteTypedOperationError<T>(
            Queue<TaskCompletionSource<T>> queue,
            string data)
        {
            if (queue.Count == 0)
                return;

            queue.Dequeue().TrySetException(new InvalidOperationException(ExtractErrorMessage(data)));
        }

        private static Task<ChannelStateValueData> RunStateValueOperation(Action operation)
            => RunTypedOperation(PendingStateValueOperations, operation);

        public static async Task<ChannelStateMutationResultData> setValue(ChannelStateValueQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            try
            {
                ChannelStateValueData payload = await RunStateValueOperation(() =>
                    GP_Channels_SetValueJson(SerializeChannelStateValueQuery(query)));

                return new ChannelStateMutationResultData
                {
                    success = true,
                    value = payload?.value
                };
            }
            catch
            {
                return new ChannelStateMutationResultData
                {
                    success = false,
                    value = null
                };
            }
#else

            ConsoleLog("SET VALUE");
            return await Task.FromResult<ChannelStateMutationResultData>(null);
#endif
        }
        internal static void addValue(int channel_ID, string key, int value) => AddValue(channel_ID, key, value);
        internal static void addValue(int channel_ID, string key, float value) => AddValue(channel_ID, key, value);
        public static async Task<ChannelStateMutationResultData> addValue(ChannelStateValueQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            try
            {
                ChannelStateValueData payload = await RunStateValueOperation(() =>
                    GP_Channels_AddValueJson(SerializeChannelStateValueQuery(query)));

                return new ChannelStateMutationResultData
                {
                    success = true,
                    value = payload?.value
                };
            }
            catch
            {
                return new ChannelStateMutationResultData
                {
                    success = false,
                    value = null
                };
            }
#else

            ConsoleLog("ADD VALUE");
            return await Task.FromResult<ChannelStateMutationResultData>(null);
#endif
        }
        public static Task<CreateChannelData> createChannel(CreateChannelFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingCreateChannelOperations, () => CreateChannel(filter));
#else
            CreateChannel(filter);
            return Task.FromResult<CreateChannelData>(null);
#endif
        }
        public static Task<UpdateChannelData> updateChannel(UpdateChannelFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingUpdateChannelOperations, () => UpdateChannel(filter));
#else
            UpdateChannel(filter);
            return Task.FromResult<UpdateChannelData>(null);
#endif
        }
        public static Task<FetchChannelsResultData> fetchChannels(FetchChannelsFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchChannelsOperations, () => FetchChannels(filter));
#else
            FetchChannels(filter);
            return Task.FromResult<FetchChannelsResultData>(null);
#endif
        }
        public static Task<FetchChannelsResultData> fetchMoreChannels(FetchMoreChannelsFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchMoreChannelsOperations, () => FetchMoreChannels(filter));
#else
            FetchMoreChannels(filter);
            return Task.FromResult<FetchChannelsResultData>(null);
#endif
        }
        public static Task<FetchMembersResultData> fetchMembers(FetchMembersFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchMembersOperations, () => FetchMembers(filter));
#else
            FetchMembers(filter);
            return Task.FromResult<FetchMembersResultData>(null);
#endif
        }
        public static Task<FetchMembersResultData> fetchMoreMembers(FetchMoreMembersFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingFetchMoreMembersOperations, () => FetchMoreMembers(filter));
#else
            FetchMoreMembers(filter);
            return Task.FromResult<FetchMembersResultData>(null);
#endif
        }
        internal static void on(string eventName, UnityAction callback) => AddSimpleEventCallback(eventName, callback);
        internal static void off(string eventName, UnityAction callback) => RemoveSimpleEventCallback(eventName, callback);
        public static void on(string eventName, UnityAction<GP_Data> callback) => AddDataEventCallback(eventName, callback);
        public static void off(string eventName, UnityAction<GP_Data> callback) => RemoveDataEventCallback(eventName, callback);
        public static void on<T>(string eventName, UnityAction<T> callback) => AddTypedEventCallback(eventName, callback);
        public static void off<T>(string eventName, UnityAction<T> callback) => RemoveTypedEventCallback(eventName, callback);
#endregion

#region Callbacks
        private void CallOnOpenChat() { OnOpenChat?.Invoke(); _onOpenChat?.Invoke(); InvokeSimpleEvent("openChat"); }
        private void CallOnCloseChat()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            SetWebGLKeyboardCapture(true);
#endif
            OnCloseChat?.Invoke();
            _onCloseChat?.Invoke();
            InvokeSimpleEvent("closeChat");
        }
        private void CallOnOpenChatError(string data)
        {
            OnOpenChatError?.Invoke();
            _onOpenChatError?.Invoke();
            InvokeDataEvent("error:openChat", CreateDataOrNull(data));
        }

        private void CallOnOpenFeedError(string data)
        {
            OnOpenChatError?.Invoke();
            _onOpenChatError?.Invoke();
            InvokeDataEvent("error:openFeed", CreateDataOrNull(data));
        }

        private void CallOnCreateChannel(string data)
        {
            CreateChannelData typedPayload = JsonUtility.FromJson<CreateChannelData>(data);
            OnCreateChannel?.Invoke(typedPayload);
            CompleteTypedOperationSuccess(PendingCreateChannelOperations, typedPayload);
            InvokeDataEvent("createChannel", new GP_Data(data));
        }

        private void CallOnCreateChannelError(string data)
        {
            OnCreateChannelError?.Invoke();
            CompleteTypedOperationError(PendingCreateChannelOperations, data);
            InvokeDataEvent("error:createChannel", CreateDataOrNull(data));
        }

        private void CallOnUpdateChannel(string data)
        {
            UpdateChannelData typedPayload = JsonUtility.FromJson<UpdateChannelData>(data);
            OnUpdateChannel?.Invoke(typedPayload);
            CompleteTypedOperationSuccess(PendingUpdateChannelOperations, typedPayload);
            InvokeDataEvent("updateChannel", new GP_Data(data));
        }

        private void CallOnUpdateChannelEvent(string data)
        {
            InvokeDataEvent("event:updateChannel", CreateDataOrNull(data));
        }

        private void CallOnUpdateChannelError(string data)
        {
            OnUpdateChannelError?.Invoke();
            CompleteTypedOperationError(PendingUpdateChannelOperations, data);
            InvokeDataEvent("error:updateChannel", CreateDataOrNull(data));
        }

        private void CallOnDeleteChannelSuccess()
        {
            OnDeleteChannelSuccess?.Invoke();
            CompleteTypedOperationSuccess(PendingDeleteChannelOperations, true);
            InvokeSimpleEvent("deleteChannel");
        }

        private void CallOnDeleteChannelEvent(string data)
        {
            FetchChannelData channel = JsonUtility.FromJson<FetchChannelData>(data);
            OnDeleteChannelEvent?.Invoke(channel.id);
            InvokeDataEvent("event:deleteChannel", new GP_Data(data));
        }

        private void CallOnDeleteChannelError(string data)
        {
            OnDeleteChannelError?.Invoke();
            CompleteTypedOperationError(PendingDeleteChannelOperations, data);
            InvokeDataEvent("error:deleteChannel", CreateDataOrNull(data));
        }

        private void CallOnChannelsConnectEvent(string data)
        {
            InvokeDataEvent("event:connect", CreateDataOrNull(data));
        }

        private void CallOnFetchChannel(string data)
        {
            FetchChannelData typedPayload = JsonUtility.FromJson<FetchChannelData>(data);
            OnFetchChannel?.Invoke(typedPayload);
            CompleteTypedOperationSuccess(PendingFetchChannelOperations, typedPayload);
            InvokeDataEvent("fetchChannel", new GP_Data(data));
        }

        private void CallOnFetchChannelError(string data)
        {
            OnFetchChannelError?.Invoke();
            CompleteTypedOperationError(PendingFetchChannelOperations, data);
            InvokeDataEvent("error:fetchChannel", CreateDataOrNull(data));
        }

        private void CallOnFetchPersonalChannel(string data)
        {
            FetchChannelData typedPayload = JsonUtility.FromJson<FetchChannelData>(data);
            OnFetchPersonalChannel?.Invoke(typedPayload);
            CompleteTypedOperationSuccess(PendingFetchPersonalChannelOperations, typedPayload);
            InvokeDataEvent("fetchPersonalChannel", new GP_Data(data));
        }

        private void CallOnFetchPersonalChannelError(string data)
        {
            OnFetchPersonalChannelError?.Invoke();
            CompleteTypedOperationError(PendingFetchPersonalChannelOperations, data);
            InvokeDataEvent("error:fetchPersonalChannel", CreateDataOrNull(data));
        }

        private void CallOnFetchFeedChannel(string data)
        {
            FetchChannelData typedPayload = JsonUtility.FromJson<FetchChannelData>(data);
            OnFetchFeedChannel?.Invoke(typedPayload);
            CompleteTypedOperationSuccess(PendingFetchFeedChannelOperations, typedPayload);
            InvokeDataEvent("fetchFeedChannel", new GP_Data(data));
        }

        private void CallOnFetchFeedChannelError(string data)
        {
            OnFetchFeedChannelError?.Invoke();
            CompleteTypedOperationError(PendingFetchFeedChannelOperations, data);
            InvokeDataEvent("error:fetchFeedChannel", CreateDataOrNull(data));
        }

        private void CallOnFetchChannels(string data)
        {
            List<FetchChannelData> typedPayload = UtilityJSON.GetList<FetchChannelData>(data);
            OnFetchChannels?.Invoke(typedPayload, _canLoadMoreFetchChannels);
            CompleteTypedOperationSuccess(
                PendingFetchChannelsOperations,
                new FetchChannelsResultData
                {
                    items = typedPayload,
                    canLoadMore = _canLoadMoreFetchChannels
                });
            InvokeDataEvent("fetchChannels", CreatePagedResultData(data, _canLoadMoreFetchChannels));
        }
        private bool _canLoadMoreFetchChannels;
        private void CallOnFetchChannelsCanLoadMore(string canLoadMore) => _canLoadMoreFetchChannels = canLoadMore == "true";
        private void CallOnFetchChannelsError(string data)
        {
            OnFetchChannelsError?.Invoke();
            CompleteTypedOperationError(PendingFetchChannelsOperations, data);
            InvokeDataEvent("error:fetchChannels", CreateDataOrNull(data));
        }

        private void CallOnFetchMoreChannels(string data)
        {
            List<FetchChannelData> typedPayload = UtilityJSON.GetList<FetchChannelData>(data);
            OnFetchMoreChannels?.Invoke(typedPayload, _canLoadMoreFetchMoreChannels);
            CompleteTypedOperationSuccess(
                PendingFetchMoreChannelsOperations,
                new FetchChannelsResultData
                {
                    items = typedPayload,
                    canLoadMore = _canLoadMoreFetchMoreChannels
                });
            InvokeDataEvent("fetchMoreChannels", CreatePagedResultData(data, _canLoadMoreFetchMoreChannels));
        }
        private bool _canLoadMoreFetchMoreChannels;
        private void CallOnFetchMoreChannelsCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreChannels = canLoadMore == "true";
        private void CallOnFetchMoreChannelsError(string data)
        {
            OnFetchMoreChannelsError?.Invoke();
            CompleteTypedOperationError(PendingFetchMoreChannelsOperations, data);
            InvokeDataEvent("error:fetchMoreChannels", CreateDataOrNull(data));
        }

        private void CallOnJoinSuccess()
        {
            OnJoinSuccess?.Invoke();
            CompleteVoidOperationSuccess(PendingJoinOperations);
            InvokeSimpleEvent("join");
        }

        private void CallOnJoinEvent(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnJoinEvent?.Invoke(payload);
            InvokeDataEvent("event:join", payload);
        }

        private void CallOnJoinError(string data)
        {
            OnJoinError?.Invoke();
            CompleteVoidOperationError(PendingJoinOperations, data);
            InvokeDataEvent("error:join", CreateDataOrNull(data));
        }

        private void CallOnJoinRequest(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnJoinRequest?.Invoke(payload);
            InvokeDataEvent("event:joinRequest", payload);
        }

        private void CallOnCancelJoinSuccess()
        {
            OnCancelJoinSuccess?.Invoke();
            CompleteVoidOperationSuccess(PendingCancelJoinOperations);
            InvokeSimpleEvent("cancelJoin");
        }

        private void CallOnCancelJoinEvent(string data)
        {
            OnCancelJoinEvent?.Invoke(JsonUtility.FromJson<CancelJoinData>(data));
            InvokeDataEvent("event:cancelJoin", new GP_Data(data));
        }

        private void CallOnCancelJoinError(string data)
        {
            OnCancelJoinError?.Invoke();
            CompleteVoidOperationError(PendingCancelJoinOperations, data);
            InvokeDataEvent("error:cancelJoin", CreateDataOrNull(data));
        }

        private void CallOnLeaveSuccess()
        {
            OnLeaveSuccess?.Invoke();
            CompleteVoidOperationSuccess(PendingLeaveOperations);
            InvokeSimpleEvent("leave");
        }

        private void CallOnLeaveEvent(string data)
        {
            OnLeaveEvent?.Invoke(JsonUtility.FromJson<MemberLeaveData>(data));
            InvokeDataEvent("event:leave", new GP_Data(data));
        }

        private void CallOnLeaveError(string data)
        {
            OnLeaveError?.Invoke();
            CompleteVoidOperationError(PendingLeaveOperations, data);
            InvokeDataEvent("error:leave", CreateDataOrNull(data));
        }


        private void CallOnKick()
        {
            OnKick?.Invoke();
            CompleteVoidOperationSuccess(PendingKickOperations);
            InvokeSimpleEvent("kick");
        }

        private void CallOnKickError(string data)
        {
            OnKickError?.Invoke();
            CompleteVoidOperationError(PendingKickOperations, data);
            InvokeDataEvent("error:kick", CreateDataOrNull(data));
        }

        private static void CompletePendingStateValueSuccess(ChannelStateValueData payload)
        {
            if (PendingStateValueOperations.Count == 0)
                return;

            PendingStateValueOperations.Dequeue().TrySetResult(payload);
        }

        private static void CompletePendingStateValueError(string data)
        {
            if (PendingStateValueOperations.Count == 0)
                return;

            PendingStateValueOperations
                .Dequeue()
                .TrySetException(new InvalidOperationException(ExtractErrorMessage(data)));
        }

        private void CallOnSetValue(string data)
        {
            ChannelStateValueData typedPayload = DeserializeJsonOrDefault<ChannelStateValueData>(data);
            GP_Data payload = new GP_Data(data);
            OnSetValue?.Invoke(typedPayload);
            CompletePendingStateValueSuccess(typedPayload);
            InvokeDataEvent("setValue", payload);
        }

        private void CallOnSetValueError(string data)
        {
            OnSetValueError?.Invoke();
            CompletePendingStateValueError(data);
            InvokeDataEvent("error:setValue", CreateDataOrNull(data));
        }

        private void CallOnChangeValue(string data)
        {
            ChannelStateValueData typedPayload = DeserializeJsonOrDefault<ChannelStateValueData>(data);
            GP_Data payload = new GP_Data(data);
            OnChangeValue?.Invoke(typedPayload);
            InvokeDataEvent("event:changeValue", payload);
        }


        private void CallOnFetchMembers(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchMembers?.Invoke(payload, _canLoadMoreFetchMembers);
            CompleteTypedOperationSuccess(
                PendingFetchMembersOperations,
                CreateTypedPagedResult<FetchMembersResultData, ChannelMemberData>(data, _canLoadMoreFetchMembers));
            InvokeDataEvent("fetchMembers", CreatePagedResultData(data, _canLoadMoreFetchMembers));
        }
        private bool _canLoadMoreFetchMembers;
        private void CallOnFetchMembersCanLoadMore(string canLoadMore) => _canLoadMoreFetchMembers = canLoadMore == "true";
        private void CallOnFetchMembersError(string data)
        {
            OnFetchMembersError?.Invoke();
            CompleteTypedOperationError(PendingFetchMembersOperations, data);
            InvokeDataEvent("error:fetchMembers", CreateDataOrNull(data));
        }


        private void CallOnFetchMoreMembers(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchMoreMembers?.Invoke(payload, _canLoadMoreFetchMoreMembers);
            CompleteTypedOperationSuccess(
                PendingFetchMoreMembersOperations,
                CreateTypedPagedResult<FetchMembersResultData, ChannelMemberData>(data, _canLoadMoreFetchMoreMembers));
            InvokeDataEvent("fetchMoreMembers", CreatePagedResultData(data, _canLoadMoreFetchMoreMembers));
        }
        private bool _canLoadMoreFetchMoreMembers;
        private void CallOnFetchMoreMembersCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreMembers = canLoadMore == "true";
        private void CallOnFetchMoreMembersError(string data)
        {
            OnFetchMoreMembersError?.Invoke();
            CompleteTypedOperationError(PendingFetchMoreMembersOperations, data);
            InvokeDataEvent("error:fetchMoreMembers", CreateDataOrNull(data));
        }


        private void CallOnMuteSuccess()
        {
            OnMuteSuccess?.Invoke();
            CompleteVoidOperationSuccess(PendingMuteOperations);
            InvokeSimpleEvent("mute");
        }

        private void CallOnMuteEvent(string data)
        {
            OnMuteEvent?.Invoke(JsonUtility.FromJson<MuteData>(data));
            InvokeDataEvent("event:mute", new GP_Data(data));
        }

        private void CallOnMuteError(string data)
        {
            OnMuteError?.Invoke();
            CompleteVoidOperationError(PendingMuteOperations, data);
            InvokeDataEvent("error:mute", CreateDataOrNull(data));
        }


        private void CallOnUnmuteSuccess()
        {
            OnUnmuteSuccess?.Invoke();
            CompleteVoidOperationSuccess(PendingUnmuteOperations);
            InvokeSimpleEvent("unmute");
        }

        private void CallOnUnmuteEvent(string data)
        {
            OnUnmuteEvent?.Invoke(JsonUtility.FromJson<UnmuteData>(data));
            InvokeDataEvent("event:unmute", new GP_Data(data));
        }

        private void CallOnUnmuteError(string data)
        {
            OnUnmuteError?.Invoke();
            CompleteVoidOperationError(PendingUnmuteOperations, data);
            InvokeDataEvent("error:unmute", CreateDataOrNull(data));
        }


        private void CallOnSendInvite()
        {
            OnSendInvite?.Invoke();
            CompleteVoidOperationSuccess(PendingSendInviteOperations);
            InvokeSimpleEvent("sendInvite");
        }

        private void CallOnSendInviteError(string data)
        {
            OnSendInviteError?.Invoke();
            CompleteVoidOperationError(PendingSendInviteOperations, data);
            InvokeDataEvent("error:sendInvite", CreateDataOrNull(data));
        }


        private void CallOnInvite(string data)
        {
            OnInvite?.Invoke(JsonUtility.FromJson<InviteData>(data));
            InvokeDataEvent("event:invite", new GP_Data(data));
        }


        private void CallOnCancelInviteSuccess()
        {
            OnCancelInviteSuccess?.Invoke();
            CompleteVoidOperationSuccess(PendingCancelInviteOperations);
            InvokeSimpleEvent("cancelInvite");
        }

        private void CallOnCancelInviteEvent(string data)
        {
            OnCancelInviteEvent?.Invoke(JsonUtility.FromJson<CancelInviteData>(data));
            InvokeDataEvent("event:cancelInvite", new GP_Data(data));
        }

        private void CallOnCancelInviteError(string data)
        {
            OnCancelInviteError?.Invoke();
            CompleteVoidOperationError(PendingCancelInviteOperations, data);
            InvokeDataEvent("error:cancelInvite", CreateDataOrNull(data));
        }


        private void CallOnAcceptInvite()
        {
            OnAcceptInvite?.Invoke();
            CompleteVoidOperationSuccess(PendingAcceptInviteOperations);
            InvokeSimpleEvent("acceptInvite");
        }

        private void CallOnAcceptInviteEvent(string data)
        {
            InvokeDataEvent("event:acceptInvite", CreateDataOrNull(data));
        }

        private void CallOnAcceptInviteError(string data)
        {
            OnAcceptInviteError?.Invoke();
            CompleteVoidOperationError(PendingAcceptInviteOperations, data);
            InvokeDataEvent("error:acceptInvite", CreateDataOrNull(data));
        }


        private void CallOnRejectInviteSuccess()
        {
            OnRejectInviteSuccess?.Invoke();
            CompleteVoidOperationSuccess(PendingRejectInviteOperations);
            InvokeSimpleEvent("rejectInvite");
        }

        private void CallOnRejectInviteEvent(string data)
        {
            OnRejectInviteEvent?.Invoke(JsonUtility.FromJson<RejectInviteData>(data));
            InvokeDataEvent("event:rejectInvite", new GP_Data(data));
        }

        private void CallOnRejectInviteError(string data)
        {
            OnRejectInviteError?.Invoke();
            CompleteVoidOperationError(PendingRejectInviteOperations, data);
            InvokeDataEvent("error:rejectInvite", CreateDataOrNull(data));
        }


        private void CallOnFetchInvites(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchInvites?.Invoke(payload, _canLoadMoreFetchInvites);
            CompleteTypedOperationSuccess(
                PendingFetchInvitesOperations,
                CreateTypedPagedResult<FetchInvitesResultData, ChannelInviteListItemData>(data, _canLoadMoreFetchInvites));
            InvokeDataEvent("fetchInvites", CreatePagedResultData(data, _canLoadMoreFetchInvites));
        }
        private bool _canLoadMoreFetchInvites;
        private void CallOnFetchInvitesCanLoadMore(string canLoadMore) => _canLoadMoreFetchInvites = canLoadMore == "true";
        private void CallOnFetchInvitesError(string data)
        {
            OnFetchInvitesError?.Invoke();
            CompleteTypedOperationError(PendingFetchInvitesOperations, data);
            InvokeDataEvent("error:fetchInvites", CreateDataOrNull(data));
        }


        private void CallOnFetchMoreInvites(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchMoreInvites?.Invoke(payload, _canLoadMoreFetchMoreInvites);
            CompleteTypedOperationSuccess(
                PendingFetchMoreInvitesOperations,
                CreateTypedPagedResult<FetchInvitesResultData, ChannelInviteListItemData>(data, _canLoadMoreFetchMoreInvites));
            InvokeDataEvent("fetchMoreInvites", CreatePagedResultData(data, _canLoadMoreFetchMoreInvites));
        }
        private bool _canLoadMoreFetchMoreInvites;
        private void CallOnFetchMoreInvitesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreInvites = canLoadMore == "true";
        private void CallOnFetchMoreInvitesError(string data)
        {
            OnFetchMoreInvitesError?.Invoke();
            CompleteTypedOperationError(PendingFetchMoreInvitesOperations, data);
            InvokeDataEvent("error:fetchMoreInvites", CreateDataOrNull(data));
        }


        private void CallOnFetchChannelInvites(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchChannelInvites?.Invoke(payload, _canLoadMoreFetchChannelInvites);
            CompleteTypedOperationSuccess(
                PendingFetchChannelInvitesOperations,
                CreateTypedPagedResult<FetchInvitesResultData, ChannelInviteListItemData>(data, _canLoadMoreFetchChannelInvites));
            InvokeDataEvent("fetchChannelInvites", CreatePagedResultData(data, _canLoadMoreFetchChannelInvites));
        }
        private bool _canLoadMoreFetchChannelInvites;
        private void CallOnFetchChannelInvitesCanLoadMore(string canLoadMore) => _canLoadMoreFetchChannelInvites = canLoadMore == "true";
        private void CallOnFetchChannelInvitesError(string data)
        {
            OnFetchChannelInvitesError?.Invoke();
            CompleteTypedOperationError(PendingFetchChannelInvitesOperations, data);
            InvokeDataEvent("error:fetchChannelInvites", CreateDataOrNull(data));
        }


        private void CallOnFetchMoreChannelInvites(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchMoreChannelInvites?.Invoke(payload, _canLoadMoreFetchMoreChannelInvites);
            CompleteTypedOperationSuccess(
                PendingFetchMoreChannelInvitesOperations,
                CreateTypedPagedResult<FetchInvitesResultData, ChannelInviteListItemData>(data, _canLoadMoreFetchMoreChannelInvites));
            InvokeDataEvent("fetchMoreChannelInvites", CreatePagedResultData(data, _canLoadMoreFetchMoreChannelInvites));
        }
        private bool _canLoadMoreFetchMoreChannelInvites;
        private void CallOnFetchMoreChannelInvitesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreChannelInvites = canLoadMore == "true";
        private void CallOnFetchMoreChannelInvitesError(string data)
        {
            OnFetchMoreChannelInvitesError?.Invoke();
            CompleteTypedOperationError(PendingFetchMoreChannelInvitesOperations, data);
            InvokeDataEvent("error:fetchMoreChannelInvites", CreateDataOrNull(data));
        }


        private void CallOnFetchSentInvites(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchSentInvites?.Invoke(payload, _canLoadMoreFetchSentInvites);
            CompleteTypedOperationSuccess(
                PendingFetchSentInvitesOperations,
                CreateTypedPagedResult<FetchInvitesResultData, ChannelInviteListItemData>(data, _canLoadMoreFetchSentInvites));
            InvokeDataEvent("fetchSentInvites", CreatePagedResultData(data, _canLoadMoreFetchSentInvites));
        }
        private bool _canLoadMoreFetchSentInvites;
        private void CallOnFetchSentInvitesCanLoadMore(string canLoadMore) => _canLoadMoreFetchSentInvites = canLoadMore == "true";
        private void CallOnFetchSentInvitesError(string data)
        {
            OnFetchSentInvitesError?.Invoke();
            CompleteTypedOperationError(PendingFetchSentInvitesOperations, data);
            InvokeDataEvent("error:fetchSentInvites", CreateDataOrNull(data));
        }


        private void CallOnFetchMoreSentInvites(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchMoreSentInvites?.Invoke(payload, _canLoadMoreFetchMoreSentInvites);
            CompleteTypedOperationSuccess(
                PendingFetchMoreSentInvitesOperations,
                CreateTypedPagedResult<FetchInvitesResultData, ChannelInviteListItemData>(data, _canLoadMoreFetchMoreSentInvites));
            InvokeDataEvent("fetchMoreSentInvites", CreatePagedResultData(data, _canLoadMoreFetchMoreSentInvites));
        }
        private bool _canLoadMoreFetchMoreSentInvites;
        private void CallOnFetchMoreSentInvitesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreSentInvites = canLoadMore == "true";
        private void CallOnFetchMoreSentInvitesError(string data)
        {
            OnFetchMoreSentInvitesError?.Invoke();
            CompleteTypedOperationError(PendingFetchMoreSentInvitesOperations, data);
            InvokeDataEvent("error:fetchMoreSentInvites", CreateDataOrNull(data));
        }


        private void CallOnAcceptJoinRequest()
        {
            OnAcceptJoinRequest?.Invoke();
            CompleteVoidOperationSuccess(PendingAcceptJoinRequestOperations);
            InvokeSimpleEvent("acceptJoinRequest");
        }

        private void CallOnAcceptJoinRequestError(string data)
        {
            OnAcceptJoinRequestError?.Invoke();
            CompleteVoidOperationError(PendingAcceptJoinRequestOperations, data);
            InvokeDataEvent("error:acceptJoinRequest", CreateDataOrNull(data));
        }


        private void CallOnRejectJoinRequestSuccess()
        {
            OnRejectJoinRequestSuccess?.Invoke();
            CompleteVoidOperationSuccess(PendingRejectJoinRequestOperations);
            InvokeSimpleEvent("rejectJoinRequest");
        }

        private void CallOnRejectJoinRequestEvent(string data)
        {
            OnRejectJoinRequestEvent?.Invoke(JsonUtility.FromJson<RejectJoinRequestData>(data));
            InvokeDataEvent("event:rejectJoinRequest", new GP_Data(data));
        }

        private void CallOnRejectJoinRequestError(string data)
        {
            OnRejectJoinRequestError?.Invoke();
            CompleteVoidOperationError(PendingRejectJoinRequestOperations, data);
            InvokeDataEvent("error:rejectJoinRequest", CreateDataOrNull(data));
        }


        private void CallOnFetchJoinRequests(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchJoinRequests?.Invoke(payload, _canLoadMoreFetchJoinRequests);
            CompleteTypedOperationSuccess(
                PendingFetchJoinRequestsOperations,
                CreateTypedPagedResult<FetchJoinRequestsResultData, ChannelJoinRequestItemData>(data, _canLoadMoreFetchJoinRequests));
            InvokeDataEvent("fetchJoinRequests", CreatePagedResultData(data, _canLoadMoreFetchJoinRequests));
        }
        private bool _canLoadMoreFetchJoinRequests;
        private void CallOnFetchJoinRequestsCanLoadMore(string canLoadMore) => _canLoadMoreFetchJoinRequests = canLoadMore == "true";
        private void CallOnFetchJoinRequestsError(string data)
        {
            OnFetchJoinRequestsError?.Invoke();
            CompleteTypedOperationError(PendingFetchJoinRequestsOperations, data);
            InvokeDataEvent("error:fetchJoinRequests", CreateDataOrNull(data));
        }


        private void CallOnFetchMoreJoinRequests(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchMoreJoinRequests?.Invoke(payload, _canLoadMoreFetchMoreJoinRequests);
            CompleteTypedOperationSuccess(
                PendingFetchMoreJoinRequestsOperations,
                CreateTypedPagedResult<FetchJoinRequestsResultData, ChannelJoinRequestItemData>(data, _canLoadMoreFetchMoreJoinRequests));
            InvokeDataEvent("fetchMoreJoinRequests", CreatePagedResultData(data, _canLoadMoreFetchMoreJoinRequests));
        }
        private bool _canLoadMoreFetchMoreJoinRequests;
        private void CallOnFetchMoreJoinRequestsCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreJoinRequests = canLoadMore == "true";
        private void CallOnFetchMoreJoinRequestsError(string data)
        {
            OnFetchMoreJoinRequestsError?.Invoke();
            CompleteTypedOperationError(PendingFetchMoreJoinRequestsOperations, data);
            InvokeDataEvent("error:fetchMoreJoinRequests", CreateDataOrNull(data));
        }


        private void CallOnFetchSentJoinRequests(string data)
        {
            OnFetchSentJoinRequests?.Invoke(UtilityJSON.GetList<JoinRequestsData>(data), _canLoadMoreFetchSentJoinRequests);
            CompleteTypedOperationSuccess(
                PendingFetchSentJoinRequestsOperations,
                CreateTypedPagedResult<FetchJoinRequestsResultData, ChannelJoinRequestItemData>(data, _canLoadMoreFetchSentJoinRequests));
            InvokeDataEvent("fetchSentJoinRequests", CreatePagedResultData(data, _canLoadMoreFetchSentJoinRequests));
        }
        private bool _canLoadMoreFetchSentJoinRequests;
        private void CallOnFetchSentJoinRequestsCanLoadMore(string canLoadMore) => _canLoadMoreFetchSentJoinRequests = canLoadMore == "true";
        private void CallOnFetchSentJoinRequestsError(string data)
        {
            OnFetchSentJoinRequestsError?.Invoke();
            CompleteTypedOperationError(PendingFetchSentJoinRequestsOperations, data);
            InvokeDataEvent("error:fetchSentJoinRequests", CreateDataOrNull(data));
        }


        private void CallOnFetchMoreSentJoinRequests(string data)
        {
            OnFetchMoreSentJoinRequests?.Invoke(UtilityJSON.GetList<JoinRequestsData>(data), _canLoadMoreFetchMoreSentJoinRequests);
            CompleteTypedOperationSuccess(
                PendingFetchMoreSentJoinRequestsOperations,
                CreateTypedPagedResult<FetchJoinRequestsResultData, ChannelJoinRequestItemData>(data, _canLoadMoreFetchMoreSentJoinRequests));
            InvokeDataEvent("fetchMoreSentJoinRequests", CreatePagedResultData(data, _canLoadMoreFetchMoreSentJoinRequests));
        }
        private bool _canLoadMoreFetchMoreSentJoinRequests;
        private void CallOnFetchMoreSentJoinRequestsCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreSentJoinRequests = canLoadMore == "true";
        private void CallOnFetchMoreSentJoinRequestsError(string data)
        {
            OnFetchMoreSentJoinRequestsError?.Invoke();
            CompleteTypedOperationError(PendingFetchMoreSentJoinRequestsOperations, data);
            InvokeDataEvent("error:fetchMoreSentJoinRequests", CreateDataOrNull(data));
        }


        private void CallOnSendMessage(string data)
        {
            ChannelMessageResultData typedPayload = DeserializeJsonOrDefault<ChannelMessageResultData>(data);
            GP_Data payload = new GP_Data(data);
            OnSendMessage?.Invoke(payload);
            if (PendingSendMessageOperations.Count > 0)
                CompleteTypedOperationSuccess(PendingSendMessageOperations, typedPayload);
            else if (PendingSendPersonalMessageOperations.Count > 0)
                CompleteTypedOperationSuccess(PendingSendPersonalMessageOperations, typedPayload);
            else if (PendingSendFeedMessageOperations.Count > 0)
                CompleteTypedOperationSuccess(PendingSendFeedMessageOperations, typedPayload);
            InvokeDataEvent("sendMessage", payload);
        }

        private void CallOnSendMessageError(string data)
        {
            OnSendMessageError?.Invoke();
            if (PendingSendMessageOperations.Count > 0)
                CompleteTypedOperationError(PendingSendMessageOperations, data);
            else if (PendingSendPersonalMessageOperations.Count > 0)
                CompleteTypedOperationError(PendingSendPersonalMessageOperations, data);
            else if (PendingSendFeedMessageOperations.Count > 0)
                CompleteTypedOperationError(PendingSendFeedMessageOperations, data);
            InvokeDataEvent("error:sendMessage", CreateDataOrNull(data));
        }


        private void CallOnMessage(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnMessage?.Invoke(payload);
            InvokeDataEvent("event:message", payload);
        }


        private void CallOnEditMessageSuccess(string data)
        {
            ChannelMessageResultData typedPayload = DeserializeJsonOrDefault<ChannelMessageResultData>(data);
            GP_Data payload = new GP_Data(data);
            OnEditMessageSuccess?.Invoke(payload);
            CompleteTypedOperationSuccess(PendingEditMessageOperations, typedPayload);
            InvokeDataEvent("editMessage", payload);
        }

        private void CallOnEditMessageEvent(string data)
        {
            OnEditMessageEvent?.Invoke(JsonUtility.FromJson<MessageData>(data));
            InvokeDataEvent("event:editMessage", new GP_Data(data));
        }

        private void CallOnEditMessageError(string data)
        {
            OnEditMessageError?.Invoke();
            CompleteTypedOperationError(PendingEditMessageOperations, data);
            InvokeDataEvent("error:editMessage", CreateDataOrNull(data));
        }



        private void CallOnDeleteMessageSuccess()
        {
            OnDeleteMessageSuccess?.Invoke();
            CompleteVoidOperationSuccess(PendingDeleteMessageOperations);
            InvokeSimpleEvent("deleteMessage");
        }

        private void CallOnDeleteMessageEvent(string data)
        {
            OnDeleteMessageEvent?.Invoke(JsonUtility.FromJson<MessageData>(data));
            InvokeDataEvent("event:deleteMessage", new GP_Data(data));
        }

        private void CallOnDeleteMessageError(string data)
        {
            OnDeleteMessageError?.Invoke();
            CompleteVoidOperationError(PendingDeleteMessageOperations, data);
            InvokeDataEvent("error:deleteMessage", CreateDataOrNull(data));
        }



        private void CallOnFetchMessages(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchMessages?.Invoke(payload, _canLoadMoreFetchMessages);
            CompleteTypedOperationSuccess(
                PendingFetchMessagesOperations,
                CreateTypedPagedResult<FetchMessagesResultData, ChannelMessageResultData>(data, _canLoadMoreFetchMessages));
            InvokeDataEvent("fetchMessages", CreatePagedResultData(data, _canLoadMoreFetchMessages));
        }
        private bool _canLoadMoreFetchMessages;
        private void CallOnFetchMessagesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMessages = canLoadMore == "true";
        private void CallOnFetchMessagesError(string data)
        {
            OnFetchMessagesError?.Invoke();
            CompleteTypedOperationError(PendingFetchMessagesOperations, data);
            InvokeDataEvent("error:fetchMessages", CreateDataOrNull(data));
        }

        private void CallOnFetchPersonalMessages(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchPersonalMessages?.Invoke(payload, _canLoadMoreFetchPersonalMessages);
            CompleteTypedOperationSuccess(
                PendingFetchPersonalMessagesOperations,
                CreateTypedPagedResult<FetchMessagesResultData, ChannelMessageResultData>(data, _canLoadMoreFetchPersonalMessages));
            InvokeDataEvent("fetchPersonalMessages", CreatePagedResultData(data, _canLoadMoreFetchPersonalMessages));
        }
        private bool _canLoadMoreFetchPersonalMessages;
        private void CallOnFetchPersonalMessagesCanLoadMore(string canLoadMore) => _canLoadMoreFetchPersonalMessages = canLoadMore == "true";
        private void CallOnFetchPersonalMessagesError(string data)
        {
            OnFetchPersonalMessagesError?.Invoke();
            CompleteTypedOperationError(PendingFetchPersonalMessagesOperations, data);
            InvokeDataEvent("error:fetchPersonalMessages", CreateDataOrNull(data));
        }


        private void CallOnFetchFeedMessages(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchFeedMessages?.Invoke(payload, _canLoadMoreFetchFeedMessages);
            CompleteTypedOperationSuccess(
                PendingFetchFeedMessagesOperations,
                CreateTypedPagedResult<FetchMessagesResultData, ChannelMessageResultData>(data, _canLoadMoreFetchFeedMessages));
            InvokeDataEvent("fetchFeedMessages", CreatePagedResultData(data, _canLoadMoreFetchFeedMessages));
        }
        private bool _canLoadMoreFetchFeedMessages;
        private void CallOnFetchFeedMessagesCanLoadMore(string canLoadMore) => _canLoadMoreFetchFeedMessages = canLoadMore == "true";
        private void CallOnFetchFeedMessagesError(string data)
        {
            OnFetchFeedMessagesError?.Invoke();
            CompleteTypedOperationError(PendingFetchFeedMessagesOperations, data);
            InvokeDataEvent("error:fetchFeedMessages", CreateDataOrNull(data));
        }



        private void CallOnFetchMoreMessages(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchMoreMessages?.Invoke(payload, _canLoadMoreFetchMoreMessages);
            CompleteTypedOperationSuccess(
                PendingFetchMoreMessagesOperations,
                CreateTypedPagedResult<FetchMessagesResultData, ChannelMessageResultData>(data, _canLoadMoreFetchMoreMessages));
            InvokeDataEvent("fetchMoreMessages", CreatePagedResultData(data, _canLoadMoreFetchMoreMessages));
        }
        private bool _canLoadMoreFetchMoreMessages;
        private void CallOnFetchMoreMessagesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreMessages = canLoadMore == "true";
        private void CallOnFetchMoreMessagesError(string data)
        {
            OnFetchMoreMessagesError?.Invoke();
            CompleteTypedOperationError(PendingFetchMoreMessagesOperations, data);
            InvokeDataEvent("error:fetchMoreMessages", CreateDataOrNull(data));
        }


        private void CallOnFetchMorePersonalMessages(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchMorePersonalMessages?.Invoke(payload, _canLoadMoreFetchMorePersonalMessages);
            CompleteTypedOperationSuccess(
                PendingFetchMorePersonalMessagesOperations,
                CreateTypedPagedResult<FetchMessagesResultData, ChannelMessageResultData>(data, _canLoadMoreFetchMorePersonalMessages));
            InvokeDataEvent("fetchMorePersonalMessages", CreatePagedResultData(data, _canLoadMoreFetchMorePersonalMessages));
        }
        private bool _canLoadMoreFetchMorePersonalMessages;
        private void CallOnFetchMorePersonalMessagesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMorePersonalMessages = canLoadMore == "true";
        private void CallOnFetchMorePersonalMessagesError(string data)
        {
            OnFetchMorePersonalMessagesError?.Invoke();
            CompleteTypedOperationError(PendingFetchMorePersonalMessagesOperations, data);
            InvokeDataEvent("error:fetchMorePersonalMessages", CreateDataOrNull(data));
        }


        private void CallOnFetchMoreFeedMessages(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnFetchMoreFeedMessages?.Invoke(payload, _canLoadMoreFetchMoreFeedMessages);
            CompleteTypedOperationSuccess(
                PendingFetchMoreFeedMessagesOperations,
                CreateTypedPagedResult<FetchMessagesResultData, ChannelMessageResultData>(data, _canLoadMoreFetchMoreFeedMessages));
            InvokeDataEvent("fetchMoreFeedMessages", CreatePagedResultData(data, _canLoadMoreFetchMoreFeedMessages));
        }
        private bool _canLoadMoreFetchMoreFeedMessages;
        private void CallOnFetchMoreFeedMessagesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreFeedMessages = canLoadMore == "true";
        private void CallOnFetchMoreFeedMessagesError(string data)
        {
            OnFetchMoreFeedMessagesError?.Invoke();
            CompleteTypedOperationError(PendingFetchMoreFeedMessagesOperations, data);
            InvokeDataEvent("error:fetchMoreFeedMessages", CreateDataOrNull(data));
        }
#endregion
    }


    [System.Serializable]
    public class CreateChannelData
    {
        public int id;
        public string[] tags;
        public string[] messageTags;
        public int templateId;
        public int capacity;
        public int ownerId;
        public string name;
        public string description;
        public bool @private;
        public bool visible;
        public bool permanent;
        public bool hasPassword;
        public bool isJoined;
        public bool isRequestSent;
        public bool isInvited;
        public bool isMuted;
        public string password;
        public int membersCount;
        public OwnerAcl ownerAcl;
        public MemberAcl memberAcl;
        public GuestAcl guestAcl;
    }

    [System.Serializable]
    public class UpdateChannelData
    {
        public int id;
        public string[] tags;
        public string[] messageTags;
        public int channelId;
        public int capacity;
        public int ownerId;
        public string name;
        public string description;
        public bool @private;
        public bool visible;
        public bool permanent;
        public bool hasPassword;
        public bool isJoined;
        public bool isRequestSent;
        public bool isInvited;
        public bool isMuted;
        public string password;
        public int membersCount;
        public OwnerAcl ownerAcl;
        public MemberAcl memberAcl;
        public GuestAcl guestAcl;
    }

    [System.Serializable]
    public class FetchChannelData
    {
        public int id;
        public string[] tags;
        public string[] messageTags;
        public int templateId;
        public int projectId;
        public int capacity;
        public int ownerId;
        public string name;
        public string description;
        public bool @private;
        public bool visible;
        public bool permanent;
        public bool hasPassword;
        public string password;
        public bool isJoined;
        public bool isInvited;
        public bool isMuted;
        public bool isRequestSent;
        public int membersCount;
        public OwnerAcl ownerAcl;
        public MemberAcl memberAcl;
        public GuestAcl guestAcl;
    }

    [System.Serializable]
    public class JoinRequestsData
    {
        public FetchChannelData channel;
        public string date;
    }

    [System.Serializable]
    public class ChannelMemberMuteData
    {
        public bool isMuted;
        public string unmuteAt;
    }

    [System.Serializable]
    public class ChannelMemberData
    {
        public int id;
        public int channelId;
        public object state;
        public bool isOnline;
        public ChannelMemberMuteData mute;
    }

    [System.Serializable]
    public class ChannelInviteListItemData
    {
        public int channelId;
        public FetchChannelData channel;
        public object playerFrom;
        public object playerTo;
        public string date;
    }

    [System.Serializable]
    public class ChannelJoinRequestItemData
    {
        public int channelId;
        public FetchChannelData channel;
        public object player;
        public string date;
    }

    [System.Serializable]
    public class MessageData
    {
        public string id;
        public int channelId;
        public int authorId;
        public string text;
        public string[] tags;
        public string createdAt;
    }

    [System.Serializable]
    public class ChannelMessageResultData
    {
        public string id;
        public int channelId;
        public int authorId;
        public string text;
        public string[] tags;
        public object player;
        public string createdAt;
        public object reactions;
        public object playerReactions;
    }

    [System.Serializable]
    public class CancelJoinData
    {
        public int channelId;
        public int playerId;
    }

    [System.Serializable]
    public class MemberLeaveData
    {
        public int channelId;
        public int playerId;
        public string reason;
    }

    [System.Serializable]
    public class MuteData
    {
        public int channelId;
        public int playerId;
        public string unmuteAt;
    }

    [System.Serializable]
    public class UnmuteData
    {
        public int channelId;
        public int playerId;
    }

    [System.Serializable]
    public class InviteData
    {
        public int channelId;
        public int playerFromId;
        public int playerToId;
        public string date;
    }

    [System.Serializable]
    public class CancelInviteData
    {
        public int channelId;
        public int playerFromId;
        public int playerToId;
    }

    [System.Serializable]
    public class RejectInviteData
    {
        public int channelId;
        public int playerFromId;
        public int playerToId;
    }

    [System.Serializable]
    public class RejectJoinRequestData
    {
        public int channelId;
        public int playerId;
    }

    [System.Serializable]
    public class CreateChannelFilter
    {
        public CreateChannelFilter(int Template_ID)
        {
            template = Template_ID.ToString();
        }
        public CreateChannelFilter(string templateId)
        {
            template = templateId;
        }
        public string template;
        public string[] tags;
        public int capacity;
        public string name;
        public string description;
        public bool @private;
        public bool visible;
        public string password;
        public OwnerAcl ownerAcl;
        public MemberAcl memberAcl;
        public GuestAcl guestAcl;
    }

    [System.Serializable]
    public class UpdateChannelFilter
    {
        public UpdateChannelFilter(int Channel_ID)
        {
            channelId = Channel_ID;
        }
        public int channelId;
        public string[] tags;
        public int? capacity;
        public string name;
        public string description;
        public bool? @private;
        public bool? visible;
        public string password;
        public int? ownerId;
        public OwnerAcl ownerAcl;
        public MemberAcl memberAcl;
        public GuestAcl guestAcl;

    }

    [System.Serializable]
    public class FetchChannelsFilter
    {
        public int[] ids;
        public string[] tags;
        public string search;
        public bool onlyJoined = false;
        public bool onlyOwned = false;
        public int limit = 100;
        public int offset = 0;
    }

    [System.Serializable]
    public class FetchMoreChannelsFilter
    {
        public int[] ids;
        public string[] tags;
        public string search;
        public bool onlyJoined = false;
        public bool onlyOwned = false;
        public int limit;
    }

    [System.Serializable]
    public class FetchMembersFilter
    {
        public FetchMembersFilter(int Channel_ID)
        {
            channelId = Channel_ID;
        }
        public int channelId;
        public string search;
        public bool onlyOnline = false;
        public int limit = 100;
        public int offset = 0;
    }

    [System.Serializable]
    public class FetchMoreMembersFilter
    {
        public FetchMoreMembersFilter(int Channel_ID)
        {
            channelId = Channel_ID;
        }
        public int channelId;
        public string search;
        public bool onlyOnline = false;
        public int limit = 100;
    }

    [System.Serializable]
    public class OpenChatQuery
    {
        public int id;
        public string[] tags;
    }

    [System.Serializable]
    public class PlayerChatQuery
    {
        public int playerId;
        public string[] tags;
    }

    [System.Serializable]
    internal class OpenChatOverlayQuery
    {
        public GP_Data channel;
        public GP_Data messages;
        public string[] tags;
    }

    [System.Serializable]
    internal class ProcessTagsQuery
    {
        public string[] tags;
        public int playerId;
    }

    [System.Serializable]
    public class ChannelQuery
    {
        public int channelId;
    }

    [System.Serializable]
    public class ChannelKeyQuery
    {
        public int channelId;
        public string key;
    }

    [System.Serializable]
    public class ChannelStateValueQuery
    {
        public int channelId;
        public string key;
        public object value;
        public int? version;
    }

    [System.Serializable]
    public class ChannelFieldVariantData
    {
        public object value;
        public string name;
    }

    [System.Serializable]
    public class ChannelFieldLimitsData
    {
        public float? min;
        public float? max;
        public bool couldGoOverLimit;
    }

    [System.Serializable]
    public class ChannelFieldIntervalIncrementData
    {
        public string interval;
        public float increment;
    }

    [System.Serializable]
    public class ChannelModelFieldData
    {
        public string name;
        public string key;
        public string type;
        public object defaultValue;
        public bool important;
        public bool @public;
        public bool atomic;
        public ChannelFieldIntervalIncrementData intervalIncrement;
        public ChannelFieldLimitsData limits;
        public ChannelFieldVariantData[] variants;

        public object @default => defaultValue;
    }

    [System.Serializable]
    public class ChannelLocalStateData
    {
        public ChannelModelFieldData[] fields;
        public Dictionary<string, object> state;
        public Dictionary<string, object> versions;

        public ChannelModelFieldData getField(string key)
        {
            if (fields == null || string.IsNullOrEmpty(key))
                return null;

            for (int i = 0; i < fields.Length; i++)
            {
                ChannelModelFieldData field = fields[i];
                if (field != null && field.key == key)
                    return field;
            }

            return null;
        }

        public object get(string key)
        {
            if (state != null && !string.IsNullOrEmpty(key) && state.TryGetValue(key, out object value))
                return value;

            ChannelModelFieldData field = getField(key);
            return field != null ? field.@default : null;
        }

        internal void _set(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (state == null)
                state = new Dictionary<string, object>();

            state[key] = value;
        }
    }

    [System.Serializable]
    public class ChannelStateValueData
    {
        public int channelId;
        public string key;
        public object value;
        public int? version;
    }

    [System.Serializable]
    public class ChannelStateMutationResultData
    {
        public bool success;
        public object value;
    }

    [System.Serializable]
    public class FetchChannelsResultData
    {
        public List<FetchChannelData> items;
        public bool canLoadMore;
    }

    [System.Serializable]
    public class PagedItemsResultData<T>
    {
        public List<T> items;
        public bool canLoadMore;
    }

    [System.Serializable]
    public class FetchMembersResultData : PagedItemsResultData<ChannelMemberData> { }

    [System.Serializable]
    public class FetchInvitesResultData : PagedItemsResultData<ChannelInviteListItemData> { }

    [System.Serializable]
    public class FetchJoinRequestsResultData : PagedItemsResultData<ChannelJoinRequestItemData> { }

    [System.Serializable]
    public class FetchMessagesResultData : PagedItemsResultData<ChannelMessageResultData> { }

    [System.Serializable]
    public class PlayerQuery
    {
        public int playerId;
    }

    [System.Serializable]
    public class ChannelPlayerQuery
    {
        public int channelId;
        public int playerId;
    }

    [System.Serializable]
    public class JoinChannelQuery
    {
        public int channelId;
        public string password;
    }

    [System.Serializable]
    public class MutePlayerQuery
    {
        public int channelId;
        public int playerId;
        public int seconds;
        public string unmuteAt;
    }

    [System.Serializable]
    public class PagingQuery
    {
        public int limit = 50;
        public int offset = 0;
    }

    [System.Serializable]
    public class LimitQuery
    {
        public int limit = 50;
    }

    [System.Serializable]
    public class ChannelPagingQuery
    {
        public int channelId;
        public int limit = 50;
        public int offset = 0;
    }

    [System.Serializable]
    public class ChannelLimitQuery
    {
        public int channelId;
        public int limit = 50;
    }

    [System.Serializable]
    public class SendChannelMessageQuery
    {
        public int channelId;
        public string text;
        public string[] tags;
    }

    [System.Serializable]
    public class SendPlayerMessageQuery
    {
        public int playerId;
        public string text;
        public string[] tags;
    }

    [System.Serializable]
    public class MessageQuery
    {
        public string messageId;
    }

    [System.Serializable]
    public class EditMessageQuery
    {
        public string messageId;
        public string text;
    }

    [System.Serializable]
    public class FetchMessagesQuery
    {
        public int channelId;
        public string[] tags;
        public int limit = 50;
        public int offset = 0;
    }

    [System.Serializable]
    public class FetchPlayerMessagesQuery
    {
        public int playerId;
        public string[] tags;
        public int limit = 50;
        public int offset = 0;
    }

    [System.Serializable]
    public class FetchFeedMessagesQuery
    {
        public int playerId;
        public int authorId;
        public string[] tags;
        public int limit = 50;
        public int offset = 0;
    }

    [System.Serializable]
    public class FetchMoreMessagesQuery
    {
        public int channelId;
        public string[] tags;
        public int limit = 50;
    }

    [System.Serializable]
    public class FetchMorePlayerMessagesQuery
    {
        public int playerId;
        public string[] tags;
        public int limit = 50;
    }

    [System.Serializable]
    public class FetchMoreFeedMessagesQuery
    {
        public int playerId;
        public int authorId;
        public string[] tags;
        public int limit = 50;
    }

    [System.Serializable]
    public class OwnerAcl
    {
        public bool canViewMessages = true;
        public bool canAddMessage = true;
        public bool canEditMessage = true;
        public bool canDeleteMessage = true;
        public bool canViewMembers = true;
        public bool canInvitePlayer = true;
        public bool canKickPlayer = true;
        public bool canAcceptJoinRequest = true;
        public bool canMutePlayer = true;
        public bool canSetValue = false;
        public bool canAddValue = false;
        public bool canSubtractValue = false;
    }

    [System.Serializable]
    public class MemberAcl
    {
        public bool canViewMessages = true;
        public bool canAddMessage = true;
        public bool canEditMessage = true;
        public bool canDeleteMessage = true;
        public bool canViewMembers = true;
        public bool canInvitePlayer = false;
        public bool canKickPlayer = false;
        public bool canAcceptJoinRequest = false;
        public bool canMutePlayer = false;
        public bool canSetValue = false;
        public bool canAddValue = false;
        public bool canSubtractValue = false;
    }

    [System.Serializable]
    public class GuestAcl
    {
        public bool canViewMessages = false;
        public bool canAddMessage = false;
        public bool canEditMessage = false;
        public bool canDeleteMessage = false;
        public bool canViewMembers = false;
        public bool canInvitePlayer = false;
        public bool canKickPlayer = false;
        public bool canAcceptJoinRequest = false;
        public bool canMutePlayer = false;
        public bool canSetValue = false;
        public bool canAddValue = false;
        public bool canSubtractValue = false;
    }
}
