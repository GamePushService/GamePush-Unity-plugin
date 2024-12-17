using UnityEngine;
using UnityEngine.UI;
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


            GP_Channels.OnJoinError += OnJoinError;
            GP_Channels.OnJoinEvent += OnJoinEvent;
            GP_Channels.OnJoinSuccess += OnJoinSuccess;

            GP_Channels.OnCancelJoinSuccess += OnCancelJoinSuccess;
            GP_Channels.OnCancelJoinError += OnCancelJoinError;
            GP_Channels.OnCancelJoinEvent += OnCancelJoinEvent;

            GP_Channels.OnLeaveSuccess += OnLeaveSuccess;
            GP_Channels.OnLeaveEvent += OnLeaveEvent;
            GP_Channels.OnLeaveError += OnLeaveError;

            GP_Channels.OnKick += OnKick;
            GP_Channels.OnKickError += OnKickError;

            GP_Channels.OnMuteSuccess += OnMuteSuccess;
            GP_Channels.OnMuteError += OnMuteError;
            GP_Channels.OnMuteEvent += OnMuteEvent;

            GP_Channels.OnUnmuteSuccess += OnUnmuteSuccess;
            GP_Channels.OnUnmuteError += OnUnmuteError;
            GP_Channels.OnUnmuteEvent += OnUnmuteEvent;

            GP_Channels.OnFetchMembers += OnFetchMembers;
            GP_Channels.OnFetchMembersError += OnFetchMembersError;

            GP_Channels.OnFetchMoreMembers += OnFetchMoreMembers;
            GP_Channels.OnFetchMoreMembersError += OnFetchMoreMembersError;
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


            GP_Channels.OnJoinError -= OnJoinError;
            GP_Channels.OnJoinEvent -= OnJoinEvent;
            GP_Channels.OnJoinSuccess -= OnJoinSuccess;

            GP_Channels.OnCancelJoinSuccess -= OnCancelJoinSuccess;
            GP_Channels.OnCancelJoinError -= OnCancelJoinError;
            GP_Channels.OnCancelJoinEvent -= OnCancelJoinEvent;

            GP_Channels.OnLeaveSuccess -= OnLeaveSuccess;
            GP_Channels.OnLeaveEvent -= OnLeaveEvent;
            GP_Channels.OnLeaveError -= OnLeaveError;

            GP_Channels.OnKick -= OnKick;
            GP_Channels.OnKickError -= OnKickError;

            GP_Channels.OnMuteError -= OnMuteError;
            GP_Channels.OnMuteEvent -= OnMuteEvent;
            GP_Channels.OnMuteSuccess -= OnMuteSuccess;

            GP_Channels.OnUnmuteError -= OnUnmuteError;
            GP_Channels.OnUnmuteEvent -= OnUnmuteEvent;
            GP_Channels.OnUnmuteSuccess -= OnUnmuteSuccess;

            GP_Channels.OnFetchMembers -= OnFetchMembers;
            GP_Channels.OnFetchMembersError -= OnFetchMembersError;

            GP_Channels.OnFetchMoreMembers -= OnFetchMoreMembers;
            GP_Channels.OnFetchMoreMembersError -= OnFetchMoreMembersError;
        }


        public void Join() => GP_Channels.Join(int.Parse(_channelIdInput.text), "12345");
        public void CancelJoin() => GP_Channels.CancelJoin(int.Parse(_channelIdInput.text));
        public void Leave() => GP_Channels.Leave(int.Parse(_channelIdInput.text));

        public void Kick() => GP_Channels.Kick(int.Parse(_channelIdInput.text), int.Parse(_playerIdInput.text));
        public void Mute() => GP_Channels.Mute(int.Parse(_channelIdInput.text), int.Parse(_playerIdInput.text), int.Parse(_muteTimeInput.text));
        public void Unmute() => GP_Channels.UnMute(int.Parse(_channelIdInput.text), int.Parse(_playerIdInput.text));


        public void FetchMembers()
        {
            var filter = new FetchMembersFilter(int.Parse(_channelIdInput.text));
            filter.limit = 50;
            GP_Channels.FetchMembers(filter);
        }

        public void FetchMoreMembers()
        {
            var filter = new FetchMoreMembersFilter(int.Parse(_channelIdInput.text));
            filter.limit = 50;
            GP_Channels.FetchMoreMembers(filter);
        }

        public void MakeOwner()
        {
            var filter = new UpdateChannelFilter(int.Parse(_channelIdInput.text));
            filter.ownerId = int.Parse(_playerIdInput.text);
            GP_Channels.UpdateChannel(filter);
        }

        private void OnJoinSuccess() => ConsoleUI.Instance.Log("JOIN: SUCCESS");
        private void OnJoinError() => ConsoleUI.Instance.Log("JOIN: ERROR");
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
        private void OnCancelJoinEvent(CancelJoinData data)
        {
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("CANCEL JOIN EVENT: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("CANCEL JOIN EVENT: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnLeaveSuccess() => ConsoleUI.Instance.Log("ON LEAVE CHANNEL: SUCCESS");
        private void OnLeaveError() => ConsoleUI.Instance.Log("ON LEAVE CHANNEL: ERROR");
        private void OnLeaveEvent(MemberLeaveData data)
        {
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("ON LEAVE CHANNEL: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("ON LEAVE CHANNEL: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log("ON LEAVE CHANNEL: REASON: " + data.reason);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnKick() => ConsoleUI.Instance.Log("KICK: SUCCESS");
        private void OnKickError() => ConsoleUI.Instance.Log("KICK: ERROR");


        private void OnUnmuteSuccess() => ConsoleUI.Instance.Log("UNMUTE: SUCCESS");
        private void OnUnmuteError() => ConsoleUI.Instance.Log("UNMUTE: ERROR");
        private void OnUnmuteEvent(UnmuteData data)
        {
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("UNMUTE EVENT: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("UNMUTE EVENT: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnMuteSuccess() => ConsoleUI.Instance.Log("MUTE: SUCCESS");
        private void OnMuteError() => ConsoleUI.Instance.Log("MUTE: ERROR");
        private void OnMuteEvent(MuteData data)
        {
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("MUTE EVENT: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("MUTE EVENT: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log("MUTE EVENT: UNMUTE AT: " + data.unmuteAt);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnFetchMembersError() => ConsoleUI.Instance.Log("FETCH MEMBERS: ERROR");
        private void OnFetchMembers(GP_Data data, bool canLoadMore)
        {
            var membersData = data.GetList<FetchMembersData>();

            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("FETCH MEMBERS: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < membersData.Count; i++)
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
        private void OnFetchMoreMembers(GP_Data data, bool canLoadMore)
        {
            var membersData = data.GetList<FetchMembersData>();

            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("FETCH MORE MEMBERS: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < membersData.Count; i++)
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