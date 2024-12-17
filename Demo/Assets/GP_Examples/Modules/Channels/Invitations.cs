using System.Collections.Generic;
using UnityEngine;

using GamePush;
using Examples.Console;
using TMPro;
using UnityEngine.UI;
using System;

namespace Examples.Channel.Invitations
{
    [System.Serializable]
    public class FetchInvitesData
    {
        public FetchChannelData channel;
        public PlayerData playerFrom; // Публичные поля игрока добавляет сам разработчик, в данном случае все поля Игрока по default
        public string date;
    }

    [System.Serializable]
    public class FetchSentInvitesData
    {
        public FetchChannelData channel;
        public PlayerData playerTo; // Публичные поля игрока добавляет сам разработчик, в данном случае все поля Игрока по default
        public string date;
    }

    [System.Serializable]
    public class FetchChannelInvitesData
    {
        public PlayerData playerFrom; // Публичные поля игрока добавляет сам разработчик, в данном случае все поля Игрока по default
        public PlayerData playerTo; // Публичные поля игрока добавляет сам разработчик, в данном случае все поля Игрока по default
        public string date;
    }

    [System.Serializable]
    public class PlayerData
    {
        // Добавлять глобальные значения которые создали в панели "Игроки" на сайте GamePush
        public string avatar;
        public string credentials;
        public int id;
        public string name;
        public string platformType;
        public int projectId;
        public int score;
    }

    public class Invitations : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputChannelIds;
        [SerializeField] private TMP_InputField _inputPlayerIds;
        [Space(15)]
        [SerializeField] private Button _sendInviteButton;
        [SerializeField] private Button _cancelInviteButton;
        [SerializeField] private Button _acceptInviteButton;
        [SerializeField] private Button _rejectInviteButton;
        [SerializeField] private Button _fetchInvitesButton;
        [SerializeField] private Button _fetchMoreInvitesButton;
        [SerializeField] private Button _fetchChannelInvitesButton;
        [SerializeField] private Button _fetchMoreChannelInvitesButton;


        [SerializeField] private Button _fetchSentInvitesButton;
        [SerializeField] private Button _fetchMoreSentInvitesButton;


        private void OnEnable()
        {
            _sendInviteButton.onClick.AddListener(SendInvite);
            _cancelInviteButton.onClick.AddListener(CancelInvite);
            _acceptInviteButton.onClick.AddListener(AcceptInvite);
            _rejectInviteButton.onClick.AddListener(RejectInvite);
            _fetchInvitesButton.onClick.AddListener(FetchInvites);
            _fetchMoreInvitesButton.onClick.AddListener(FetchMoreInvites);
            _fetchChannelInvitesButton.onClick.AddListener(FetchChannelInvites);
            _fetchMoreChannelInvitesButton.onClick.AddListener(FetchChannelMoreInvites);
            _fetchSentInvitesButton.onClick.AddListener(FetchSentInvites);
            _fetchMoreSentInvitesButton.onClick.AddListener(FetchMoreSentInvites);


            GP_Channels.OnSendInvite += OnSendInvite;
            GP_Channels.OnSendInviteError += OnSendInviteError;

            GP_Channels.OnCancelInviteError += OnCancelInviteError;
            GP_Channels.OnCancelInviteEvent += OnCancelInviteEvent;
            GP_Channels.OnCancelInviteSuccess += OnCancelInviteSuccess;

            GP_Channels.OnAcceptInvite += OnAcceptInvite;
            GP_Channels.OnAcceptInviteError += OnAcceptInviteError;

            GP_Channels.OnRejectInviteError += OnRejectInviteError;
            GP_Channels.OnRejectInviteEvent += OnRejectInviteEvent;
            GP_Channels.OnRejectInviteSuccess += OnRejectInviteSuccess;

            GP_Channels.OnFetchInvites += OnFetchInvites;
            GP_Channels.OnFetchInvitesError += OnFetchInvitesError;

            GP_Channels.OnFetchMoreInvites += OnFetchMoreInvites;
            GP_Channels.OnFetchMoreInvitesError += OnFetchMoreInvitesError;

            GP_Channels.OnFetchChannelInvites += OnFetchChannelInvites;
            GP_Channels.OnFetchChannelInvitesError += OnFetchChannelInvitesError;

            GP_Channels.OnFetchMoreChannelInvites += OnFetchMoreChannelInvites;
            GP_Channels.OnFetchMoreChannelInvitesError += OnFetchMoreChannelInvitesError;

            GP_Channels.OnFetchSentInvites += OnFetchSentInvites;
            GP_Channels.OnFetchSentInvitesError += OnFetchSentInvitesError;
            GP_Channels.OnFetchMoreSentInvites += OnFetchMoreSentInvites;
            GP_Channels.OnFetchMoreSentInvitesError += OnFetchMoreSentInvitesError;
        }

        private void OnDisable()
        {
            _sendInviteButton.onClick.RemoveListener(SendInvite);
            _cancelInviteButton.onClick.RemoveListener(CancelInvite);
            _acceptInviteButton.onClick.RemoveListener(AcceptInvite);
            _rejectInviteButton.onClick.RemoveListener(RejectInvite);
            _fetchInvitesButton.onClick.RemoveListener(FetchInvites);
            _fetchMoreInvitesButton.onClick.RemoveListener(FetchMoreInvites);
            _fetchChannelInvitesButton.onClick.RemoveListener(FetchChannelInvites);
            _fetchMoreChannelInvitesButton.onClick.RemoveListener(FetchChannelMoreInvites);
            _fetchSentInvitesButton.onClick.RemoveListener(FetchSentInvites);
            _fetchMoreSentInvitesButton.onClick.RemoveListener(FetchMoreSentInvites);


            GP_Channels.OnSendInvite -= OnSendInvite;
            GP_Channels.OnSendInviteError -= OnSendInviteError;

            GP_Channels.OnCancelInviteError -= OnCancelInviteError;
            GP_Channels.OnCancelInviteEvent -= OnCancelInviteEvent;
            GP_Channels.OnCancelInviteSuccess -= OnCancelInviteSuccess;

            GP_Channels.OnAcceptInvite -= OnAcceptInvite;
            GP_Channels.OnAcceptInviteError -= OnAcceptInviteError;

            GP_Channels.OnRejectInviteError -= OnRejectInviteError;
            GP_Channels.OnRejectInviteEvent -= OnRejectInviteEvent;
            GP_Channels.OnRejectInviteSuccess -= OnRejectInviteSuccess;

            GP_Channels.OnFetchInvites -= OnFetchInvites;
            GP_Channels.OnFetchInvitesError -= OnFetchInvitesError;

            GP_Channels.OnFetchMoreInvites -= OnFetchMoreInvites;
            GP_Channels.OnFetchMoreInvitesError -= OnFetchMoreInvitesError;

            GP_Channels.OnFetchChannelInvites -= OnFetchChannelInvites;
            GP_Channels.OnFetchChannelInvitesError -= OnFetchChannelInvitesError;

            GP_Channels.OnFetchMoreChannelInvites -= OnFetchMoreChannelInvites;
            GP_Channels.OnFetchMoreChannelInvitesError -= OnFetchMoreChannelInvitesError;

            GP_Channels.OnFetchSentInvites -= OnFetchSentInvites;
            GP_Channels.OnFetchSentInvitesError -= OnFetchSentInvitesError;
            GP_Channels.OnFetchMoreSentInvites -= OnFetchMoreSentInvites;
            GP_Channels.OnFetchMoreSentInvitesError -= OnFetchMoreSentInvitesError;
        }



        public void SendInvite() => GP_Channels.SendInvite(int.Parse(_inputChannelIds.text), int.Parse(_inputPlayerIds.text));
        public void CancelInvite() => GP_Channels.CancelInvite(int.Parse(_inputChannelIds.text), int.Parse(_inputPlayerIds.text));
        public void AcceptInvite() => GP_Channels.AcceptInvite(int.Parse(_inputChannelIds.text));
        public void RejectInvite() => GP_Channels.RejectInvite(int.Parse(_inputChannelIds.text));

        public void FetchInvites() => GP_Channels.FetchInvites(20, 0);
        public void FetchMoreInvites() => GP_Channels.FetchMoreInvites(20);

        public void FetchChannelInvites() => GP_Channels.FetchChannelInvites(int.Parse(_inputChannelIds.text), 20);
        public void FetchChannelMoreInvites() => GP_Channels.FetchMoreChannelInvites(int.Parse(_inputChannelIds.text), 20);

        public void FetchSentInvites() => GP_Channels.FetchSentInvites(int.Parse(_inputChannelIds.text), 10, 0);
        public void FetchMoreSentInvites() => GP_Channels.FetchMoreSentInvites(int.Parse(_inputChannelIds.text), 10);



        private void OnSendInvite() => ConsoleUI.Instance.Log("ON SEND INVITE");
        private void OnSendInviteError() => ConsoleUI.Instance.Log("SEND INVITE: ERROR");


        private void OnCancelInviteSuccess() => ConsoleUI.Instance.Log("CANCEL INVITE: SUCCESS");
        private void OnCancelInviteError() => ConsoleUI.Instance.Log("CANCEL INVITE: ERROR");
        private void OnCancelInviteEvent(CancelInviteData data)
        {
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("CANCEL INVITE EVENT: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("CANCEL INVITE EVENT: PLAYER FROM: ID: " + data.playerFromId);
            ConsoleUI.Instance.Log("CANCEL INVITE EVENT: PLAYER TO: ID: " + data.playerToId);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnAcceptInvite() => ConsoleUI.Instance.Log("ON ACCEPT INVITE");
        private void OnAcceptInviteError() => ConsoleUI.Instance.Log("ACCEPT INVITE: ERROR");


        private void OnRejectInviteSuccess() => ConsoleUI.Instance.Log("REJECT INVITE: SUCCESS");
        private void OnRejectInviteError() => ConsoleUI.Instance.Log("REJECT INVITE: ERROR");
        private void OnRejectInviteEvent(RejectInviteData data)
        {
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("REJECT INVITE EVENT: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("REJECT INVITE EVENT: PLAYER FROM: ID: " + data.playerFromId);
            ConsoleUI.Instance.Log("REJECT INVITE EVENT: PLAYER TO: ID: " + data.playerToId);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnFetchInvitesError() => ConsoleUI.Instance.Log("FETCH INVITES: ERROR");
        private void OnFetchInvites(GP_Data data, bool canLoadMore)
        {
            var fetchInvitesData = data.GetList<FetchInvitesData>();

            ConsoleUI.Instance.Log("FETCH INVITES: CAN LOAD MORE: " + canLoadMore);


            for (int i = 0; i < fetchInvitesData.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: ID: " + fetchInvitesData[i].channel.id);

                for (int x = 0; x < fetchInvitesData[i].channel.tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: TAGS: " + fetchInvitesData[i].channel.tags[x]);
                }

                for (int a = 0; a < fetchInvitesData[i].channel.messageTags.Length; a++)
                {
                    ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: MESSAGE TAGS: " + fetchInvitesData[i].channel.messageTags[a]);
                }

                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: TEMPLATE ID: " + fetchInvitesData[i].channel.templateId);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: PROJECT ID: " + fetchInvitesData[i].channel.projectId);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: CAPACITY: " + fetchInvitesData[i].channel.capacity);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: OWNER ID: " + fetchInvitesData[i].channel.ownerId);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: NAME: " + fetchInvitesData[i].channel.name);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: DESCRIPTION: " + fetchInvitesData[i].channel.description);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: PRIVATE: " + fetchInvitesData[i].channel.ch_private);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: VISIBLE: " + fetchInvitesData[i].channel.visible);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: PERMANENT: " + fetchInvitesData[i].channel.permanent);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: HAS PASSWORD: " + fetchInvitesData[i].channel.hasPassword);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: PASSWORD: " + fetchInvitesData[i].channel.password);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: IS JOINED: " + fetchInvitesData[i].channel.isJoined);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: IS INVITED: " + fetchInvitesData[i].channel.isInvited);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: IS MUTED: " + fetchInvitesData[i].channel.isMuted);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: IS REQUEST SENT: " + fetchInvitesData[i].channel.isRequestSent);
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: MEMBERS COUNT: " + fetchInvitesData[i].channel.membersCount);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: OWNER ACL: " + JsonUtility.ToJson(fetchInvitesData[i].channel.ownerAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: MEMBER ACL: " + JsonUtility.ToJson(fetchInvitesData[i].channel.memberAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH INVITES: CHANNEL: GUEST ACL: " + JsonUtility.ToJson(fetchInvitesData[i].channel.guestAcl));

                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH INVITES: PLAYER FROM: AVATAR: " + fetchInvitesData[i].playerFrom.avatar);
                ConsoleUI.Instance.Log("FETCH INVITES: PLAYER FROM: CREDITIALS: " + fetchInvitesData[i].playerFrom.credentials);
                ConsoleUI.Instance.Log("FETCH INVITES: PLAYER FROM: ID: " + fetchInvitesData[i].playerFrom.id);
                ConsoleUI.Instance.Log("FETCH INVITES: PLAYER FROM: NAME: " + fetchInvitesData[i].playerFrom.name);
                ConsoleUI.Instance.Log("FETCH INVITES: PLAYER FROM: PLATFORM TYPE: " + fetchInvitesData[i].playerFrom.platformType);
                ConsoleUI.Instance.Log("FETCH INVITES: PLAYER FROM: PROJECT ID: " + fetchInvitesData[i].playerFrom.projectId);
                ConsoleUI.Instance.Log("FETCH INVITES: PLAYER FROM: SCORE: " + fetchInvitesData[i].playerFrom.score);
                ConsoleUI.Instance.Log(" ");
            }
        }

        private void OnFetchMoreInvitesError() => ConsoleUI.Instance.Log("FETCH MORE INVITES: ERROR");

        private void OnFetchMoreInvites(GP_Data data, bool canLoadMore)
        {
            var fetchInvitesData = data.GetList<FetchInvitesData>();

            ConsoleUI.Instance.Log("FETCH MORE INVITES: CAN LOAD MORE: " + canLoadMore);


            for (int i = 0; i < fetchInvitesData.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: ID: " + fetchInvitesData[i].channel.id);

                for (int x = 0; x < fetchInvitesData[i].channel.tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: TAGS: " + fetchInvitesData[i].channel.tags[x]);
                }

                for (int a = 0; a < fetchInvitesData[i].channel.messageTags.Length; a++)
                {
                    ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: MESSAGE TAGS: " + fetchInvitesData[i].channel.messageTags[a]);
                }

                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: TEMPLATE ID: " + fetchInvitesData[i].channel.templateId);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: PROJECT ID: " + fetchInvitesData[i].channel.projectId);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: CAPACITY: " + fetchInvitesData[i].channel.capacity);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: OWNER ID: " + fetchInvitesData[i].channel.ownerId);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: NAME: " + fetchInvitesData[i].channel.name);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: DESCRIPTION: " + fetchInvitesData[i].channel.description);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: PRIVATE: " + fetchInvitesData[i].channel.ch_private);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: VISIBLE: " + fetchInvitesData[i].channel.visible);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: PERMANENT: " + fetchInvitesData[i].channel.permanent);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: HAS PASSWORD: " + fetchInvitesData[i].channel.hasPassword);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: PASSWORD: " + fetchInvitesData[i].channel.password);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: IS JOINED: " + fetchInvitesData[i].channel.isJoined);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: IS INVITED: " + fetchInvitesData[i].channel.isInvited);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: IS MUTED: " + fetchInvitesData[i].channel.isMuted);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: IS REQUEST SENT: " + fetchInvitesData[i].channel.isRequestSent);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: MEMBERS COUNT: " + fetchInvitesData[i].channel.membersCount);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: OWNER ACL: " + JsonUtility.ToJson(fetchInvitesData[i].channel.ownerAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: MEMBER ACL: " + JsonUtility.ToJson(fetchInvitesData[i].channel.memberAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE INVITES: CHANNEL: GUEST ACL: " + JsonUtility.ToJson(fetchInvitesData[i].channel.guestAcl));

                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH MORE INVITES: PLAYER FROM: AVATAR: " + fetchInvitesData[i].playerFrom.avatar);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: PLAYER FROM: CREDITIALS: " + fetchInvitesData[i].playerFrom.credentials);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: PLAYER FROM: ID: " + fetchInvitesData[i].playerFrom.id);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: PLAYER FROM: NAME: " + fetchInvitesData[i].playerFrom.name);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: PLAYER FROM: PLATFORM TYPE: " + fetchInvitesData[i].playerFrom.platformType);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: PLAYER FROM: PROJECT ID: " + fetchInvitesData[i].playerFrom.projectId);
                ConsoleUI.Instance.Log("FETCH MORE INVITES: PLAYER FROM: SCORE: " + fetchInvitesData[i].playerFrom.score);
                ConsoleUI.Instance.Log(" ");
            }
        }


        private void OnFetchChannelInvitesError() => ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: ERROR");
        private void OnFetchChannelInvites(GP_Data data, bool canLoadMore)
        {
            var fetchChannelInvites = data.GetList<FetchChannelInvitesData>();

            ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < fetchChannelInvites.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: DATE: " + fetchChannelInvites[i].date);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER FROM: AVATAR: " + fetchChannelInvites[i].playerFrom.avatar);
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER FROM: CREDITIALS: " + fetchChannelInvites[i].playerFrom.credentials);
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER FROM: ID: " + fetchChannelInvites[i].playerFrom.id);
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER FROM: NAME: " + fetchChannelInvites[i].playerFrom.name);
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER FROM: PLATFORM TYPE: " + fetchChannelInvites[i].playerFrom.platformType);
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER FROM: PROJECT ID: " + fetchChannelInvites[i].playerFrom.projectId);
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER FROM: SCORE: " + fetchChannelInvites[i].playerFrom.score);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER TO: AVATAR: " + fetchChannelInvites[i].playerTo.avatar);
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER TO: CREDITIALS: " + fetchChannelInvites[i].playerTo.credentials);
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER TO: ID: " + fetchChannelInvites[i].playerTo.id);
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER TO: NAME: " + fetchChannelInvites[i].playerTo.name);
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER TO: PLATFORM TYPE: " + fetchChannelInvites[i].playerTo.platformType);
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER TO: PROJECT ID: " + fetchChannelInvites[i].playerTo.projectId);
                ConsoleUI.Instance.Log("FETCH CHANNEL INVITES: PLAYER TO: SCORE: " + fetchChannelInvites[i].playerTo.score);
                ConsoleUI.Instance.Log(" ");
            }
        }

        private void OnFetchMoreChannelInvitesError() => ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: ERROR");
        private void OnFetchMoreChannelInvites(GP_Data data, bool canLoadMore)
        {
            var fetchChannelInvites = data.GetList<FetchChannelInvitesData>();

            ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < fetchChannelInvites.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: DATE: " + fetchChannelInvites[i].date);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER FROM: AVATAR: " + fetchChannelInvites[i].playerFrom.avatar);
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER FROM: CREDITIALS: " + fetchChannelInvites[i].playerFrom.credentials);
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER FROM: ID: " + fetchChannelInvites[i].playerFrom.id);
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER FROM: NAME: " + fetchChannelInvites[i].playerFrom.name);
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER FROM: PLATFORM TYPE: " + fetchChannelInvites[i].playerFrom.platformType);
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER FROM: PROJECT ID: " + fetchChannelInvites[i].playerFrom.projectId);
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER FROM: SCORE: " + fetchChannelInvites[i].playerFrom.score);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER TO: AVATAR: " + fetchChannelInvites[i].playerTo.avatar);
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER TO: CREDITIALS: " + fetchChannelInvites[i].playerTo.credentials);
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER TO: ID: " + fetchChannelInvites[i].playerTo.id);
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER TO: NAME: " + fetchChannelInvites[i].playerTo.name);
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER TO: PLATFORM TYPE: " + fetchChannelInvites[i].playerTo.platformType);
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER TO: PROJECT ID: " + fetchChannelInvites[i].playerTo.projectId);
                ConsoleUI.Instance.Log("FETCH MORE CHANNEL INVITES: PLAYER TO: SCORE: " + fetchChannelInvites[i].playerTo.score);
                ConsoleUI.Instance.Log(" ");
            }
        }


        private void OnFetchSentInvitesError() => ConsoleUI.Instance.Log("FETCH SENT INVITES: ERROR");
        private void OnFetchSentInvites(GP_Data data, bool canLoadMore)
        {
            var fetchSentInvites = data.GetList<FetchSentInvitesData>();

            ConsoleUI.Instance.Log("FETCH SENT INVITES: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < fetchSentInvites.Count; i++)
            {
                ConsoleUI.Instance.Log("FETCH SENT INVITES: DATE: " + fetchSentInvites[i].date);


                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: ID: " + fetchSentInvites[i].channel.id);

                for (int x = 0; x < fetchSentInvites[i].channel.tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: TAGS: " + fetchSentInvites[i].channel.tags[x]);
                }

                for (int a = 0; a < fetchSentInvites[i].channel.messageTags.Length; a++)
                {
                    ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: MESSAGE TAGS: " + fetchSentInvites[i].channel.messageTags[a]);
                }

                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: TEMPLATE ID: " + fetchSentInvites[i].channel.templateId);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: PROJECT ID: " + fetchSentInvites[i].channel.projectId);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: CAPACITY: " + fetchSentInvites[i].channel.capacity);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: OWNER ID: " + fetchSentInvites[i].channel.ownerId);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: NAME: " + fetchSentInvites[i].channel.name);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: DESCRIPTION: " + fetchSentInvites[i].channel.description);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: PRIVATE: " + fetchSentInvites[i].channel.ch_private);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: VISIBLE: " + fetchSentInvites[i].channel.visible);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: PERMANENT: " + fetchSentInvites[i].channel.permanent);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: HAS PASSWORD: " + fetchSentInvites[i].channel.hasPassword);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: PASSWORD: " + fetchSentInvites[i].channel.password);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: IS JOINED: " + fetchSentInvites[i].channel.isJoined);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: IS INVITED: " + fetchSentInvites[i].channel.isInvited);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: IS MUTED: " + fetchSentInvites[i].channel.isMuted);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: IS REQUEST SENT: " + fetchSentInvites[i].channel.isRequestSent);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: MEMBERS COUNT: " + fetchSentInvites[i].channel.membersCount);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: OWNER ACL: " + JsonUtility.ToJson(fetchSentInvites[i].channel.ownerAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: MEMBER ACL: " + JsonUtility.ToJson(fetchSentInvites[i].channel.memberAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH SENT INVITES: CHANNEL: GUEST ACL: " + JsonUtility.ToJson(fetchSentInvites[i].channel.guestAcl));

                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH SENT INVITES: PLAYER TO: AVATAR: " + fetchSentInvites[i].playerTo.avatar);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: PLAYER TO: CREDITIALS: " + fetchSentInvites[i].playerTo.credentials);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: PLAYER TO: ID: " + fetchSentInvites[i].playerTo.id);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: PLAYER TO: NAME: " + fetchSentInvites[i].playerTo.name);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: PLAYER TO: PLATFORM TYPE: " + fetchSentInvites[i].playerTo.platformType);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: PLAYER TO: PROJECT ID: " + fetchSentInvites[i].playerTo.projectId);
                ConsoleUI.Instance.Log("FETCH SENT INVITES: PLAYER TO: SCORE: " + fetchSentInvites[i].playerTo.score);
                ConsoleUI.Instance.Log(" ");
            }
        }

        private void OnFetchMoreSentInvitesError() => ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: ERROR");
        private void OnFetchMoreSentInvites(GP_Data data, bool canLoadMore)
        {
            var fetchSentInvites = data.GetList<FetchSentInvitesData>();

            ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < fetchSentInvites.Count; i++)
            {
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: DATE: " + fetchSentInvites[i].date);


                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: ID: " + fetchSentInvites[i].channel.id);

                for (int x = 0; x < fetchSentInvites[i].channel.tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: TAGS: " + fetchSentInvites[i].channel.tags[x]);
                }

                for (int a = 0; a < fetchSentInvites[i].channel.messageTags.Length; a++)
                {
                    ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: MESSAGE TAGS: " + fetchSentInvites[i].channel.messageTags[a]);
                }

                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: TEMPLATE ID: " + fetchSentInvites[i].channel.templateId);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: PROJECT ID: " + fetchSentInvites[i].channel.projectId);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: CAPACITY: " + fetchSentInvites[i].channel.capacity);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: OWNER ID: " + fetchSentInvites[i].channel.ownerId);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: NAME: " + fetchSentInvites[i].channel.name);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: DESCRIPTION: " + fetchSentInvites[i].channel.description);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: PRIVATE: " + fetchSentInvites[i].channel.ch_private);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: VISIBLE: " + fetchSentInvites[i].channel.visible);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: PERMANENT: " + fetchSentInvites[i].channel.permanent);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: HAS PASSWORD: " + fetchSentInvites[i].channel.hasPassword);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: PASSWORD: " + fetchSentInvites[i].channel.password);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: IS JOINED: " + fetchSentInvites[i].channel.isJoined);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: IS INVITED: " + fetchSentInvites[i].channel.isInvited);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: IS MUTED: " + fetchSentInvites[i].channel.isMuted);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: IS REQUEST SENT: " + fetchSentInvites[i].channel.isRequestSent);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: MEMBERS COUNT: " + fetchSentInvites[i].channel.membersCount);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: OWNER ACL: " + JsonUtility.ToJson(fetchSentInvites[i].channel.ownerAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: MEMBER ACL: " + JsonUtility.ToJson(fetchSentInvites[i].channel.memberAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: CHANNEL: GUEST ACL: " + JsonUtility.ToJson(fetchSentInvites[i].channel.guestAcl));

                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: PLAYER TO: AVATAR: " + fetchSentInvites[i].playerTo.avatar);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: PLAYER TO: CREDITIALS: " + fetchSentInvites[i].playerTo.credentials);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: PLAYER TO: ID: " + fetchSentInvites[i].playerTo.id);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: PLAYER TO: NAME: " + fetchSentInvites[i].playerTo.name);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: PLAYER TO: PLATFORM TYPE: " + fetchSentInvites[i].playerTo.platformType);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: PLAYER TO: PROJECT ID: " + fetchSentInvites[i].playerTo.projectId);
                ConsoleUI.Instance.Log("FETCH MORE SENT INVITES: PLAYER TO: SCORE: " + fetchSentInvites[i].playerTo.score);
                ConsoleUI.Instance.Log(" ");
            }
        }

    }
}