using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Native;
using GamePush.Overlays.Widgets;

namespace GamePush.Overlays.Views
{
    /// <summary>
    /// Support requests. Compact swaps the thread in over the list with a back button;
    /// Wide keeps the list on the left and the thread on the right.
    /// </summary>
    public sealed class GP_FeedbacksView : GP_OverlayView
    {
        const int PageSize = 20;

        [Header("List")]
        public GameObject listPanel;

        public GP_OverlayList feedbackList;
        public Button newButton;

        [Header("Thread")]
        public GameObject threadPanel;

        public GP_OverlayList threadList;
        public TMP_Text threadTitle;
        public TMP_InputField input;
        public Button sendButton;
        public Button backButton;
        public RectTransform composer;

        public GP_OverlayLayoutMode layoutMode;

        protected override Transform StatusHost =>
            listPanel != null ? listPanel.transform : null;

        readonly List<FeedbackData> _feedbacks = new List<FeedbackData>();

        GP_FeedbacksArgs _args = new GP_FeedbacksArgs();
        FeedbackData _selected;
        bool _composingNew;
        bool _loadingMore;
        bool _canLoadMore;
        float _keyboardInset;

        public override void Bind(object args)
        {
            _args = args as GP_FeedbacksArgs ?? new GP_FeedbacksArgs();
            _feedbacks.Clear();
            _selected = null;
            _composingNew = false;

            SetTitle(GP_OverlayStrings.Feedbacks);
            ShowLoading();

            if (input != null)
            {
                input.text = "";
                if (input.placeholder is TMP_Text placeholder)
                    placeholder.text = GP_OverlayStrings.MessagePlaceholder;
            }

            if (sendButton != null)
            {
                sendButton.onClick.RemoveAllListeners();
                sendButton.onClick.AddListener(Send);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(BackToList);
            }

            if (newButton != null)
            {
                newButton.onClick.RemoveAllListeners();
                newButton.onClick.AddListener(StartNew);
            }

            if (feedbackList != null)
            {
                feedbackList.OnNeedMore += LoadMore;
                feedbackList.SetCanLoadMore(false);
            }

            if (layoutMode != null)
                layoutMode.ModeChanged += OnModeChanged;

            ApplyPanels();
            LoadPage(0);

            if (!string.IsNullOrEmpty(_args.feedbackId))
                OpenById(_args.feedbackId);
        }

        protected override void OnClosing()
        {
            if (feedbackList != null)
                feedbackList.OnNeedMore -= LoadMore;
            if (layoutMode != null)
                layoutMode.ModeChanged -= OnModeChanged;
            if (_selected != null || _composingNew)
                GP_Feedbacks.NativeFireCloseFeedback();
            GP_Feedbacks.NativeFireCloseList();
        }

        protected override void Update()
        {
            base.Update();
            ApplyKeyboardInset();
        }

        void ApplyKeyboardInset()
        {
            GP_OverlayKeyboardInset.Apply(composer, Host != null ? Host.Root : null, ref _keyboardInset);
        }

        void OnModeChanged(GP_LayoutMode mode) => ApplyPanels();

        bool Wide => layoutMode != null && layoutMode.Mode == GP_LayoutMode.Wide;

        void ApplyPanels()
        {
            var threadOpen = _selected != null || _composingNew;
            if (listPanel != null)
                listPanel.SetActive(Wide || !threadOpen);
            if (threadPanel != null)
                threadPanel.SetActive(threadOpen);
            if (backButton != null)
                backButton.gameObject.SetActive(!Wide && threadOpen);
        }

        void LoadPage(int offset)
        {
            NativeFeedbacks.FetchForOverlay(_args.type, _args.status, PageSize, offset, page =>
            {
                _loadingMore = false;
                if (offset == 0)
                    _feedbacks.Clear();
                _feedbacks.AddRange(page.items);
                _canLoadMore = page.canLoadMore;
                feedbackList?.SetCanLoadMore(page.canLoadMore);
                RenderList();
                if (offset == 0)
                    GP_Feedbacks.NativeFireOpenList();
            }, error =>
            {
                _loadingMore = false;
                ShowError(error);
            });
        }

        void LoadMore()
        {
            if (_loadingMore || !_canLoadMore)
                return;
            _loadingMore = true;
            LoadPage(_feedbacks.Count);
        }

        void RenderList()
        {
            if (_feedbacks.Count == 0)
            {
                ShowEmpty();
                feedbackList?.Clear();
                return;
            }

            SetStatus(null);
            feedbackList?.Bind(_feedbacks.Count, (row, index) =>
            {
                var component = row.GetComponent<GP_FeedbackRow>();
                if (component == null)
                    return;
                var feedback = _feedbacks[index];
                var selected = _selected != null && _selected.id == feedback.id;
                component.Bind(feedback, index, selected, () => Select(feedback));
            }, Skin.feedbackRow);
        }

        void OpenById(string feedbackId)
        {
            NativeFeedbacks.FetchOne(feedbackId, feedback =>
            {
                _selected = feedback;
                _composingNew = false;
                ApplyPanels();
                RenderList();
                RenderThread();
                GP_Feedbacks.NativeFireOpenFeedback();
            }, error =>
            {
                ShowError(error);
                GP_Feedbacks.NativeFireOpenFeedbackError(error);
            });
        }

        void Select(FeedbackData feedback)
        {
            // The list payload carries only the head of each thread; pull the full one.
            OpenById(feedback.id);
        }

        void StartNew()
        {
            _selected = null;
            _composingNew = true;
            ApplyPanels();
            RenderList();
            RenderThread();
        }

        void BackToList()
        {
            _selected = null;
            _composingNew = false;
            ApplyPanels();
            RenderList();
            RenderThread();
        }

        void RenderThread()
        {
            if (threadTitle != null)
            {
                threadTitle.text = _composingNew || _selected == null
                    ? GP_OverlayStrings.NewFeedback
                    : string.IsNullOrEmpty(_selected.text) ? GP_OverlayStrings.NewFeedback : _selected.text;
                GP_OverlayTone.Paint(threadTitle, Skin, GP_OverlayColorRole.Text);
            }

            if (input != null)
                input.text = "";

            if (threadList == null)
                return;

            var messages = _selected?.messages;
            if (messages == null || messages.Length == 0)
            {
                threadList.Clear();
                return;
            }

            threadList.Bind(messages.Length, (row, index) =>
            {
                var component = row.GetComponent<GP_MessageRow>();
                if (component == null)
                    return;
                var message = messages[index];
                // "PLAYER" marks the player's own side of the thread; anything else is support.
                var isOwn = string.Equals(message.author, "PLAYER", System.StringComparison.OrdinalIgnoreCase);
                component.Bind(message, isOwn);
            }, Skin.messageRow);
            threadList.ScrollToBottom();
        }

        void Send()
        {
            if (input == null)
                return;
            var text = input.text?.Trim();
            if (string.IsNullOrEmpty(text))
                return;
            input.text = "";

            if (_composingNew || _selected == null)
            {
                var draft = new FeedbackData { text = text, type = _args.type ?? "" };
                NativeFeedbacks.Send(draft);
                // Fetch runs again so the new request shows up with its server id.
                _composingNew = false;
                LoadPage(0);
                ApplyPanels();
                return;
            }

            NativeFeedbacks.SendMessage(new FeedbackMessageData
            {
                feedbackId = _selected.id,
                text = text
            }, message =>
            {
                var list = new List<FeedbackMessageData>(_selected.messages ?? System.Array.Empty<FeedbackMessageData>())
                {
                    message
                };
                _selected.messages = list.ToArray();
                RenderThread();
            }, ShowError);
        }
    }
}
