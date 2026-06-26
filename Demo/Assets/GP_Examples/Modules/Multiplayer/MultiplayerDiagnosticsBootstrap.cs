using System;
using System.Globalization;
using System.Threading.Tasks;
using Examples.Console;
using GamePush;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Examples.Multiplayer
{
    public class MultiplayerDiagnosticsBootstrap : MonoBehaviour
    {
        private const string SceneName = "ExamplesScene";
        private const string RootName = "MultiplayerDiagnostics";
        private const string DefaultSchemaJson = "{\"score\":{\"interpolate\":false},\"ready\":{\"interpolate\":false},\"nickname\":{\"readonly\":true}}";
        private const string DefaultGlobalSchemaJson = "{\"round\":{\"interpolate\":false},\"hostTime\":{\"interpolate\":false}}";
        private const string DefaultInitializerJson = "{\"score\":0,\"ready\":false,\"nickname\":\"player\"}";
        private const string DefaultStateJson = "{\"score\":1,\"ready\":true}";
        private const string DefaultGlobalStateJson = "{\"round\":1,\"hostTime\":0}";
        private const string DefaultMessageJson = "{\"text\":\"hello\"}";
        private const string DefaultEventName = "demo:message";

        private Canvas _canvas;
        private GameObject _panel;
        private RectTransform _content;
        private TMP_InputField _channelInput;
        private TMP_InputField _schemaInput;
        private TMP_InputField _globalSchemaInput;
        private TMP_InputField _initializerInput;
        private TMP_InputField _stateInput;
        private TMP_InputField _globalStateInput;
        private TMP_InputField _eventInput;
        private TMP_InputField _messageInput;
        private TMP_InputField _targetInput;
        private Toggle _echoToggle;
        private bool _eventsSubscribed;
        private bool _messageSubscribed;
        private bool _tickSubscribed;
        private int _tickCount;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            TryInstallInActiveScene();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            TryInstallInActiveScene();
        }

        private static void TryInstallInActiveScene()
        {
            if (SceneManager.GetActiveScene().name != SceneName)
                return;

            if (GameObject.Find(RootName) != null)
                return;

            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
                return;

            GameObject root = new GameObject(RootName);
            DontDestroyOnLoad(root);
            MultiplayerDiagnosticsBootstrap bootstrap = root.AddComponent<MultiplayerDiagnosticsBootstrap>();
            bootstrap.Build(canvas);
        }

        private async void Start()
        {
            await GP_Init.Ready;
            Log("MULTIPLAYER: diagnostics ready");
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void Build(Canvas canvas)
        {
            _canvas = canvas;
            AddNavigationButton();
            BuildPanel();
            SubscribeEvents();
        }

        private void AddNavigationButton()
        {
            Transform modules = GameObject.Find("Modules")?.transform;
            Button template = modules == null ? null : modules.GetComponentInChildren<Button>(true);
            Button button = template == null
                ? CreateButton(modules == null ? _canvas.transform : modules, "MULTIPLAYER", ShowPanel)
                : Instantiate(template, template.transform.parent);

            button.name = "Multiplayer Button";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ShowPanel);

            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
                label.text = "MULTIPLAYER";

            button.gameObject.SetActive(true);
        }

        private void BuildPanel()
        {
            _panel = new GameObject("MULTIPLAYER");
            _panel.transform.SetParent(_canvas.transform, false);
            _panel.SetActive(false);

            RectTransform panelRect = _panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = new Vector2(24f, 24f);
            panelRect.offsetMax = new Vector2(-24f, -24f);

            Image background = _panel.AddComponent<Image>();
            background.color = new Color(0.05f, 0.06f, 0.08f, 0.96f);

            VerticalLayoutGroup layout = _panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.spacing = 12f;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            HorizontalLayoutGroup header = CreateHorizontal(_panel.transform, "Header", 8f);
            CreateLabel(header.transform, "MULTIPLAYER", 28, FontStyles.Bold);
            CreateButton(header.transform, "Close", HidePanel, 120f, 42f);

            ScrollRect scroll = CreateScroll(_panel.transform);
            _content = scroll.content;

            _channelInput = CreateInput("Channel ID", "0");
            _schemaInput = CreateInput("Player schema JSON", DefaultSchemaJson, 82f);
            _globalSchemaInput = CreateInput("Global schema JSON", DefaultGlobalSchemaJson, 70f);
            _initializerInput = CreateInput("Initializer state JSON", DefaultInitializerJson, 70f);
            _stateInput = CreateInput("Player state JSON", DefaultStateJson, 70f);
            _globalStateInput = CreateInput("Global state JSON", DefaultGlobalStateJson, 70f);
            _eventInput = CreateInput("Event name", DefaultEventName);
            _messageInput = CreateInput("Message JSON", DefaultMessageJson, 70f);
            _targetInput = CreateInput("Target playerId or all", "all");
            _echoToggle = CreateToggle("Echo custom event locally");

            CreateSection("Connection");
            CreateButtonRow(("Connect", ConnectAsync), ("Disconnect", DisconnectAsync));
            CreateButtonRow(("Read isConnected", ReadIsConnected), ("Read isHost", ReadIsHost));
            CreateButtonRow(("Read players", ReadConnectedPlayers), ("Read stats", ReadNetworkStats));

            CreateSection("State");
            CreateButtonRow(("FAST mode", SetFastMode), ("SMOOTH mode", SetSmoothMode));
            CreateButtonRow(("Define player schema", DefinePlayerSchema), ("Define global schema", DefineGlobalSchema));
            CreateButtonRow(("Enable initializer", EnableInitializerAsync), ("Disable initializer", DisableInitializerAsync));
            CreateButtonRow(("Set player state", SetPlayerState), ("Set global state", SetGlobalState));
            CreateButtonRow(("Read myState", ReadMyState), ("Read playersState", ReadPlayersState), ("Read globalState", ReadGlobalState));

            CreateSection("Realtime");
            CreateButtonRow(("Send message", SendMessage), ("Off message", DisableMessageListener), ("Off tick", DisableTickListener));
        }

        private void SubscribeEvents()
        {
            if (_eventsSubscribed)
                return;

            GP_Multiplayer.on("connect", OnConnect);
            GP_Multiplayer.on("disconnect", OnDisconnect);
            GP_Multiplayer.on("error:connect", OnConnectError);
            GP_Multiplayer.on("error:disconnect", OnDisconnectError);
            GP_Multiplayer.on("error:sendState", OnSendStateError);
            GP_Multiplayer.on("playerJoined", OnPlayerJoined);
            GP_Multiplayer.on("playerLeft", OnPlayerLeft);
            GP_Multiplayer.on("playersUpdated", OnPlayersUpdated);
            GP_Multiplayer.on("globalStateUpdated", OnGlobalStateUpdated);
            GP_Multiplayer.on("customEvent", OnCustomEvent);
            GP_Multiplayer.on("hostMigrated", OnHostMigrated);
            GP_Multiplayer.on("becameHost", OnBecameHost);
            GP_Multiplayer.on("becamePeer", OnBecamePeer);
            BindMessageListener();
            BindTickListener();
            _eventsSubscribed = true;
        }

        private void UnsubscribeEvents()
        {
            if (!_eventsSubscribed)
                return;

            GP_Multiplayer.off("connect", OnConnect);
            GP_Multiplayer.off("disconnect", OnDisconnect);
            GP_Multiplayer.off("error:connect", OnConnectError);
            GP_Multiplayer.off("error:disconnect", OnDisconnectError);
            GP_Multiplayer.off("error:sendState", OnSendStateError);
            GP_Multiplayer.off("playerJoined", OnPlayerJoined);
            GP_Multiplayer.off("playerLeft", OnPlayerLeft);
            GP_Multiplayer.off("playersUpdated", OnPlayersUpdated);
            GP_Multiplayer.off("globalStateUpdated", OnGlobalStateUpdated);
            GP_Multiplayer.off("customEvent", OnCustomEvent);
            GP_Multiplayer.off("hostMigrated", OnHostMigrated);
            GP_Multiplayer.off("becameHost", OnBecameHost);
            GP_Multiplayer.off("becamePeer", OnBecamePeer);
            UnbindMessageListener();
            UnbindTickListener();
            _eventsSubscribed = false;
        }

        private async void ConnectAsync()
        {
            if (!TryGetChannelId(out int channelId))
            {
                Log("MULTIPLAYER: connect skipped, channelId is invalid");
                return;
            }

            try
            {
                Log($"MULTIPLAYER: connect channelId={channelId}");
                MultiplayerConnectResultData result =
                    await GP_Multiplayer.connect(new MultiplayerChannelQuery { channelId = channelId });
                Log($"MULTIPLAYER: connect result success={result != null && result.success}");
            }
            catch (Exception exception)
            {
                LogException("MULTIPLAYER: connect exception", exception);
            }
        }

        private async void DisconnectAsync()
        {
            if (!TryGetChannelId(out int channelId))
            {
                Log("MULTIPLAYER: disconnect skipped, channelId is invalid");
                return;
            }

            try
            {
                Log($"MULTIPLAYER: disconnect channelId={channelId}");
                await GP_Multiplayer.disconnect(new MultiplayerChannelQuery { channelId = channelId });
                Log("MULTIPLAYER: disconnect completed");
            }
            catch (Exception exception)
            {
                LogException("MULTIPLAYER: disconnect exception", exception);
            }
        }

        private void SetFastMode()
        {
            GP_Multiplayer.setMode(MultiplayerMode.FAST);
            Log("MULTIPLAYER: mode fast");
        }

        private void SetSmoothMode()
        {
            GP_Multiplayer.setMode(MultiplayerMode.SMOOTH);
            Log("MULTIPLAYER: mode smooth");
        }

        private void DefinePlayerSchema()
        {
            GP_Multiplayer.definePlayerSchema(new GP_Data(GetText(_schemaInput, DefaultSchemaJson)));
            Log("MULTIPLAYER: define player schema");
        }

        private void DefineGlobalSchema()
        {
            GP_Multiplayer.defineGlobalSchema(new GP_Data(GetText(_globalSchemaInput, DefaultGlobalSchemaJson)));
            Log("MULTIPLAYER: define global schema");
        }

        private async void EnableInitializerAsync()
        {
            await GP_Multiplayer.setPlayerInitializer(CreateInitializerPayloadAsync);
            Log("MULTIPLAYER: initializer enabled");
        }

        private async void DisableInitializerAsync()
        {
            await GP_Multiplayer.setPlayerInitializer((Func<int, MultiplayerConnectedPlayerData, GP_Data>)null);
            Log("MULTIPLAYER: initializer disabled");
        }

        private Task<GP_Data> CreateInitializerPayloadAsync(int playerId, MultiplayerConnectedPlayerData player)
        {
            string json = GetText(_initializerInput, DefaultInitializerJson).Trim();
            string suffix =
                $",\"playerId\":{playerId.ToString(CultureInfo.InvariantCulture)},\"isHost\":{(player != null && player.isHost ? "true" : "false")}";

            if (json.EndsWith("}", StringComparison.Ordinal))
                json = json.Substring(0, json.Length - 1) + suffix + "}";

            Log($"MULTIPLAYER: initializer request playerId={playerId}");
            return Task.FromResult(new GP_Data(json));
        }

        private void SetPlayerState()
        {
            GP_Multiplayer.setPlayerState(new GP_Data(GetText(_stateInput, DefaultStateJson)));
            Log("MULTIPLAYER: set player state");
        }

        private void SetGlobalState()
        {
            GP_Multiplayer.setGlobalState(new GP_Data(GetText(_globalStateInput, DefaultGlobalStateJson)));
            Log("MULTIPLAYER: set global state");
        }

        private void SendMessage()
        {
            MultiplayerSendMessageOptions options = new MultiplayerSendMessageOptions
            {
                target = GetText(_targetInput, "all"),
                echo = _echoToggle != null && _echoToggle.isOn
            };

            GP_Multiplayer.sendMessage(
                GetText(_eventInput, DefaultEventName),
                new GP_Data(GetText(_messageInput, DefaultMessageJson)),
                options);

            Log($"MULTIPLAYER: send message target={options.target} echo={options.echo}");
        }

        private void ReadIsConnected() => Log($"MULTIPLAYER: isConnected={GP_Multiplayer.isConnected}");
        private void ReadIsHost() => Log($"MULTIPLAYER: isHost={GP_Multiplayer.isHost}");
        private void ReadConnectedPlayers() => LogJson("MULTIPLAYER: connectedPlayers", GP_Multiplayer.connectedPlayers);
        private void ReadNetworkStats() => LogJson("MULTIPLAYER: networkStats", GP_Multiplayer.networkStats);
        private void ReadMyState() => LogJson("MULTIPLAYER: myState", GP_Multiplayer.myState);
        private void ReadPlayersState() => LogJson("MULTIPLAYER: playersState", GP_Multiplayer.playersState);
        private void ReadGlobalState() => LogJson("MULTIPLAYER: globalState", GP_Multiplayer.globalState);

        private void DisableMessageListener()
        {
            UnbindMessageListener();
            Log("MULTIPLAYER: onMessage disabled; customEvent event remains subscribed");
        }

        private void DisableTickListener()
        {
            UnbindTickListener();
            Log("MULTIPLAYER: onTick disabled");
        }

        private void BindMessageListener()
        {
            if (_messageSubscribed)
                return;

            GP_Multiplayer.onMessage(OnMessage);
            _messageSubscribed = true;
        }

        private void UnbindMessageListener()
        {
            if (!_messageSubscribed)
                return;

            GP_Multiplayer.offMessage(OnMessage);
            _messageSubscribed = false;
        }

        private void BindTickListener()
        {
            if (_tickSubscribed)
                return;

            GP_Multiplayer.onTick(OnTick);
            _tickSubscribed = true;
            _tickCount = 0;
        }

        private void UnbindTickListener()
        {
            if (!_tickSubscribed)
                return;

            GP_Multiplayer.offTick(OnTick);
            _tickSubscribed = false;
        }

        private void OnConnect(GP_Data payload) => LogJson("MULTIPLAYER: event connect", payload);
        private void OnDisconnect(GP_Data payload) => LogJson("MULTIPLAYER: event disconnect", payload);
        private void OnConnectError(GP_Data payload) => LogJson("MULTIPLAYER: event error:connect", payload);
        private void OnDisconnectError(GP_Data payload) => LogJson("MULTIPLAYER: event error:disconnect", payload);
        private void OnSendStateError(GP_Data payload) => LogJson("MULTIPLAYER: event error:sendState", payload);
        private void OnPlayerJoined(GP_Data payload) => LogJson("MULTIPLAYER: event playerJoined", payload);
        private void OnPlayerLeft(GP_Data payload) => LogJson("MULTIPLAYER: event playerLeft", payload);
        private void OnPlayersUpdated(GP_Data payload) => LogJson("MULTIPLAYER: event playersUpdated", payload);
        private void OnGlobalStateUpdated(GP_Data payload) => LogJson("MULTIPLAYER: event globalStateUpdated", payload);
        private void OnCustomEvent(GP_Data payload) => LogJson("MULTIPLAYER: event customEvent", payload);
        private void OnHostMigrated(GP_Data payload) => LogJson("MULTIPLAYER: event hostMigrated", payload);
        private void OnBecameHost() => Log("MULTIPLAYER: event becameHost");
        private void OnBecamePeer() => Log("MULTIPLAYER: event becamePeer");
        private void OnMessage(GP_Data payload) => LogJson("MULTIPLAYER: onMessage", payload);

        private void OnTick(float delta)
        {
            _tickCount++;
            if (_tickCount == 1 || _tickCount % 30 == 0)
                Log($"MULTIPLAYER: tick count={_tickCount} delta={delta.ToString("0.000", CultureInfo.InvariantCulture)}");
        }

        private void ShowPanel() => _panel.SetActive(true);
        private void HidePanel() => _panel.SetActive(false);

        private bool TryGetChannelId(out int channelId)
        {
            return int.TryParse(
                GetText(_channelInput, "0"),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out channelId) && channelId > 0;
        }

        private static string GetText(TMP_InputField input, string fallback)
        {
            string value = input == null ? string.Empty : input.text;
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static void LogJson(string label, GP_Data payload)
        {
            Log($"{label}: {payload?.Data ?? "null"}");
        }

        private static void LogException(string label, Exception exception)
        {
            Log($"{label}: {exception.GetType().Name}: {exception.Message}");
        }

        private static void Log(string message)
        {
            if (ConsoleUI.Instance != null)
                ConsoleUI.Instance.Log(message);
            else
                Debug.Log(message);
        }

        private TMP_InputField CreateInput(string label, string value, float height = 46f)
        {
            VerticalLayoutGroup row = CreateVertical(_content, label + " Row", 4f);
            CreateLabel(row.transform, label, 17, FontStyles.Normal);

            GameObject inputObject = new GameObject(label + " Input");
            inputObject.transform.SetParent(row.transform, false);
            Image image = inputObject.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.12f);

            TMP_InputField input = inputObject.AddComponent<TMP_InputField>();
            RectTransform rect = inputObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, height);

            TextMeshProUGUI text = CreateText(inputObject.transform, value, 16, FontStyles.Normal);
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.margin = new Vector4(10f, 4f, 10f, 4f);

            TextMeshProUGUI placeholder = CreateText(inputObject.transform, label, 16, FontStyles.Italic);
            placeholder.color = new Color(1f, 1f, 1f, 0.35f);
            placeholder.alignment = TextAlignmentOptions.MidlineLeft;
            placeholder.margin = new Vector4(10f, 4f, 10f, 4f);

            input.textComponent = text;
            input.placeholder = placeholder;
            input.text = value;
            input.lineType = height > 50f ? TMP_InputField.LineType.MultiLineNewline : TMP_InputField.LineType.SingleLine;

            LayoutElement layout = inputObject.AddComponent<LayoutElement>();
            layout.preferredHeight = height;
            return input;
        }

        private Toggle CreateToggle(string label)
        {
            GameObject toggleObject = new GameObject(label);
            toggleObject.transform.SetParent(_content, false);

            Toggle toggle = toggleObject.AddComponent<Toggle>();
            HorizontalLayoutGroup layout = toggleObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            toggleObject.AddComponent<LayoutElement>().preferredHeight = 38f;

            GameObject box = new GameObject("Box");
            box.transform.SetParent(toggleObject.transform, false);
            Image boxImage = box.AddComponent<Image>();
            boxImage.color = new Color(1f, 1f, 1f, 0.22f);
            box.AddComponent<LayoutElement>().preferredWidth = 26f;

            GameObject check = new GameObject("Checkmark");
            check.transform.SetParent(box.transform, false);
            Image checkImage = check.AddComponent<Image>();
            checkImage.color = new Color(0.2f, 0.8f, 0.45f, 1f);

            RectTransform checkRect = check.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.2f, 0.2f);
            checkRect.anchorMax = new Vector2(0.8f, 0.8f);
            checkRect.offsetMin = Vector2.zero;
            checkRect.offsetMax = Vector2.zero;

            CreateLabel(toggleObject.transform, label, 17, FontStyles.Normal);
            toggle.targetGraphic = boxImage;
            toggle.graphic = checkImage;
            return toggle;
        }

        private void CreateSection(string title)
        {
            TextMeshProUGUI text = CreateLabel(_content, title, 20, FontStyles.Bold);
            text.color = new Color(0.55f, 0.85f, 1f, 1f);
        }

        private void CreateButtonRow(params (string title, UnityAction action)[] buttons)
        {
            HorizontalLayoutGroup row = CreateHorizontal(_content, "Button Row", 8f);
            foreach ((string title, UnityAction action) in buttons)
                CreateButton(row.transform, title, action);
        }

        private ScrollRect CreateScroll(Transform parent)
        {
            GameObject scrollObject = new GameObject("Scroll View");
            scrollObject.transform.SetParent(parent, false);
            ScrollRect scroll = scrollObject.AddComponent<ScrollRect>();
            scrollObject.AddComponent<LayoutElement>().flexibleHeight = 1f;

            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollObject.transform, false);
            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = new Color(0f, 0f, 0f, 0f);
            viewport.AddComponent<Mask>().showMaskGraphic = false;

            RectTransform viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;

            VerticalLayoutGroup contentLayout = content.AddComponent<VerticalLayoutGroup>();
            contentLayout.spacing = 8f;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;
            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.viewport = viewportRect;
            scroll.content = contentRect;
            scroll.horizontal = false;
            return scroll;
        }

        private static Button CreateButton(Transform parent, string title, UnityAction action, float width = 0f, float height = 42f)
        {
            GameObject buttonObject = new GameObject(title);
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.13f, 0.38f, 0.58f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            button.onClick.AddListener(action);

            TextMeshProUGUI label = CreateText(buttonObject.transform, title, 16, FontStyles.Bold);
            label.alignment = TextAlignmentOptions.Center;

            LayoutElement layout = buttonObject.AddComponent<LayoutElement>();
            layout.preferredHeight = height;
            if (width > 0f)
                layout.preferredWidth = width;
            else
                layout.flexibleWidth = 1f;

            return button;
        }

        private static TextMeshProUGUI CreateLabel(Transform parent, string text, int fontSize, FontStyles style)
        {
            TextMeshProUGUI label = CreateText(parent, text, fontSize, style);
            label.gameObject.AddComponent<LayoutElement>().preferredHeight = fontSize + 12f;
            return label;
        }

        private static TextMeshProUGUI CreateText(Transform parent, string text, int fontSize, FontStyles style)
        {
            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(parent, false);
            TextMeshProUGUI tmp = textObject.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.color = Color.white;

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return tmp;
        }

        private static HorizontalLayoutGroup CreateHorizontal(Transform parent, string name, float spacing)
        {
            GameObject row = new GameObject(name);
            row.transform.SetParent(parent, false);
            HorizontalLayoutGroup layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            row.AddComponent<LayoutElement>().preferredHeight = 48f;
            return layout;
        }

        private static VerticalLayoutGroup CreateVertical(Transform parent, string name, float spacing)
        {
            GameObject row = new GameObject(name);
            row.transform.SetParent(parent, false);
            VerticalLayoutGroup layout = row.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            row.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return layout;
        }
    }
}
