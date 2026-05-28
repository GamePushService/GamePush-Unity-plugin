using System;
using System.Globalization;
using System.Threading.Tasks;
using Examples.Console;
using GamePush;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Examples.Multiplayer
{
    public class Multiplayer : MonoBehaviour
    {
        private const string DefaultSchemaJson = "{\"score\":\"number\",\"ready\":\"boolean\",\"nickname\":\"string\"}";
        private const string DefaultStateJson = "{\"score\":1,\"ready\":true,\"nickname\":\"tester\"}";
        private const string DefaultInitializerJson = "{\"score\":0,\"ready\":false,\"nickname\":\"player\"}";
        private const string DefaultMessageJson = "{\"text\":\"hello\"}";
        private const string DefaultEventName = "demo:message";
        private const string DefaultBroadcastTarget = "all";
        private const int TickLogInterval = 30;

        [Header("Inputs")]
        [SerializeField] private TMP_InputField _channelIdInput;
        [SerializeField] private TMP_InputField _schemaJsonInput;
        [SerializeField] private TMP_InputField _initializerStateJsonInput;
        [SerializeField] private TMP_InputField _stateJsonInput;
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
        [SerializeField] private Button _enableInitializerButton;
        [SerializeField] private Button _enableAsyncInitializerButton;
        [SerializeField] private Button _disableInitializerButton;
        [SerializeField] private Button _setPlayerStateButton;
        [SerializeField] private Button _sendMessageButton;
        [SerializeField] private Button _offMessageButton;
        [SerializeField] private Button _readTickRateButton;
        [SerializeField] private Button _offTickButton;
        [SerializeField] private Button _readIsConnectedButton;
        [SerializeField] private Button _readIsHostButton;
        [SerializeField] private Button _readMyStateButton;
        [SerializeField] private Button _readPlayersStateButton;
        [SerializeField] private Button _readConnectedPlayersButton;
        [SerializeField] private Button _readNetworkStatsButton;

        private int _tickCount;
        private bool _isMessageListenerSubscribed;
        private bool _isTickListenerSubscribed;

        private async void Start()
        {
            await GP_Init.Ready;
            ApplyInputDefaults();
            Log("MULTIPLAYER: READY");
        }

        private void OnEnable()
        {
            BindButtons();
            BindEvents();
        }

        private void OnDisable()
        {
            UnbindButtons();
            UnbindEvents();
        }

        private void BindButtons()
        {
            AddListener(_connectButton, ConnectAsync);
            AddListener(_disconnectButton, DisconnectAsync);
            AddListener(_fastModeButton, SetFastMode);
            AddListener(_smoothModeButton, SetSmoothMode);
            AddListener(_defineSchemaButton, DefinePlayerSchema);
            AddListener(_enableInitializerButton, EnableDefaultInitializerAsync);
            AddListener(_enableAsyncInitializerButton, EnableAsyncInitializerAsync);
            AddListener(_disableInitializerButton, DisableInitializerAsync);
            AddListener(_setPlayerStateButton, SetPlayerState);
            AddListener(_sendMessageButton, SendMessage);
            AddListener(_offMessageButton, DisableMessageListener);
            AddListener(_readTickRateButton, ReadTickRate);
            AddListener(_offTickButton, DisableTickListener);
            AddListener(_readIsConnectedButton, ReadIsConnected);
            AddListener(_readIsHostButton, ReadIsHost);
            AddListener(_readMyStateButton, ReadMyState);
            AddListener(_readPlayersStateButton, ReadPlayersState);
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
            RemoveListener(_enableInitializerButton, EnableDefaultInitializerAsync);
            RemoveListener(_enableAsyncInitializerButton, EnableAsyncInitializerAsync);
            RemoveListener(_disableInitializerButton, DisableInitializerAsync);
            RemoveListener(_setPlayerStateButton, SetPlayerState);
            RemoveListener(_sendMessageButton, SendMessage);
            RemoveListener(_offMessageButton, DisableMessageListener);
            RemoveListener(_readTickRateButton, ReadTickRate);
            RemoveListener(_offTickButton, DisableTickListener);
            RemoveListener(_readIsConnectedButton, ReadIsConnected);
            RemoveListener(_readIsHostButton, ReadIsHost);
            RemoveListener(_readMyStateButton, ReadMyState);
            RemoveListener(_readPlayersStateButton, ReadPlayersState);
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
            GP_Multiplayer.on("customEvent", OnCustomEvent);
            GP_Multiplayer.on("hostMigrated", OnHostMigrated);
            GP_Multiplayer.on("becameHost", OnBecameHost);
            GP_Multiplayer.on("becamePeer", OnBecamePeer);
            BindMessageListener();
            BindTickListener();
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
            GP_Multiplayer.off("customEvent", OnCustomEvent);
            GP_Multiplayer.off("hostMigrated", OnHostMigrated);
            GP_Multiplayer.off("becameHost", OnBecameHost);
            GP_Multiplayer.off("becamePeer", OnBecamePeer);
            UnbindMessageListener();
            UnbindTickListener();
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
            GP_Data schema = CreateDataFromInput(_schemaJsonInput, DefaultSchemaJson);
            Log("MULTIPLAYER: DEFINE SCHEMA");
            LogJson("MULTIPLAYER: SCHEMA", schema);
            GP_Multiplayer.definePlayerSchema(schema);
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
            GP_Data state = CreateDataFromInput(_stateJsonInput, DefaultStateJson);
            Log("MULTIPLAYER: SET PLAYER STATE");
            LogJson("MULTIPLAYER: STATE", state);
            GP_Multiplayer.setPlayerState(state);
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

        public void DisableMessageListener()
        {
            if (!_isMessageListenerSubscribed)
            {
                Log("MULTIPLAYER: OFF MESSAGE: already disabled");
                return;
            }

            UnbindMessageListener();
            Log("MULTIPLAYER: OFF MESSAGE: listener disabled; customEvent stays active");
        }

        public void ReadTickRate() => Log($"MULTIPLAYER: TICK RATE: {GP_Multiplayer.tickRate}");
        public void DisableTickListener()
        {
            if (!_isTickListenerSubscribed)
            {
                Log("MULTIPLAYER: OFF TICK: already disabled");
                return;
            }

            UnbindTickListener();
            Log("MULTIPLAYER: OFF TICK: listener disabled");
        }

        public void ReadIsConnected() => Log($"MULTIPLAYER: IS CONNECTED: {GP_Multiplayer.isConnected}");
        public void ReadIsHost() => Log($"MULTIPLAYER: IS HOST: {GP_Multiplayer.isHost}");
        public void ReadMyState() => LogJson("MULTIPLAYER: MY STATE", GP_Multiplayer.myState);
        public void ReadPlayersState() => LogJson("MULTIPLAYER: PLAYERS STATE", GP_Multiplayer.playersState);
        public void ReadConnectedPlayers() => LogJson("MULTIPLAYER: CONNECTED PLAYERS", GP_Multiplayer.connectedPlayers);
        public void ReadNetworkStats() => LogJson("MULTIPLAYER: NETWORK STATS", GP_Multiplayer.networkStats);

        private GP_Data CreateDefaultInitializerPayload(int playerId, MultiplayerConnectedPlayerData player)
        {
            JObject state = ParseObjectOrDefault(GetInputOrDefault(_initializerStateJsonInput, DefaultInitializerJson), DefaultInitializerJson);
            state["playerId"] = playerId;
            state["isHost"] = player != null && player.isHost;

            if (player != null)
            {
                state["ping"] = player.ping;
                state["connectionStability"] = player.connectionStability;
                state["sessionDuration"] = player.sessionDuration;
            }

            GP_Data result = new GP_Data(state.ToString(Formatting.None));
            Log($"MULTIPLAYER: INITIALIZER REQUEST: playerId={playerId}");
            LogJson("MULTIPLAYER: INITIALIZER RESULT", result);
            return result;
        }

        private async Task<GP_Data> CreateDefaultInitializerPayloadAsync(int playerId, MultiplayerConnectedPlayerData player)
        {
            await Task.Delay(100);
            JObject state = ParseObjectOrDefault(GetInputOrDefault(_initializerStateJsonInput, DefaultInitializerJson), DefaultInitializerJson);
            state["playerId"] = playerId;
            state["isHost"] = player != null && player.isHost;
            state["async"] = true;

            if (player != null)
            {
                state["ping"] = player.ping;
                state["connectionStability"] = player.connectionStability;
                state["sessionDuration"] = player.sessionDuration;
            }

            GP_Data result = new GP_Data(state.ToString(Formatting.None));
            Log($"MULTIPLAYER: ASYNC INITIALIZER REQUEST: playerId={playerId}");
            LogJson("MULTIPLAYER: ASYNC INITIALIZER RESULT", result);
            return result;
        }

        private void OnConnect(GP_Data payload) => LogJson("MULTIPLAYER: EVENT connect", payload);
        private void OnDisconnect(GP_Data payload) => LogJson("MULTIPLAYER: EVENT disconnect", payload);
        private void OnConnectError(GP_Data payload) => LogJson("MULTIPLAYER: EVENT error:connect", payload);
        private void OnDisconnectError(GP_Data payload) => LogJson("MULTIPLAYER: EVENT error:disconnect", payload);
        private void OnSendStateError(GP_Data payload) => LogJson("MULTIPLAYER: EVENT error:sendState", payload);
        private void OnPlayerJoined(GP_Data payload) => LogJson("MULTIPLAYER: EVENT playerJoined", payload);
        private void OnPlayerLeft(GP_Data payload) => LogJson("MULTIPLAYER: EVENT playerLeft", payload);
        private void OnPlayersUpdated(GP_Data payload) => LogJson("MULTIPLAYER: EVENT playersUpdated", payload);
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
        }

        private void UnbindMessageListener()
        {
            if (!_isMessageListenerSubscribed)
                return;

            GP_Multiplayer.offMessage(OnMessage);
            _isMessageListenerSubscribed = false;
        }

        private void BindTickListener()
        {
            if (_isTickListenerSubscribed)
                return;

            GP_Multiplayer.onTick(OnTick);
            _isTickListenerSubscribed = true;
            _tickCount = 0;
        }

        private void UnbindTickListener()
        {
            if (!_isTickListenerSubscribed)
                return;

            GP_Multiplayer.offTick(OnTick);
            _isTickListenerSubscribed = false;
        }

        private void ApplyInputDefaults()
        {
            SetIfEmpty(_schemaJsonInput, DefaultSchemaJson);
            SetIfEmpty(_initializerStateJsonInput, DefaultInitializerJson);
            SetIfEmpty(_stateJsonInput, DefaultStateJson);
            SetIfEmpty(_eventNameInput, DefaultEventName);
            SetIfEmpty(_messageJsonInput, DefaultMessageJson);
            SetIfEmpty(_targetInput, DefaultBroadcastTarget);
            SetIfEmpty(_echoInput, "false");
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
                   int.TryParse(_channelIdInput.text, NumberStyles.Integer, CultureInfo.InvariantCulture, out channelId);
        }

        private static string GetInputOrDefault(TMP_InputField input, string fallback)
        {
            string value = input == null ? string.Empty : input.text;
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static GP_Data CreateDataFromInput(TMP_InputField input, string fallbackJson)
        {
            string raw = GetInputOrDefault(input, fallbackJson);
            return new GP_Data(raw);
        }

        private static JObject ParseObjectOrDefault(string raw, string fallbackJson)
        {
            try
            {
                return JObject.Parse(string.IsNullOrWhiteSpace(raw) ? fallbackJson : raw);
            }
            catch
            {
                return JObject.Parse(fallbackJson);
            }
        }

        private static bool ParseBool(TMP_InputField input, bool fallback)
        {
            string raw = GetInputOrDefault(input, fallback ? "true" : "false");
            return bool.TryParse(raw, out bool value) ? value : fallback;
        }

        private static string Pretty(object value)
        {
            if (value == null)
                return "null";

            string json = JsonConvert.SerializeObject(value, Formatting.Indented);
            return string.IsNullOrEmpty(json) ? value.ToString() : json;
        }

        private static string PrettyJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "null";

            try
            {
                return JToken.Parse(json).ToString(Formatting.Indented);
            }
            catch
            {
                return json;
            }
        }

        private void LogJson(string label, GP_Data payload)
        {
            Log($"{label}: {PrettyJson(payload?.Data)}");
        }

        private void Log(string message)
        {
            if (ConsoleUI.Instance != null)
                ConsoleUI.Instance.Log(message);
            else
                Debug.Log(message);
        }

        private void LogException(string prefix, Exception exception)
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
