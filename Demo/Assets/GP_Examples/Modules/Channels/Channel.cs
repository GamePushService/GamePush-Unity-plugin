using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

using GamePush;
using Examples.Console;

namespace Examples.Channel.Channel
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
    public class Mute
    {
        public bool isMuted;
        public string unmuteAt;
    }

    [System.Serializable]
    public class FetchChannelsPayload
    {
        public FetchChannelData[] items;
        public bool canLoadMore;
    }

    public class Channel : MonoBehaviour
    {
        #region Variables
        [SerializeField] private TMP_InputField _inputChannelIds;
        [SerializeField] private TMP_InputField _inputChannelIds2;
        [SerializeField] private TMP_InputField _inputTags;
        [Space(15)]
        [SerializeField] private TMP_Text _channelName;
        [SerializeField] private TMP_Text _channelDescription;
        [SerializeField] private TMP_Text _channelCapacity;
        [SerializeField] private TMP_Text _channelOwnerId;
        [SerializeField] private TMP_Text _channelPassword;
        [SerializeField] private TMP_Text _channelTemplateId;
        [Space(15)]
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _updateButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _fetchButton;
        [SerializeField] private Button _fetchChannelsButton;
        [SerializeField] private Button _fetchMoreChannelsButton;
        #endregion

        private void OnEnable()
        {
            _createButton.onClick.AddListener(Create);
            _updateButton.onClick.AddListener(UpdateChannel);
            _deleteButton.onClick.AddListener(Delete);
            _fetchButton.onClick.AddListener(Fetch);
            _fetchChannelsButton.onClick.AddListener(FetchChannels);
            _fetchMoreChannelsButton.onClick.AddListener(FetchMoreChannels);


            GP_Channels.on("createChannel", (UnityAction<GP_Data>)OnCreateChannel);
            GP_Channels.on("error:createChannel", (UnityAction<GP_Data>)OnCreateChannelError);

            GP_Channels.on("updateChannel", (UnityAction<GP_Data>)OnUpdateChannel);
            GP_Channels.on("error:updateChannel", (UnityAction<GP_Data>)OnUpdateChannelError);

            GP_Channels.on("deleteChannel", (UnityAction<GP_Data>)OnDeleteChannelSuccessEvent);
            GP_Channels.on("event:deleteChannel", (UnityAction<GP_Data>)OnDeleteChannelEvent);
            GP_Channels.on("error:deleteChannel", (UnityAction<GP_Data>)OnDeleteChannelError);

            GP_Channels.on("fetchChannel", (UnityAction<GP_Data>)OnFetchChannel);
            GP_Channels.on("error:fetchChannel", (UnityAction<GP_Data>)OnFetchChannelError);

            GP_Channels.on("fetchChannels", (UnityAction<GP_Data>)OnFetchChannels);
            GP_Channels.on("error:fetchChannels", (UnityAction<GP_Data>)OnFetchChannelsError);

            GP_Channels.on("fetchMoreChannels", (UnityAction<GP_Data>)OnFetchMoreChannels);
            GP_Channels.on("error:fetchMoreChannels", (UnityAction<GP_Data>)OnFetchMoreChannelsError);
        }

        private void OnDisable()
        {
            _createButton.onClick.RemoveListener(Create);
            _updateButton.onClick.RemoveListener(UpdateChannel);
            _deleteButton.onClick.RemoveListener(Delete);
            _fetchButton.onClick.RemoveListener(Fetch);
            _fetchChannelsButton.onClick.RemoveListener(FetchChannels);
            _fetchMoreChannelsButton.onClick.RemoveListener(FetchMoreChannels);


            GP_Channels.off("createChannel", (UnityAction<GP_Data>)OnCreateChannel);
            GP_Channels.off("error:createChannel", (UnityAction<GP_Data>)OnCreateChannelError);

            GP_Channels.off("updateChannel", (UnityAction<GP_Data>)OnUpdateChannel);
            GP_Channels.off("error:updateChannel", (UnityAction<GP_Data>)OnUpdateChannelError);

            GP_Channels.off("deleteChannel", (UnityAction<GP_Data>)OnDeleteChannelSuccessEvent);
            GP_Channels.off("event:deleteChannel", (UnityAction<GP_Data>)OnDeleteChannelEvent);
            GP_Channels.off("error:deleteChannel", (UnityAction<GP_Data>)OnDeleteChannelError);

            GP_Channels.off("fetchChannel", (UnityAction<GP_Data>)OnFetchChannel);
            GP_Channels.off("error:fetchChannel", (UnityAction<GP_Data>)OnFetchChannelError);

            GP_Channels.off("fetchChannels", (UnityAction<GP_Data>)OnFetchChannels);
            GP_Channels.off("error:fetchChannels", (UnityAction<GP_Data>)OnFetchChannelsError);

            GP_Channels.off("fetchMoreChannels", (UnityAction<GP_Data>)OnFetchMoreChannels);
            GP_Channels.off("error:fetchMoreChannels", (UnityAction<GP_Data>)OnFetchMoreChannelsError);
        }


        #region Button Methods
        public void Create()
        {
            var filter = new CreateChannelFilter(5);
            filter.name = "CLAN_OF_DARKNESS";
            filter.visible = true;
            filter.tags = _inputTags.text.Split(",");
            GP_Channels.createChannel(filter);
        }
        public void UpdateChannel()
        {
            var filter = new UpdateChannelFilter(int.Parse(_inputChannelIds.text));

            filter.capacity = 50;
            filter.description = "ONE OF THE POWERFUL CLAN";

            var guestAcl = new GuestAcl();
            guestAcl.canViewMessages = true;
            guestAcl.canAddMessage = true;
            guestAcl.canEditMessage = true;
            guestAcl.canDeleteMessage = true;
            guestAcl.canViewMembers = true;
            guestAcl.canInvitePlayer = true;
            guestAcl.canKickPlayer = true;
            guestAcl.canAcceptJoinRequest = true;
            guestAcl.canMutePlayer = true;

            filter.guestAcl = guestAcl;
            filter.@private = true;

            GP_Channels.updateChannel(filter);
        }
        public void Delete() => GP_Channels.deleteChannel(new ChannelQuery { channelId = int.Parse(_inputChannelIds.text) });

        public void Fetch() => GP_Channels.fetchChannel(new ChannelQuery { channelId = int.Parse(_inputChannelIds.text) });
        public void FetchChannels()
        {
            var filter = new FetchChannelsFilter();
            filter.ids = new int[] { int.Parse(_inputChannelIds.text), int.Parse(_inputChannelIds2.text) };
            filter.limit = 10;
            filter.tags = _inputTags.text.Split(",");

            GP_Channels.fetchChannels(filter);
        }
        public void FetchMoreChannels()
        {
            var filter = new FetchMoreChannelsFilter();
            filter.ids = new int[] { int.Parse(_inputChannelIds.text), int.Parse(_inputChannelIds2.text) };
            filter.limit = 10;
            filter.tags = _inputTags.text.Split(",");

            GP_Channels.fetchMoreChannels(filter);
        }
        #endregion



        private void OnCreateChannel(GP_Data payload)
        {
            CreateChannelData data = payload.Get<CreateChannelData>();
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: ID: " + data.id);

            for (int x = 0; x < data.tags.Length; x++)
            {
                ConsoleUI.Instance.Log("ON CHANNEL CREATE: TAGS: " + data.tags[x]);
            }

            for (int i = 0; i < data.messageTags.Length; i++)
            {
                ConsoleUI.Instance.Log("ON CHANNEL CREATE: MESSAGE TAGS: " + data.messageTags[i]);
            }

            ConsoleUI.Instance.Log("ON CHANNEL CREATE: TEMPLATE ID: " + data.templateId);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: CAPACITY: " + data.capacity);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: OWNER ID: " + data.ownerId);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: NAME: " + data.name);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: DESCRIPTION: " + data.description);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: PRIVATE: " + data.@private);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: VISIBLE: " + data.visible);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: PERMANENT: " + data.permanent);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: HAS PASSWORD: " + data.hasPassword);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: IS JOINED: " + data.isJoined);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: IS REQUEST SENT: " + data.isRequestSent);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: IS INVITED: " + data.isInvited);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: IS MUTED: " + data.isMuted);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: PASSWORD: " + data.password);
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: MEMBERS COUNT: " + data.membersCount);
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: OWNER ACL: " + JsonUtility.ToJson(data.ownerAcl));
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: MemberAcl: " + JsonUtility.ToJson(data.memberAcl));
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("ON CHANNEL CREATE: Guest Acl: " + JsonUtility.ToJson(data.guestAcl));
            ConsoleUI.Instance.Log(" ");
        }
        private void OnCreateChannelError() => ConsoleUI.Instance.Log("CREATE CHANNEL: ERROR");
        private void OnCreateChannelError(GP_Data _) => OnCreateChannelError();

        private void OnUpdateChannel(GP_Data payload)
        {
            UpdateChannelData data = payload.Get<UpdateChannelData>();
            ConsoleUI.Instance.Log("UPDATE CHANNEL: ID: " + data.id);

            for (int i = 0; i < data.tags.Length; i++)
            {
                ConsoleUI.Instance.Log("UPDATE CHANNEL: TAGS: " + data.tags[i]);
            }

            for (int i = 0; i < data.messageTags.Length; i++)
            {
                ConsoleUI.Instance.Log("UPDATE CHANNEL: MESSAGE TAGS: " + data.messageTags[i]);
            }

            ConsoleUI.Instance.Log("UPDATE CHANNEL: CAPACITY: " + data.capacity);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: OWNER ID: " + data.ownerId);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: NAME: " + data.name);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: DESCRIPTION: " + data.description);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: PRIVATE: " + data.@private);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: VISIBLE: " + data.visible);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: PERMANENT: " + data.permanent);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: HAS PASSWORD: " + data.hasPassword);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: PASSWORD: " + data.password);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: IS JOINED: " + data.isJoined);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: IS INVITED: " + data.isInvited);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: IS MUTED: " + data.isMuted);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: IS REQUEST SENT: " + data.isRequestSent);
            ConsoleUI.Instance.Log("UPDATE CHANNEL: MEMBERS COUNT: " + data.membersCount);
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("UPDATE CHANNEL: OWNER ACL: " + JsonUtility.ToJson(data.ownerAcl));
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("UPDATE CHANNEL: MEMBER ACL: " + JsonUtility.ToJson(data.memberAcl));
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("UPDATE CHANNEL: GUEST ACL: " + JsonUtility.ToJson(data.guestAcl));
            ConsoleUI.Instance.Log(" ");
        }
        private void OnUpdateChannelError() => ConsoleUI.Instance.Log("UPDATE CHANNEL: ERROR");
        private void OnUpdateChannelError(GP_Data _) => OnUpdateChannelError();


        private void OnFetchChannel(GP_Data payload)
        {
            FetchChannelData data = payload.Get<FetchChannelData>();
            SetUI();

            ConsoleUI.Instance.Log("FETCH CHANNEL: ID: " + data.id);

            for (int i = 0; i < data.tags.Length; i++)
            {
                ConsoleUI.Instance.Log("FETCH CHANNEL: TAGS: " + data.tags[i]);
            }

            for (int i = 0; i < data.messageTags.Length; i++)
            {
                ConsoleUI.Instance.Log("FETCH CHANNEL: MESSAGE TAGS: " + data.messageTags[i]);
            }

            ConsoleUI.Instance.Log("FETCH CHANNEL: TEMPLATE ID: " + data.templateId);
            ConsoleUI.Instance.Log("FETCH CHANNEL: PROJECT ID: " + data.projectId);
            ConsoleUI.Instance.Log("FETCH CHANNEL: CAPACITY: " + data.capacity);
            ConsoleUI.Instance.Log("FETCH CHANNEL: OWNER ID: " + data.ownerId);
            ConsoleUI.Instance.Log("FETCH CHANNEL: NAME: " + data.name);
            ConsoleUI.Instance.Log("FETCH CHANNEL: DESCRIPTION: " + data.description);
            ConsoleUI.Instance.Log("FETCH CHANNEL: PRIVATE: " + data.@private);
            ConsoleUI.Instance.Log("FETCH CHANNEL: VISIBLE: " + data.visible);
            ConsoleUI.Instance.Log("FETCH CHANNEL: PERMANENT: " + data.permanent);
            ConsoleUI.Instance.Log("FETCH CHANNEL: HAS PASSWORD: " + data.hasPassword);
            ConsoleUI.Instance.Log("FETCH CHANNEL: PASSWORD: " + data.password);
            ConsoleUI.Instance.Log("FETCH CHANNEL: IS JOINED: " + data.isJoined);
            ConsoleUI.Instance.Log("FETCH CHANNEL: IS INVITED: " + data.isInvited);
            ConsoleUI.Instance.Log("FETCH CHANNEL: IS MUTED: " + data.isMuted);
            ConsoleUI.Instance.Log("FETCH CHANNEL: IS REQUEST SENT: " + data.isRequestSent);
            ConsoleUI.Instance.Log("FETCH CHANNEL: MEMBERS COUNT: " + data.membersCount);
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("FETCH CHANNEL: OWNER ACL: " + JsonUtility.ToJson(data.ownerAcl));
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("FETCH CHANNEL: MEMBER ACL: " + JsonUtility.ToJson(data.memberAcl));
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("FETCH CHANNEL: GUEST ACL: " + JsonUtility.ToJson(data.guestAcl));
            ConsoleUI.Instance.Log(" ");

            void SetUI()
            {
                _channelName.text = data.name;
                _channelDescription.text = data.description;
                _channelCapacity.text = "" + data.capacity;
                _channelOwnerId.text = "" + data.ownerId;
                _channelPassword.text = data.password;
                _channelTemplateId.text = "" + data.templateId;
            }
        }
        private void OnFetchChannelError() => ConsoleUI.Instance.Log("FETCH CHANNEL: ERROR");
        private void OnFetchChannelError(GP_Data _) => OnFetchChannelError();


        private void OnFetchChannels(GP_Data payload)
        {
            FetchChannelsPayload paged = payload.Get<FetchChannelsPayload>();
            List<FetchChannelData> data = paged.items == null ? new List<FetchChannelData>() : new List<FetchChannelData>(paged.items);
            bool canLoadMore = paged.canLoadMore;
            ConsoleUI.Instance.Log("FETCH CHANNELS: CAN LOAD MORE: " + canLoadMore);
            ConsoleUI.Instance.Log(" ");
            for (int i = 0; i < data.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH CHANNELS: ID: " + data[i].id);
                ConsoleUI.Instance.Log("FETCH CHANNELS: NAME: " + data[i].name);
                ConsoleUI.Instance.Log("FETCH CHANNELS: DESCRIPTION: " + data[i].description);
                ConsoleUI.Instance.Log("FETCH CHANNELS: HAS PASSWORD: " + data[i].hasPassword);
                ConsoleUI.Instance.Log("FETCH CHANNELS: MEMBERS COUNT: " + data[i].membersCount);
                ConsoleUI.Instance.Log("FETCH CHANNELS: OWNER ID: " + data[i].ownerId);
                ConsoleUI.Instance.Log("FETCH CHANNELS: PROJECT ID: " + data[i].projectId);

                for (int x = 0; x < data[i].tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH CHANNELS: TAGS: " + data[i].tags[x]);
                }

                for (int a = 0; a < data[i].messageTags.Length; a++)
                {
                    ConsoleUI.Instance.Log("FETCH CHANNELS: MESSAGE TAGS: " + data[i].messageTags[a]);
                }

                ConsoleUI.Instance.Log("FETCH CHANNELS: TEMPLATE ID: " + data[i].templateId);
                ConsoleUI.Instance.Log("FETCH CHANNELS: CAPACITY: " + data[i].capacity);
                ConsoleUI.Instance.Log("FETCH CHANNELS: VISIBLE: " + data[i].visible);
                ConsoleUI.Instance.Log("FETCH CHANNELS: PRIVATE: " + data[i].@private);
                ConsoleUI.Instance.Log("FETCH CHANNELS: PERMANENT: " + data[i].permanent);
                ConsoleUI.Instance.Log("FETCH CHANNELS: HAS PASSWORD: " + data[i].hasPassword);
                ConsoleUI.Instance.Log("FETCH CHANNELS: PASSWORD: " + data[i].password);
                ConsoleUI.Instance.Log("FETCH CHANNELS: IS JOINED: " + data[i].isJoined);
                ConsoleUI.Instance.Log("FETCH CHANNELS: IS INVITED: " + data[i].isInvited);
                ConsoleUI.Instance.Log("FETCH CHANNELS: IS MUTED: " + data[i].isMuted);
                ConsoleUI.Instance.Log("FETCH CHANNELS: IS REQUEST SENT: " + data[i].isRequestSent);
                ConsoleUI.Instance.Log("FETCH CHANNELS: MEMBERS COUNT: " + data[i].membersCount);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH CHANNELS: OWNER ACL: " + JsonUtility.ToJson(data[i].ownerAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH CHANNELS: MEMBER ACL: " + JsonUtility.ToJson(data[i].memberAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH CHANNELS: GUEST ACL: " + JsonUtility.ToJson(data[i].guestAcl));
                ConsoleUI.Instance.Log(" ");
            }
        }
        private void OnFetchChannelsError() => ConsoleUI.Instance.Log("FETCH CHANNELS: ERROR");
        private void OnFetchChannelsError(GP_Data _) => OnFetchChannelsError();


        private void OnFetchMoreChannels(GP_Data payload)
        {
            FetchChannelsPayload paged = payload.Get<FetchChannelsPayload>();
            List<FetchChannelData> data = paged.items == null ? new List<FetchChannelData>() : new List<FetchChannelData>(paged.items);
            bool canLoadMore = paged.canLoadMore;
            ConsoleUI.Instance.Log("FETCH MORE CHANNELS: CAN LOAD MORE: " + canLoadMore);
            ConsoleUI.Instance.Log(" ");
            for (int i = 0; i < data.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: ID: " + data[i].id);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: NAME: " + data[i].name);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: DESCRIPTION: " + data[i].description);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: HAS PASSWORD: " + data[i].hasPassword);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: MEMBERS COUNT: " + data[i].membersCount);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: OWNER ID: " + data[i].ownerId);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: PROJECT ID: " + data[i].projectId);

                for (int x = 0; x < data[i].tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH MORE CHANNELS: TAGS: " + data[i].tags[x]);
                }

                for (int a = 0; a < data[i].messageTags.Length; a++)
                {
                    ConsoleUI.Instance.Log("FETCH MORE CHANNELS: MESSAGE TAGS: " + data[i].messageTags[a]);
                }
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: TEMPLATE ID: " + data[i].templateId);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: CAPACITY: " + data[i].capacity);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: VISIBLE: " + data[i].visible);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: PRIVATE: " + data[i].@private);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: PERMANENT: " + data[i].permanent);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: HAS PASSWORD: " + data[i].hasPassword);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: PASSWORD: " + data[i].password);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: IS JOINED: " + data[i].isJoined);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: IS INVITED: " + data[i].isInvited);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: IS MUTED: " + data[i].isMuted);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: IS REQUEST SENT: " + data[i].isRequestSent);
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: MEMBERS COUNT: " + data[i].membersCount);
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: OWNER ACL: " + JsonUtility.ToJson(data[i].ownerAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: MEMBER ACL: " + JsonUtility.ToJson(data[i].memberAcl));
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE CHANNELS: GUEST ACL: " + JsonUtility.ToJson(data[i].guestAcl));
                ConsoleUI.Instance.Log(" ");
            }
        }
        private void OnFetchMoreChannelsError() => ConsoleUI.Instance.Log("FETCH MORE CHANNELS: ERROR");
        private void OnFetchMoreChannelsError(GP_Data _) => OnFetchMoreChannelsError();


        private void OnDeleteChannelSuccess() => ConsoleUI.Instance.Log("DELETE CHANNEL: SUCCESS");
        private void OnDeleteChannelSuccessEvent(GP_Data _) => OnDeleteChannelSuccess();
        private void OnDeleteChannelEvent(GP_Data payload) => ConsoleUI.Instance.Log("DELETE CHANNEL EVENT: CHANNEL ID: " + payload.Get<FetchChannelData>().id);
        private void OnDeleteChannelError() => ConsoleUI.Instance.Log("DELETE CHANNEL: ERROR");
        private void OnDeleteChannelError(GP_Data _) => OnDeleteChannelError();
    }
}
