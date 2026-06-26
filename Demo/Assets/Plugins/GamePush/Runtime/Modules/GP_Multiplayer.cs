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

        private static event UnityAction<GP_Data> _connect;
        private static event UnityAction<GP_Data> _disconnect;
        private static event UnityAction<GP_Data> _connectError;
        private static event UnityAction<GP_Data> _disconnectError;
        private static event UnityAction<GP_Data> _sendStateError;
        private static event UnityAction<GP_Data> _playerJoined;
        private static event UnityAction<GP_Data> _playerLeft;
        private static event UnityAction<GP_Data> _playersUpdated;
        private static event UnityAction<GP_Data> _globalStateUpdated;
        private static event UnityAction<GP_Data> _customEvent;
        private static event UnityAction<GP_Data> _hostMigrated;
        private static event UnityAction _becameHost;
        private static event UnityAction _becamePeer;
        private static event UnityAction<GP_Data> _onMessage;
        private static event UnityAction<float> _onTick;

        private static Func<int, MultiplayerConnectedPlayerData, GP_Data> _playerInitializer;
        private static Func<int, MultiplayerConnectedPlayerData, Task<GP_Data>> _playerInitializerAsync;

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

        private static Task<T> RunOperation<T>(Queue<TaskCompletionSource<T>> queue, Action action)
        {
            TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
            queue.Enqueue(completionSource);

            try
            {
                action();
            }
            catch (Exception exception)
            {
                if (queue.Count > 0 && ReferenceEquals(queue.Peek(), completionSource))
                    queue.Dequeue();

                completionSource.TrySetException(exception);
            }

            return completionSource.Task;
        }

        private static void CompleteSuccess<T>(Queue<TaskCompletionSource<T>> queue, T result)
        {
            if (queue.Count > 0)
                queue.Dequeue().TrySetResult(result);
        }

        private static void CompleteError<T>(Queue<TaskCompletionSource<T>> queue, string data)
        {
            if (queue.Count > 0)
                queue.Dequeue().TrySetException(new Exception(ExtractErrorMessage(data)));
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_Connect(string query);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_Disconnect(string query);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_DefinePlayerSchema(string schema);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_DefineGlobalSchema(string schema);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SetPlayerInitializer();
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_ClearPlayerInitializer();
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_ResolvePlayerInitializer(int requestId, string state);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SetPlayerState(string state);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SetGlobalState(string state);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SetMode(string mode);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SendMessage(string eventName, string data, string options);
        [DllImport("__Internal")]
        private static extern int GP_Multiplayer_TickRate();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_IsConnected();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_IsHost();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_ConnectedPlayers();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_NetworkStats();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_MyState();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_PlayersState();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_GlobalState();
#endif

        public static Task<MultiplayerConnectResultData> connect(MultiplayerChannelQuery query)
        {
            string payload = JsonUtility.ToJson(query ?? new MultiplayerChannelQuery());
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunOperation(PendingConnectOperations, () => GP_Multiplayer_Connect(payload));
#else
            ConsoleLog($"CONNECT: {payload}");
            return Task.FromResult<MultiplayerConnectResultData>(null);
#endif
        }

        public static Task disconnect(MultiplayerChannelQuery query)
        {
            string payload = JsonUtility.ToJson(query ?? new MultiplayerChannelQuery());
#if !UNITY_EDITOR && UNITY_WEBGL
            return RunOperation(PendingDisconnectOperations, () => GP_Multiplayer_Disconnect(payload));
#else
            ConsoleLog($"DISCONNECT: {payload}");
            return Task.CompletedTask;
#endif
        }

        public static void definePlayerSchema(GP_Data schema)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_DefinePlayerSchema(schema?.Data ?? "{}");
#else
            ConsoleLog($"DEFINE PLAYER SCHEMA: {schema?.Data ?? "{}"}");
#endif
        }

        public static void defineGlobalSchema(GP_Data schema)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_DefineGlobalSchema(schema?.Data ?? "{}");
#else
            ConsoleLog($"DEFINE GLOBAL SCHEMA: {schema?.Data ?? "{}"}");
#endif
        }

        public static Task setPlayerInitializer(Func<int, MultiplayerConnectedPlayerData, GP_Data> initializer)
        {
            _playerInitializer = initializer;
            _playerInitializerAsync = null;
            ApplyPlayerInitializer(initializer != null);
            return Task.CompletedTask;
        }

        public static Task setPlayerInitializer(Func<int, MultiplayerConnectedPlayerData, Task<GP_Data>> initializer)
        {
            _playerInitializer = null;
            _playerInitializerAsync = initializer;
            ApplyPlayerInitializer(initializer != null);
            return Task.CompletedTask;
        }

        private static void ApplyPlayerInitializer(bool enabled)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            if (enabled)
                GP_Multiplayer_SetPlayerInitializer();
            else
                GP_Multiplayer_ClearPlayerInitializer();
#else
            ConsoleLog(enabled ? "ENABLE PLAYER INITIALIZER" : "DISABLE PLAYER INITIALIZER");
#endif
        }

        public static void setPlayerState(GP_Data state)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_SetPlayerState(state?.Data ?? "{}");
#else
            ConsoleLog($"SET PLAYER STATE: {state?.Data ?? "{}"}");
#endif
        }

        public static void setGlobalState(GP_Data state)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_SetGlobalState(state?.Data ?? "{}");
#else
            ConsoleLog($"SET GLOBAL STATE: {state?.Data ?? "{}"}");
#endif
        }

        public static void setMode(MultiplayerMode mode)
        {
            string value = mode == MultiplayerMode.FAST ? "fast" : "smooth";
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_SetMode(value);
#else
            ConsoleLog($"SET MODE: {value}");
#endif
        }

        public static void sendMessage(string eventName, GP_Data data)
        {
            SendMessageInternal(eventName, data?.Data ?? "null", null);
        }

        public static void sendMessage(
            string eventName,
            GP_Data data,
            MultiplayerSendMessageOptions options)
        {
            SendMessageInternal(
                eventName,
                data?.Data ?? "null",
                options == null ? null : JsonUtility.ToJson(options));
        }

        public static void sendMessage(string eventName, GP_Data data, int target)
        {
            SendMessageInternal(
                eventName,
                data?.Data ?? "null",
                target.ToString(CultureInfo.InvariantCulture));
        }

        public static void sendMessage(string eventName, GP_Data data, string target)
        {
            SendMessageInternal(eventName, data?.Data ?? "null", target);
        }

        private static void SendMessageInternal(string eventName, string data, string options)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Multiplayer_SendMessage(eventName ?? string.Empty, data, options ?? "undefined");
#else
            ConsoleLog($"SEND MESSAGE: {eventName}, {data}, {options ?? "undefined"}");
#endif
        }

        public static int tickRate
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL
                return GP_Multiplayer_TickRate();
#else
                return 0;
#endif
            }
        }

        public static bool isConnected
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL
                return GP_Multiplayer_IsConnected() == "true";
#else
                return false;
#endif
            }
        }

        public static bool isHost
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL
                return GP_Multiplayer_IsHost() == "true";
#else
                return false;
#endif
            }
        }

        public static GP_Data connectedPlayers
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL
                return CreateDataOrNull(GP_Multiplayer_ConnectedPlayers());
#else
                return null;
#endif
            }
        }

        public static GP_Data networkStats
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL
                return CreateDataOrNull(GP_Multiplayer_NetworkStats());
#else
                return null;
#endif
            }
        }

        public static GP_Data myState
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL
                return CreateDataOrNull(GP_Multiplayer_MyState());
#else
                return null;
#endif
            }
        }

        public static GP_Data playersState
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL
                return CreateDataOrNull(GP_Multiplayer_PlayersState());
#else
                return null;
#endif
            }
        }

        public static GP_Data globalState
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL
                return CreateDataOrNull(GP_Multiplayer_GlobalState());
#else
                return null;
#endif
            }
        }

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
                case "globalStateUpdated":
                    _globalStateUpdated += callback;
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
                case "globalStateUpdated":
                    _globalStateUpdated -= callback;
                    break;
                case "customEvent":
                    _customEvent -= callback;
                    break;
                case "hostMigrated":
                    _hostMigrated -= callback;
                    break;
            }
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

        public static void onMessage(UnityAction<GP_Data> callback) => _onMessage += callback;
        public static void offMessage(UnityAction<GP_Data> callback) => _onMessage -= callback;
        public static void onTick(UnityAction<float> callback) => _onTick += callback;
        public static void offTick(UnityAction<float> callback) => _onTick -= callback;

        private void CallOnMultiplayerConnect(string data)
        {
            GP_Data payload = new GP_Data(data);
            _connect?.Invoke(payload);

            MultiplayerConnectResultData result = null;
            try
            {
                result = payload.Get<MultiplayerConnectResultData>();
            }
            catch
            {
            }

            CompleteSuccess(PendingConnectOperations, result);
        }

        private void CallOnMultiplayerDisconnect(string data)
        {
            _disconnect?.Invoke(new GP_Data(data));
            CompleteSuccess(PendingDisconnectOperations, true);
        }

        private void CallOnMultiplayerConnectError(string data)
        {
            _connectError?.Invoke(new GP_Data(data));
            CompleteError(PendingConnectOperations, data);
        }

        private void CallOnMultiplayerDisconnectError(string data)
        {
            _disconnectError?.Invoke(new GP_Data(data));
            CompleteError(PendingDisconnectOperations, data);
        }

        private void CallOnMultiplayerSendStateError(string data) => _sendStateError?.Invoke(new GP_Data(data));
        private void CallOnMultiplayerPlayerJoined(string data) => _playerJoined?.Invoke(new GP_Data(data));
        private void CallOnMultiplayerPlayerLeft(string data) => _playerLeft?.Invoke(new GP_Data(data));
        private void CallOnMultiplayerPlayersUpdated(string data) => _playersUpdated?.Invoke(new GP_Data(data));
        private void CallOnMultiplayerGlobalStateUpdated(string data) => _globalStateUpdated?.Invoke(new GP_Data(data));
        private void CallOnMultiplayerCustomEvent(string data)
        {
            GP_Data payload = new GP_Data(data);
            _customEvent?.Invoke(payload);
            _onMessage?.Invoke(payload);
        }

        private void CallOnMultiplayerTick(string data)
        {
            float.TryParse(data, NumberStyles.Float, CultureInfo.InvariantCulture, out float delta);
            _onTick?.Invoke(delta);
        }

        private void CallOnMultiplayerHostMigrated(string data) => _hostMigrated?.Invoke(new GP_Data(data));
        private void CallOnMultiplayerBecameHost() => _becameHost?.Invoke();
        private void CallOnMultiplayerBecamePeer() => _becamePeer?.Invoke();

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
    public class MultiplayerConnectResultData
    {
        public bool success;
    }

    [Serializable]
    public class MultiplayerPlayerInitializerRequestData
    {
        public int requestId;
        public int playerId;
        public MultiplayerConnectedPlayerData player;
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
