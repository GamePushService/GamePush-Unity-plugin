using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GamePush;
using Examples.Console;
using System;

namespace Examples.Channels.Messages
{
    [System.Serializable]
    public class Message_Data
    {
        public string id;
        public int channelId;
        public int authorId;
        public string text;
        public string[] tags;
        public PlayerData player; // Публичные поля игрока добавляет сам разработчик, в данном случае все поля Игрока по default
        public string createdAt;
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

    public class Messages : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _channelIdInput;
        [SerializeField] private TMP_InputField _playerIdInput;

        [Space(10)]
        [SerializeField] private TMP_InputField _tagsInput;
        [SerializeField] private TMP_InputField _messageIdInput;

        [Space(10)]
        [SerializeField] private TMP_InputField _messageInput;

        [Space(20)]
        [SerializeField] private Button _sendMessageButton;
        [SerializeField] private Button _sendPersonalMessageButton;
        [SerializeField] private Button _sendFeedMessageButton;
        [SerializeField] private Button _editMessageButton;
        [SerializeField] private Button _deleteMessageButton;

        [SerializeField] private Button _fetchMessagesButton;
        [SerializeField] private Button _fetchMoreMessagesButton;

        [SerializeField] private Button _fetchPersonalMessagesButton;
        [SerializeField] private Button _fetchMorePersonalMessagesButton;

        [SerializeField] private Button _fetchFeedMessagesButton;
        [SerializeField] private Button _fetchMoreFeedMessagesButton;

        [SerializeField] private TMP_Text _chatUI;

        private void OnEnable()
        {
            _sendMessageButton.onClick.AddListener(SendMessage);
            _sendPersonalMessageButton.onClick.AddListener(SendPersonalMessage);
            _sendFeedMessageButton.onClick.AddListener(SendFeedMessage);
            _editMessageButton.onClick.AddListener(EditMessage);
            _deleteMessageButton.onClick.AddListener(DeleteMessage);
            _fetchMessagesButton.onClick.AddListener(FetchMessages);
            _fetchMoreMessagesButton.onClick.AddListener(FetchMoreMessages);
            _fetchPersonalMessagesButton.onClick.AddListener(FetchPersonalMessages);
            _fetchMorePersonalMessagesButton.onClick.AddListener(FetchMorePersonalMessages);
            _fetchFeedMessagesButton.onClick.AddListener(FetchFeedMessages);
            _fetchMoreFeedMessagesButton.onClick.AddListener(FetchMoreFeedMessages);


            GP_Channels.OnMessage += OnMessage;

            GP_Channels.OnSendMessage += OnSendMessage;
            GP_Channels.OnSendMessageError += OnSendMessageError;

            GP_Channels.OnEditMessageSuccess += OnEditMessageSuccess;
            GP_Channels.OnEditMessageError += OnEditMessageError;
            GP_Channels.OnEditMessageEvent += OnEditMessageEvent;

            GP_Channels.OnDeleteMessageSuccess += OnDeleteMessageSuccess;
            GP_Channels.OnDeleteMessageError += OnDeleteMessageError;
            GP_Channels.OnDeleteMessageEvent += OnDeleteMessageEvent;

            GP_Channels.OnFetchMessages += OnFetchMessages;
            GP_Channels.OnFetchMessagesError += OnFetchMessagesError;

            GP_Channels.OnFetchPersonalMessages += OnFetchPersonalMessages;
            GP_Channels.OnFetchPersonalMessagesError += OnFetchPersonalMessagesError;

            GP_Channels.OnFetchFeedMessages += OnFetchFeedMessages;
            GP_Channels.OnFetchFeedMessagesError += OnFetchFeedMessagesError;


            GP_Channels.OnFetchMoreMessages += OnFetchMoreMessages;
            GP_Channels.OnFetchMoreMessagesError += OnFetchMoreMessagesError;

            GP_Channels.OnFetchMorePersonalMessages += OnFetchMorePersonalMessages;
            GP_Channels.OnFetchMorePersonalMessagesError += OnFetchMorePersonalMessagesError;

            GP_Channels.OnFetchMoreFeedMessages += OnFetchMoreFeedMessages;
            GP_Channels.OnFetchMoreFeedMessagesError += OnFetchMoreFeedMessagesError;

        }

        private void OnDisable()
        {
            _sendMessageButton.onClick.RemoveListener(SendMessage);
            _sendPersonalMessageButton.onClick.RemoveListener(SendPersonalMessage);
            _sendFeedMessageButton.onClick.RemoveListener(SendFeedMessage);
            _editMessageButton.onClick.RemoveListener(EditMessage);
            _deleteMessageButton.onClick.RemoveListener(DeleteMessage);
            _fetchMessagesButton.onClick.RemoveListener(FetchMessages);
            _fetchMoreMessagesButton.onClick.RemoveListener(FetchMoreMessages);
            _fetchPersonalMessagesButton.onClick.RemoveListener(FetchPersonalMessages);
            _fetchMorePersonalMessagesButton.onClick.RemoveListener(FetchMorePersonalMessages);
            _fetchFeedMessagesButton.onClick.RemoveListener(FetchFeedMessages);
            _fetchMoreFeedMessagesButton.onClick.RemoveListener(FetchMoreFeedMessages);


            GP_Channels.OnMessage += OnMessage;

            GP_Channels.OnSendMessage -= OnSendMessage;
            GP_Channels.OnSendMessageError -= OnSendMessageError;

            GP_Channels.OnEditMessageSuccess -= OnEditMessageSuccess;
            GP_Channels.OnEditMessageError -= OnEditMessageError;
            GP_Channels.OnEditMessageEvent -= OnEditMessageEvent;

            GP_Channels.OnDeleteMessageSuccess -= OnDeleteMessageSuccess;
            GP_Channels.OnDeleteMessageError -= OnDeleteMessageError;
            GP_Channels.OnDeleteMessageEvent -= OnDeleteMessageEvent;


            GP_Channels.OnFetchMessages -= OnFetchMessages;
            GP_Channels.OnFetchMessagesError -= OnFetchMessagesError;

            GP_Channels.OnFetchPersonalMessages -= OnFetchPersonalMessages;
            GP_Channels.OnFetchPersonalMessagesError -= OnFetchPersonalMessagesError;

            GP_Channels.OnFetchFeedMessages -= OnFetchFeedMessages;
            GP_Channels.OnFetchFeedMessagesError -= OnFetchFeedMessagesError;


            GP_Channels.OnFetchMoreMessages -= OnFetchMoreMessages;
            GP_Channels.OnFetchMoreMessagesError -= OnFetchMoreMessagesError;

            GP_Channels.OnFetchMorePersonalMessages -= OnFetchMorePersonalMessages;
            GP_Channels.OnFetchMorePersonalMessagesError -= OnFetchMorePersonalMessagesError;

            GP_Channels.OnFetchMoreFeedMessages -= OnFetchMoreFeedMessages;
            GP_Channels.OnFetchMoreFeedMessagesError -= OnFetchMoreFeedMessagesError;
        }


        public void SendMessage() => GP_Channels.SendMessage(int.Parse(_channelIdInput.text), _messageInput.text, _tagsInput.text);
        public void SendPersonalMessage() => GP_Channels.SendPersonalMessage(int.Parse(_playerIdInput.text), _messageInput.text, _tagsInput.text);
        public void SendFeedMessage() => GP_Channels.SendFeedMessage(int.Parse(_playerIdInput.text), _messageInput.text, _tagsInput.text);

        public void EditMessage() => GP_Channels.EditMessage(_messageIdInput.text, _messageInput.text);
        public void DeleteMessage() => GP_Channels.DeleteMessage(_messageIdInput.text);

        public void FetchMessages() => GP_Channels.FetchMessages(int.Parse(_channelIdInput.text), _tagsInput.text, 10, 0);
        public void FetchMoreMessages() => GP_Channels.FetchMoreMessages(int.Parse(_channelIdInput.text), _tagsInput.text, 10);

        public void FetchPersonalMessages() => GP_Channels.FetchPersonalMessages(int.Parse(_playerIdInput.text), _tagsInput.text, 10, 0);
        public void FetchMorePersonalMessages() => GP_Channels.FetchMorePersonalMessages(int.Parse(_playerIdInput.text), _tagsInput.text, 10);

        public void FetchFeedMessages() => GP_Channels.FetchFeedMessages(int.Parse(_playerIdInput.text), _tagsInput.text, 10, 0);
        public void FetchMoreFeedMessages() => GP_Channels.FetchMoreFeedMessages(int.Parse(_playerIdInput.text), _tagsInput.text, 10);

        private void OnMessage(GP_Data data)
        {
            var messageData = data.Get<Message_Data>();

            _chatUI.text += $"\n {messageData.text}";

            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("ON MESSAGE EVENT: CHANNEL ID: " + messageData.channelId);
            ConsoleUI.Instance.Log("ON MESSAGE EVENT: AUTHOR ID: " + messageData.authorId);
            ConsoleUI.Instance.Log("ON MESSAGE EVENT: ID: " + messageData.id);
            ConsoleUI.Instance.Log("ON MESSAGE EVENT: TEXT: " + messageData.text);
            for (int i = 0; i < messageData.tags.Length; i++)
            {
                ConsoleUI.Instance.Log("ON MESSAGE EVENT: TAGS: " + messageData.tags[i]);
            }
            ConsoleUI.Instance.Log("ON MESSAGE EVENT: CREATED AT: " + messageData.createdAt);

            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("ON MESSAGE EVENT: PLAYER: AVATAR: " + messageData.player.avatar);
            ConsoleUI.Instance.Log("ON MESSAGE EVENT: PLAYER: CREDITIALS: " + messageData.player.credentials);
            ConsoleUI.Instance.Log("ON MESSAGE EVENT: PLAYER: ID: " + messageData.player.id);
            ConsoleUI.Instance.Log("ON MESSAGE EVENT: PLAYER: NAME: " + messageData.player.name);
            ConsoleUI.Instance.Log("ON MESSAGE EVENT: PLAYER: PLATFORM TYPE: " + messageData.player.platformType);
            ConsoleUI.Instance.Log("ON MESSAGE EVENT: PLAYER: PROJECT ID: " + messageData.player.projectId);
            ConsoleUI.Instance.Log("ON MESSAGE EVENT: PLAYER: SCORE: " + messageData.player.score);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnSendMessageError() => ConsoleUI.Instance.Log("SEND MESSAGE: ERROR");
        private void OnSendMessage(GP_Data data)
        {
            var sendMessageData = data.Get<Message_Data>();

            _chatUI.text += $"\n {sendMessageData.text}";

            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("SEND MESSAGE: CHANNEL ID: " + sendMessageData.channelId);
            ConsoleUI.Instance.Log("SEND MESSAGE: AUTHOR ID: " + sendMessageData.authorId);
            ConsoleUI.Instance.Log("SEND MESSAGE: ID: " + sendMessageData.id);
            ConsoleUI.Instance.Log("SEND MESSAGE: TEXT: " + sendMessageData.text);
            for (int i = 0; i < sendMessageData.tags.Length; i++)
            {
                ConsoleUI.Instance.Log("SEND MESSAGE: TAGS: " + sendMessageData.tags[i]);
            }
            ConsoleUI.Instance.Log("SEND MESSAGE: CREATED AT: " + sendMessageData.createdAt);

            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("SEND MESSAGE: PLAYER: AVATAR: " + sendMessageData.player.avatar);
            ConsoleUI.Instance.Log("SEND MESSAGE: PLAYER: CREDITIALS: " + sendMessageData.player.credentials);
            ConsoleUI.Instance.Log("SEND MESSAGE: PLAYER: ID: " + sendMessageData.player.id);
            ConsoleUI.Instance.Log("SEND MESSAGE: PLAYER: NAME: " + sendMessageData.player.name);
            ConsoleUI.Instance.Log("SEND MESSAGE: PLAYER: PLATFORM TYPE: " + sendMessageData.player.platformType);
            ConsoleUI.Instance.Log("SEND MESSAGE: PLAYER: PROJECT ID: " + sendMessageData.player.projectId);
            ConsoleUI.Instance.Log("SEND MESSAGE: PLAYER: SCORE: " + sendMessageData.player.score);
            ConsoleUI.Instance.Log(" ");
        }


        private void OnEditMessageError() => ConsoleUI.Instance.Log("EDIT MESSAGE: ERROR");
        private void OnEditMessageSuccess(GP_Data data)
        {
            var editMessageData = data.Get<Message_Data>();

            _chatUI.text += $"\n {editMessageData.text}";

            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: CHANNEL ID: " + editMessageData.channelId);
            ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: AUTHOR ID: " + editMessageData.authorId);
            ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: ID: " + editMessageData.id);
            ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: TEXT: " + editMessageData.text);
            for (int i = 0; i < editMessageData.tags.Length; i++)
            {
                ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: TAGS: " + editMessageData.tags[i]);
            }
            ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: CREATED AT: " + editMessageData.createdAt);

            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: PLAYER: AVATAR " + editMessageData.player.avatar);
            ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: PLAYER: CREDITIALS " + editMessageData.player.credentials);
            ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: PLAYER: ID " + editMessageData.player.id);
            ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: PLAYER: NAME " + editMessageData.player.name);
            ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: PLAYER: PLATFORM TYPE " + editMessageData.player.platformType);
            ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: PLAYER: PROJECT ID " + editMessageData.player.projectId);
            ConsoleUI.Instance.Log("EDIT MESSAGE SUCCESS: PLAYER: SCORE " + editMessageData.player.score);
            ConsoleUI.Instance.Log(" ");
        }
        private void OnEditMessageEvent(MessageData data)
        {
            _chatUI.text += $"\n {data.text}";

            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("EDIT MESSAGE EVENT: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("EDIT MESSAGE EVENT: AUTHOR ID: " + data.authorId);
            ConsoleUI.Instance.Log("EDIT MESSAGE EVENT: ID: " + data.id);
            ConsoleUI.Instance.Log("EDIT MESSAGE EVENT: TEXT: " + data.text);
            ConsoleUI.Instance.Log("EDIT MESSAGE EVENT: CREATED AT: " + data.createdAt);
            for (int i = 0; i < data.tags.Length; i++)
            {
                ConsoleUI.Instance.Log("EDIT MESSAGE EVENT: TAGS: " + data.tags[i]);
            }
            ConsoleUI.Instance.Log(" ");
        }


        private void OnDeleteMessageSuccess() => ConsoleUI.Instance.Log("DELETE MESSAGE: Success");
        private void OnDeleteMessageError() => ConsoleUI.Instance.Log("DELETE MESSAGE: ERROR");
        private void OnDeleteMessageEvent(MessageData data)
        {
            ConsoleUI.Instance.Log(" ");
            ConsoleUI.Instance.Log("DELETE MESSAGE EVENT: ID: " + data.id);
            ConsoleUI.Instance.Log("DELETE MESSAGE EVENT: CHANNEL ID: " + data.channelId);
            ConsoleUI.Instance.Log("DELETE MESSAGE EVENT: AUTHOR ID: " + data.authorId);
            ConsoleUI.Instance.Log("DELETE MESSAGE EVENT: TEXT: " + data.text);
            ConsoleUI.Instance.Log("DELETE MESSAGE EVENT: CREATED AT: " + data.createdAt);
            for (int i = 0; i < data.tags.Length; i++)
            {
                ConsoleUI.Instance.Log("DELETE MESSAGE EVENT: TAGS: " + data.tags[i]);
            }
            ConsoleUI.Instance.Log(" ");
        }


        private void OnFetchMessagesError() => ConsoleUI.Instance.Log("FETCH MESSAGES: ERROR");
        private void OnFetchMessages(GP_Data data, bool canLoadMore)
        {
            var fetchMessageData = data.GetList<Message_Data>();
            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("FETCH MESSAGES: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < fetchMessageData.Count; i++)
            {
                _chatUI.text += $"\n {fetchMessageData[i].text}";
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MESSAGES: CHANNEL ID: " + fetchMessageData[i].channelId);
                ConsoleUI.Instance.Log("FETCH MESSAGES: AUTHOR ID: " + fetchMessageData[i].authorId);
                ConsoleUI.Instance.Log("FETCH MESSAGES: ID " + fetchMessageData[i].id);
                ConsoleUI.Instance.Log("FETCH MESSAGES: TEXT: " + fetchMessageData[i].text);
                for (int x = 0; x < fetchMessageData[i].tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH MESSAGES: TAGS: " + fetchMessageData[i].tags[x]);
                }
                ConsoleUI.Instance.Log("FETCH MESSAGES: CREATED AT: " + fetchMessageData[i].createdAt);

                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH MESSAGES: PLAYER: AVATAR: " + fetchMessageData[i].player.avatar);
                ConsoleUI.Instance.Log("FETCH MESSAGES: PLAYER: CREDITIALS: " + fetchMessageData[i].player.credentials);
                ConsoleUI.Instance.Log("FETCH MESSAGES: PLAYER: ID: " + fetchMessageData[i].player.id);
                ConsoleUI.Instance.Log("FETCH MESSAGES: PLAYER: NAME: " + fetchMessageData[i].player.name);
                ConsoleUI.Instance.Log("FETCH MESSAGES: PLAYER: PLATFORM TYPE: " + fetchMessageData[i].player.platformType);
                ConsoleUI.Instance.Log("FETCH MESSAGES: PLAYER: PROJECT ID: " + fetchMessageData[i].player.projectId);
                ConsoleUI.Instance.Log("FETCH MESSAGES: PLAYER: SCORE: " + fetchMessageData[i].player.score);
                ConsoleUI.Instance.Log(" ");
            }

        }


        private void OnFetchPersonalMessagesError() => ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: ERROR");
        private void OnFetchPersonalMessages(GP_Data data, bool canLoadMore)
        {
            var fetchMessageData = data.GetList<Message_Data>();
            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < fetchMessageData.Count; i++)
            {
                _chatUI.text += $"\n {fetchMessageData[i].text}";
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: CHANNEL ID: " + fetchMessageData[i].channelId);
                ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: AUTHOR ID: " + fetchMessageData[i].authorId);
                ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: ID " + fetchMessageData[i].id);
                ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: TEXT: " + fetchMessageData[i].text);
                for (int x = 0; x < fetchMessageData[i].tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: TAGS: " + fetchMessageData[i].tags[x]);
                }
                ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: CREATED AT: " + fetchMessageData[i].createdAt);

                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: PLAYER: AVATAR: " + fetchMessageData[i].player.avatar);
                ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: PLAYER: CREDITIALS: " + fetchMessageData[i].player.credentials);
                ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: PLAYER: ID: " + fetchMessageData[i].player.id);
                ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: PLAYER: NAME: " + fetchMessageData[i].player.name);
                ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: PLAYER: PLATFORM TYPE: " + fetchMessageData[i].player.platformType);
                ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: PLAYER: PROJECT ID: " + fetchMessageData[i].player.projectId);
                ConsoleUI.Instance.Log("FETCH PERSONAL MESSAGES: PLAYER: SCORE: " + fetchMessageData[i].player.score);
                ConsoleUI.Instance.Log(" ");
            }
        }


        private void OnFetchFeedMessagesError() => ConsoleUI.Instance.Log("FETCH FEED MESSAGES: ERROR");
        private void OnFetchFeedMessages(GP_Data data, bool canLoadMore)
        {
            var fetchMessageData = data.GetList<Message_Data>();
            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("FETCH FEED MESSAGES: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < fetchMessageData.Count; i++)
            {
                _chatUI.text += $"\n {fetchMessageData[i].text}";
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH FEED MESSAGES: CHANNEL ID: " + fetchMessageData[i].channelId);
                ConsoleUI.Instance.Log("FETCH FEED MESSAGES: AUTHOR ID: " + fetchMessageData[i].authorId);
                ConsoleUI.Instance.Log("FETCH FEED MESSAGES: ID " + fetchMessageData[i].id);
                ConsoleUI.Instance.Log("FETCH FEED MESSAGES: TEXT: " + fetchMessageData[i].text);
                for (int x = 0; x < fetchMessageData[i].tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH FEED MESSAGES: TAGS: " + fetchMessageData[i].tags[x]);
                }
                ConsoleUI.Instance.Log("FETCH FEED MESSAGES: CREATED AT: " + fetchMessageData[i].createdAt);

                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH FEED MESSAGES: PLAYER: AVATAR: " + fetchMessageData[i].player.avatar);
                ConsoleUI.Instance.Log("FETCH FEED MESSAGES: PLAYER: CREDITIALS: " + fetchMessageData[i].player.credentials);
                ConsoleUI.Instance.Log("FETCH FEED MESSAGES: PLAYER: ID: " + fetchMessageData[i].player.id);
                ConsoleUI.Instance.Log("FETCH FEED MESSAGES: PLAYER: NAME: " + fetchMessageData[i].player.name);
                ConsoleUI.Instance.Log("FETCH FEED MESSAGES: PLAYER: PLATFORM TYPE: " + fetchMessageData[i].player.platformType);
                ConsoleUI.Instance.Log("FETCH FEED MESSAGES: PLAYER: PROJECT ID: " + fetchMessageData[i].player.projectId);
                ConsoleUI.Instance.Log("FETCH FEED MESSAGES: PLAYER: SCORE: " + fetchMessageData[i].player.score);
                ConsoleUI.Instance.Log(" ");
            }
        }


        private void OnFetchMoreMessagesError() => ConsoleUI.Instance.Log("FETCH MORE MESSAGES: ERROR");
        private void OnFetchMoreMessages(GP_Data data, bool canLoadMore)
        {
            var fetchMoreMessageData = data.GetList<Message_Data>();

            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("FETCH MORE MESSAGES: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < fetchMoreMessageData.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE MESSAGES: CHANNEL ID: " + fetchMoreMessageData[i].channelId);
                ConsoleUI.Instance.Log("FETCH MORE MESSAGES: AUTHOR ID: " + fetchMoreMessageData[i].authorId);
                ConsoleUI.Instance.Log("FETCH MORE MESSAGES: ID: " + fetchMoreMessageData[i].id);
                ConsoleUI.Instance.Log("FETCH MORE MESSAGES: TEXT: " + fetchMoreMessageData[i].text);
                for (int x = 0; x < fetchMoreMessageData[i].tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH MORE MESSAGES: TAGS: " + fetchMoreMessageData[i].tags[x]);
                }
                ConsoleUI.Instance.Log("FETCH MORE MESSAGES: CREATED AT: " + fetchMoreMessageData[i].createdAt);

                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH MORE MESSAGES: PLAYER: AVATAR: " + fetchMoreMessageData[i].player.avatar);
                ConsoleUI.Instance.Log("FETCH MORE MESSAGES: PLAYER: CREDITIALS: " + fetchMoreMessageData[i].player.credentials);
                ConsoleUI.Instance.Log("FETCH MORE MESSAGES: PLAYER: ID: " + fetchMoreMessageData[i].player.id);
                ConsoleUI.Instance.Log("FETCH MORE MESSAGES: PLAYER: NAME: " + fetchMoreMessageData[i].player.name);
                ConsoleUI.Instance.Log("FETCH MORE MESSAGES: PLAYER: PLATFORM TYPE: " + fetchMoreMessageData[i].player.platformType);
                ConsoleUI.Instance.Log("FETCH MORE MESSAGES: PLAYER: PROJECT ID: " + fetchMoreMessageData[i].player.projectId);
                ConsoleUI.Instance.Log("FETCH MORE MESSAGES: PLAYER: SCORE: " + fetchMoreMessageData[i].player.score);
                ConsoleUI.Instance.Log(" ");
            }
        }

        private void OnFetchMorePersonalMessagesError() => ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: ERROR");
        private void OnFetchMorePersonalMessages(GP_Data data, bool canLoadMore)
        {
            var fetchMoreMessageData = data.GetList<Message_Data>();

            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < fetchMoreMessageData.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: CHANNEL ID: " + fetchMoreMessageData[i].channelId);
                ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: AUTHOR ID: " + fetchMoreMessageData[i].authorId);
                ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: ID: " + fetchMoreMessageData[i].id);
                ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: TEXT: " + fetchMoreMessageData[i].text);
                for (int x = 0; x < fetchMoreMessageData[i].tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: TAGS: " + fetchMoreMessageData[i].tags[x]);
                }
                ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: CREATED AT: " + fetchMoreMessageData[i].createdAt);

                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: PLAYER: AVATAR: " + fetchMoreMessageData[i].player.avatar);
                ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: PLAYER: CREDITIALS: " + fetchMoreMessageData[i].player.credentials);
                ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: PLAYER: ID: " + fetchMoreMessageData[i].player.id);
                ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: PLAYER: NAME: " + fetchMoreMessageData[i].player.name);
                ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: PLAYER: PLATFORM TYPE: " + fetchMoreMessageData[i].player.platformType);
                ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: PLAYER: PROJECT ID: " + fetchMoreMessageData[i].player.projectId);
                ConsoleUI.Instance.Log("FETCH MORE PERSONAL MESSAGES: PLAYER: SCORE: " + fetchMoreMessageData[i].player.score);
                ConsoleUI.Instance.Log(" ");
            }
        }

        private void OnFetchMoreFeedMessagesError() => ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: ERROR");
        private void OnFetchMoreFeedMessages(GP_Data data, bool canLoadMore)
        {
            var fetchMoreMessageData = data.GetList<Message_Data>();

            ConsoleUI.Instance.Log(" ");

            ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < fetchMoreMessageData.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: CHANNEL ID: " + fetchMoreMessageData[i].channelId);
                ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: AUTHOR ID: " + fetchMoreMessageData[i].authorId);
                ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: ID: " + fetchMoreMessageData[i].id);
                ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: TEXT: " + fetchMoreMessageData[i].text);
                for (int x = 0; x < fetchMoreMessageData[i].tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: TAGS: " + fetchMoreMessageData[i].tags[x]);
                }
                ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: CREATED AT: " + fetchMoreMessageData[i].createdAt);

                ConsoleUI.Instance.Log(" ");

                ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: PLAYER: AVATAR: " + fetchMoreMessageData[i].player.avatar);
                ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: PLAYER: CREDITIALS: " + fetchMoreMessageData[i].player.credentials);
                ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: PLAYER: ID: " + fetchMoreMessageData[i].player.id);
                ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: PLAYER: NAME: " + fetchMoreMessageData[i].player.name);
                ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: PLAYER: PLATFORM TYPE: " + fetchMoreMessageData[i].player.platformType);
                ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: PLAYER: PROJECT ID: " + fetchMoreMessageData[i].player.projectId);
                ConsoleUI.Instance.Log("FETCH MORE FEED MESSAGES: PLAYER: SCORE: " + fetchMoreMessageData[i].player.score);
                ConsoleUI.Instance.Log(" ");
            }
        }
    }
}