using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

using GamePush;
using Examples.Console;

namespace Examples.Channel.Members
{
    [System.Serializable]
    public class FetchMembersData
    {
        public int id;
        public bool isOnline;
        public PlayerData state; // Публичные поля игрока добавляет сам разработчик, в данном случае все поля Игрока по default
        public Mute mute;
    }

    [System.Serializable]
    public class Mute
    {
        public bool isMuted;
        public string unmuteAt;
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

    [System.Serializable]
    public class JoinData
    {
        public int channelId;
        public int id;
        public PlayerData state; // Публичные поля игрока добавляет сам разработчик, в данном случае все поля Игрока по default
        public Mute mute;
    }

    [System.Serializable]
    public class JoinRequestData
    {
        public int channelId;
        public int playerId;
        public PlayerData player; // Публичные поля игрока добавляет сам разработчик, в данном случае все поля Игрока по default
        public string date;
    }

    [System.Serializable]
    public class FetchMembersPayload
    {
        public FetchMembersData[] items;
        public bool canLoadMore;
    }


    public class Members : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _channelIdInput;
        [SerializeField] private TMP_InputField _playerIdInput;
        [SerializeField] private TMP_InputField _muteTimeInput;
        [Space(15)]
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _cancelJoinButton;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _kickButton;
        [SerializeField] private Button _muteButton;
        [SerializeField] private Button _unmuteButton;
        [SerializeField] private Button _fetchButton;
        [SerializeField] private Button _fetchMoreButton;


        private void OnEnable()
        {
            _joinButton.onClick.AddListener(Join);
            _cancelJoinButton.onClick.AddListener(CancelJoin);
            _leaveButton.onClick.AddListener(Leave);
            _kickButton.onClick.AddListener(Kick);
            _muteButton.onClick.AddListener(Mute);
            _unmuteButton.onClick.AddListener(Unmute);
            _fetchButton.onClick.AddListener(FetchMembers);
            _fetchMoreButton.onClick.AddListener(FetchMoreMembers);


            GP_Channels.on("error:join", (UnityAction<GP_Data>)OnJoinError);
            GP_Channels.on("event:join", (UnityAction<GP_Data>)OnJoinEvent);
            GP_Channels.on("join", (UnityAction<GP_Data>)OnJoinSuccessEvent);

            GP_Channels.on("cancelJoin", (UnityAction<GP_Data>)OnCancelJoinSuccessEvent);
            GP_Channels.on("error:cancelJoin", (UnityAction<GP_Data>)OnCancelJoinError);
            GP_Channels.on("event:cancelJoin", (UnityAction<GP_Data>)OnCancelJoinEvent);

            GP_Channels.on("leave", (UnityAction<GP_Data>)OnLeaveSuccessEvent);
            GP_Channels.on("event:leave", (UnityAction<GP_Data>)OnLeaveEvent);
            GP_Channels.on("error:leave", (UnityAction<GP_Data>)OnLeaveError);

            GP_Channels.on("kick", (UnityAction<GP_Data>)OnKickEvent);
            GP_Channels.on("error:kick", (UnityAction<GP_Data>)OnKickError);

            GP_Channels.on("mute", (UnityAction<GP_Data>)OnMuteSuccessEvent);
            GP_Channels.on("error:mute", (UnityAction<GP_Data>)OnMuteError);
            GP_Channels.on("event:mute", (UnityAction<GP_Data>)OnMuteEvent);

            GP_Channels.on("unmute", (UnityAction<GP_Data>)OnUnmuteSuccessEvent);
            GP_Channels.on("error:unmute", (UnityAction<GP_Data>)OnUnmuteError);
            GP_Channels.on("event:unmute", (UnityAction<GP_Data>)OnUnmuteEvent);

            GP_Channels.on("fetchMembers", (UnityAction<GP_Data>)OnFetchMembers);
            GP_Channels.on("error:fetchMembers", (UnityAction<GP_Data>)OnFetchMembersError);

            GP_Channels.on("fetchMoreMembers", (UnityAction<GP_Data>)OnFetchMoreMembers);
            GP_Channels.on("error:fetchMoreMembers", (UnityAction<GP_Data>)OnFetchMoreMembersError);
        }
        private void OnDisable()
        {
            _joinButton.onClick.RemoveListener(Join);
            _cancelJoinButton.onClick.RemoveListener(CancelJoin);
            _leaveButton.onClick.RemoveListener(Leave);
            _kickButton.onClick.RemoveListener(Kick);
            _muteButton.onClick.RemoveListener(Mute);
            _unmuteButton.onClick.RemoveListener(Unmute);
            _fetchButton.onClick.RemoveListener(FetchMembers);
            _fetchMoreButton.onClick.RemoveListener(FetchMoreMembers);


            GP_Channels.off("error:join", (UnityAction<GP_Data>)OnJoinError);
            GP_Channels.off("event:join", (UnityAction<GP_Data>)OnJoinEvent);
            GP_Channels.off("join", (UnityAction<GP_Data>)OnJoinSuccessEvent);

            GP_Channels.off("cancelJoin", (UnityAction<GP_Data>)OnCancelJoinSuccessEvent);
            GP_Channels.off("error:cancelJoin", (UnityAction<GP_Data>)OnCancelJoinError);
            GP_Channels.off("event:cancelJoin", (UnityAction<GP_Data>)OnCancelJoinEvent);

            GP_Channels.off("leave", (UnityAction<GP_Data>)OnLeaveSuccessEvent);
            GP_Channels.off("event:leave", (UnityAction<GP_Data>)OnLeaveEvent);
            GP_Channels.off("error:leave", (UnityAction<GP_Data>)OnLeaveError);

            GP_Channels.off("kick", (UnityAction<GP_Data>)OnKickEvent);
            GP_Channels.off("error:kick", (UnityAction<GP_Data>)OnKickError);

            GP_Channels.off("error:mute", (UnityAction<GP_Data>)OnMuteError);
            GP_Channels.off("event:mute", (UnityAction<GP_Data>)OnMuteEvent);
            GP_Channels.off("mute", (UnityAction<GP_Data>)OnMuteSuccessEvent);

            GP_Channels.off("error:unmute", (UnityAction<GP_Data>)OnUnmuteError);
            GP_Channels.off("event:unmute", (UnityAction<GP_Data>)OnUnmuteEvent);
            GP_Channels.off("unmute", (UnityAction<GP_Data>)OnUnmuteSuccessEvent);

            GP_Channels.off("fetchMembers", (UnityAction<GP_Data>)OnFetchMembers);
            GP_Channels.off("error:fetchMembers", (UnityAction<GP_Data>)OnFetchMembersError);

            GP_Channels.off("fetchMoreMembers", (UnityAction<GP_Data>)OnFetchMoreMembers);
            GP_Channels.off("error:fetchMoreMembers", (UnityAction<GP_Data>)OnFetchMoreMembersError);
        }


        public void Join() => GP_Channels.join(new JoinChannelQuery
        {
            channelId = int.Parse(_channelIdInput.text),
            password = "12345"
        });

        public void CancelJoin() => GP_Channels.cancelJoin(new ChannelQuery
        {
            channelId = int.Parse(_channelIdInput.text)
        });

        public void Leave() => GP_Channels.leave(new ChannelQuery
        {
            channelId = int.Parse(_channelIdInput.text)
        });

        public void Kick() => GP_Channels.kick(new ChannelPlayerQuery
        {
            channelId = int.Parse(_channelIdInput.text),
            playerId = int.Parse(_playerIdInput.text)
        });

        public void Mute() => GP_Channels.mute(new MutePlayerQuery
        {
            channelId = int.Parse(_channelIdInput.text),
            playerId = int.Parse(_playerIdInput.text),
            seconds = int.Parse(_muteTimeInput.text)
        });

        public void Unmute() => GP_Channels.unmute(new ChannelPlayerQuery
        {
            channelId = int.Parse(_channelIdInput.text),
            playerId = int.Parse(_playerIdInput.text)
        });


        public void FetchMembers()
        {
            var filter = new FetchMembersFilter(int.Parse(_channelIdInput.text));
            filter.limit = 50;
            GP_Channels.fetchMembers(filter);
        }

        public void FetchMoreMembers()
        {
            var filter = new FetchMoreMembersFilter(int.Parse(_channelIdInput.text));
            filter.limit = 50;
            GP_Channels.fetchMoreMembers(filter);
        }

        public void MakeOwner()
        {
            var filter = new UpdateChannelFilter(int.Parse(_channelIdInput.text));
            filter.ownerId = int.Parse(_playerIdInput.text);
            GP_Channels.updateChannel(filter);
        }

        private void OnJoinSuccess() => ConsoleUI.Instance.Log("JOIN: SUCCESS");
        private void OnJoinSuccessEvent(GP_Data _) => OnJoinSuccess();
        private void OnJoinError() => ConsoleUI.Instance.Log("JOIN: ERROR");
        private void OnJoinError(GP_Data _) => OnJoinError();
        private void OnJoinEvent(GP_Data data)
        {
            var joinData = data.Get<JoinData>();
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("JOIN EVENT: CHANNEL ID: " + joinData.channelId);
            ConsoleUI.Instance.Log("JOIN EVENT: ID: " + joinData.id);

            ConsoleUI.Instance.Log("JOIN EVENT: MUTE: IS MUTED: " + joinData.mute.isMuted);
            ConsoleUI.Instance.Log("JOIN EVENT: MUTE: UNMUTE AT: " + joinData.mute.unmuteAt);

            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("JOIN EVENT: PLAYER STATE: AVATAR: " + joinData.state.avatar);
            ConsoleUI.Instance.Log("JOIN EVENT: PLAYER STATE: CREDITIALS: " + joinData.state.credentials);
            ConsoleUI.Instance.Log("JOIN EVENT: PLAYER STATE: ID: " + joinData.state.id);
            ConsoleUI.Instance.Log("JOIN EVENT: PLAYER STATE: NAME: " + joinData.state.name);
            ConsoleUI.Instance.Log("JOIN EVENT: PLAYER STATE: PLATFORM TYPE: " + joinData.state.platformType);
            ConsoleUI.Instance.Log("JOIN EVENT: PLAYER STATE: PROJECT ID: " + joinData.state.projectId);
            ConsoleUI.Instance.Log("JOIN EVENT: PLAYER STATE: SCORE: " + joinData.state.score);
            ConsoleUI.Instance.Log(" ");
        }

        private void OnJoinRequest(GP_Data data)
        {
            var joinRequestData = data.Get<JoinRequestData>();

            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("JOIN REQUEST EVENT: CHANNEL ID: " + joinRequestData.channelId);
            ConsoleUI.Instance.Log("JOIN REQUEST EVENT: ID: " + joinRequestData.playerId);
            ConsoleUI.Instance.Log("JOIN REQUEST EVENT: DATE: " + joinRequestData.date);

            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("JOIN REQUEST EVENT: PLAYER STATE: AVATAR: " + joinRequestData.player.avatar);
            ConsoleUI.Instance.Log("JOIN REQUEST EVENT: PLAYER STATE: CREDITIALS: " + joinRequestData.player.credentials);
            ConsoleUI.Instance.Log("JOIN REQUEST EVENT: PLAYER STATE: ID: " + joinRequestData.player.id);
            ConsoleUI.Instance.Log("JOIN REQUEST EVENT: PLAYER STATE: NAME: " + joinRequestData.player.name);
            ConsoleUI.Instance.Log("JOIN REQUEST EVENT: PLAYER STATE: PLATFORM TYPE: " + joinRequestData.player.platformType);
            ConsoleUI.Instance.Log("JOIN REQUEST EVENT: PLAYER STATE: PROJECT ID: " + joinRequestData.player.projectId);
            ConsoleUI.Instance.Log("JOIN REQUEST EVENT: PLAYER STATE: SCORE: " + joinRequestData.player.score);
            ConsoleUI.Instance.Log(" ");
        }

        private void OnCancelJoinError() => ConsoleUI.Instance.Log("CANCEL JOIN: ERROR");
        private void OnCancelJoinSuccess() => ConsoleUI.Instance.Log("CANCEL JOIN: SUCCESS");
        private void OnCancelJoinSuccessEvent(GP_Data _) => OnCancelJoinSuccess();
        private void OnCancelJoinError(GP_Data _) => OnCancelJoinError();
        private void OnCancelJoinEvent(GP_Data payload)
        {
            CancelJoinData data = payload.Get<CancelJoinData>();
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("CANCEL JOIN EVENT: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("CANCEL JOIN EVENT: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnLeaveSuccess() => ConsoleUI.Instance.Log("ON LEAVE CHANNEL: SUCCESS");
        private void OnLeaveSuccessEvent(GP_Data _) => OnLeaveSuccess();
        private void OnLeaveError() => ConsoleUI.Instance.Log("ON LEAVE CHANNEL: ERROR");
        private void OnLeaveError(GP_Data _) => OnLeaveError();
        private void OnLeaveEvent(GP_Data payload)
        {
            MemberLeaveData data = payload.Get<MemberLeaveData>();
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("ON LEAVE CHANNEL: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("ON LEAVE CHANNEL: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log("ON LEAVE CHANNEL: REASON: " + data.reason);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnKick() => ConsoleUI.Instance.Log("KICK: SUCCESS");
        private void OnKickEvent(GP_Data _) => OnKick();
        private void OnKickError() => ConsoleUI.Instance.Log("KICK: ERROR");
        private void OnKickError(GP_Data _) => OnKickError();


        private void OnUnmuteSuccess() => ConsoleUI.Instance.Log("UNMUTE: SUCCESS");
        private void OnUnmuteSuccessEvent(GP_Data _) => OnUnmuteSuccess();
        private void OnUnmuteError() => ConsoleUI.Instance.Log("UNMUTE: ERROR");
        private void OnUnmuteError(GP_Data _) => OnUnmuteError();
        private void OnUnmuteEvent(GP_Data payload)
        {
            UnmuteData data = payload.Get<UnmuteData>();
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("UNMUTE EVENT: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("UNMUTE EVENT: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnMuteSuccess() => ConsoleUI.Instance.Log("MUTE: SUCCESS");
        private void OnMuteSuccessEvent(GP_Data _) => OnMuteSuccess();
        private void OnMuteError() => ConsoleUI.Instance.Log("MUTE: ERROR");
        private void OnMuteError(GP_Data _) => OnMuteError();
        private void OnMuteEvent(GP_Data payload)
        {
            MuteData data = payload.Get<MuteData>();
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("MUTE EVENT: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("MUTE EVENT: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log("MUTE EVENT: UNMUTE AT: " + data.unmuteAt);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnFetchMembersError() => ConsoleUI.Instance.Log("FETCH MEMBERS: ERROR");
        private void OnFetchMembersError(GP_Data _) => OnFetchMembersError();
        private void OnFetchMembers(GP_Data payload)
        {
            FetchMembersPayload data = payload.Get<FetchMembersPayload>();
            FetchMembersData[] membersData = data.items ?? new FetchMembersData[0];

            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("FETCH MEMBERS: CAN LOAD MORE: " + data.canLoadMore);

            for (int i = 0; i < membersData.Length; i++)
            {
                ConsoleUI.Instance.Log("FETCH MEMBERS: MEMBER: ID: " + membersData[i].id);
                ConsoleUI.Instance.Log("FETCH MEMBERS: MEMBER: IS ONLINE: " + membersData[i].isOnline);
                ConsoleUI.Instance.Log("FETCH MEMBERS: MEMBER: IS MUTED: " + membersData[i].mute.isMuted);
                ConsoleUI.Instance.Log("FETCH MEMBERS: MEMBER: UNMUTE AT: " + membersData[i].mute.unmuteAt);

                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH MEMBERS: MEMBER: AVATAR: " + membersData[i].state.avatar);
                ConsoleUI.Instance.Log("FETCH MEMBERS: MEMBER: CREDITIALS: " + membersData[i].state.credentials);
                ConsoleUI.Instance.Log("FETCH MEMBERS: MEMBER: ID: " + membersData[i].state.id);
                ConsoleUI.Instance.Log("FETCH MEMBERS: MEMBER: NAME: " + membersData[i].state.name);
                ConsoleUI.Instance.Log("FETCH MEMBERS: MEMBER: PLATFORM TYPE: " + membersData[i].state.platformType);
                ConsoleUI.Instance.Log("FETCH MEMBERS: MEMBER: PROJECT ID: " + membersData[i].state.projectId);
                ConsoleUI.Instance.Log("FETCH MEMBERS: MEMBER: SCORE: " + membersData[i].state.score);
                ConsoleUI.Instance.Log(" ");
            }
        }


        private void OnFetchMoreMembersError() => ConsoleUI.Instance.Log("FETCH MORE MEMBERS: ERROR");
        private void OnFetchMoreMembersError(GP_Data _) => OnFetchMoreMembersError();
        private void OnFetchMoreMembers(GP_Data payload)
        {
            FetchMembersPayload data = payload.Get<FetchMembersPayload>();
            FetchMembersData[] membersData = data.items ?? new FetchMembersData[0];

            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("FETCH MORE MEMBERS: CAN LOAD MORE: " + data.canLoadMore);

            for (int i = 0; i < membersData.Length; i++)
            {
                ConsoleUI.Instance.Log("FETCH MORE MEMBERS: MEMBER: ID: " + membersData[i].id);
                ConsoleUI.Instance.Log("FETCH MORE MEMBERS: MEMBER: IS ONLINE: " + membersData[i].isOnline);
                ConsoleUI.Instance.Log("FETCH MORE MEMBERS: MEMBER: IS MUTED: " + membersData[i].mute.isMuted);
                ConsoleUI.Instance.Log("FETCH MORE MEMBERS: MEMBER: UNMUTE AT: " + membersData[i].mute.unmuteAt);

                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH MORE MEMBERS: MEMBER: AVATAR: " + membersData[i].state.avatar);
                ConsoleUI.Instance.Log("FETCH MORE MEMBERS: MEMBER: CREDITIALS: " + membersData[i].state.credentials);
                ConsoleUI.Instance.Log("FETCH MORE MEMBERS: MEMBER: ID: " + membersData[i].state.id);
                ConsoleUI.Instance.Log("FETCH MORE MEMBERS: MEMBER: NAME: " + membersData[i].state.name);
                ConsoleUI.Instance.Log("FETCH MORE MEMBERS: MEMBER: PLATFORM TYPE: " + membersData[i].state.platformType);
                ConsoleUI.Instance.Log("FETCH MORE MEMBERS: MEMBER: PROJECT ID: " + membersData[i].state.projectId);
                ConsoleUI.Instance.Log("FETCH MORE MEMBERS: MEMBER: SCORE: " + membersData[i].state.score);
                ConsoleUI.Instance.Log(" ");
            }
        }

    }
}
