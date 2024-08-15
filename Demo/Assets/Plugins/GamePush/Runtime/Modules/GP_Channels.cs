using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Utilities;
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

        #endregion

        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenChat(int channel_ID);
        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenChatWithTags(int channel_ID, string tags);

        public static void OpenChat(int channel_ID, Action onOpen = null, Action onClose = null, Action onOpenError = null)
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

        public static void OpenChat(string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null)
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

        public static void OpenChat(int channel_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null)
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

        public static void OpenChat(Action onOpen = null, Action onClose = null, Action onOpenError = null)
        {
            _onOpenChat = onOpen;
            _onCloseChat = onClose;
            _onOpenChatError = onOpenError;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_OpenChat(-10);
            WebGLInput.captureAllKeyboardInput = false;
#else

            ConsoleLog("OPEN CHAT");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenPersonalChat(int player_ID, string tags);
        public static void OpenPersonalChat(int player_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null)
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

        [DllImport("__Internal")]
        private static extern void GP_Channels_OpenFeed(int player_ID, string tags);
        public static void OpenFeed(int player_ID, string tags, Action onOpen = null, Action onClose = null, Action onOpenError = null)
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

        [DllImport("__Internal")]
        private static extern string GP_Channels_IsMainChatEnabled();
        public static bool IsMainChatEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Channels_IsMainChatEnabled() == "true";
#else

            Console.Log("IS MAIN CHAT ENABLED: TRUE");
            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern int GP_Channels_MainChatId();
        public static int MainChatId()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Channels_MainChatId();
#else

            Console.Log("MAIN CHAT ID: 0");
            return 0;
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Channels_Join(int channel_ID, string password);
        public static void Join(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_Join(channel_ID, "");
#else

            ConsoleLog("JOIN");
#endif
        }
        public static void Join(int channel_ID, string password)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_Join(channel_ID, password);
#else

            ConsoleLog("JOIN");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_CancelJoin(int channel_ID);
        public static void CancelJoin(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_CancelJoin(channel_ID);
#else

            ConsoleLog("CANCEL JOIN");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_Leave(int channel_ID);
        public static void Leave(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_Leave(channel_ID);
#else

            ConsoleLog("LEAVE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_Kick(int channel_ID, int player_ID);
        public static void Kick(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_Kick(channel_ID, player_ID);
#else

            ConsoleLog("KICK");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_Mute_Seconds(int channel_ID, int player_ID, int seconds);
        public static void Mute(int channel_ID, int player_ID, int seconds)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_Mute_Seconds(channel_ID, player_ID, seconds);
#else

            ConsoleLog("MUTE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_Mute_UnmuteAt(int channel_ID, int player_ID, string unmuteAt);
        public static void Mute(int channel_ID, int player_ID, string unmuteAT)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_Mute_UnmuteAt(channel_ID, player_ID, unmuteAT);
#else

            ConsoleLog("MUTE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_UnMute(int channel_ID, int player_ID);
        public static void UnMute(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_UnMute(channel_ID, player_ID);
#else

            ConsoleLog("UNMUTE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_SendInvite(int channel_ID, int player_ID);
        public static void SendInvite(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_SendInvite(channel_ID, player_ID);
#else

            ConsoleLog("SEND INVITE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_CancelInvite(int channel_ID, int player_ID);
        public static void CancelInvite(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_CancelInvite(channel_ID, player_ID);
#else

            ConsoleLog("CANCEL INVITE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_AcceptInvite(int channel_ID);
        public static void AcceptInvite(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_AcceptInvite(channel_ID);
#else

            ConsoleLog("ACCEPT INVITE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_RejectInvite(int channel_ID);
        public static void RejectInvite(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_RejectInvite(channel_ID);
#else

            ConsoleLog("REJECT INVITE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchInvites(int limit, int offset);
        public static void FetchInvites(int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchInvites(limit, offset);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH INVITES");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreInvites(int limit);
        public static void FetchMoreInvites(int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreInvites(limit);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE INVITES");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchChannelInvites(int channel_ID, int limit, int offset);
        public static void FetchChannelInvites(int channel_ID, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchChannelInvites(channel_ID, limit, offset);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH CHANNEL INVITES");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreChannelInvites(int channel_ID, int limit);
        public static void FetchMoreChannelInvites(int channel_ID, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreChannelInvites(channel_ID, limit);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE CHANNEL INVITES");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchSentInvites(int channel_ID, int limit, int offset);
        public static void FetchSentInvites(int channel_ID, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchSentInvites(channel_ID, limit, offset);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH SENT INVITES");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreSentInvites(int channel_ID, int limit);
        public static void FetchMoreSentInvites(int channel_ID, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreSentInvites(channel_ID, limit);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE SENT INVITES");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_AcceptJoinRequest(int channel_ID, int player_ID);
        public static void AcceptJoinRequest(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_AcceptJoinRequest(channel_ID, player_ID);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("ACCEPT JOIN REQUEST");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_RejectJoinRequest(int channel_ID, int player_ID);
        public static void RejectJoinRequest(int channel_ID, int player_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_RejectJoinRequest(channel_ID, player_ID);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("REJECT JOIN REQUEST");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchJoinRequests(int channel_ID, int limit, int offset);
        public static void FetchJoinRequests(int channel_ID, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchJoinRequests(channel_ID, limit, offset);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH JOIN REQUESTS");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreJoinRequests(int channel_ID, int limit);
        public static void FetchMoreJoinRequests(int channel_ID, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreJoinRequests(channel_ID, limit);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE JOIN REQUESTS");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchSentJoinRequests(int limit, int offset);
        public static void FetchSentJoinRequests(int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchSentJoinRequests(limit, offset);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH SENT JOIN REQUESTS");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreSentJoinRequests(int limit);
        public static void FetchMoreSentJoinRequests(int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreSentJoinRequests(limit);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE SENT JOIN REQUESTS");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_SendMessage(int channel_ID, string text, string tags);
        public static void SendMessage(int channel_ID, string text, string tags = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_SendMessage(channel_ID, text, tags);
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("SEND MESSAGE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_SendPersonalMessage(int player_ID, string text, string tags);
        public static void SendPersonalMessage(int player_ID, string text, string tags = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_SendPersonalMessage(player_ID, text, tags);
#else

            ConsoleLog("SEND PERSONAL MESSAGE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_SendFeedMessage(int player_ID, string text, string tags);
        public static void SendFeedMessage(int player_ID, string text, string tags = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_SendFeedMessage(player_ID, text, tags);
#else

            ConsoleLog("SEND FEED MESSAGE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_EditMessage(string message_ID, string text);
        public static void EditMessage(string message_ID, string text)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_EditMessage(message_ID, text);
#else

            ConsoleLog("EDIT MESSAGE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_DeleteMessage(string message_ID);
        public static void DeleteMessage(string message_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_DeleteMessage(message_ID);
#else

            ConsoleLog("DELETE MESSAGE");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMessages(int channel_ID, string tags, int limit, int offset);
        public static void FetchMessages(int channel_ID, string tags, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMessages(channel_ID, tags, limit, offset);
#else

            ConsoleLog("FETCH MESSAGES");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchPersonalMessages(int player_ID, string tags, int limit, int offset);
        public static void FetchPersonalMessages(int player_ID, string tags, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchPersonalMessages(player_ID, tags, limit, offset);
#else

            ConsoleLog("FETCH PERSONAL MESSAGES");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchFeedMessages(int player_ID, string tags, int limit, int offset);
        public static void FetchFeedMessages(int player_ID, string tags, int limit = 50, int offset = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchFeedMessages(player_ID, tags, limit, offset);
#else

            ConsoleLog("FETCH FEED MESSAGES");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreMessages(int channel_ID, string tags, int limit);
        public static void FetchMoreMessages(int channel_ID, string tags, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreMessages(channel_ID, tags, limit);
#else

            ConsoleLog("FETCH MORE MESSAGES");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMorePersonalMessages(int player_ID, string tags, int limit);
        public static void FetchMorePersonalMessages(int player_ID, string tags, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMorePersonalMessages(player_ID, tags, limit);
#else

            ConsoleLog("FETCH MORE PERSONAL MESSAGES");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreFeedMessages(int player_ID, string tags, int limit);
        public static void FetchMoreFeedMessages(int player_ID, string tags, int limit = 50)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreFeedMessages(player_ID, tags, limit);
#else

            ConsoleLog("FETCH MORE FEED MESSAGES");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Channels_DeleteChannel(int channel_ID);
        public static void DeleteChannel(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_DeleteChannel(channel_ID);
#else

            ConsoleLog("DELETE CHANNEL");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchChannel(int channel_ID);
        public static void FetchChannel(int channel_ID)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchChannel(channel_ID);
#else

            ConsoleLog("FETCH CHANNEL");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Channels_CreateChannel(string filter);
        public static void CreateChannel(CreateChannelFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_CreateChannel(JsonUtility.ToJson(filter));
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("CREATE CHANNEL");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Channels_UpdateChannel(string filter);
        public static void UpdateChannel(UpdateChannelFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_UpdateChannel(JsonUtility.ToJson(filter));
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("UPDATE CHANNEL");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchChannels(string filter);
        public static void FetchChannels(FetchChannelsFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchChannels(JsonUtility.ToJson(filter));
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH CHANNELS");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreChannels(string filter);
        public static void FetchMoreChannels(FetchMoreChannelsFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreChannels(JsonUtility.ToJson(filter));
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE CHANNELS");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMembers(string filter);
        public static void FetchMembers(FetchMembersFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMembers(JsonUtility.ToJson(filter));
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MEMBERS");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Channels_FetchMoreMembers(string filter);
        public static void FetchMoreMembers(FetchMoreMembersFilter filter)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Channels_FetchMoreMembers(JsonUtility.ToJson(filter));
#else
            //if (GP_ConsoleController.Instance.ChannelConsoleLogs)
            ConsoleLog("FETCH MORE MEMBERS");
#endif
        }


        #region CALL
        private void CallOnOpenChat() { OnOpenChat?.Invoke(); _onOpenChat?.Invoke(); }
        private void CallOnCloseChat()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = true;
#endif
            OnCloseChat?.Invoke();
            _onCloseChat?.Invoke();
        }
        private void CallOnOpenChatError() { OnOpenChatError?.Invoke(); _onOpenChatError?.Invoke(); }

        private void CallOnCreateChannel(string data) => OnCreateChannel?.Invoke(JsonUtility.FromJson<CreateChannelData>(data));
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
    }
}