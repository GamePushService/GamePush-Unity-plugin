using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GamePush
{
    public class GP_Multiplayer : GP_Module
    {
        private static readonly Queue<TaskCompletionSource<MultiplayerConnectResultData>> PendingConnectOperations =
            new Queue<TaskCompletionSource<MultiplayerConnectResultData>>();
        private static readonly Queue<TaskCompletionSource<bool>> PendingDisconnectOperations =
            new Queue<TaskCompletionSource<bool>>();

        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Multiplayer);

        private static GP_Data CreateDataOrNull(string data)
        {
            if (string.IsNullOrEmpty(data) || data == "undefined" || data == "null")
                return null;

            return new GP_Data(data);
        }

        private static string ExtractErrorMessage(string data)
        {
            if (string.IsNullOrEmpty(data) || data == "undefined" || data == "null")
                return "unknown_error";

            try
            {
                MultiplayerErrorData error = JsonUtility.FromJson<MultiplayerErrorData>(data);
                if (!string.IsNullOrEmpty(error?.message))
                    return error.message;
            }
            catch
            {
            }

            return data;
        }

        private static T DeserializePayloadOrDefault<T>(GP_Data payload)
        {
            if (payload == null)
                return default;

            try
            {
                return payload.Get<T>();
            }
            catch
            {
                return default;
            }
        }

        private static Task<T> RunTypedOperation<T>(Queue<TaskCompletionSource<T>> queue, Action action)
        {
            TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
            queue.Enqueue(completionSource);

            try
            {
                action?.Invoke();
            }
            catch (Exception exception)
            {
                if (queue.Count > 0 && ReferenceEquals(queue.Peek(), completionSource))
                    queue.Dequeue();

                completionSource.TrySetException(exception);
            }

            return completionSource.Task;
        }

        private static Task RunVoidOperation(Queue<TaskCompletionSource<bool>> queue, Action action)
        {
            return RunTypedOperation(queue, action);
        }

        private static void CompleteTypedOperationSuccess<T>(Queue<TaskCompletionSource<T>> queue, T result)
        {
            if (queue.Count == 0)
                return;

            queue.Dequeue().TrySetResult(result);
        }

        private static void CompleteTypedOperationError<T>(Queue<TaskCompletionSource<T>> queue, string data)
        {
            if (queue.Count == 0)
                return;

            queue.Dequeue().TrySetException(new Exception(ExtractErrorMessage(data)));
        }

        internal static event UnityAction<GP_Data> OnConnect;
        internal static event UnityAction<GP_Data> OnDisconnect;
        internal static event UnityAction<GP_Data> OnConnectError;
        internal static event UnityAction<GP_Data> OnDisconnectError;
        internal static event UnityAction<GP_Data> OnSendStateError;
        internal static event UnityAction<GP_Data> OnPlayerJoined;
        internal static event UnityAction<GP_Data> OnPlayerLeft;
        internal static event UnityAction<GP_Data> OnPlayersUpdated;
        internal static event UnityAction<GP_Data> OnCustomEvent;
        internal static event UnityAction OnBecameHost;
        internal static event UnityAction OnBecamePeer;
        internal static event UnityAction<GP_Data> OnHostMigrated;
        internal static event UnityAction<GP_Data> OnTickEvent;
        internal static event UnityAction<float> OnTickDelta;

        private static event UnityAction<GP_Data> _connect;
        private static event UnityAction<GP_Data> _disconnect;
        private static event UnityAction<GP_Data> _connectError;
        private static event UnityAction<GP_Data> _disconnectError;
        private static event UnityAction<GP_Data> _sendStateError;
        private static event UnityAction<GP_Data> _playerJoined;
        private static event UnityAction<GP_Data> _playerLeft;
        private static event UnityAction<GP_Data> _playersUpdated;
        private static event UnityAction<GP_Data> _customEvent;
        private static event UnityAction<GP_Data> _hostMigrated;
        private static event UnityAction _becameHost;
        private static event UnityAction _becamePeer;

        private static event UnityAction<GP_Data> _onMessage;
        private static event UnityAction<GP_Data> _onTick;
        private static event UnityAction<float> _onTickDelta;
        private static readonly Dictionary<string, Dictionary<Delegate, UnityAction<GP_Data>>> _typedEventCallbackWrappers =
            new Dictionary<string, Dictionary<Delegate, UnityAction<GP_Data>>>();
        private static readonly Dictionary<Delegate, UnityAction<GP_Data>> _typedMessageCallbackWrappers =
            new Dictionary<Delegate, UnityAction<GP_Data>>();

        private static Func<int, MultiplayerConnectedPlayerData, GP_Data> _playerInitializer;
        private static Func<int, MultiplayerConnectedPlayerData, Task<GP_Data>> _playerInitializerAsync;

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_Connect(string query);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_Disconnect(string query);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SetMode(string mode);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_DefinePlayerSchema(string schema);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SetPlayerInitializer();
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_ClearPlayerInitializer();
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_ResolvePlayerInitializer(int request_ID, string state);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SetPlayerState(string state);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SendMessage(string eventName, string data, string options);
        [DllImport("__Internal")]
        private static extern int GP_Multiplayer_TickRate();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_IsConnected();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_IsHost();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_MyState();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_PlayersState();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_ConnectedPlayers();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_NetworkStats();
#endif

        internal static void Connect(string query = "{}")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_Connect(query);
#else
            ConsoleLog("CONNECT");
#endif
        }

        internal static void Disconnect(string query = "{}")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_Disconnect(query);
#else
            ConsoleLog("DISCONNECT");
#endif
        }

        internal static void SetMode(string mode)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_SetMode(mode);
#else
            ConsoleLog("SET MODE");
#endif
        }

        internal static void SetMode(MultiplayerMode mode) => SetMode(mode == MultiplayerMode.FAST ? "fast" : "smooth");

        internal static void DefinePlayerSchema(string schema)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_DefinePlayerSchema(schema);
#else
            ConsoleLog("DEFINE PLAYER SCHEMA");
#endif
        }

        internal static void SetPlayerInitializer(Func<int, MultiplayerConnectedPlayerData, GP_Data> initializer)
        {
            _playerInitializer = initializer;
            _playerInitializerAsync = null;
            ApplyPlayerInitializer(initializer != null);
        }

        internal static void SetPlayerInitializer(Func<int, MultiplayerConnectedPlayerData, Task<GP_Data>> initializer)
        {
            _playerInitializer = null;
            _playerInitializerAsync = initializer;
            ApplyPlayerInitializer(initializer != null);
        }

        private static void ApplyPlayerInitializer(bool hasInitializer)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            if (hasInitializer)
                GP_Multiplayer_SetPlayerInitializer();
            else
                GP_Multiplayer_ClearPlayerInitializer();
#else
            ConsoleLog("SET PLAYER INITIALIZER");
#endif
        }

        internal static void SetPlayerState(string state)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_SetPlayerState(state);
#else
            ConsoleLog("SET PLAYER STATE");
#endif
        }

        internal static void SendMessage(string eventName, string data, string options = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_SendMessage(eventName, data, options ?? "undefined");
#else
            ConsoleLog("SEND MESSAGE");
#endif
        }

        internal static void SendMessage(string eventName, GP_Data data, MultiplayerSendMessageOptions options) =>
            SendMessage(eventName, data?.Data ?? "null", options == null ? null : JsonUtility.ToJson(options));

        internal static int TickRate()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Multiplayer_TickRate();
#else
            return 0;
#endif
        }

        internal static bool IsConnected()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Multiplayer_IsConnected() == "true";
#else
            return false;
#endif
        }

        internal static bool IsHost()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Multiplayer_IsHost() == "true";
#else
            return false;
#endif
        }

        internal static GP_Data MyState()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return CreateDataOrNull(GP_Multiplayer_MyState());
#else
            return null;
#endif
        }

        internal static GP_Data PlayersState()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return CreateDataOrNull(GP_Multiplayer_PlayersState());
#else
            return null;
#endif
        }

        internal static GP_Data ConnectedPlayers()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return CreateDataOrNull(GP_Multiplayer_ConnectedPlayers());
#else
            return null;
#endif
        }

        internal static GP_Data NetworkStats()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return CreateDataOrNull(GP_Multiplayer_NetworkStats());
#else
            return null;
#endif
        }

        public static Task<MultiplayerConnectResultData> connect(MultiplayerChannelQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunTypedOperation(PendingConnectOperations, () =>
                Connect(JsonUtility.ToJson(query ?? new MultiplayerChannelQuery())));
#else
            Connect(JsonUtility.ToJson(query ?? new MultiplayerChannelQuery()));
            return Task.FromResult<MultiplayerConnectResultData>(null);
#endif
        }

        public static Task disconnect(MultiplayerChannelQuery query)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunVoidOperation(PendingDisconnectOperations, () =>
                Disconnect(JsonUtility.ToJson(query ?? new MultiplayerChannelQuery())));
#else
            Disconnect(JsonUtility.ToJson(query ?? new MultiplayerChannelQuery()));
            return Task.CompletedTask;
#endif
        }
        internal static void setMode(string mode) => SetMode(mode);
        public static void setMode(MultiplayerMode mode) => SetMode(mode);
        public static void definePlayerSchema(GP_Data schema) => DefinePlayerSchema(schema?.Data ?? "{}");
        public static Task setPlayerInitializer(Func<int, MultiplayerConnectedPlayerData, GP_Data> initializer)
        {
            SetPlayerInitializer(initializer);
            return Task.CompletedTask;
        }

        public static Task setPlayerInitializer(Func<int, MultiplayerConnectedPlayerData, Task<GP_Data>> initializer)
        {
            SetPlayerInitializer(initializer);
            return Task.CompletedTask;
        }
        public static void setPlayerState(GP_Data state) => SetPlayerState(state?.Data ?? "{}");
        public static void sendMessage(string eventName, GP_Data data, MultiplayerSendMessageOptions options) => SendMessage(eventName, data, options);
        internal static void sendMessage(string eventName, GP_Data data, int target) =>
            SendMessage(eventName, data?.Data ?? "null", target.ToString(CultureInfo.InvariantCulture));
        internal static void sendMessage(string eventName, GP_Data data, string target) =>
            SendMessage(eventName, data?.Data ?? "null", target);
        public static int tickRate => TickRate();
        public static bool isConnected => IsConnected();
        public static bool isHost => IsHost();
        public static GP_Data myState => MyState();
        public static GP_Data playersState => PlayersState();
        public static GP_Data connectedPlayers => ConnectedPlayers();
        public static GP_Data networkStats => NetworkStats();

        public static void on(string eventName, UnityAction<GP_Data> callback)
        {
            switch (eventName)
            {
                case "connect":
                    _connect += callback;
                    break;
                case "disconnect":
                    _disconnect += callback;
                    break;
                case "error:connect":
                    _connectError += callback;
                    break;
                case "error:disconnect":
                    _disconnectError += callback;
                    break;
                case "error:sendState":
                    _sendStateError += callback;
                    break;
                case "playerJoined":
                    _playerJoined += callback;
                    break;
                case "playerLeft":
                    _playerLeft += callback;
                    break;
                case "playersUpdated":
                    _playersUpdated += callback;
                    break;
                case "customEvent":
                    _customEvent += callback;
                    break;
                case "hostMigrated":
                    _hostMigrated += callback;
                    break;
            }
        }

        public static void off(string eventName, UnityAction<GP_Data> callback)
        {
            switch (eventName)
            {
                case "connect":
                    _connect -= callback;
                    break;
                case "disconnect":
                    _disconnect -= callback;
                    break;
                case "error:connect":
                    _connectError -= callback;
                    break;
                case "error:disconnect":
                    _disconnectError -= callback;
                    break;
                case "error:sendState":
                    _sendStateError -= callback;
                    break;
                case "playerJoined":
                    _playerJoined -= callback;
                    break;
                case "playerLeft":
                    _playerLeft -= callback;
                    break;
                case "playersUpdated":
                    _playersUpdated -= callback;
                    break;
                case "customEvent":
                    _customEvent -= callback;
                    break;
                case "hostMigrated":
                    _hostMigrated -= callback;
                    break;
            }
        }

        public static void on<T>(string eventName, UnityAction<T> callback)
        {
            if (callback == null)
                return;

            if (!_typedEventCallbackWrappers.TryGetValue(eventName, out Dictionary<Delegate, UnityAction<GP_Data>> wrappers))
            {
                wrappers = new Dictionary<Delegate, UnityAction<GP_Data>>();
                _typedEventCallbackWrappers[eventName] = wrappers;
            }

            if (wrappers.ContainsKey(callback))
                return;

            UnityAction<GP_Data> wrapper = payload => callback.Invoke(ConvertEventPayload<T>(payload));
            wrappers[callback] = wrapper;
            on(eventName, wrapper);
        }

        public static void off<T>(string eventName, UnityAction<T> callback)
        {
            if (callback == null || !_typedEventCallbackWrappers.TryGetValue(eventName, out Dictionary<Delegate, UnityAction<GP_Data>> wrappers))
                return;

            if (!wrappers.TryGetValue(callback, out UnityAction<GP_Data> wrapper))
                return;

            off(eventName, wrapper);
            wrappers.Remove(callback);

            if (wrappers.Count == 0)
                _typedEventCallbackWrappers.Remove(eventName);
        }

        public static void on(string eventName, UnityAction callback)
        {
            switch (eventName)
            {
                case "becameHost":
                    _becameHost += callback;
                    break;
                case "becamePeer":
                    _becamePeer += callback;
                    break;
            }
        }

        public static void off(string eventName, UnityAction callback)
        {
            switch (eventName)
            {
                case "becameHost":
                    _becameHost -= callback;
                    break;
                case "becamePeer":
                    _becamePeer -= callback;
                    break;
            }
        }

        public static void onMessage(UnityAction<GP_Data> callback) => OnMessage(callback);
        public static void offMessage(UnityAction<GP_Data> callback) => OffMessage(callback);
        public static void onMessage<T>(UnityAction<T> callback)
        {
            if (callback == null || _typedMessageCallbackWrappers.ContainsKey(callback))
                return;

            UnityAction<GP_Data> wrapper = payload => callback.Invoke(ConvertEventPayload<T>(payload));
            _typedMessageCallbackWrappers[callback] = wrapper;
            OnMessage(wrapper);
        }

        public static void offMessage<T>(UnityAction<T> callback)
        {
            if (callback == null || !_typedMessageCallbackWrappers.TryGetValue(callback, out UnityAction<GP_Data> wrapper))
                return;

            OffMessage(wrapper);
            _typedMessageCallbackWrappers.Remove(callback);
        }
        public static void onTick(UnityAction<float> callback) => OnTick(callback);
        public static void offTick(UnityAction<float> callback) => OffTick(callback);

        internal static void OnMessage(UnityAction<GP_Data> callback) => _onMessage += callback;
        internal static void OffMessage(UnityAction<GP_Data> callback) => _onMessage -= callback;
        internal static void OnTick(UnityAction<GP_Data> callback) => _onTick += callback;
        internal static void OffTick(UnityAction<GP_Data> callback) => _onTick -= callback;
        internal static void OnTick(UnityAction<float> callback) => _onTickDelta += callback;
        internal static void OffTick(UnityAction<float> callback) => _onTickDelta -= callback;

        private static T ConvertEventPayload<T>(GP_Data payload)
        {
            if (payload == null)
                return default;

            if (typeof(T) == typeof(GP_Data))
                return (T)(object)payload;

            if (typeof(T) == typeof(string))
                return (T)(object)payload.Data;

            try
            {
                return payload.Get<T>();
            }
            catch
            {
                return default;
            }
        }

        private void CallOnMultiplayerConnect(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnConnect?.Invoke(payload);
            _connect?.Invoke(payload);
            CompleteTypedOperationSuccess(PendingConnectOperations, DeserializePayloadOrDefault<MultiplayerConnectResultData>(payload));
        }

        private void CallOnMultiplayerDisconnect(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnDisconnect?.Invoke(payload);
            _disconnect?.Invoke(payload);
            CompleteTypedOperationSuccess(PendingDisconnectOperations, true);
        }

        private void CallOnMultiplayerConnectError(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnConnectError?.Invoke(payload);
            _connectError?.Invoke(payload);
            CompleteTypedOperationError(PendingConnectOperations, data);
        }

        private void CallOnMultiplayerDisconnectError(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnDisconnectError?.Invoke(payload);
            _disconnectError?.Invoke(payload);
            CompleteTypedOperationError(PendingDisconnectOperations, data);
        }

        private void CallOnMultiplayerSendStateError(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnSendStateError?.Invoke(payload);
            _sendStateError?.Invoke(payload);
        }

        private void CallOnMultiplayerPlayerJoined(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnPlayerJoined?.Invoke(payload);
            _playerJoined?.Invoke(payload);
        }

        private void CallOnMultiplayerPlayerLeft(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnPlayerLeft?.Invoke(payload);
            _playerLeft?.Invoke(payload);
        }

        private void CallOnMultiplayerPlayersUpdated(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnPlayersUpdated?.Invoke(payload);
            _playersUpdated?.Invoke(payload);
        }

        private void CallOnMultiplayerCustomEvent(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnCustomEvent?.Invoke(payload);
            _customEvent?.Invoke(payload);
            _onMessage?.Invoke(payload);
        }

        private void CallOnMultiplayerBecameHost()
        {
            OnBecameHost?.Invoke();
            _becameHost?.Invoke();
        }

        private void CallOnMultiplayerBecamePeer()
        {
            OnBecamePeer?.Invoke();
            _becamePeer?.Invoke();
        }

        private void CallOnMultiplayerHostMigrated(string data)
        {
            GP_Data payload = new GP_Data(data);
            OnHostMigrated?.Invoke(payload);
            _hostMigrated?.Invoke(payload);
        }

        private void CallOnMultiplayerTick(string data)
        {
            GP_Data payload = new GP_Data(data);
            float.TryParse(data, NumberStyles.Float, CultureInfo.InvariantCulture, out float delta);
            OnTickEvent?.Invoke(payload);
            _onTick?.Invoke(payload);
            OnTickDelta?.Invoke(delta);
            _onTickDelta?.Invoke(delta);
        }

        private async void CallOnMultiplayerPlayerInitializerRequest(string data)
        {
            MultiplayerPlayerInitializerRequestData request =
                JsonUtility.FromJson<MultiplayerPlayerInitializerRequestData>(data);

            string state = "null";

            if (_playerInitializer != null)
            {
                GP_Data result = _playerInitializer.Invoke(request.playerId, request.player);
                if (result != null && !string.IsNullOrEmpty(result.Data))
                    state = result.Data;
            }
            else if (_playerInitializerAsync != null)
            {
                GP_Data result = await _playerInitializerAsync.Invoke(request.playerId, request.player);
                if (result != null && !string.IsNullOrEmpty(result.Data))
                    state = result.Data;
            }

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_ResolvePlayerInitializer(request.requestId, state);
#endif
        }
    }

    [Serializable]
    public class MultiplayerConnectedPlayerData
    {
        public int playerId;
        public bool isHost;
        public int ping;
        public float connectionStability;
        public int sessionDuration;
    }

    [Serializable]
    public class MultiplayerPlayerInitializerRequestData
    {
        public int requestId;
        public int playerId;
        public MultiplayerConnectedPlayerData player;
    }

    [Serializable]
    public class MultiplayerConnectResultData
    {
        public bool success;
    }

    [Serializable]
    public class MultiplayerDisconnectResultData
    {
        public string reason;
    }

    [Serializable]
    public class MultiplayerErrorData
    {
        public string message;
        public string name;
        public string code;
    }

    [Serializable]
    public class MultiplayerChannelQuery
    {
        public int channelId;
    }

    [Serializable]
    public class MultiplayerSendMessageOptions
    {
        public string target;
        public bool echo;
    }

    public enum MultiplayerMode
    {
        FAST,
        SMOOTH
    }
}
