using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Utilities;
using GamePush.Native;
using GamePush.Overlays;
using System;

namespace GamePush
{
    public class GP_Channels : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Channels);

        #region Actions

        public static event UnityAction<CreateChannelData> OnCreateChannel;
        public static event UnityAction OnCreateChannelError;

        public static event UnityAction<UpdateChannelData> OnUpdateChannel;
        public static event UnityAction OnUpdateChannelError;

        public static event UnityAction OnDeleteChannelSuccess;
        public static event UnityAction<int> OnDeleteChannelEvent;
        public static event UnityAction OnDeleteChannelError;

        public static event UnityAction<FetchChannelData> OnFetchChannel;
        public static event UnityAction OnFetchChannelError;

        public static event UnityAction<List<FetchChannelData>, bool> OnFetchChannels;
        public static event UnityAction OnFetchChannelsError;

        public static event UnityAction<List<FetchChannelData>, bool> OnFetchMoreChannels;
        public static event UnityAction OnFetchMoreChannelsError;

        public static event UnityAction OnJoinSuccess;
        public static event UnityAction<GP_Data> OnJoinEvent;
        public static event UnityAction OnJoinError;

        public static event UnityAction<GP_Data> OnJoinRequest;

        public static event UnityAction OnCancelJoinSuccess;
        public static event UnityAction<CancelJoinData> OnCancelJoinEvent;
        public static event UnityAction OnCancelJoinError;

        public static event UnityAction OnLeaveSuccess;
        public static event UnityAction<MemberLeaveData> OnLeaveEvent;
        public static event UnityAction OnLeaveError;

        public static event UnityAction OnKick;
        public static event UnityAction OnKickError;

        public static event UnityAction<GP_Data, bool> OnFetchMembers;
        public static event UnityAction OnFetchMembersError;

        public static event UnityAction<GP_Data, bool> OnFetchMoreMembers;
        public static event UnityAction OnFetchMoreMembersError;

        public static event UnityAction OnMuteSuccess;
        public static event UnityAction<MuteData> OnMuteEvent;
        public static event UnityAction OnMuteError;

        public static event UnityAction OnUnmuteSuccess;
        public static event UnityAction<UnmuteData> OnUnmuteEvent;
        public static event UnityAction OnUnmuteError;

        public static event UnityAction OnSendInvite;
        public static event UnityAction OnSendInviteError;

        public static event UnityAction<InviteData> OnInvite;

        public static event UnityAction OnCancelInviteSuccess;
        public static event UnityAction<CancelInviteData> OnCancelInviteEvent;
        public static event UnityAction OnCancelInviteError;

        public static event UnityAction OnAcceptInvite;
        public static event UnityAction OnAcceptInviteError;

        public static event UnityAction OnRejectInviteSuccess;
        public static event UnityAction<RejectInviteData> OnRejectInviteEvent;
        public static event UnityAction OnRejectInviteError;

        public static event UnityAction<GP_Data, bool> OnFetchInvites;
        public static event UnityAction OnFetchInvitesError;

        public static event UnityAction<GP_Data, bool> OnFetchMoreInvites;
        public static event UnityAction OnFetchMoreInvitesError;

        public static event UnityAction<GP_Data, bool> OnFetchChannelInvites;
        public static event UnityAction OnFetchChannelInvitesError;


        public static event UnityAction<GP_Data, bool> OnFetchMoreChannelInvites;
        public static event UnityAction OnFetchMoreChannelInvitesError;

        public static event UnityAction<GP_Data, bool> OnFetchSentInvites;
        public static event UnityAction OnFetchSentInvitesError;

        public static event UnityAction<GP_Data, bool> OnFetchMoreSentInvites;
        public static event UnityAction OnFetchMoreSentInvitesError;

        public static event UnityAction OnAcceptJoinRequest;
        public static event UnityAction OnAcceptJoinRequestError;

        public static event UnityAction OnRejectJoinRequestSuccess;
        public static event UnityAction<RejectJoinRequestData> OnRejectJoinRequestEvent;
        public static event UnityAction OnRejectJoinRequestError;

        public static event UnityAction<GP_Data, bool> OnFetchJoinRequests;
        public static event UnityAction OnFetchJoinRequestsError;

        public static event UnityAction<GP_Data, bool> OnFetchMoreJoinRequests;
        public static event UnityAction OnFetchMoreJoinRequestsError;

        public static event UnityAction<List<JoinRequestsData>, bool> OnFetchSentJoinRequests;
        public static event UnityAction OnFetchSentJoinRequestsError;

        public static event UnityAction<List<JoinRequestsData>, bool> OnFetchMoreSentJoinRequests;
        public static event UnityAction OnFetchMoreSentJoinRequestsError;

        public static event UnityAction<GP_Data> OnSendMessage;
        public static event UnityAction OnSendMessageError;

        public static event UnityAction<GP_Data> OnMessage;

        public static event UnityAction<GP_Data> OnEditMessageSuccess;
        public static event UnityAction<MessageData> OnEditMessageEvent;
        public static event UnityAction OnEditMessageError;

        public static event UnityAction OnDeleteMessageSuccess;
        public static event UnityAction<MessageData> OnDeleteMessageEvent;
        public static event UnityAction OnDeleteMessageError;


        public static event UnityAction<GP_Data, bool> OnFetchMessages;
        public static event UnityAction OnFetchMessagesError;

        public static event UnityAction<GP_Data, bool> OnFetchPersonalMessages;
        public static event UnityAction OnFetchPersonalMessagesError;

        public static event UnityAction<GP_Data, bool> OnFetchFeedMessages;
        public static event UnityAction OnFetchFeedMessagesError;

        public static event UnityAction<GP_Data, bool> OnFetchMoreMessages;
        public static event UnityAction OnFetchMoreMessagesError;

        public static event UnityAction<GP_Data, bool> OnFetchMorePersonalMessages;
        public static event UnityAction OnFetchMorePersonalMessagesError;

        public static event UnityAction<GP_Data, bool> OnFetchMoreFeedMessages;
        public static event UnityAction OnFetchMoreFeedMessagesError;

        public static event UnityAction OnOpenChat;
        public static event UnityAction OnOpenChatError;
        public static event UnityAction OnCloseChat;

        private static event Action _onOpenChat;
        private static event Action _onOpenChatError;
        private static event Action _onCloseChat;

        internal static void NativeFireFetchChannels(List<FetchChannelData> list, bool more) => OnFetchChannels?.Invoke(list, more);
        internal static void NativeFireFetchChannelsError() => OnFetchChannelsError?.Invoke();
        internal static void NativeFireFetchChannel(FetchChannelData data) => OnFetchChannel?.Invoke(data);
        internal static void NativeFireFetchChannelError() => OnFetchChannelError?.Invoke();
        internal static void NativeFireCreateChannel(CreateChannelData data) => OnCreateChannel?.Invoke(data);
        internal static void NativeFireCreateChannelError() => OnCreateChannelError?.Invoke();
        internal static void NativeFireUpdateChannel(UpdateChannelData data) => OnUpdateChannel?.Invoke(data);
        internal static void NativeFireUpdateChannelError() => OnUpdateChannelError?.Invoke();
        internal static void NativeFireJoinSuccess() => OnJoinSuccess?.Invoke();
        internal static void NativeFireJoinError() => OnJoinError?.Invoke();
        internal static void NativeFireLeaveSuccess() => OnLeaveSuccess?.Invoke();
        internal static void NativeFireLeaveError() => OnLeaveError?.Invoke();
        internal static void NativeFireFetchMembers(GP_Data data, bool more) => OnFetchMembers?.Invoke(data, more);
        internal static void NativeFireFetchMembersError() => OnFetchMembersError?.Invoke();

        // Offsets for the FetchMore* calls, which take no offset in the public API.
        private static int _messagesLoaded;
        private static int _personalMessagesLoaded;
        private static int _feedMessagesLoaded;

        internal static void NativeFireFetchMessages(Native.NativeChatPage page, bool more)
        {
            _messagesLoaded = more ? _messagesLoaded + page.items.Count : page.items.Count;
            var data = new GP_Data(page.json);
            if (more)
                OnFetchMoreMessages?.Invoke(data, page.more);
            else
                OnFetchMessages?.Invoke(data, page.more);
        }

        internal static void NativeFireFetchMessagesError(bool more)
        {
            if (more)
                OnFetchMoreMessagesError?.Invoke();
            else
                OnFetchMessagesError?.Invoke();
        }

        internal static void NativeFireFetchPersonalMessages(Native.NativeChatPage page, bool more)
        {
            _personalMessagesLoaded = more ? _personalMessagesLoaded + page.items.Count : page.items.Count;
            var data = new GP_Data(page.json);
            if (more)
                OnFetchMorePersonalMessages?.Invoke(data, page.more);
            else
                OnFetchPersonalMessages?.Invoke(data, page.more);
        }

        internal static void NativeFireFetchPersonalMessagesError(bool more)
        {
            if (more)
                OnFetchMorePersonalMessagesError?.Invoke();
            else
                OnFetchPersonalMessagesError?.Invoke();
        }

        internal static void NativeFireFetchFeedMessages(Native.NativeChatPage page, bool more)
        {
            _feedMessagesLoaded = more ? _feedMessagesLoaded + page.items.Count : page.items.Count;
            var data = new GP_Data(page.json);
            if (more)
                OnFetchMoreFeedMessages?.Invoke(data, page.more);
            else
                OnFetchFeedMessages?.Invoke(data, page.more);
        }

        internal static void NativeFireFetchFeedMessagesError(bool more)
        {
            if (more)
                OnFetchMoreFeedMessagesError?.Invoke();
            else
                OnFetchFeedMessagesError?.Invoke();
        }

        internal static void NativeFireSendMessage(string raw) => OnSendMessage?.Invoke(new GP_Data(raw ?? "{}"));
        internal static void NativeFireSendMessageError() => OnSendMessageError?.Invoke();
        internal static void NativeFireIncomingMessage(string raw) => OnMessage?.Invoke(new GP_Data(raw ?? "{}"));

        internal static void NativeFireEditMessage(string raw)
        {
            OnEditMessageSuccess?.Invoke(new GP_Data(raw ?? "{}"));
            OnEditMessageEvent?.Invoke(UtilityJSON.Get<MessageData>(raw ?? "{}"));
        }

        internal static void NativeFireEditMessageError() => OnEditMessageError?.Invoke();
        internal static void NativeFireDeleteMessage() => OnDeleteMessageSuccess?.Invoke();
        internal static void NativeFireDeleteMessageError() => OnDeleteMessageError?.Invoke();

        internal static void NativeFireDeleteChannel(int channel_ID = 0)
        {
            OnDeleteChannelSuccess?.Invoke();
            OnDeleteChannelEvent?.Invoke(channel_ID);
        }

        internal static void NativeFireDeleteChannelError() => OnDeleteChannelError?.Invoke();

        internal static void NativeFireCancelJoin(int channel_ID)
        {
            OnCancelJoinSuccess?.Invoke();
            OnCancelJoinEvent?.Invoke(new CancelJoinData { channelId = channel_ID });
        }

        internal static void NativeFireCancelJoinError() => OnCancelJoinError?.Invoke();

        internal static void NativeFireKick(int channel_ID, int player_ID)
        {
            OnKick?.Invoke();
            OnLeaveEvent?.Invoke(new MemberLeaveData { channelId = channel_ID, playerId = player_ID });
        }

        internal static void NativeFireKickError() => OnKickError?.Invoke();

        internal static void NativeFireMute(int channel_ID, int player_ID, string unmuteAt)
        {
            OnMuteSuccess?.Invoke();
            OnMuteEvent?.Invoke(new MuteData { channelId = channel_ID, playerId = player_ID, unmuteAt = unmuteAt });
        }

        internal static void NativeFireMuteError() => OnMuteError?.Invoke();

        internal static void NativeFireUnmute(int channel_ID, int player_ID)
        {
            OnUnmuteSuccess?.Invoke();
            OnUnmuteEvent?.Invoke(new UnmuteData { channelId = channel_ID, playerId = player_ID });
        }

        internal static void NativeFireUnmuteError() => OnUnmuteError?.Invoke();
        internal static void NativeFireSendInvite(int channel_ID, int player_ID) => OnSendInvite?.Invoke();
        internal static void NativeFireSendInviteError() => OnSendInviteError?.Invoke();

        internal static void NativeFireCancelInvite(int channel_ID, int player_ID)
        {
            OnCancelInviteSuccess?.Invoke();
            OnCancelInviteEvent?.Invoke(new CancelInviteData { channelId = channel_ID, playerToId = player_ID });
        }

        internal static void NativeFireCancelInviteError() => OnCancelInviteError?.Invoke();
        internal static void NativeFireAcceptInvite(int channel_ID) => OnAcceptInvite?.Invoke();
        internal static void NativeFireAcceptInviteError() => OnAcceptInviteError?.Invoke();

        internal static void NativeFireRejectInvite(int channel_ID)
        {
            OnRejectInviteSuccess?.Invoke();
            OnRejectInviteEvent?.Invoke(new RejectInviteData { channelId = channel_ID });
        }

        internal static void NativeFireRejectInviteError() => OnRejectInviteError?.Invoke();
        internal static void NativeFireAcceptJoinRequest(int channel_ID, int player_ID) => OnAcceptJoinRequest?.Invoke();
        internal static void NativeFireAcceptJoinRequestError() => OnAcceptJoinRequestError?.Invoke();

        internal static void NativeFireRejectJoinRequest(int channel_ID, int player_ID)
        {
            OnRejectJoinRequestSuccess?.Invoke();
            OnRejectJoinRequestEvent?.Invoke(new RejectJoinRequestData { channelId = channel_ID, playerId = player_ID });
        }

        internal static void NativeFireRejectJoinRequestError() => OnRejectJoinRequestError?.Invoke();

        internal static void NativeFireFetchChannelInvites(GP_Data data, bool more)
        {
            if (more)
                OnFetchMoreChannelInvites?.Invoke(data, false);
            else
                OnFetchChannelInvites?.Invoke(data, false);
        }

        internal static void NativeFireFetchChannelInvitesError(bool more)
        {
            if (more)
                OnFetchMoreChannelInvitesError?.Invoke();
            else
                OnFetchChannelInvitesError?.Invoke();
        }

        internal static void NativeFireFetchJoinRequests(GP_Data data, bool more)
        {
            if (more)
                OnFetchMoreJoinRequests?.Invoke(data, false);
            else
                OnFetchJoinRequests?.Invoke(data, false);
        }

        internal static void NativeFireFetchJoinRequestsError(bool more)
        {
            if (more)
                OnFetchMoreJoinRequestsError?.Invoke();
            else
                OnFetchJoinRequestsError?.Invoke();
        }

        internal static void NativeFireOpenChat()
        {
            OnOpenChat?.Invoke();
            _onOpenChat?.Invoke();
        }

        internal static void NativeFireOpenChatError()
        {
            OnOpenChatError?.Invoke();
            _onOpenChatError?.Invoke();
        }

        internal static void NativeFireCloseChat()
        {
            OnCloseChat?.Invoke();
            _onCloseChat?.Invoke();
        }

        #endregion

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenChat(int channel_ID);
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenChatWithTags(int channel_ID, string tags);
        #endif

        public static void OpenChat(int channel_ID, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_OpenChat(channel_ID);
            GP_WebGLInput.Release();
#else
            if (OpenNativeChat(NativeChatScope.Channel, channel_ID, "")) return;
            if (GP_Play2Web.Call("GP_Channels_OpenChat", channel_ID)) return;

            ConsoleLog("OPEN CHAT: CHANNEL ID: " + channel_ID);
            _onOpenChat?.Invoke();
#endif
        }

        private static bool OpenNativeChat(NativeChatScope scope, int target, string tags)
        {
            if (!GamePushHost.UseNativeCore)
                return false;
            // -10 is the module's sentinel for "the project's main chat".
            if (scope == NativeChatScope.Channel && target == -10)
                target = MainChatId();
            if (scope == NativeChatScope.Channel && target <= 0)
                return false;
            return GP_Overlays.Open(GP_OverlayKind.Chat, new GP_ChatArgs
            {
                scope = scope,
                target = target,
                tags = tags ?? ""
            });
        }

        public static void OpenChat(string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_OpenChatWithTags(-10, tags);
            GP_WebGLInput.Release();
#else
            if (OpenNativeChat(NativeChatScope.Channel, -10, tags)) return;
            if (GP_Play2Web.Call("GP_Channels_OpenChatWithTags", -10, tags)) return;

            ConsoleLog("OPEN CHAT");
            _onOpenChat?.Invoke();
#endif
        }

        public static void OpenChat(int channel_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_OpenChatWithTags(channel_ID, tags);
            GP_WebGLInput.Release();
#else
            if (OpenNativeChat(NativeChatScope.Channel, channel_ID, tags)) return;
            if (GP_Play2Web.Call("GP_Channels_OpenChatWithTags", channel_ID, tags)) return;

            ConsoleLog("OPEN CHAT: CHANNEL ID: " + channel_ID);
            _onOpenChat?.Invoke();
#endif
        }

        public static void OpenChat(Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_OpenChat(-10);
            GP_WebGLInput.Release();
#else
            if (OpenNativeChat(NativeChatScope.Channel, -10, "")) return;
            if (GP_Play2Web.Call("GP_Channels_OpenChat", -10)) return;

            ConsoleLog("OPEN CHAT");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenPersonalChat(int player_ID, string tags);
        #endif
        public static void OpenPersonalChat(int player_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_OpenPersonalChat(player_ID, tags);
            GP_WebGLInput.Release();
#else
            if (OpenNativeChat(NativeChatScope.Personal, player_ID, tags)) return;
            if (GP_Play2Web.Call("GP_Channels_OpenPersonalChat", player_ID, tags)) return;

            ConsoleLog("OPEN PERSONAL CHAT: PLAYER ID: " + player_ID);
            _onOpenChat?.Invoke();
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenFeed(int player_ID, string tags);
        #endif
        public static void OpenFeed(int player_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_OpenFeed(player_ID, tags);
            GP_WebGLInput.Release();
#else
            if (OpenNativeChat(NativeChatScope.Feed, player_ID, tags)) return;
            if (GP_Play2Web.Call("GP_Channels_OpenFeed", player_ID, tags)) return;

            ConsoleLog("OPEN FEED: PLAYER ID: " + player_ID);
            _onOpenChat?.Invoke();
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Channels_IsMainChatEnabled();
        #endif
        public static bool IsMainChatEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Channels_IsMainChatEnabled() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeCore.MainChatEnabled;
            if (GP_Play2Web.TryGetBool("GP_Channels_IsMainChatEnabled", out var _gpLive)) return _gpLive;

            Console.Log("IS MAIN CHAT ENABLED: TRUE");
            return true;
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int GP_Channels_MainChatId();
        #endif
        public static int MainChatId()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Channels_MainChatId();
#else
            if (GamePushHost.UseNativeCore)
                return NativeCore.MainChatId;
            if (GP_Play2Web.TryGetInt("GP_Channels_MainChatId", out var _gpLiveInt)) return _gpLiveInt;

            Console.Log("MAIN CHAT ID: 0");
            return 0;
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_Join(int channel_ID, string password);
        #endif
        public static void Join(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_Join(channel_ID, "");
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.Join(channel_ID, "");
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_Join", channel_ID, "")) return;

            ConsoleLog("JOIN");
#endif
        }
        public static void Join(int channel_ID, string password)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_Join(channel_ID, password);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.Join(channel_ID, password);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_Join", channel_ID, password)) return;

            ConsoleLog("JOIN");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_CancelJoin(int channel_ID);
        #endif
        public static void CancelJoin(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_CancelJoin(channel_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.CancelJoin(channel_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_CancelJoin", channel_ID)) return;

            ConsoleLog("CANCEL JOIN");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_Leave(int channel_ID);
        #endif
        public static void Leave(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_Leave(channel_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.Leave(channel_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_Leave", channel_ID)) return;

            ConsoleLog("LEAVE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_Kick(int channel_ID, int player_ID);
        #endif
        public static void Kick(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_Kick(channel_ID, player_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.Kick(channel_ID, player_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_Kick", channel_ID, player_ID)) return;

            ConsoleLog("KICK");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_Mute_Seconds(int channel_ID, int player_ID, int seconds);
        #endif
        public static void Mute(int channel_ID, int player_ID, int seconds)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_Mute_Seconds(channel_ID, player_ID, seconds);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.Mute(channel_ID, player_ID, seconds, null);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_Mute_Seconds", channel_ID, player_ID, seconds)) return;

            ConsoleLog("MUTE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_Mute_UnmuteAt(int channel_ID, int player_ID, string unmuteAt);
        #endif
        public static void Mute(int channel_ID, int player_ID, string unmuteAT)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_Mute_UnmuteAt(channel_ID, player_ID, unmuteAT);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.Mute(channel_ID, player_ID, 0, unmuteAT);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_Mute_UnmuteAt", channel_ID, player_ID, unmuteAT)) return;

            ConsoleLog("MUTE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_UnMute(int channel_ID, int player_ID);
        #endif
        public static void UnMute(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_UnMute(channel_ID, player_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.Unmute(channel_ID, player_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_UnMute", channel_ID, player_ID)) return;

            ConsoleLog("UNMUTE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_SendInvite(int channel_ID, int player_ID);
        #endif
        public static void SendInvite(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_SendInvite(channel_ID, player_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.SendInvite(channel_ID, player_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_SendInvite", channel_ID, player_ID)) return;

            ConsoleLog("SEND INVITE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_CancelInvite(int channel_ID, int player_ID);
        #endif
        public static void CancelInvite(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_CancelInvite(channel_ID, player_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.CancelInvite(channel_ID, player_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_CancelInvite", channel_ID, player_ID)) return;

            ConsoleLog("CANCEL INVITE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_AcceptInvite(int channel_ID);
        #endif
        public static void AcceptInvite(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_AcceptInvite(channel_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.AcceptInvite(channel_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_AcceptInvite", channel_ID)) return;

            ConsoleLog("ACCEPT INVITE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_RejectInvite(int channel_ID);
        #endif
        public static void RejectInvite(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_RejectInvite(channel_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.RejectInvite(channel_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_RejectInvite", channel_ID)) return;

            ConsoleLog("REJECT INVITE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchInvites(int limit, int offset);
        #endif
        public static void FetchInvites(int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchInvites(limit, offset);
#else
            if (GP_Play2Web.Call("GP_Channels_FetchInvites", limit, offset)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH INVITES");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreInvites(int limit);
        #endif
        public static void FetchMoreInvites(int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchMoreInvites(limit);
#else
            if (GP_Play2Web.Call("GP_Channels_FetchMoreInvites", limit)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE INVITES");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchChannelInvites(int channel_ID, int limit, int offset);
        #endif
        public static void FetchChannelInvites(int channel_ID, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchChannelInvites(channel_ID, limit, offset);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.FetchChannelInvites(channel_ID, limit, offset, false);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_FetchChannelInvites", channel_ID, limit, offset)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH CHANNEL INVITES");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreChannelInvites(int channel_ID, int limit);
        #endif
        public static void FetchMoreChannelInvites(int channel_ID, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchMoreChannelInvites(channel_ID, limit);
#else
            if (GP_Play2Web.Call("GP_Channels_FetchMoreChannelInvites", channel_ID, limit)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE CHANNEL INVITES");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchSentInvites(int channel_ID, int limit, int offset);
        #endif
        public static void FetchSentInvites(int channel_ID, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchSentInvites(channel_ID, limit, offset);
#else
            if (GP_Play2Web.Call("GP_Channels_FetchSentInvites", channel_ID, limit, offset)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH SENT INVITES");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreSentInvites(int channel_ID, int limit);
        #endif
        public static void FetchMoreSentInvites(int channel_ID, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchMoreSentInvites(channel_ID, limit);
#else
            if (GP_Play2Web.Call("GP_Channels_FetchMoreSentInvites", channel_ID, limit)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE SENT INVITES");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_AcceptJoinRequest(int channel_ID, int player_ID);
        #endif
        public static void AcceptJoinRequest(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_AcceptJoinRequest(channel_ID, player_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.AcceptJoinRequest(channel_ID, player_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_AcceptJoinRequest", channel_ID, player_ID)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("ACCEPT JOIN REQUEST");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_RejectJoinRequest(int channel_ID, int player_ID);
        #endif
        public static void RejectJoinRequest(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_RejectJoinRequest(channel_ID, player_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.RejectJoinRequest(channel_ID, player_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_RejectJoinRequest", channel_ID, player_ID)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("REJECT JOIN REQUEST");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchJoinRequests(int channel_ID, int limit, int offset);
        #endif
        public static void FetchJoinRequests(int channel_ID, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchJoinRequests(channel_ID, limit, offset);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.FetchJoinRequests(channel_ID, limit, offset, false);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_FetchJoinRequests", channel_ID, limit, offset)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH JOIN REQUESTS");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreJoinRequests(int channel_ID, int limit);
        #endif
        public static void FetchMoreJoinRequests(int channel_ID, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchMoreJoinRequests(channel_ID, limit);
#else
            if (GP_Play2Web.Call("GP_Channels_FetchMoreJoinRequests", channel_ID, limit)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE JOIN REQUESTS");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchSentJoinRequests(int limit, int offset);
        #endif
        public static void FetchSentJoinRequests(int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchSentJoinRequests(limit, offset);
#else
            if (GP_Play2Web.Call("GP_Channels_FetchSentJoinRequests", limit, offset)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH SENT JOIN REQUESTS");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreSentJoinRequests(int limit);
        #endif
        public static void FetchMoreSentJoinRequests(int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchMoreSentJoinRequests(limit);
#else
            if (GP_Play2Web.Call("GP_Channels_FetchMoreSentJoinRequests", limit)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE SENT JOIN REQUESTS");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_SendMessage(int channel_ID, string text, string tags);
        #endif
        public static void SendMessage(int channel_ID, string text, string tags = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_SendMessage(channel_ID, text, tags);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.SendMessage(NativeChatScope.Channel, channel_ID, text, tags);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_SendMessage", channel_ID, text, tags)) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("SEND MESSAGE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_SendPersonalMessage(int player_ID, string text, string tags);
        #endif
        public static void SendPersonalMessage(int player_ID, string text, string tags = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_SendPersonalMessage(player_ID, text, tags);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.SendMessage(NativeChatScope.Personal, player_ID, text, tags);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_SendPersonalMessage", player_ID, text, tags)) return;

            ConsoleLog("SEND PERSONAL MESSAGE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_SendFeedMessage(int player_ID, string text, string tags);
        #endif
        public static void SendFeedMessage(int player_ID, string text, string tags = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_SendFeedMessage(player_ID, text, tags);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.SendMessage(NativeChatScope.Feed, player_ID, text, tags);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_SendFeedMessage", player_ID, text, tags)) return;

            ConsoleLog("SEND FEED MESSAGE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_EditMessage(string message_ID, string text);
        #endif
        public static void EditMessage(string message_ID, string text)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_EditMessage(message_ID, text);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.EditMessage(message_ID, text);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_EditMessage", message_ID, text)) return;

            ConsoleLog("EDIT MESSAGE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_DeleteMessage(string message_ID);
        #endif
        public static void DeleteMessage(string message_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_DeleteMessage(message_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.DeleteMessage(message_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_DeleteMessage", message_ID)) return;

            ConsoleLog("DELETE MESSAGE");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMessages(int channel_ID, string tags, int limit, int offset);
        #endif
        public static void FetchMessages(int channel_ID, string tags, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchMessages(channel_ID, tags, limit, offset);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.FetchMessages(channel_ID, tags, limit, offset, false);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_FetchMessages", channel_ID, tags, limit, offset)) return;

            ConsoleLog("FETCH MESSAGES");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchPersonalMessages(int player_ID, string tags, int limit, int offset);
        #endif
        public static void FetchPersonalMessages(int player_ID, string tags, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchPersonalMessages(player_ID, tags, limit, offset);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.FetchPersonalMessages(player_ID, tags, limit, offset, false);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_FetchPersonalMessages", player_ID, tags, limit, offset)) return;

            ConsoleLog("FETCH PERSONAL MESSAGES");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchFeedMessages(int player_ID, string tags, int limit, int offset);
        #endif
        public static void FetchFeedMessages(int player_ID, string tags, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchFeedMessages(player_ID, tags, limit, offset);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.FetchFeedMessages(player_ID, tags, limit, offset, false);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_FetchFeedMessages", player_ID, tags, limit, offset)) return;

            ConsoleLog("FETCH FEED MESSAGES");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreMessages(int channel_ID, string tags, int limit);
        #endif
        public static void FetchMoreMessages(int channel_ID, string tags, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchMoreMessages(channel_ID, tags, limit);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.FetchMessages(channel_ID, tags, limit, _messagesLoaded, true);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_FetchMoreMessages", channel_ID, tags, limit)) return;

            ConsoleLog("FETCH MORE MESSAGES");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMorePersonalMessages(int player_ID, string tags, int limit);
        #endif
        public static void FetchMorePersonalMessages(int player_ID, string tags, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchMorePersonalMessages(player_ID, tags, limit);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.FetchPersonalMessages(player_ID, tags, limit, _personalMessagesLoaded, true);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_FetchMorePersonalMessages", player_ID, tags, limit)) return;

            ConsoleLog("FETCH MORE PERSONAL MESSAGES");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreFeedMessages(int player_ID, string tags, int limit);
        #endif
        public static void FetchMoreFeedMessages(int player_ID, string tags, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchMoreFeedMessages(player_ID, tags, limit);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.FetchFeedMessages(player_ID, tags, limit, _feedMessagesLoaded, true);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_FetchMoreFeedMessages", player_ID, tags, limit)) return;

            ConsoleLog("FETCH MORE FEED MESSAGES");
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_DeleteChannel(int channel_ID);
        #endif
        public static void DeleteChannel(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_DeleteChannel(channel_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.DeleteChannel(channel_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_DeleteChannel", channel_ID)) return;

            ConsoleLog("DELETE CHANNEL");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchChannel(int channel_ID);
        #endif
        public static void FetchChannel(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchChannel(channel_ID);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.FetchChannel(channel_ID);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_FetchChannel", channel_ID)) return;

            ConsoleLog("FETCH CHANNEL");
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_CreateChannel(string filter);
        #endif
        public static void CreateChannel(CreateChannelFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_CreateChannel(JsonUtility.ToJson(filter));
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.CreateChannel(filter);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_CreateChannel", JsonUtility.ToJson(filter))) return;
            ConsoleLog("CREATE CHANNEL");
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_UpdateChannel(string filter);
        #endif
        public static void UpdateChannel(UpdateChannelFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_UpdateChannel(JsonUtility.ToJson(filter));
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.UpdateChannel(JsonUtility.ToJson(filter));
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_UpdateChannel", JsonUtility.ToJson(filter))) return;
            ConsoleLog("UPDATE CHANNEL");
#endif
        }

        public static void UpdateChannel(GP_Data filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_UpdateChannel(filter?.Data ?? "{}");
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.UpdateChannel(filter?.Data ?? "{}");
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_UpdateChannel", filter?.Data ?? "{}")) return;
            ConsoleLog("UPDATE CHANNEL");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchChannels(string filter);
        #endif
        public static void FetchChannels(FetchChannelsFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchChannels(JsonUtility.ToJson(filter));
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.FetchChannels(filter);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_FetchChannels", JsonUtility.ToJson(filter))) return;
            ConsoleLog("FETCH CHANNELS");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreChannels(string filter);
        #endif
        public static void FetchMoreChannels(FetchMoreChannelsFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchMoreChannels(JsonUtility.ToJson(filter));
#else
            if (GP_Play2Web.Call("GP_Channels_FetchMoreChannels", JsonUtility.ToJson(filter))) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE CHANNELS");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMembers(string filter);
        #endif
        public static void FetchMembers(FetchMembersFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchMembers(JsonUtility.ToJson(filter));
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeChannels.FetchMembers(filter);
                return;
            }
            if (GP_Play2Web.Call("GP_Channels_FetchMembers", JsonUtility.ToJson(filter))) return;
            ConsoleLog("FETCH MEMBERS");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreMembers(string filter);
        #endif
        public static void FetchMoreMembers(FetchMoreMembersFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Channels_FetchMoreMembers(JsonUtility.ToJson(filter));
#else
            if (GP_Play2Web.Call("GP_Channels_FetchMoreMembers", JsonUtility.ToJson(filter))) return;
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE MEMBERS");
#endif
        }


        #region CALL
        private void CallOnOpenChat() { OnOpenChat?.Invoke(); _onOpenChat?.Invoke(); }
        private void CallOnCloseChat()
        {
            GP_WebGLInput.Restore();
            OnCloseChat?.Invoke();
            _onCloseChat?.Invoke();
        }
        private void CallOnOpenChatError()
        {
            GP_WebGLInput.Restore();
            OnOpenChatError?.Invoke();
            _onOpenChatError?.Invoke();
        }

        private void CallOnCreateChannel(string data)
        {
            try
            {
                var channel = JsonUtility.FromJson<CreateChannelData>(data);
                if (channel == null || channel.id <= 0)
                    throw new InvalidOperationException("createChannel returned an invalid channel id");
                OnCreateChannel?.Invoke(channel);
            }
            catch (Exception exception)
            {
                GP_Logger.Error("Channels", "createChannel response parse failed: " + exception.Message + " payload=" + data);
                OnCreateChannelError?.Invoke();
            }
        }
        private void CallOnCreateChannelError() => OnCreateChannelError?.Invoke();


        private void CallOnUpdateChannel(string data) => OnUpdateChannel?.Invoke(JsonUtility.FromJson<UpdateChannelData>(data));
        private void CallOnUpdateChannelError() => OnUpdateChannelError?.Invoke();

        private void CallOnDeleteChannelSuccess() => OnDeleteChannelSuccess?.Invoke();
        private void CallOnDeleteChannelEvent(int channel_ID) => OnDeleteChannelEvent?.Invoke(channel_ID);
        private void CallOnDeleteChannelError() => OnDeleteChannelError?.Invoke();


        private void CallOnFetchChannel(string data) => OnFetchChannel?.Invoke(JsonUtility.FromJson<FetchChannelData>(data));
        private void CallOnFetchChannelError() => OnFetchChannelError?.Invoke();


        private void CallOnFetchChannels(string data) => OnFetchChannels?.Invoke(UtilityJSON.GetList<FetchChannelData>(data), _canLoadMoreFetchChannels);
        private bool _canLoadMoreFetchChannels;
        private void CallOnFetchChannelsCanLoadMore(string canLoadMore) => _canLoadMoreFetchChannels = canLoadMore == "true";
        private void CallOnFetchChannelsError() => OnFetchChannelsError?.Invoke();


        private void CallOnFetchMoreChannels(string data) => OnFetchMoreChannels?.Invoke(UtilityJSON.GetList<FetchChannelData>(data), _canLoadMoreFetchMoreChannels);
        private bool _canLoadMoreFetchMoreChannels;
        private void CallOnFetchMoreChannelsCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreChannels = canLoadMore == "true";
        private void CallOnFetchMoreChannelsError() => OnFetchMoreChannelsError?.Invoke();


        private void CallOnJoinSuccess() => OnJoinSuccess?.Invoke();
        private void CallOnJoinEvent(string data) => OnJoinEvent?.Invoke(new GP_Data(data));
        private void CallOnJoinError() => OnJoinError?.Invoke();


        private void CallOnJoinRequest(string data) => OnJoinRequest?.Invoke(new GP_Data(data));


        private void CallOnCancelJoinSuccess() => OnCancelJoinSuccess?.Invoke();
        private void CallOnCancelJoinEvent(string data) => OnCancelJoinEvent?.Invoke(JsonUtility.FromJson<CancelJoinData>(data));
        private void CallOnCancelJoinError() => OnCancelJoinError?.Invoke();


        private void CallOnLeaveSuccess() => OnLeaveSuccess?.Invoke();
        private void CallOnLeaveEvent(string data) => OnLeaveEvent?.Invoke(JsonUtility.FromJson<MemberLeaveData>(data));
        private void CallOnLeaveError() => OnLeaveError?.Invoke();


        private void CallOnKick() => OnKick?.Invoke();
        private void CallOnKickError() => OnKickError?.Invoke();


        private void CallOnFetchMembers(string data) => OnFetchMembers?.Invoke(new GP_Data(data), _canLoadMoreFetchMembers);
        private bool _canLoadMoreFetchMembers;
        private void CallOnFetchMembersCanLoadMore(string canLoadMore) => _canLoadMoreFetchMembers = canLoadMore == "true";
        private void CallOnFetchMembersError() => OnFetchMembersError?.Invoke();


        private void CallOnFetchMoreMembers(string data) => OnFetchMoreMembers?.Invoke(new GP_Data(data), _canLoadMoreFetchMoreMembers);
        private bool _canLoadMoreFetchMoreMembers;
        private void CallOnFetchMoreMembersCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreMembers = canLoadMore == "true";
        private void CallOnFetchMoreMembersError() => OnFetchMoreMembersError?.Invoke();


        private void CallOnMuteSuccess() => OnMuteSuccess?.Invoke();
        private void CallOnMuteEvent(string data) => OnMuteEvent?.Invoke(JsonUtility.FromJson<MuteData>(data));
        private void CallOnMuteError() => OnMuteError?.Invoke();


        private void CallOnUnmuteSuccess() => OnUnmuteSuccess?.Invoke();
        private void CallOnUnmuteEvent(string data) => OnUnmuteEvent?.Invoke(JsonUtility.FromJson<UnmuteData>(data));
        private void CallOnUnmuteError() => OnUnmuteError?.Invoke();


        private void CallOnSendInvite() => OnSendInvite?.Invoke();
        private void CallOnSendInviteError() => OnSendInviteError?.Invoke();


        private void CallOnInvite(string data) => OnInvite?.Invoke(JsonUtility.FromJson<InviteData>(data));


        private void CallOnCancelInviteSuccess() => OnCancelInviteSuccess?.Invoke();
        private void CallOnCancelInviteEvent(string data) => OnCancelInviteEvent?.Invoke(JsonUtility.FromJson<CancelInviteData>(data));
        private void CallOnCancelInviteError() => OnCancelInviteError?.Invoke();


        private void CallOnAcceptInvite() => OnAcceptInvite?.Invoke();
        private void CallOnAcceptInviteError() => OnAcceptInviteError?.Invoke();


        private void CallOnRejectInviteSuccess() => OnRejectInviteSuccess?.Invoke();
        private void CallOnRejectInviteEvent(string data) => OnRejectInviteEvent?.Invoke(JsonUtility.FromJson<RejectInviteData>(data));
        private void CallOnRejectInviteError() => OnRejectInviteError?.Invoke();


        private void CallOnFetchInvites(string data) => OnFetchInvites?.Invoke(new GP_Data(data), _canLoadMoreFetchInvites);
        private bool _canLoadMoreFetchInvites;
        private void CallOnFetchInvitesCanLoadMore(string canLoadMore) => _canLoadMoreFetchInvites = canLoadMore == "true";
        private void CallOnFetchInvitesError() => OnFetchInvitesError?.Invoke();


        private void CallOnFetchMoreInvites(string data) => OnFetchMoreInvites?.Invoke(new GP_Data(data), _canLoadMoreFetchMoreInvites);
        private bool _canLoadMoreFetchMoreInvites;
        private void CallOnFetchMoreInvitesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreInvites = canLoadMore == "true";
        private void CallOnFetchMoreInvitesError() => OnFetchMoreInvitesError?.Invoke();


        private void CallOnFetchChannelInvites(string data) => OnFetchChannelInvites?.Invoke(new GP_Data(data), _canLoadMoreFetchChannelInvites);
        private bool _canLoadMoreFetchChannelInvites;
        private void CallOnFetchChannelInvitesCanLoadMore(string canLoadMore) => _canLoadMoreFetchChannelInvites = canLoadMore == "true";
        private void CallOnFetchChannelInvitesError() => OnFetchChannelInvitesError?.Invoke();


        private void CallOnFetchMoreChannelInvites(string data) => OnFetchMoreChannelInvites?.Invoke(new GP_Data(data), _canLoadMoreFetchMoreChannelInvites);
        private bool _canLoadMoreFetchMoreChannelInvites;
        private void CallOnFetchMoreChannelInvitesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreChannelInvites = canLoadMore == "true";
        private void CallOnFetchMoreChannelInvitesError() => OnFetchMoreChannelInvitesError?.Invoke();


        private void CallOnFetchSentInvites(string data) => OnFetchSentInvites?.Invoke(new GP_Data(data), _canLoadMoreFetchSentInvites);
        private bool _canLoadMoreFetchSentInvites;
        private void CallOnFetchSentInvitesCanLoadMore(string canLoadMore) => _canLoadMoreFetchSentInvites = canLoadMore == "true";
        private void CallOnFetchSentInvitesError() => OnFetchSentInvitesError?.Invoke();


        private void CallOnFetchMoreSentInvites(string data) => OnFetchMoreSentInvites?.Invoke(new GP_Data(data), _canLoadMoreFetchMoreSentInvites);
        private bool _canLoadMoreFetchMoreSentInvites;
        private void CallOnFetchMoreSentInvitesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreSentInvites = canLoadMore == "true";
        private void CallOnFetchMoreSentInvitesError() => OnFetchMoreSentInvitesError?.Invoke();


        private void CallOnAcceptJoinRequest() => OnAcceptJoinRequest?.Invoke();
        private void CallOnAcceptJoinRequestError() => OnAcceptJoinRequestError?.Invoke();


        private void CallOnRejectJoinRequestSuccess() => OnRejectJoinRequestSuccess?.Invoke();
        private void CallOnRejectJoinRequestEvent(string data) => OnRejectJoinRequestEvent?.Invoke(JsonUtility.FromJson<RejectJoinRequestData>(data));
        private void CallOnRejectJoinRequestError() => OnRejectJoinRequestError?.Invoke();


        private void CallOnFetchJoinRequests(string data) => OnFetchJoinRequests?.Invoke(new GP_Data(data), _canLoadMoreFetchJoinRequests);
        private bool _canLoadMoreFetchJoinRequests;
        private void CallOnFetchJoinRequestsCanLoadMore(string canLoadMore) => _canLoadMoreFetchJoinRequests = canLoadMore == "true";
        private void CallOnFetchJoinRequestsError() => OnFetchJoinRequestsError?.Invoke();


        private void CallOnFetchMoreJoinRequests(string data) => OnFetchMoreJoinRequests?.Invoke(new GP_Data(data), _canLoadMoreFetchMoreJoinRequests);
        private bool _canLoadMoreFetchMoreJoinRequests;
        private void CallOnFetchMoreJoinRequestsCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreJoinRequests = canLoadMore == "true";
        private void CallOnFetchMoreJoinRequestsError() => OnFetchMoreJoinRequestsError?.Invoke();


        private void CallOnFetchSentJoinRequests(string data) => OnFetchSentJoinRequests?.Invoke(UtilityJSON.GetList<JoinRequestsData>(data), _canLoadMoreFetchSentJoinRequests);
        private bool _canLoadMoreFetchSentJoinRequests;
        private void CallOnFetchSentJoinRequestsCanLoadMore(string canLoadMore) => _canLoadMoreFetchSentJoinRequests = canLoadMore == "true";
        private void CallOnFetchSentJoinRequestsError() => OnFetchSentJoinRequestsError?.Invoke();


        private void CallOnFetchMoreSentJoinRequests(string data) => OnFetchMoreSentJoinRequests?.Invoke(UtilityJSON.GetList<JoinRequestsData>(data), _canLoadMoreFetchMoreSentJoinRequests);
        private bool _canLoadMoreFetchMoreSentJoinRequests;
        private void CallOnFetchMoreSentJoinRequestsCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreSentJoinRequests = canLoadMore == "true";
        private void CallOnFetchMoreSentJoinRequestsError() => OnFetchMoreSentJoinRequestsError?.Invoke();


        private void CallOnSendMessage(string data) => OnSendMessage?.Invoke(new GP_Data(data));
        private void CallOnSendMessageError() => OnSendMessageError?.Invoke();


        private void CallOnMessage(string data) => OnMessage?.Invoke(new GP_Data(data));


        private void CallOnEditMessageSuccess(string data) => OnEditMessageSuccess?.Invoke(new GP_Data(data));
        private void CallOnEditMessageEvent(string data) => OnEditMessageEvent?.Invoke(JsonUtility.FromJson<MessageData>(data));
        private void CallOnEditMessageError() => OnEditMessageError?.Invoke();



        private void CallOnDeleteMessageSuccess() => OnDeleteMessageSuccess?.Invoke();
        private void CallOnDeleteMessageEvent(string data) => OnDeleteMessageEvent?.Invoke(JsonUtility.FromJson<MessageData>(data));
        private void CallOnDeleteMessageError() => OnDeleteMessageError?.Invoke();



        private void CallOnFetchMessages(string data) => OnFetchMessages?.Invoke(new GP_Data(data), _canLoadMoreFetchMessages);
        private bool _canLoadMoreFetchMessages;
        private void CallOnFetchMessagesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMessages = canLoadMore == "true";
        private void CallOnFetchMessagesError() => OnFetchMessagesError?.Invoke();

        private void CallOnFetchPersonalMessages(string data) => OnFetchPersonalMessages?.Invoke(new GP_Data(data), _canLoadMoreFetchMessages);
        private bool _canLoadMoreFetchPersonalMessages;
        private void CallOnFetchPersonalMessagesCanLoadMore(string canLoadMore) => _canLoadMoreFetchPersonalMessages = canLoadMore == "true";
        private void CallOnFetchPersonalMessagesError() => OnFetchPersonalMessagesError?.Invoke();


        private void CallOnFetchFeedMessages(string data) => OnFetchFeedMessages?.Invoke(new GP_Data(data), _canLoadMoreFetchMessages);
        private bool _canLoadMoreFetchFeedMessages;
        private void CallOnFetchFeedMessagesCanLoadMore(string canLoadMore) => _canLoadMoreFetchFeedMessages = canLoadMore == "true";
        private void CallOnFetchFeedMessagesError() => OnFetchFeedMessagesError?.Invoke();



        private void CallOnFetchMoreMessages(string data) => OnFetchMoreMessages?.Invoke(new GP_Data(data), _canLoadMoreFetchMoreMessages);
        private bool _canLoadMoreFetchMoreMessages;
        private void CallOnFetchMoreMessagesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreMessages = canLoadMore == "true";
        private void CallOnFetchMoreMessagesError() => OnFetchMoreMessagesError?.Invoke();


        private void CallOnFetchMorePersonalMessages(string data) => OnFetchMorePersonalMessages?.Invoke(new GP_Data(data), _canLoadMoreFetchMoreMessages);
        private bool _canLoadMoreFetchMorePersonalMessages;
        private void CallOnFetchMorePersonalMessagesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMorePersonalMessages = canLoadMore == "true";
        private void CallOnFetchMorePersonalMessagesError() => OnFetchMorePersonalMessagesError?.Invoke();


        private void CallOnFetchMoreFeedMessages(string data) => OnFetchMoreFeedMessages?.Invoke(new GP_Data(data), _canLoadMoreFetchMoreMessages);
        private bool _canLoadMoreFetchMoreFeedMessages;
        private void CallOnFetchMoreFeedMessagesCanLoadMore(string canLoadMore) => _canLoadMoreFetchMoreFeedMessages = canLoadMore == "true";
        private void CallOnFetchMoreFeedMessagesError() => OnFetchMoreFeedMessagesError?.Invoke();
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
        public bool ch_private;
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
        public bool ch_private;
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
        public bool ch_private;
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
            template = Template_ID;
        }
        public int template;
        public string[] tags;
        public int capacity;
        public string name;
        public string description;
        public bool ch_private;
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
        public int capacity;
        public string name;
        public string description;
        public bool ch_private;
        public bool visible;
        public string password;
        public int ownerId;
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
        public bool canSetValue = true;
        public bool canAddValue = true;
        public bool canSubtractValue = true;
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
