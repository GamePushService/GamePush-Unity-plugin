using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Native;
using GamePush.Overlays.Widgets;

namespace GamePush.Overlays.Views
{
    /// <summary>
    /// One prefab serves channel chat, personal chat and the player feed. Compact keeps the member
    /// list in a drawer behind a button; Wide shows the feed and the members side by side.
    /// </summary>
    public sealed class GP_ChatView : GP_OverlayView
    {
        const int PageSize = 30;

        [Header("Feed")]
        public GP_OverlayList messageList;
        public GameObject messagesPanel;

        public GP_OverlayLayoutMode layoutMode;

        [Header("Composer")]
        public TMP_InputField input;

        public Button sendButton;
        public RectTransform composer;

        [Header("Members")]
        public GameObject membersPanel;

        public GP_OverlayList memberList;
        public Button membersToggle;
        public GameObject compactTabs;
        public Button messagesTab;
        public Button membersTab;
        public TMP_Text membersTitle;

        protected override Transform StatusHost =>
            messagesPanel != null ? messagesPanel.transform : null;

        readonly List<NativeChatMessage> _messages = new List<NativeChatMessage>();
        readonly Dictionary<string, int> _index = new Dictionary<string, int>();

        GP_ChatArgs _args = new GP_ChatArgs();
        bool _loadingMore;
        bool _membersVisible;
        float _keyboardInset;

        public override void Bind(object args)
        {
            _args = args as GP_ChatArgs ?? new GP_ChatArgs();
            _messages.Clear();
            _index.Clear();
            _membersVisible = false;
            messageList?.Clear();

            SetTitle(_args.scope == NativeChatScope.Feed ? GP_OverlayStrings.Feed : GP_OverlayStrings.Chat);
            ShowLoading();

            if (input != null)
            {
                input.text = "";
                if (input.placeholder is TMP_Text placeholder)
                    placeholder.text = GP_OverlayStrings.MessagePlaceholder;
                input.onSubmit.RemoveAllListeners();
                input.onSubmit.AddListener(_ => Send());
            }

            if (sendButton != null)
            {
                sendButton.onClick.RemoveAllListeners();
                sendButton.onClick.AddListener(Send);
            }

            if (messageList != null)
            {
                messageList.OnNeedMore += LoadMore;
                messageList.SetCanLoadMore(true);
            }

            if (membersToggle != null)
            {
                membersToggle.onClick.RemoveAllListeners();
                membersToggle.onClick.AddListener(ToggleMembers);
                membersToggle.gameObject.SetActive(false);
            }

            if (messagesTab != null)
            {
                messagesTab.onClick.RemoveAllListeners();
                messagesTab.onClick.AddListener(() => SetMembersVisible(false));
            }

            if (membersTab != null)
            {
                membersTab.onClick.RemoveAllListeners();
                membersTab.onClick.AddListener(() => SetMembersVisible(true));
            }

            if (membersTitle != null)
                membersTitle.text = GP_OverlayStrings.Members;

            if (layoutMode != null)
                layoutMode.ModeChanged += OnModeChanged;
            ApplyMembersVisibility();

            // Reopening a chat of a different scope rebinds this view instead of creating a new
            // one, so drop any previous subscription before taking a fresh one.
            NativeChannelsRealtime.MessageReceived -= OnLiveMessage;
            NativeChannelsRealtime.MessageReceived += OnLiveMessage;

            if (_args.scope != NativeChatScope.Channel)
            {
                LoadPage(0, true);
                return;
            }

            NativeChannels.EnsureJoined(_args.target, () =>
            {
                LoadPage(0, true);
                LoadMembers();
            }, error =>
            {
                ShowError(error);
                GP_Channels.NativeFireOpenChatError();
            });
        }

        protected override void OnClosing()
        {
            NativeChannelsRealtime.MessageReceived -= OnLiveMessage;
            NativeChannelsRealtime.Stop();
            if (messageList != null)
                messageList.OnNeedMore -= LoadMore;
            if (layoutMode != null)
                layoutMode.ModeChanged -= OnModeChanged;
            GP_Channels.NativeFireCloseChat();
        }

        protected override void Update()
        {
            base.Update();
            ApplyKeyboardInset();
        }

        /// <summary>Lifts the composer above the Android soft keyboard.</summary>
        void ApplyKeyboardInset()
        {
            GP_OverlayKeyboardInset.Apply(composer, Host != null ? Host.Root : null, ref _keyboardInset);
        }

        void OnModeChanged(GP_LayoutMode mode) => ApplyMembersVisibility();

        void ToggleMembers() => SetMembersVisible(!_membersVisible);

        void SetMembersVisible(bool visible)
        {
            _membersVisible = visible;
            ApplyMembersVisibility();
        }

        void ApplyMembersVisibility()
        {
            if (membersPanel == null)
                return;
            var wide = layoutMode != null && layoutMode.Mode == GP_LayoutMode.Wide;
            var channel = _args.scope == NativeChatScope.Channel;
            var showTabs = channel && !wide;
            if (compactTabs != null)
                compactTabs.SetActive(showTabs);
            if (membersToggle != null)
                membersToggle.gameObject.SetActive(showTabs && compactTabs == null);
            StyleTab(messagesTab, !_membersVisible);
            StyleTab(membersTab, _membersVisible);
            var showMembers = channel && (wide || _membersVisible);
            membersPanel.SetActive(showMembers);
            if (messagesPanel != null)
                messagesPanel.SetActive(wide || !_membersVisible || !channel);
            if (composer != null)
                composer.gameObject.SetActive(wide || !_membersVisible || !channel);
        }

        static void StyleTab(Button tab, bool selected)
        {
            if (tab == null)
                return;
            var chip = tab.GetComponent<GP_OverlayChip>();
            if (chip != null)
            {
                chip.Bind(tab.name == "MembersTab" ? GP_OverlayStrings.Members : GP_OverlayStrings.Messages,
                    null, selected);
                return;
            }

            var label = tab.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.color = selected ? GP_OverlaySkin.Instance.accent : GP_OverlaySkin.Instance.textMuted;
        }

        void LoadPage(int offset, bool initial)
        {
            NativeChannels.FetchMessagesForOverlay(_args.scope, _args.target, _args.tags, PageSize, offset,
                page =>
                {
                    _loadingMore = false;
                    Merge(page.items);
                    messageList?.SetCanLoadMore(page.more);
                    Render(initial);
                    if (initial)
                    {
                        GP_Channels.NativeFireOpenChat();
                        NativeChannelsRealtime.Watch(_args.scope, _args.target, _args.tags, _messages);
                    }
                },
                error =>
                {
                    _loadingMore = false;
                    if (initial)
                    {
                        ShowError(error);
                        GP_Channels.NativeFireOpenChatError();
                    }
                });
        }

        void LoadMore()
        {
            if (_loadingMore)
                return;
            _loadingMore = true;
            LoadPage(_messages.Count, false);
        }

        void Merge(List<NativeChatMessage> incoming)
        {
            if (incoming == null)
                return;
            foreach (var message in incoming)
            {
                if (string.IsNullOrEmpty(message.id) || _index.ContainsKey(message.id))
                    continue;
                _index[message.id] = _messages.Count;
                _messages.Add(message);
            }
            // The API returns newest first; the feed reads oldest at the top.
            _messages.Sort((a, b) => string.CompareOrdinal(a.createdAt, b.createdAt));
            _index.Clear();
            for (var i = 0; i < _messages.Count; i++)
                _index[_messages[i].id] = i;
        }

        void OnLiveMessage(NativeChatMessage message)
        {
            if (message == null || string.IsNullOrEmpty(message.id) || _index.ContainsKey(message.id))
                return;
            Merge(new List<NativeChatMessage> { message });
            Render(true);
        }

        void Render(bool scrollToBottom)
        {
            if (_messages.Count == 0)
            {
                ShowEmpty();
                messageList?.Clear();
                return;
            }

            SetStatus(null);
            var selfId = NativePlayer.Id;
            messageList?.Bind(_messages.Count, (row, index) =>
            {
                var component = row.GetComponent<GP_MessageRow>();
                if (component == null)
                    return;
                var message = _messages[index];
                var isOwn = message.authorId == selfId;
                component.Bind(message, isOwn, isOwn ? () => Delete(message) : (System.Action)null);
            }, Skin.messageRow);

            if (scrollToBottom)
                messageList?.ScrollToBottom();
        }

        void Send()
        {
            if (input == null)
                return;
            var text = input.text?.Trim();
            if (string.IsNullOrEmpty(text))
                return;

            input.text = "";
            input.ActivateInputField();

            NativeChannels.SendMessage(_args.scope, _args.target, text, _args.tags, message =>
            {
                NativeChannelsRealtime.Acknowledge(message);
                Merge(new List<NativeChatMessage> { message });
                Render(true);
            }, ShowError);
        }

        void Delete(NativeChatMessage message)
        {
            NativeChannels.DeleteMessage(message.id);
            if (_index.TryGetValue(message.id, out var index))
            {
                _messages.RemoveAt(index);
                _index.Clear();
                for (var i = 0; i < _messages.Count; i++)
                    _index[_messages[i].id] = i;
                Render(false);
            }
        }

        void LoadMembers()
        {
            if (memberList == null)
                return;
            GP_Channels.OnFetchMembers += OnMembers;
            GP_Channels.FetchMembers(new FetchMembersFilter(_args.target));
        }

        void OnMembers(GP_Data data, bool more)
        {
            GP_Channels.OnFetchMembers -= OnMembers;
            if (memberList == null)
                return;
            var items = GpJson.SplitArray(data?.Data);
            memberList.Bind(items.Count, (row, index) =>
            {
                var component = row.GetComponent<GP_MemberRow>();
                if (component == null)
                    return;
                var json = items[index];
                var playerId = GpJson.GetInt(json, "id");
                component.Bind(json, index, true, true,
                    () => NativeChannels.Mute(_args.target, playerId, 600, null),
                    () => NativeChannels.Kick(_args.target, playerId));
            }, Skin.memberRow);
        }
    }
}
