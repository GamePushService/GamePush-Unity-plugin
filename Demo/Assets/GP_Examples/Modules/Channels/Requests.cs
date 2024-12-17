using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GamePush;
using TMPro;
using System;
using Examples.Console;
using UnityEngine.UI;

namespace Examples.Channel.Requests
{
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

    [System.Serializable]
    public class FetchJoinRequestsData
    {
        public PlayerData player; // Публичные поля игрока добавляет сам разработчик, в данном случае все поля Игрока по default
        public string date;
    }


    public class Requests : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputChannelIds;
        [SerializeField] private TMP_InputField _inputPlayerIds;
        [Space(15)]
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _rejectButton;
        [SerializeField] private Button _fetchButton;
        [SerializeField] private Button _fetchMoreButton;
        [SerializeField] private Button _fetchSentButton;
        [SerializeField] private Button _fetchSentMoreButton;

        private void OnEnable()
        {
            _acceptButton.onClick.AddListener(AcceptJoinRequest);
            _rejectButton.onClick.AddListener(RejectJoinRequest);
            _fetchButton.onClick.AddListener(FetchJoinRequest);
            _fetchMoreButton.onClick.AddListener(FetchMoreJoinRequest);
            _fetchSentButton.onClick.AddListener(FetchSentJoinRequest);
            _fetchSentMoreButton.onClick.AddListener(FetchMoreSentJoinRequest);


            GP_Channels.OnAcceptJoinRequest += OnAcceptJoinRequest;
            GP_Channels.OnAcceptJoinRequestError += OnAcceptJoinRequestError;

            GP_Channels.OnRejectJoinRequestSuccess += OnRejectJoinRequestSuccess;
            GP_Channels.OnRejectJoinRequestError += OnRejectJoinRequestError;
            GP_Channels.OnRejectJoinRequestEvent += OnRejectJoinRequestEvent;

            GP_Channels.OnFetchJoinRequests += OnFetchJoinRequests;
            GP_Channels.OnFetchJoinRequestsError += OnFetchJoinRequestsError;

            GP_Channels.OnFetchMoreJoinRequests += OnFetchMoreJoinRequests;
            GP_Channels.OnFetchMoreJoinRequestsError += OnFetchMoreJoinRequestsError;

            GP_Channels.OnFetchSentJoinRequests += OnFetchSentJoinRequests;
            GP_Channels.OnFetchSentJoinRequestsError += OnFetchSentJoinRequestsError;

            GP_Channels.OnFetchMoreSentJoinRequests += OnFetchMoreSentJoinRequests;
            GP_Channels.OnFetchMoreSentJoinRequestsError += OnFetchMoreSentJoinRequestsError;
        }
        private void OnDisable()
        {
            _acceptButton.onClick.RemoveListener(AcceptJoinRequest);
            _rejectButton.onClick.RemoveListener(RejectJoinRequest);
            _fetchButton.onClick.RemoveListener(FetchJoinRequest);
            _fetchMoreButton.onClick.RemoveListener(FetchMoreJoinRequest);
            _fetchSentButton.onClick.RemoveListener(FetchSentJoinRequest);
            _fetchSentMoreButton.onClick.RemoveListener(FetchMoreSentJoinRequest);


            GP_Channels.OnAcceptJoinRequest -= OnAcceptJoinRequest;
            GP_Channels.OnAcceptJoinRequestError -= OnAcceptJoinRequestError;

            GP_Channels.OnRejectJoinRequestSuccess -= OnRejectJoinRequestSuccess;
            GP_Channels.OnRejectJoinRequestError -= OnRejectJoinRequestError;
            GP_Channels.OnRejectJoinRequestEvent -= OnRejectJoinRequestEvent;

            GP_Channels.OnFetchJoinRequests -= OnFetchJoinRequests;
            GP_Channels.OnFetchJoinRequestsError -= OnFetchJoinRequestsError;

            GP_Channels.OnFetchMoreJoinRequests -= OnFetchMoreJoinRequests;
            GP_Channels.OnFetchMoreJoinRequestsError -= OnFetchMoreJoinRequestsError;

            GP_Channels.OnFetchSentJoinRequests -= OnFetchSentJoinRequests;
            GP_Channels.OnFetchSentJoinRequestsError -= OnFetchSentJoinRequestsError;

            GP_Channels.OnFetchMoreSentJoinRequests -= OnFetchMoreSentJoinRequests;
            GP_Channels.OnFetchMoreSentJoinRequestsError -= OnFetchMoreSentJoinRequestsError;
        }


        public void AcceptJoinRequest() => GP_Channels.AcceptJoinRequest(int.Parse(_inputChannelIds.text), int.Parse(_inputPlayerIds.text));
        public void RejectJoinRequest() => GP_Channels.RejectJoinRequest(int.Parse(_inputChannelIds.text), int.Parse(_inputPlayerIds.text));
        public void FetchJoinRequest() => GP_Channels.FetchJoinRequests(int.Parse(_inputChannelIds.text), 10);
        public void FetchMoreJoinRequest() => GP_Channels.FetchMoreJoinRequests(int.Parse(_inputChannelIds.text), 10);
        public void FetchSentJoinRequest() => GP_Channels.FetchSentJoinRequests(10);
        public void FetchMoreSentJoinRequest() => GP_Channels.FetchMoreSentJoinRequests(10);



        private void OnAcceptJoinRequest() => ConsoleUI.Instance.Log("ACCEPT JOIN REQUEST: SUCCESS");
        private void OnAcceptJoinRequestError() => ConsoleUI.Instance.Log("ACCEPT JOIN REQUEST: ERROR");


        private void OnRejectJoinRequestSuccess() => ConsoleUI.Instance.Log("REJECT JOIN REQUESTS: SUCCESS");
        private void OnRejectJoinRequestError() => ConsoleUI.Instance.Log("REJECT JOIN REQUESTS: ERROR");
        private void OnRejectJoinRequestEvent(RejectJoinRequestData data)
        {
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("REJECT JOIN REQUEST EVENT: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("REJECT JOIN REQUEST EVENT: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnFetchJoinRequestsError() => ConsoleUI.Instance.Log("FETCH JOIN REQUESTS: ERROR");
        private void OnFetchJoinRequests(GP_Data data, bool canLoadMore)
        {
            var fetchJoinRequestsData = data.GetList<FetchJoinRequestsData>();

            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("FETCH JOIN REQUESTS: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < fetchJoinRequestsData.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH JOIN REQUESTS: DATE: " + fetchJoinRequestsData[i].date);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH JOIN REQUESTS: PLAYER AVATAR: " + fetchJoinRequestsData[i].player.avatar);
                ConsoleUI.Instance.Log("FETCH JOIN REQUESTS: PLAYER CREDITIALS: " + fetchJoinRequestsData[i].player.credentials);
                ConsoleUI.Instance.Log("FETCH JOIN REQUESTS: PLAYER ID: " + fetchJoinRequestsData[i].player.id);
                ConsoleUI.Instance.Log("FETCH JOIN REQUESTS: PLAYER NAME: " + fetchJoinRequestsData[i].player.name);
                ConsoleUI.Instance.Log("FETCH JOIN REQUESTS: PLAYER PLATFORM TYPE: " + fetchJoinRequestsData[i].player.platformType);
                ConsoleUI.Instance.Log("FETCH JOIN REQUESTS: PLAYER PROJECT ID: " + fetchJoinRequestsData[i].player.projectId);
                ConsoleUI.Instance.Log("FETCH JOIN REQUESTS: PLAYER SCORE: " + fetchJoinRequestsData[i].player.score);
                ConsoleUI.Instance.Log(" ");
            }

        }


        private void OnFetchMoreJoinRequestsError() => ConsoleUI.Instance.Log("FETCH MORE JOIN REQUESTS: ERROR");
        private void OnFetchMoreJoinRequests(GP_Data data, bool canLoadMore)
        {
            var fetchJoinRequestsData = data.GetList<FetchJoinRequestsData>();

            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("FETCH MORE JOIN REQUESTS: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < fetchJoinRequestsData.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE JOIN REQUESTS: DATE: " + fetchJoinRequestsData[i].date);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE JOIN REQUESTS: PLAYER AVATAR: " + fetchJoinRequestsData[i].player.avatar);
                ConsoleUI.Instance.Log("FETCH MORE JOIN REQUESTS: PLAYER CREDITIALS: " + fetchJoinRequestsData[i].player.credentials);
                ConsoleUI.Instance.Log("FETCH MORE JOIN REQUESTS: PLAYER ID: " + fetchJoinRequestsData[i].player.id);
                ConsoleUI.Instance.Log("FETCH MORE JOIN REQUESTS: PLAYER NAME: " + fetchJoinRequestsData[i].player.name);
                ConsoleUI.Instance.Log("FETCH MORE JOIN REQUESTS: PLAYER PLATFORM TYPE: " + fetchJoinRequestsData[i].player.platformType);
                ConsoleUI.Instance.Log("FETCH MORE JOIN REQUESTS: PLAYER PROJECT ID: " + fetchJoinRequestsData[i].player.projectId);
                ConsoleUI.Instance.Log("FETCH MORE JOIN REQUESTS: PLAYER SCORE: " + fetchJoinRequestsData[i].player.score);
                ConsoleUI.Instance.Log(" ");
            }
        }



        private void OnFetchSentJoinRequestsError() => ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: ERROR");
        private void OnFetchSentJoinRequests(List<JoinRequestsData> data, bool canLoadMore)
        {
            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < data.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: DATE: " + data[i].date);

                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: ID: " + data[i].channel.id);

                for (int x = 0; x < data[i].channel.tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: TAGS: " + data[i].channel.tags[x]);
                }

                for (int a = 0; a < data[i].channel.messageTags.Length; a++)
                {
                    ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: MESSAGE TAGS: " + data[i].channel.messageTags[a]);
                }

                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: TEMPLATE ID: " + data[i].channel.templateId);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: PROJECT ID: " + data[i].channel.projectId);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: CAPACITY: " + data[i].channel.capacity);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: OWNER ID: " + data[i].channel.ownerId);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: NAME: " + data[i].channel.name);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: DESCRIPTION: " + data[i].channel.description);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: PRIVATE: " + data[i].channel.ch_private);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: VISIBLE: " + data[i].channel.visible);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: PERMANENT: " + data[i].channel.permanent);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: HAS PASSWORD: " + data[i].channel.hasPassword);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: PASSWORD: " + data[i].channel.password);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: IS JOINED: " + data[i].channel.isJoined);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: IS INVITED: " + data[i].channel.isInvited);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: IS MUTED: " + data[i].channel.isMuted);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: IS REQUEST SENT: " + data[i].channel.isRequestSent);
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: MEMBERS: COUNT: " + data[i].channel.membersCount);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: OWNER ACL: " + JsonUtility.ToJson(data[i].channel.ownerAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: MEMBER ACL: " + JsonUtility.ToJson(data[i].channel.memberAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: GUEST ACL: " + JsonUtility.ToJson(data[i].channel.guestAcl));
                ConsoleUI.Instance.Log(" ");
            }
        }


        private void OnFetchMoreSentJoinRequestsError() => ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: ERROR");
        private void OnFetchMoreSentJoinRequests(List<JoinRequestsData> data, bool canLoadMore)
        {
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CAN LOAD MORE " + canLoadMore);

            for (int i = 0; i < data.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: DATE: " + data[i].date);

                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: ID: " + data[i].channel.id);

                for (int x = 0; x < data[i].channel.tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: TAGS: " + data[i].channel.tags[x]);
                }

                for (int a = 0; a < data[i].channel.messageTags.Length; a++)
                {
                    ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: MESSAGE TAGS: " + data[i].channel.messageTags[a]);
                }

                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: TEMPLATE ID: " + data[i].channel.templateId);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: PROJECT ID: " + data[i].channel.projectId);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: CAPACITY: " + data[i].channel.capacity);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: OWNER ID: " + data[i].channel.ownerId);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: NAME: " + data[i].channel.name);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: DESCRIPTION: " + data[i].channel.description);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: PRIVATE: " + data[i].channel.ch_private);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: VISIBLE: " + data[i].channel.visible);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: PERMANENT: " + data[i].channel.permanent);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: HAS PASSWORD: " + data[i].channel.hasPassword);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: PASSWORD: " + data[i].channel.password);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: IS JOINED: " + data[i].channel.isJoined);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: IS INVITED: " + data[i].channel.isInvited);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: IS MUTED: " + data[i].channel.isMuted);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: IS REQUEST SENT: " + data[i].channel.isRequestSent);
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: MEMBERS: COUNT: " + data[i].channel.membersCount);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: OWNER ACL: " + JsonUtility.ToJson(data[i].channel.ownerAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: MEMBER ACL: " + JsonUtility.ToJson(data[i].channel.memberAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: GUEST ACL: " + JsonUtility.ToJson(data[i].channel.guestAcl));
                ConsoleUI.Instance.Log(" ");
            }
        }
    }
}