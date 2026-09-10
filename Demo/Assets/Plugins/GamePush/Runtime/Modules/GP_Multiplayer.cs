using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Native;

namespace GamePush
{
    public class GP_Multiplayer : GP_Module
    {
        private sealed class ActiveOperation<T>
        {
            public int Generation;
            public TaskCompletionSource<T> Completion;
            public CancellationTokenRegistration Cancellation = default;
        }

        private static int _operationGeneration;
        private static ActiveOperation<MultiplayerConnectResultData> _connectOperation;
        private static ActiveOperation<bool> _disconnectOperation;

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

#if UNITY_EDITOR
        static bool Play2WebLive => GP_Play2Web.Enabled;
        static bool NativeSession => GamePushHost.UseNativeCore || Play2WebLive;
#else
        static bool Play2WebLive => false;
        static bool NativeSession => GamePushHost.UseNativeCore;
#endif
        static bool _liveConnected;
        static bool _liveHost;

        static GP_Data LiveGet(string method)
        {
#if UNITY_EDITOR
            if (!Play2WebLive)
                return null;
            if (GP_Play2Web.TryGet(method, out var json) && !string.IsNullOrEmpty(json))
                return CreateDataOrNull(json);
            GP_Play2Web.Call(method);
            return GP_Play2Web.TryGet(method, out json) ? CreateDataOrNull(json) : null;
#else
            return null;
#endif
        }

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

        private static ActiveOperation<T> BeginOperation<T>(ref ActiveOperation<T> active)
        {
            if (active != null)
                throw new InvalidOperationException("Multiplayer operation is already in progress");
            var operation = new ActiveOperation<T>
            {
                Generation = Interlocked.Increment(ref _operationGeneration),
                Completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously),
            };
            active = operation;
            return operation;
        }

        private static void CancelConnect(ActiveOperation<MultiplayerConnectResultData> operation)
        {
            if (!ReferenceEquals(_connectOperation, operation)) return;
            _connectOperation = null;
            operation.Completion.TrySetCanceled();
        }

        private static void CancelDisconnect(ActiveOperation<bool> operation)
        {
            if (!ReferenceEquals(_disconnectOperation, operation)) return;
            _disconnectOperation = null;
            operation.Completion.TrySetCanceled();
        }

        private static Task<T> CreateFaultedTask<T>(Exception exception)
        {
            TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
            completionSource.SetException(exception);
            return completionSource.Task;
        }

        private static bool CompleteSuccess<T>(ref ActiveOperation<T> active, int generation, T result)
        {
            ActiveOperation<T> operation = active;
            if (operation == null || operation.Generation != generation) return false;
            active = null;
            operation.Cancellation.Dispose();
            operation.Completion.TrySetResult(result);
            return true;
        }

        private static bool CompleteError<T>(ref ActiveOperation<T> active, int generation, string data)
        {
            ActiveOperation<T> operation = active;
            if (operation == null || operation.Generation != generation) return false;
            active = null;
            operation.Cancellation.Dispose();
            operation.Completion.TrySetException(new Exception(ExtractErrorMessage(data)));
            return true;
        }

#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_Connect(string query, int generation);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_Disconnect(string query, int generation);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_DefinePlayerSchema(string schema);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_DefineGlobalSchema(string schema);
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SetPlayerInitializer();
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_ClearPlayerInitializer();
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_ResolvePlayerInitializer(int requestId, string state);
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SetPlayerState(string state);
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SetGlobalState(string state);
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SetMode(string mode);
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Multiplayer_SendMessage(string eventName, string data, string options);
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int GP_Multiplayer_TickRate();
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_IsConnected();
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_IsHost();
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_ConnectedPlayers();
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_NetworkStats();
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_MyState();
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_PlayersState();
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_GlobalState();
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_RuntimeCapabilities();
        #endif
#endif

        public static Task<MultiplayerConnectResultData> connect(MultiplayerChannelQuery query) =>
            connect(query, CancellationToken.None);

        public static Task<MultiplayerConnectResultData> connect(MultiplayerChannelQuery query,
            CancellationToken cancellationToken)
        {
            string payload = JsonUtility.ToJson(query ?? new MultiplayerChannelQuery());
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            ActiveOperation<MultiplayerConnectResultData> operation;
            try { operation = BeginOperation(ref _connectOperation); }
            catch (Exception exception) { return CreateFaultedTask<MultiplayerConnectResultData>(exception); }
            if (cancellationToken.CanBeCanceled)
                operation.Cancellation = cancellationToken.Register(() => CancelConnect(operation));
            if (!ReferenceEquals(_connectOperation, operation))
                return operation.Completion.Task;
            try { GP_Multiplayer_Connect(payload, operation.Generation); }
            catch (Exception exception)
            {
                CompleteError(ref _connectOperation, operation.Generation, exception.Message);
            }
            return operation.Completion.Task;
#else
            if (GamePushHost.UseNativeCore)
                return NativeMultiplayer.Connect(query, cancellationToken);
            if (Play2WebLive)
            {
                ActiveOperation<MultiplayerConnectResultData> operation;
                try { operation = BeginOperation(ref _connectOperation); }
                catch (Exception exception) { return CreateFaultedTask<MultiplayerConnectResultData>(exception); }
                if (cancellationToken.CanBeCanceled)
                    operation.Cancellation = cancellationToken.Register(() => CancelConnect(operation));
                if (!ReferenceEquals(_connectOperation, operation))
                    return operation.Completion.Task;
                NativePlayer.Adopt(GP_Player.GetID(), GP_Player.GetName());
                if (!GP_Play2Web.Call("Multiplayer_Connect", payload, operation.Generation))
                    CompleteError(ref _connectOperation, operation.Generation, "Play2Web unavailable");
                return operation.Completion.Task;
            }
            ConsoleLog($"CONNECT: {payload}");
            return Task.FromResult<MultiplayerConnectResultData>(null);
#endif
        }

        public static Task disconnect(MultiplayerChannelQuery query) =>
            disconnect(query, CancellationToken.None);

        public static Task disconnect(MultiplayerChannelQuery query, CancellationToken cancellationToken)
        {
            string payload = JsonUtility.ToJson(query ?? new MultiplayerChannelQuery());
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            ActiveOperation<bool> operation;
            try { operation = BeginOperation(ref _disconnectOperation); }
            catch (Exception exception) { return CreateFaultedTask<bool>(exception); }
            if (cancellationToken.CanBeCanceled)
                operation.Cancellation = cancellationToken.Register(() => CancelDisconnect(operation));
            if (!ReferenceEquals(_disconnectOperation, operation))
                return operation.Completion.Task;
            try { GP_Multiplayer_Disconnect(payload, operation.Generation); }
            catch (Exception exception)
            {
                CompleteError(ref _disconnectOperation, operation.Generation, exception.Message);
            }
            return operation.Completion.Task;
#else
            if (GamePushHost.UseNativeCore)
                return NativeMultiplayer.Disconnect(query, cancellationToken);
            if (Play2WebLive)
            {
                ActiveOperation<bool> operation;
                try { operation = BeginOperation(ref _disconnectOperation); }
                catch (Exception exception) { return CreateFaultedTask<bool>(exception); }
                if (cancellationToken.CanBeCanceled)
                    operation.Cancellation = cancellationToken.Register(() => CancelDisconnect(operation));
                if (!ReferenceEquals(_disconnectOperation, operation))
                    return operation.Completion.Task;
                NativeMultiplayer.CloseLocal();
                if (!GP_Play2Web.Call("Multiplayer_Disconnect", payload, operation.Generation))
                    CompleteError(ref _disconnectOperation, operation.Generation, "Play2Web unavailable");
                return operation.Completion.Task;
            }
            ConsoleLog($"DISCONNECT: {payload}");
            return Task.CompletedTask;
#endif
        }

        public static void definePlayerSchema(GP_Data schema)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Multiplayer_DefinePlayerSchema(schema?.Data ?? "{}");
#else
            if (NativeSession)
            {
                NativeMultiplayer.DefinePlayerSchema(schema?.Data ?? "{}");
                return;
            }
            if (GP_Play2Web.Call("Multiplayer_DefinePlayerSchema", schema?.Data ?? "{}"))
                return;
            ConsoleLog($"DEFINE PLAYER SCHEMA: {schema?.Data ?? "{}"}");
#endif
        }

        public static void defineGlobalSchema(GP_Data schema)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Multiplayer_DefineGlobalSchema(schema?.Data ?? "{}");
#else
            if (NativeSession)
            {
                NativeMultiplayer.DefineGlobalSchema(schema?.Data ?? "{}");
                return;
            }
            if (GP_Play2Web.Call("Multiplayer_DefineGlobalSchema", schema?.Data ?? "{}"))
                return;
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
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            if (enabled)
                GP_Multiplayer_SetPlayerInitializer();
            else
                GP_Multiplayer_ClearPlayerInitializer();
#else
            if (NativeSession)
            {
                ConsoleLog(enabled ? "ENABLE PLAYER INITIALIZER" : "DISABLE PLAYER INITIALIZER");
                return;
            }
            if (enabled)
            {
                if (GP_Play2Web.Call("Multiplayer_SetPlayerInitializer"))
                    return;
            }
            else if (GP_Play2Web.Call("Multiplayer_ClearPlayerInitializer"))
                return;
            ConsoleLog(enabled ? "ENABLE PLAYER INITIALIZER" : "DISABLE PLAYER INITIALIZER");
#endif
        }

        public static void setPlayerState(GP_Data state)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Multiplayer_SetPlayerState(state?.Data ?? "{}");
#else
            if (NativeSession)
            {
                NativeMultiplayer.SetPlayerState(state?.Data ?? "{}");
                return;
            }
            if (GP_Play2Web.Call("Multiplayer_SetPlayerState", state?.Data ?? "{}"))
                return;
            ConsoleLog($"SET PLAYER STATE: {state?.Data ?? "{}"}");
#endif
        }

        public static void setGlobalState(GP_Data state)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Multiplayer_SetGlobalState(state?.Data ?? "{}");
#else
            if (NativeSession)
            {
                NativeMultiplayer.SetGlobalState(state?.Data ?? "{}");
                return;
            }
            if (GP_Play2Web.Call("Multiplayer_SetGlobalState", state?.Data ?? "{}"))
                return;
            ConsoleLog($"SET GLOBAL STATE: {state?.Data ?? "{}"}");
#endif
        }

        public static void setMode(MultiplayerMode mode)
        {
            string value = mode == MultiplayerMode.FAST ? "fast" : "smooth";
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Multiplayer_SetMode(value);
#else
            if (NativeSession)
            {
                NativeMultiplayer.SetMode(value);
                return;
            }
            if (GP_Play2Web.Call("Multiplayer_SetMode", value))
                return;
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
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Multiplayer_SendMessage(eventName ?? string.Empty, data, options ?? "undefined");
#else
            if (NativeSession)
            {
                NativeMultiplayer.SendMessage(eventName ?? string.Empty, data, options ?? "undefined");
                return;
            }
            if (GP_Play2Web.Call("Multiplayer_SendMessage", eventName ?? string.Empty, data, options ?? "undefined"))
                return;
            ConsoleLog($"SEND MESSAGE: {eventName}, {data}, {options ?? "undefined"}");
#endif
        }

        public static int tickRate
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
                return GP_Multiplayer_TickRate();
#else
                if (NativeSession)
                    return NativeMultiplayer.TickRate;
                if (Play2WebLive)
                {
                    if (GP_Play2Web.TryGetInt("Multiplayer_TickRate", out var rate) && rate > 0)
                        return rate;
                    GP_Play2Web.Call("Multiplayer_TickRate");
                    return 20;
                }
                return 0;
#endif
            }
        }

        public static bool isConnected
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
                return GP_Multiplayer_IsConnected() == "true";
#else
                if (NativeSession)
                    return NativeMultiplayer.IsConnected;
                if (Play2WebLive)
                    return _liveConnected;
                return false;
#endif
            }
        }

        public static bool isHost
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
                return GP_Multiplayer_IsHost() == "true";
#else
                if (NativeSession)
                    return NativeMultiplayer.IsHost;
                if (Play2WebLive)
                    return _liveHost;
                return false;
#endif
            }
        }

        public static GP_Data connectedPlayers
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
                return CreateDataOrNull(GP_Multiplayer_ConnectedPlayers());
#else
                if (NativeSession)
                    return CreateDataOrNull(NativeMultiplayer.ConnectedPlayersJson());
                if (Play2WebLive)
                    return LiveGet("Multiplayer_ConnectedPlayers");
                return null;
#endif
            }
        }

        public static GP_Data networkStats
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
                return CreateDataOrNull(GP_Multiplayer_NetworkStats());
#else
                if (NativeSession)
                    return CreateDataOrNull(NativeMultiplayer.NetworkStatsJson());
                if (Play2WebLive)
                    return LiveGet("Multiplayer_NetworkStats");
                return null;
#endif
            }
        }

        public static GP_Data myState
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
                return CreateDataOrNull(GP_Multiplayer_MyState());
#else
                if (NativeSession)
                    return CreateDataOrNull(NativeMultiplayer.MyStateJson());
                if (Play2WebLive)
                    return LiveGet("Multiplayer_MyState");
                return null;
#endif
            }
        }

        public static GP_Data playersState
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
                return CreateDataOrNull(GP_Multiplayer_PlayersState());
#else
                if (NativeSession)
                    return CreateDataOrNull(NativeMultiplayer.PlayersStateJson());
                if (Play2WebLive)
                    return LiveGet("Multiplayer_PlayersState");
                return null;
#endif
            }
        }

        public static GP_Data globalState
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
                return CreateDataOrNull(GP_Multiplayer_GlobalState());
#else
                if (NativeSession)
                    return CreateDataOrNull(NativeMultiplayer.GlobalStateJson());
                if (Play2WebLive)
                    return LiveGet("Multiplayer_GlobalState");
                return null;
#endif
            }
        }

        public static GP_Data runtimeCapabilities
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
                return CreateDataOrNull(GP_Multiplayer_RuntimeCapabilities());
#else
                if (NativeSession)
                    return CreateDataOrNull(NativeMultiplayer.RuntimeCapabilitiesJson());
                if (Play2WebLive)
                    return LiveGet("Multiplayer_RuntimeCapabilities");
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

        internal static void NativeEmit(string eventName, GP_Data data)
        {
            switch (eventName)
            {
                case "connect":
                    _liveConnected = true;
                    _connect?.Invoke(data);
                    break;
                case "disconnect":
                    _liveConnected = false;
                    _liveHost = false;
                    _disconnect?.Invoke(data);
                    break;
                case "error:connect":
                    _connectError?.Invoke(data);
                    break;
                case "error:disconnect":
                    _disconnectError?.Invoke(data);
                    break;
                case "error:sendState":
                    _sendStateError?.Invoke(data);
                    break;
                case "playerJoined":
                    _playerJoined?.Invoke(data);
                    break;
                case "playerLeft":
                    _playerLeft?.Invoke(data);
                    break;
                case "playersUpdated":
                    _playersUpdated?.Invoke(data);
                    break;
                case "globalStateUpdated":
                    _globalStateUpdated?.Invoke(data);
                    break;
                case "customEvent":
                    _customEvent?.Invoke(data);
                    _onMessage?.Invoke(data);
                    break;
                case "hostMigrated":
                    _hostMigrated?.Invoke(data);
                    break;
            }
        }

        internal static void NativeEmit(string eventName)
        {
            switch (eventName)
            {
                case "becameHost":
                    _liveHost = true;
                    _becameHost?.Invoke();
                    break;
                case "becamePeer":
                    _liveHost = false;
                    _becamePeer?.Invoke();
                    break;
            }
        }

        internal static void NativeEmitMessage(GP_Data data) => _onMessage?.Invoke(data);
        internal static void NativeEmitTick(float delta) => _onTick?.Invoke(delta);

        internal static string NativeInitPlayer(int playerId, MultiplayerConnectedPlayerData player)
        {
            try
            {
                if (_playerInitializer != null)
                {
                    var result = _playerInitializer(playerId, player);
                    return result?.Data ?? "null";
                }
            }
            catch (Exception exception)
            {
                ConsoleLog("PLAYER INITIALIZER ERROR: " + exception.Message);
            }
            return "null";
        }

        internal static async Task<string> NativeInitPlayerAsync(int playerId, MultiplayerConnectedPlayerData player)
        {
            try
            {
                if (_playerInitializer != null)
                {
                    var result = _playerInitializer(playerId, player);
                    return result?.Data ?? "null";
                }
                if (_playerInitializerAsync != null)
                {
                    var result = await _playerInitializerAsync(playerId, player);
                    return result?.Data ?? "null";
                }
            }
            catch (Exception exception)
            {
                ConsoleLog("PLAYER INITIALIZER ERROR: " + exception.Message);
            }
            return "null";
        }

        private async void CallOnMultiplayerConnectCredentials(string data)
        {
            MultiplayerOperationEnvelope envelope = ParseOperationEnvelope(data);
            if (envelope.generation != 0 && (_connectOperation == null ||
                _connectOperation.Generation != envelope.generation)) return;
            try
            {
                NativePlayer.Adopt(GP_Player.GetID(), GP_Player.GetName());
                MultiplayerConnectResultData result =
                    await NativeMultiplayer.ConnectWithTransport(envelope.data, CancellationToken.None);
                if (envelope.generation == 0 ||
                    CompleteSuccess(ref _connectOperation, envelope.generation, result))
                    _liveConnected = true;
            }
            catch (Exception exception)
            {
                CompleteError(ref _connectOperation, envelope.generation,
                    "{\"message\":\"" + (exception.Message ?? "connect_failed").Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"}");
            }
        }

        private void CallOnMultiplayerConnect(string data)
        {
            MultiplayerOperationEnvelope envelope = ParseOperationEnvelope(data);
            if (envelope.generation != 0 && (_connectOperation == null ||
                _connectOperation.Generation != envelope.generation)) return;
            GP_Data payload = new GP_Data(envelope.data);

            MultiplayerConnectResultData result = null;
            try
            {
                result = payload.Get<MultiplayerConnectResultData>();
            }
            catch
            {
            }

            if (envelope.generation == 0 ||
                CompleteSuccess(ref _connectOperation, envelope.generation, result))
            {
#if UNITY_EDITOR
                _liveConnected = true;
                if (Play2WebLive)
                {
                    GP_Play2Web.Call("Multiplayer_IsHost");
                    GP_Play2Web.Call("Multiplayer_ConnectedPlayers");
                    GP_Play2Web.Call("Multiplayer_TickRate");
                }
#endif
                _connect?.Invoke(payload);
            }
        }

        private void CallOnMultiplayerDisconnect(string data)
        {
            MultiplayerOperationEnvelope envelope = ParseOperationEnvelope(data);
            if (envelope.generation != 0 && (_disconnectOperation == null ||
                _disconnectOperation.Generation != envelope.generation)) return;
            if (envelope.generation == 0 ||
                CompleteSuccess(ref _disconnectOperation, envelope.generation, true))
            {
#if UNITY_EDITOR
                _liveConnected = false;
                _liveHost = false;
#endif
                _disconnect?.Invoke(new GP_Data(envelope.data));
            }
        }

        private void CallOnMultiplayerConnectError(string data)
        {
            MultiplayerOperationEnvelope envelope = ParseOperationEnvelope(data);
            if (envelope.generation != 0 && (_connectOperation == null ||
                _connectOperation.Generation != envelope.generation)) return;
            if (envelope.generation == 0 ||
                CompleteError(ref _connectOperation, envelope.generation, envelope.data))
                _connectError?.Invoke(new GP_Data(envelope.data));
        }

        private void CallOnMultiplayerDisconnectError(string data)
        {
            MultiplayerOperationEnvelope envelope = ParseOperationEnvelope(data);
            if (envelope.generation != 0 && (_disconnectOperation == null ||
                _disconnectOperation.Generation != envelope.generation)) return;
            if (envelope.generation == 0 ||
                CompleteError(ref _disconnectOperation, envelope.generation, envelope.data))
                _disconnectError?.Invoke(new GP_Data(envelope.data));
        }

        private static MultiplayerOperationEnvelope ParseOperationEnvelope(string data)
        {
            try
            {
                MultiplayerOperationEnvelope envelope = JsonUtility.FromJson<MultiplayerOperationEnvelope>(data);
                if (envelope != null && envelope.wrapped) return envelope;
            }
            catch
            {
            }
            return new MultiplayerOperationEnvelope { data = data, generation = 0 };
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
        private void CallOnMultiplayerBecameHost()
        {
#if UNITY_EDITOR
            _liveHost = true;
#endif
            _becameHost?.Invoke();
        }

        private void CallOnMultiplayerBecamePeer()
        {
#if UNITY_EDITOR
            _liveHost = false;
#endif
            _becamePeer?.Invoke();
        }

        private async void CallOnMultiplayerPlayerInitializerRequest(string data)
        {
            MultiplayerPlayerInitializerRequestData request =
                JsonUtility.FromJson<MultiplayerPlayerInitializerRequestData>(data);

            string state = "null";

            try
            {
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
            }
            catch (Exception exception)
            {
                ConsoleLog($"PLAYER INITIALIZER ERROR: {exception.Message}");
            }

#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Multiplayer_ResolvePlayerInitializer(request.requestId, state);
#else
            GP_Play2Web.Call("Multiplayer_ResolvePlayerInitializer", request.requestId, state);
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
    public class MultiplayerPlayerJoinedData
    {
        public MultiplayerConnectedPlayerData player;
        public bool isSelf;
    }

    [Serializable]
    public class MultiplayerHostMigratedData
    {
        public int oldHost;
        public int newHost;
    }

    [Serializable]
    public class MultiplayerMessageEventData
    {
        public string eventName = string.Empty;
        public string senderId = string.Empty;
        public string data = string.Empty;
        public double timestamp;
    }

    [Serializable]
    public class MultiplayerPlayerStateEntryData
    {
        public string playerId = string.Empty;
        public string state = string.Empty;
    }

    [Serializable]
    public class MultiplayerPlayerStateEntriesData
    {
        public MultiplayerPlayerStateEntryData[] players = Array.Empty<MultiplayerPlayerStateEntryData>();
    }

    [Serializable]
    public class MultiplayerRuntimeCapabilitiesData
    {
        public bool connect;
        public bool disconnect;
        public bool setPlayerState;
        public bool setGlobalState;
        public bool sendMessage;
        public bool hostMigrationEvents;

        public bool IsSupported => connect && disconnect && setPlayerState && setGlobalState &&
                                   sendMessage && hostMigrationEvents;
    }

    [Serializable]
    public class MultiplayerOperationEnvelope
    {
        public bool wrapped;
        public int generation;
        public string data = string.Empty;
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
