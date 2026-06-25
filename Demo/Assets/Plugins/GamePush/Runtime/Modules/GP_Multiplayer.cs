using System;
using System.Collections.Generic;
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
        private static event UnityAction<GP_Data> _playerJoined;
        private static event UnityAction<GP_Data> _playerLeft;
        private static event UnityAction<GP_Data> _hostMigrated;
        private static event UnityAction _becameHost;
        private static event UnityAction _becamePeer;

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
        private static extern string GP_Multiplayer_IsConnected();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_IsHost();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_ConnectedPlayers();
        [DllImport("__Internal")]
        private static extern string GP_Multiplayer_NetworkStats();
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
                case "playerJoined":
                    _playerJoined += callback;
                    break;
                case "playerLeft":
                    _playerLeft += callback;
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
                case "playerJoined":
                    _playerJoined -= callback;
                    break;
                case "playerLeft":
                    _playerLeft -= callback;
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

        private void CallOnMultiplayerPlayerJoined(string data) => _playerJoined?.Invoke(new GP_Data(data));
        private void CallOnMultiplayerPlayerLeft(string data) => _playerLeft?.Invoke(new GP_Data(data));
        private void CallOnMultiplayerHostMigrated(string data) => _hostMigrated?.Invoke(new GP_Data(data));
        private void CallOnMultiplayerBecameHost() => _becameHost?.Invoke();
        private void CallOnMultiplayerBecamePeer() => _becamePeer?.Invoke();
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
}
