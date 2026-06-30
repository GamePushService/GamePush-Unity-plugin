using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Examples.Console;
using GamePush;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Examples.Multiplayer
{
    public class Multiplayer : MonoBehaviour
    {
        private const string DefaultPlayerSchemaJson = "{\"score\":{\"interpolate\":false},\"ready\":{\"interpolate\":false},\"nickname\":{\"readonly\":true}}";
        private const string DefaultGlobalSchemaJson = "{\"round\":{\"interpolate\":false},\"hostTime\":{\"interpolate\":false}}";
        private const string DefaultInitializerJson = "{\"score\":0,\"ready\":false,\"nickname\":\"player\"}";
        private const string DefaultPlayerStateJson = "{\"score\":1,\"ready\":true}";
        private const string DefaultGlobalStateJson = "{\"round\":1,\"hostTime\":0}";
        private const string DefaultMessageJson = "{\"text\":\"hello\"}";
        private const string DefaultEventName = "demo:message";
        private const string DefaultBroadcastTarget = "all";
        private const int TickLogInterval = 30;

        [Header("Inputs")]
        [SerializeField] private TMP_InputField _channelIdInput;
        [SerializeField] private TMP_InputField _schemaJsonInput;
        [SerializeField] private TMP_InputField _globalSchemaJsonInput;
        [SerializeField] private TMP_InputField _initializerStateJsonInput;
        [SerializeField] private TMP_InputField _stateJsonInput;
        [SerializeField] private TMP_InputField _globalStateJsonInput;
        [SerializeField] private TMP_InputField _eventNameInput;
        [SerializeField] private TMP_InputField _messageJsonInput;
        [SerializeField] private TMP_InputField _targetInput;
        [SerializeField] private TMP_InputField _echoInput;

        [Header("Buttons")]
        [SerializeField] private Button _connectButton;
        [SerializeField] private Button _disconnectButton;
        [SerializeField] private Button _fastModeButton;
        [SerializeField] private Button _smoothModeButton;
        [SerializeField] private Button _defineSchemaButton;
        [SerializeField] private Button _defineGlobalSchemaButton;
        [SerializeField] private Button _enableInitializerButton;
        [SerializeField] private Button _enableAsyncInitializerButton;
        [SerializeField] private Button _disableInitializerButton;
        [SerializeField] private Button _setPlayerStateButton;
        [SerializeField] private Button _setGlobalStateButton;
        [SerializeField] private Button _sendMessageButton;
        [SerializeField] private Button _offMessageButton;
        [SerializeField] private Button _readTickRateButton;
        [SerializeField] private Button _offTickButton;
        [SerializeField] private Button _readIsConnectedButton;
        [SerializeField] private Button _readIsHostButton;
        [SerializeField] private Button _readMyStateButton;
        [SerializeField] private Button _readPlayersStateButton;
        [SerializeField] private Button _readGlobalStateButton;
        [SerializeField] private Button _readConnectedPlayersButton;
        [SerializeField] private Button _readNetworkStatsButton;

        [Header("Info")]
        [SerializeField] private TMP_Text _playerIdText;

        private int _tickCount;
        private bool _isMessageListenerSubscribed;
        private bool _isTickListenerSubscribed;
        private Button _clearConsoleButton;

        private async void Start()
        {
            await GP_Init.Ready;
            ApplyInputDefaults();
            UpdatePlayerIdText();
            Log("MULTIPLAYER: READY");
        }

        private void OnEnable()
        {
            BindButtons();
            BindEvents();
            BindPlayerEvents();

            if (GP_Init.isReady)
                UpdatePlayerIdText();
        }

        private void OnDisable()
        {
            UnbindButtons();
            UnbindEvents();
            UnbindPlayerEvents();
        }

        private void BindButtons()
        {
            EnsureClearConsoleButton();
            AddListener(_connectButton, ConnectAsync);
            AddListener(_disconnectButton, DisconnectAsync);
            AddListener(_fastModeButton, SetFastMode);
            AddListener(_smoothModeButton, SetSmoothMode);
            AddListener(_defineSchemaButton, DefinePlayerSchema);
            AddListener(_defineGlobalSchemaButton, DefineGlobalSchema);
            AddListener(_enableInitializerButton, EnableDefaultInitializerAsync);
            AddListener(_enableAsyncInitializerButton, EnableAsyncInitializerAsync);
            AddListener(_disableInitializerButton, DisableInitializerAsync);
            AddListener(_setPlayerStateButton, SetPlayerState);
            AddListener(_setGlobalStateButton, SetGlobalState);
            AddListener(_sendMessageButton, SendMessage);
            AddListener(_offMessageButton, ToggleMessageListener);
            AddListener(_clearConsoleButton, ClearConsole);
            AddListener(_readTickRateButton, ReadTickRate);
            AddListener(_offTickButton, ToggleTickListener);
            AddListener(_readIsConnectedButton, ReadIsConnected);
            AddListener(_readIsHostButton, ReadIsHost);
            AddListener(_readMyStateButton, ReadMyState);
            AddListener(_readPlayersStateButton, ReadPlayersState);
            AddListener(_readGlobalStateButton, ReadGlobalState);
            AddListener(_readConnectedPlayersButton, ReadConnectedPlayers);
            AddListener(_readNetworkStatsButton, ReadNetworkStats);
        }

        private void UnbindButtons()
        {
            RemoveListener(_connectButton, ConnectAsync);
            RemoveListener(_disconnectButton, DisconnectAsync);
            RemoveListener(_fastModeButton, SetFastMode);
            RemoveListener(_smoothModeButton, SetSmoothMode);
            RemoveListener(_defineSchemaButton, DefinePlayerSchema);
            RemoveListener(_defineGlobalSchemaButton, DefineGlobalSchema);
            RemoveListener(_enableInitializerButton, EnableDefaultInitializerAsync);
            RemoveListener(_enableAsyncInitializerButton, EnableAsyncInitializerAsync);
            RemoveListener(_disableInitializerButton, DisableInitializerAsync);
            RemoveListener(_setPlayerStateButton, SetPlayerState);
            RemoveListener(_setGlobalStateButton, SetGlobalState);
            RemoveListener(_sendMessageButton, SendMessage);
            RemoveListener(_offMessageButton, ToggleMessageListener);
            RemoveListener(_clearConsoleButton, ClearConsole);
            RemoveListener(_readTickRateButton, ReadTickRate);
            RemoveListener(_offTickButton, ToggleTickListener);
            RemoveListener(_readIsConnectedButton, ReadIsConnected);
            RemoveListener(_readIsHostButton, ReadIsHost);
            RemoveListener(_readMyStateButton, ReadMyState);
            RemoveListener(_readPlayersStateButton, ReadPlayersState);
            RemoveListener(_readGlobalStateButton, ReadGlobalState);
            RemoveListener(_readConnectedPlayersButton, ReadConnectedPlayers);
            RemoveListener(_readNetworkStatsButton, ReadNetworkStats);
        }

        private void BindEvents()
        {
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
        }

        private void BindPlayerEvents()
        {
            GP_Player.OnConnect += UpdatePlayerIdText;
            GP_Player.OnPlayerChange += UpdatePlayerIdText;
            GP_Player.OnLoadComplete += UpdatePlayerIdText;
            GP_Player.OnLoginComplete += UpdatePlayerIdText;
        }

        private void UnbindEvents()
        {
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
        }

        private void UnbindPlayerEvents()
        {
            GP_Player.OnConnect -= UpdatePlayerIdText;
            GP_Player.OnPlayerChange -= UpdatePlayerIdText;
            GP_Player.OnLoadComplete -= UpdatePlayerIdText;
            GP_Player.OnLoginComplete -= UpdatePlayerIdText;
        }

        public async void ConnectAsync()
        {
            if (!TryGetChannelId(out int channelId))
            {
                Log("MULTIPLAYER: CONNECT: invalid channelId");
                return;
            }

            Log($"MULTIPLAYER: CONNECT: channelId={channelId}");

            try
            {
                MultiplayerConnectResultData result =
                    await GP_Multiplayer.connect(new MultiplayerChannelQuery { channelId = channelId });

                Log($"MULTIPLAYER: CONNECT RESULT: {Pretty(result)}");
            }
            catch (Exception exception)
            {
                LogException("MULTIPLAYER: CONNECT EXCEPTION", exception);
            }
        }

        public async void DisconnectAsync()
        {
            if (!TryGetChannelId(out int channelId))
            {
                Log("MULTIPLAYER: DISCONNECT: invalid channelId");
                return;
            }

            Log($"MULTIPLAYER: DISCONNECT: channelId={channelId}");

            try
            {
                await GP_Multiplayer.disconnect(new MultiplayerChannelQuery { channelId = channelId });
                Log("MULTIPLAYER: DISCONNECT RESULT: completed");
            }
            catch (Exception exception)
            {
                LogException("MULTIPLAYER: DISCONNECT EXCEPTION", exception);
            }
        }

        public void SetFastMode()
        {
            GP_Multiplayer.setMode(MultiplayerMode.FAST);
            Log("MULTIPLAYER: SET MODE: fast");
        }

        public void SetSmoothMode()
        {
            GP_Multiplayer.setMode(MultiplayerMode.SMOOTH);
            Log("MULTIPLAYER: SET MODE: smooth");
        }

        public void DefinePlayerSchema()
        {
            GP_Data schema = CreateDataFromInput(_schemaJsonInput, DefaultPlayerSchemaJson);
            Log("MULTIPLAYER: DEFINE PLAYER SCHEMA");
            LogJson("MULTIPLAYER: PLAYER SCHEMA", schema);
            GP_Multiplayer.definePlayerSchema(schema);
        }

        public void DefineGlobalSchema()
        {
            GP_Data schema = CreateDataFromInput(_globalSchemaJsonInput, DefaultGlobalSchemaJson);
            Log("MULTIPLAYER: DEFINE GLOBAL SCHEMA");
            LogJson("MULTIPLAYER: GLOBAL SCHEMA", schema);
            GP_Multiplayer.defineGlobalSchema(schema);
        }

        public async void EnableDefaultInitializerAsync()
        {
            await GP_Multiplayer.setPlayerInitializer(CreateDefaultInitializerPayload);
            Log("MULTIPLAYER: INITIALIZER: enabled default sync initializer");
        }

        public async void EnableAsyncInitializerAsync()
        {
            await GP_Multiplayer.setPlayerInitializer(CreateDefaultInitializerPayloadAsync);
            Log("MULTIPLAYER: INITIALIZER: enabled default async initializer");
        }

        public async void DisableInitializerAsync()
        {
            await GP_Multiplayer.setPlayerInitializer((Func<int, MultiplayerConnectedPlayerData, GP_Data>)null);
            Log("MULTIPLAYER: INITIALIZER: disabled");
        }

        public void SetPlayerState()
        {
            GP_Data state = CreateDataFromInput(_stateJsonInput, DefaultPlayerStateJson);
            Log("MULTIPLAYER: SET PLAYER STATE");
            LogJson("MULTIPLAYER: PLAYER STATE", state);
            GP_Multiplayer.setPlayerState(state);
        }

        public void SetGlobalState()
        {
            GP_Data state = CreateDataFromInput(_globalStateJsonInput, DefaultGlobalStateJson);
            Log("MULTIPLAYER: SET GLOBAL STATE");
            LogJson("MULTIPLAYER: GLOBAL STATE", state);
            GP_Multiplayer.setGlobalState(state);
        }

        public void SendMessage()
        {
            string eventName = GetInputOrDefault(_eventNameInput, DefaultEventName);
            GP_Data message = CreateDataFromInput(_messageJsonInput, DefaultMessageJson);
            MultiplayerSendMessageOptions options = new MultiplayerSendMessageOptions
            {
                target = GetInputOrDefault(_targetInput, DefaultBroadcastTarget),
                echo = ParseBool(_echoInput, false)
            };

            Log($"MULTIPLAYER: SEND MESSAGE: event={eventName}, target={options.target}, echo={options.echo}");
            LogJson("MULTIPLAYER: MESSAGE", message);

            GP_Multiplayer.sendMessage(eventName, message, options);
        }

        public void ToggleMessageListener()
        {
            if (_isMessageListenerSubscribed)
            {
                UnbindMessageListener();
                Log("MULTIPLAYER: MESSAGE: listener disabled; customEvent stays active");
            }
            else
            {
                BindMessageListener();
                Log("MULTIPLAYER: MESSAGE: listener enabled");
            }
        }

        public void ClearConsole()
        {
            if (ConsoleUI.Instance != null)
                ConsoleUI.Instance.Clear();
        }

        public void ReadTickRate() => Log($"MULTIPLAYER: TICK RATE: {GP_Multiplayer.tickRate}");

        public void ToggleTickListener()
        {
            if (_isTickListenerSubscribed)
            {
                UnbindTickListener();
                Log("MULTIPLAYER: TICK: listener disabled");
            }
            else
            {
                BindTickListener();
                Log("MULTIPLAYER: TICK: listener enabled");
            }
        }

        public void ReadIsConnected() => Log($"MULTIPLAYER: IS CONNECTED: {GP_Multiplayer.isConnected}");
        public void ReadIsHost() => Log($"MULTIPLAYER: IS HOST: {GP_Multiplayer.isHost}");
        public void ReadMyState() => LogJson("MULTIPLAYER: MY STATE", GP_Multiplayer.myState);
        public void ReadPlayersState() => LogJson("MULTIPLAYER: PLAYERS STATE", GP_Multiplayer.playersState);
        public void ReadGlobalState() => LogJson("MULTIPLAYER: GLOBAL STATE", GP_Multiplayer.globalState);
        public void ReadConnectedPlayers() => LogJson("MULTIPLAYER: CONNECTED PLAYERS", GP_Multiplayer.connectedPlayers);
        public void ReadNetworkStats() => LogJson("MULTIPLAYER: NETWORK STATS", GP_Multiplayer.networkStats);

        private GP_Data CreateDefaultInitializerPayload(int playerId, MultiplayerConnectedPlayerData player)
        {
            GP_Data result = new GP_Data(CreateInitializerJson(playerId, player, false));
            Log($"MULTIPLAYER: INITIALIZER REQUEST: playerId={playerId}");
            LogJson("MULTIPLAYER: INITIALIZER RESULT", result);
            return result;
        }

        private async Task<GP_Data> CreateDefaultInitializerPayloadAsync(int playerId, MultiplayerConnectedPlayerData player)
        {
            await Task.Delay(100);
            GP_Data result = new GP_Data(CreateInitializerJson(playerId, player, true));
            Log($"MULTIPLAYER: ASYNC INITIALIZER REQUEST: playerId={playerId}");
            LogJson("MULTIPLAYER: ASYNC INITIALIZER RESULT", result);
            return result;
        }

        private string CreateInitializerJson(int playerId, MultiplayerConnectedPlayerData player, bool isAsync)
        {
            string json = GetInputOrDefault(_initializerStateJsonInput, DefaultInitializerJson);
            if (!json.StartsWith("{", StringComparison.Ordinal) || !json.EndsWith("}", StringComparison.Ordinal))
                json = DefaultInitializerJson;

            List<string> fields = new List<string>
            {
                "\"playerId\":" + playerId.ToString(CultureInfo.InvariantCulture),
                "\"isHost\":" + (player != null && player.isHost ? "true" : "false")
            };

            if (isAsync)
                fields.Add("\"async\":true");

            if (player != null)
            {
                fields.Add("\"ping\":" + player.ping.ToString(CultureInfo.InvariantCulture));
                fields.Add("\"connectionStability\":" + player.connectionStability.ToString("0.###", CultureInfo.InvariantCulture));
                fields.Add("\"sessionDuration\":" + player.sessionDuration.ToString(CultureInfo.InvariantCulture));
            }

            string suffix = string.Join(",", fields);
            if (json == "{}")
                return "{" + suffix + "}";

            return json.Substring(0, json.Length - 1) + "," + suffix + "}";
        }

        private void OnConnect(GP_Data payload) => LogJson("MULTIPLAYER: EVENT connect", payload);
        private void OnDisconnect(GP_Data payload) => LogJson("MULTIPLAYER: EVENT disconnect", payload);
        private void OnConnectError(GP_Data payload) => LogJson("MULTIPLAYER: EVENT error:connect", payload);
        private void OnDisconnectError(GP_Data payload) => LogJson("MULTIPLAYER: EVENT error:disconnect", payload);
        private void OnSendStateError(GP_Data payload) => LogJson("MULTIPLAYER: EVENT error:sendState", payload);
        private void OnPlayerJoined(GP_Data payload) => LogJson("MULTIPLAYER: EVENT playerJoined", payload);
        private void OnPlayerLeft(GP_Data payload) => LogJson("MULTIPLAYER: EVENT playerLeft", payload);
        private void OnPlayersUpdated(GP_Data payload) => LogJson("MULTIPLAYER: EVENT playersUpdated", payload);
        private void OnGlobalStateUpdated(GP_Data payload) => LogJson("MULTIPLAYER: EVENT globalStateUpdated", payload);
        private void OnCustomEvent(GP_Data payload) => LogJson("MULTIPLAYER: EVENT customEvent", payload);
        private void OnHostMigrated(GP_Data payload) => LogJson("MULTIPLAYER: EVENT hostMigrated", payload);
        private void OnBecameHost() => Log("MULTIPLAYER: EVENT becameHost");
        private void OnBecamePeer() => Log("MULTIPLAYER: EVENT becamePeer");
        private void OnMessage(GP_Data payload) => LogJson("MULTIPLAYER: EVENT onMessage", payload);

        private void OnTick(float delta)
        {
            _tickCount++;
            if (_tickCount == 1 || _tickCount % TickLogInterval == 0)
                Log($"MULTIPLAYER: EVENT tick: count={_tickCount}, delta={delta.ToString("0.000", CultureInfo.InvariantCulture)}");
        }

        private void BindMessageListener()
        {
            if (_isMessageListenerSubscribed)
                return;

            GP_Multiplayer.onMessage(OnMessage);
            _isMessageListenerSubscribed = true;
            UpdateMessageButtonText();
        }

        private void UnbindMessageListener()
        {
            if (!_isMessageListenerSubscribed)
                return;

            GP_Multiplayer.offMessage(OnMessage);
            _isMessageListenerSubscribed = false;
            UpdateMessageButtonText();
        }

        private void UpdateMessageButtonText()
        {
            if (_offMessageButton == null)
                return;

            TMP_Text label = _offMessageButton.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
                label.text = _isMessageListenerSubscribed ? "MESSAGE OFF" : "MESSAGE ON";
        }

        private void EnsureClearConsoleButton()
        {
            if (_clearConsoleButton != null || _offMessageButton == null)
                return;

            Transform existing = _offMessageButton.transform.parent.Find("CLEAR CONSOLE");
            if (existing != null && existing.TryGetComponent(out _clearConsoleButton))
                return;

            _clearConsoleButton = Instantiate(_offMessageButton, _offMessageButton.transform.parent);
            _clearConsoleButton.name = "CLEAR CONSOLE";
            _clearConsoleButton.transform.SetSiblingIndex(_offMessageButton.transform.GetSiblingIndex() + 1);

            TMP_Text label = _clearConsoleButton.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
                label.text = "CLEAR CONSOLE";
        }

        private void BindTickListener()
        {
            if (_isTickListenerSubscribed)
                return;

            GP_Multiplayer.onTick(OnTick);
            _isTickListenerSubscribed = true;
            _tickCount = 0;
            UpdateTickButtonText();
        }

        private void UnbindTickListener()
        {
            if (!_isTickListenerSubscribed)
                return;

            GP_Multiplayer.offTick(OnTick);
            _isTickListenerSubscribed = false;
            UpdateTickButtonText();
        }

        private void UpdateTickButtonText()
        {
            if (_offTickButton == null)
                return;

            TMP_Text label = _offTickButton.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
                label.text = _isTickListenerSubscribed ? "TICK OFF" : "TICK ON";
        }

        private void ApplyInputDefaults()
        {
            SetIfEmpty(_schemaJsonInput, DefaultPlayerSchemaJson);
            SetIfEmpty(_globalSchemaJsonInput, DefaultGlobalSchemaJson);
            SetIfEmpty(_initializerStateJsonInput, DefaultInitializerJson);
            SetIfEmpty(_stateJsonInput, DefaultPlayerStateJson);
            SetIfEmpty(_globalStateJsonInput, DefaultGlobalStateJson);
            SetIfEmpty(_eventNameInput, DefaultEventName);
            SetIfEmpty(_messageJsonInput, DefaultMessageJson);
            SetIfEmpty(_targetInput, DefaultBroadcastTarget);
            SetIfEmpty(_echoInput, "false");
        }

        private void UpdatePlayerIdText()
        {
            ResolvePlayerIdText();

            if (_playerIdText != null)
            {
                string id = GP_Player.GetID().ToString(CultureInfo.InvariantCulture);
                _playerIdText.text = IsPlayerIdValueText(_playerIdText) ? id : $"PLAYER ID: {id}";
            }
        }

        private void ResolvePlayerIdText()
        {
            if (_playerIdText != null)
                return;

            Transform content = FindChildIgnoreCase(transform, "Content");
            Transform info = FindChildIgnoreCase(transform, "INFO") ?? FindChildIgnoreCase(content, "INFO");

            if (info != null)
            {
                _playerIdText = FindTextByName(info, "ID (TMP)") ?? FindTextByName(info, "ID");

                if (_playerIdText != null)
                    return;

                _playerIdText = FindTextByName(info, "Player ID (TMP)") ?? FindTextByName(info, "Player ID");

                if (_playerIdText != null)
                    return;

                foreach (TMP_Text text in info.GetComponentsInChildren<TMP_Text>(true))
                {
                    if (!string.IsNullOrEmpty(text.text) &&
                        text.text.TrimStart().StartsWith("PLAYER ID", StringComparison.OrdinalIgnoreCase))
                    {
                        _playerIdText = text;
                        return;
                    }
                }
            }

            _playerIdText = transform.Find("INFO/Player ID (TMP)")?.GetComponent<TMP_Text>() ??
                            transform.Find("Info/Player ID (TMP)")?.GetComponent<TMP_Text>() ??
                            transform.Find("Content/INFO/Player ID (TMP)")?.GetComponent<TMP_Text>();
        }

        private static Transform FindChildIgnoreCase(Transform root, string childName)
        {
            if (root == null)
                return null;

            Transform exact = root.Find(childName);

            if (exact != null)
                return exact;

            foreach (Transform child in root)
            {
                if (string.Equals(child.name, childName, StringComparison.OrdinalIgnoreCase))
                    return child;
            }

            return null;
        }

        private static TMP_Text FindTextByName(Transform root, string textName)
        {
            foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
            {
                if (string.Equals(text.gameObject.name, textName, StringComparison.OrdinalIgnoreCase))
                    return text;
            }

            return null;
        }

        private static bool IsPlayerIdValueText(TMP_Text text)
        {
            return string.Equals(text.gameObject.name, "ID (TMP)", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(text.gameObject.name, "ID", StringComparison.OrdinalIgnoreCase);
        }

        private static void AddListener(Button button, UnityAction action)
        {
            if (button != null)
                button.onClick.AddListener(action);
        }

        private static void RemoveListener(Button button, UnityAction action)
        {
            if (button != null)
                button.onClick.RemoveListener(action);
        }

        private bool TryGetChannelId(out int channelId)
        {
            channelId = 0;
            return _channelIdInput != null &&
                   int.TryParse(_channelIdInput.text, NumberStyles.Integer, CultureInfo.InvariantCulture, out channelId) &&
                   channelId > 0;
        }

        private static string GetInputOrDefault(TMP_InputField input, string fallback)
        {
            string value = input == null ? string.Empty : input.text;
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static GP_Data CreateDataFromInput(TMP_InputField input, string fallbackJson)
        {
            return new GP_Data(GetInputOrDefault(input, fallbackJson));
        }

        private static bool ParseBool(TMP_InputField input, bool fallback)
        {
            string raw = GetInputOrDefault(input, fallback ? "true" : "false");
            return bool.TryParse(raw, out bool value) ? value : fallback;
        }

        private static string Pretty(MultiplayerConnectResultData value)
        {
            return value == null ? "null" : JsonUtility.ToJson(value, true);
        }

        private static void LogJson(string label, GP_Data payload)
        {
            Log($"{label}: {payload?.Data ?? "null"}");
        }

        private static void Log(string message)
        {
            if (ConsoleUI.Instance != null)
                ConsoleUI.Instance.Log(message);
            else
                Debug.Log(message);
        }

        private static void LogException(string prefix, Exception exception)
        {
            Log($"{prefix}: {exception.GetType().Name}: {exception.Message}");
        }

        private static void SetIfEmpty(TMP_InputField input, string value)
        {
            if (input != null && string.IsNullOrWhiteSpace(input.text))
                input.text = value;
        }
    }
}
