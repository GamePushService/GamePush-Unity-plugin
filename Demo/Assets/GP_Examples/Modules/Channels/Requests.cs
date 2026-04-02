using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GamePush;
using TMPro;
using System;
using Examples.Console;
using UnityEngine.UI;
using UnityEngine.Events;

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

    [System.Serializable]
    public class FetchJoinRequestsPayload
    {
        public FetchJoinRequestsData[] items;
        public bool canLoadMore;
    }

    [System.Serializable]
    public class FetchSentJoinRequestsPayload
    {
        public JoinRequestsData[] items;
        public bool canLoadMore;
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


            GP_Channels.on("acceptJoinRequest", (UnityAction<GP_Data>)OnAcceptJoinRequestEvent);
            GP_Channels.on("error:acceptJoinRequest", (UnityAction<GP_Data>)OnAcceptJoinRequestError);

            GP_Channels.on("rejectJoinRequest", (UnityAction<GP_Data>)OnRejectJoinRequestSuccessEvent);
            GP_Channels.on("error:rejectJoinRequest", (UnityAction<GP_Data>)OnRejectJoinRequestError);
            GP_Channels.on("event:rejectJoinRequest", (UnityAction<GP_Data>)OnRejectJoinRequestEvent);

            GP_Channels.on("fetchJoinRequests", (UnityAction<GP_Data>)OnFetchJoinRequests);
            GP_Channels.on("error:fetchJoinRequests", (UnityAction<GP_Data>)OnFetchJoinRequestsError);

            GP_Channels.on("fetchMoreJoinRequests", (UnityAction<GP_Data>)OnFetchMoreJoinRequests);
            GP_Channels.on("error:fetchMoreJoinRequests", (UnityAction<GP_Data>)OnFetchMoreJoinRequestsError);

            GP_Channels.on("fetchSentJoinRequests", (UnityAction<GP_Data>)OnFetchSentJoinRequests);
            GP_Channels.on("error:fetchSentJoinRequests", (UnityAction<GP_Data>)OnFetchSentJoinRequestsError);

            GP_Channels.on("fetchMoreSentJoinRequests", (UnityAction<GP_Data>)OnFetchMoreSentJoinRequests);
            GP_Channels.on("error:fetchMoreSentJoinRequests", (UnityAction<GP_Data>)OnFetchMoreSentJoinRequestsError);
        }
        private void OnDisable()
        {
            _acceptButton.onClick.RemoveListener(AcceptJoinRequest);
            _rejectButton.onClick.RemoveListener(RejectJoinRequest);
            _fetchButton.onClick.RemoveListener(FetchJoinRequest);
            _fetchMoreButton.onClick.RemoveListener(FetchMoreJoinRequest);
            _fetchSentButton.onClick.RemoveListener(FetchSentJoinRequest);
            _fetchSentMoreButton.onClick.RemoveListener(FetchMoreSentJoinRequest);


            GP_Channels.off("acceptJoinRequest", (UnityAction<GP_Data>)OnAcceptJoinRequestEvent);
            GP_Channels.off("error:acceptJoinRequest", (UnityAction<GP_Data>)OnAcceptJoinRequestError);

            GP_Channels.off("rejectJoinRequest", (UnityAction<GP_Data>)OnRejectJoinRequestSuccessEvent);
            GP_Channels.off("error:rejectJoinRequest", (UnityAction<GP_Data>)OnRejectJoinRequestError);
            GP_Channels.off("event:rejectJoinRequest", (UnityAction<GP_Data>)OnRejectJoinRequestEvent);

            GP_Channels.off("fetchJoinRequests", (UnityAction<GP_Data>)OnFetchJoinRequests);
            GP_Channels.off("error:fetchJoinRequests", (UnityAction<GP_Data>)OnFetchJoinRequestsError);

            GP_Channels.off("fetchMoreJoinRequests", (UnityAction<GP_Data>)OnFetchMoreJoinRequests);
            GP_Channels.off("error:fetchMoreJoinRequests", (UnityAction<GP_Data>)OnFetchMoreJoinRequestsError);

            GP_Channels.off("fetchSentJoinRequests", (UnityAction<GP_Data>)OnFetchSentJoinRequests);
            GP_Channels.off("error:fetchSentJoinRequests", (UnityAction<GP_Data>)OnFetchSentJoinRequestsError);

            GP_Channels.off("fetchMoreSentJoinRequests", (UnityAction<GP_Data>)OnFetchMoreSentJoinRequests);
            GP_Channels.off("error:fetchMoreSentJoinRequests", (UnityAction<GP_Data>)OnFetchMoreSentJoinRequestsError);
        }


        public void AcceptJoinRequest() => GP_Channels.acceptJoinRequest(new ChannelPlayerQuery
        {
            channelId = int.Parse(_inputChannelIds.text),
            playerId = int.Parse(_inputPlayerIds.text)
        });

        public void RejectJoinRequest() => GP_Channels.rejectJoinRequest(new ChannelPlayerQuery
        {
            channelId = int.Parse(_inputChannelIds.text),
            playerId = int.Parse(_inputPlayerIds.text)
        });

        public void FetchJoinRequest() => GP_Channels.fetchJoinRequests(new ChannelPagingQuery
        {
            channelId = int.Parse(_inputChannelIds.text),
            limit = 10,
            offset = 0
        });

        public void FetchMoreJoinRequest() => GP_Channels.fetchMoreJoinRequests(new ChannelLimitQuery
        {
            channelId = int.Parse(_inputChannelIds.text),
            limit = 10
        });

        public void FetchSentJoinRequest() => GP_Channels.fetchSentJoinRequests(new PagingQuery
        {
            limit = 10,
            offset = 0
        });

        public void FetchMoreSentJoinRequest() => GP_Channels.fetchMoreSentJoinRequests(new LimitQuery
        {
            limit = 10
        });



        private void OnAcceptJoinRequest() => ConsoleUI.Instance.Log("ACCEPT JOIN REQUEST: SUCCESS");
        private void OnAcceptJoinRequestEvent(GP_Data _) => OnAcceptJoinRequest();
        private void OnAcceptJoinRequestError() => ConsoleUI.Instance.Log("ACCEPT JOIN REQUEST: ERROR");
        private void OnAcceptJoinRequestError(GP_Data _) => OnAcceptJoinRequestError();


        private void OnRejectJoinRequestSuccess() => ConsoleUI.Instance.Log("REJECT JOIN REQUESTS: SUCCESS");
        private void OnRejectJoinRequestSuccessEvent(GP_Data _) => OnRejectJoinRequestSuccess();
        private void OnRejectJoinRequestError() => ConsoleUI.Instance.Log("REJECT JOIN REQUESTS: ERROR");
        private void OnRejectJoinRequestError(GP_Data _) => OnRejectJoinRequestError();
        private void OnRejectJoinRequestEvent(GP_Data payload)
        {
            RejectJoinRequestData data = payload.Get<RejectJoinRequestData>();
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("REJECT JOIN REQUEST EVENT: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("REJECT JOIN REQUEST EVENT: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnFetchJoinRequestsError() => ConsoleUI.Instance.Log("FETCH JOIN REQUESTS: ERROR");
        private void OnFetchJoinRequestsError(GP_Data _) => OnFetchJoinRequestsError();
        private void OnFetchJoinRequests(GP_Data payload)
        {
            FetchJoinRequestsPayload data = payload.Get<FetchJoinRequestsPayload>();
            FetchJoinRequestsData[] fetchJoinRequestsData = data.items ?? new FetchJoinRequestsData[0];

            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("FETCH JOIN REQUESTS: CAN LOAD MORE: " + data.canLoadMore);

            for (int i = 0; i < fetchJoinRequestsData.Length; i++)
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
        private void OnFetchMoreJoinRequestsError(GP_Data _) => OnFetchMoreJoinRequestsError();
        private void OnFetchMoreJoinRequests(GP_Data payload)
        {
            FetchJoinRequestsPayload data = payload.Get<FetchJoinRequestsPayload>();
            FetchJoinRequestsData[] fetchJoinRequestsData = data.items ?? new FetchJoinRequestsData[0];

            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("FETCH MORE JOIN REQUESTS: CAN LOAD MORE: " + data.canLoadMore);

            for (int i = 0; i < fetchJoinRequestsData.Length; i++)
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
        private void OnFetchSentJoinRequestsError(GP_Data _) => OnFetchSentJoinRequestsError();
        private void OnFetchSentJoinRequests(GP_Data payload)
        {
            FetchSentJoinRequestsPayload result = payload.Get<FetchSentJoinRequestsPayload>();
            JoinRequestsData[] data = result.items ?? new JoinRequestsData[0];
            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CAN LOAD MORE: " + result.canLoadMore);

            for (int i = 0; i < data.Length; i++)
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
                ConsoleUI.Instance.Log("FETCH SENT JOIN REQUESTS: CHANNEL: PRIVATE: " + data[i].channel.@private);
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
        private void OnFetchMoreSentJoinRequestsError(GP_Data _) => OnFetchMoreSentJoinRequestsError();
        private void OnFetchMoreSentJoinRequests(GP_Data payload)
        {
            FetchSentJoinRequestsPayload result = payload.Get<FetchSentJoinRequestsPayload>();
            JoinRequestsData[] data = result.items ?? new JoinRequestsData[0];
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CAN LOAD MORE " + result.canLoadMore);

            for (int i = 0; i < data.Length; i++)
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
                ConsoleUI.Instance.Log("FETCH MORE SENT JOIN REQUESTS: CHANNEL: PRIVATE: " + data[i].channel.@private);
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
