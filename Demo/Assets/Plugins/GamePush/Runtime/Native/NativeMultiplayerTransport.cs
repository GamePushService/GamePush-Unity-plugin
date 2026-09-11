using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GamePush;

namespace GamePush.Native
{
    sealed class NativeMultiplayerTransport : IDisposable
    {
        public event Action Reconnecting;
        public event Action Reconnected;
        public event Action<string> Disconnected;
        public event Action<NativeMultiplayerWireMessage> Message;

        NativeCentrifugo _centrifuge;
        NativeSubscription _hostSub;
        NativeSubscription _stateSub;
        bool _live;
        int _loggedWire;

        // The initial Connect() waits for both subscriptions. During a reconnect the
        // WebGL bridge may not repeat the subscribe acknowledgement even though the
        // socket is usable; gating the whole multiplayer loop on that callback can leave
        // a creator without heartbeat/host election forever.
        public bool IsLive => _live && _centrifuge != null;

        public async Task Connect(string credentialsJson, CancellationToken cancellationToken)
        {
            NativeMainThread.Ensure();
            var connection = GpJson.GetObject(credentialsJson, "connection");
            var hostInfo = GpJson.GetObject(credentialsJson, "hostSubscription");
            var stateInfo = GpJson.GetObject(credentialsJson, "stateSubscription");
            var endpoint = GpJson.TryGetString(connection, "endpoint", out var ep) && !string.IsNullOrEmpty(ep)
                ? ep
                : NativeCore.CentrifugeWs;
            endpoint = EnsureProtobufEndpoint(endpoint);
            var token = GpJson.TryGetString(connection, "token", out var tok) ? tok : "";
            var hostChannel = GpJson.TryGetString(hostInfo, "channel", out var hc) ? hc : "";
            var hostToken = GpJson.TryGetString(hostInfo, "token", out var ht) ? ht : "";
            var stateChannel = GpJson.TryGetString(stateInfo, "channel", out var sc) ? sc : "";
            var stateToken = GpJson.TryGetString(stateInfo, "token", out var st) ? st : "";
            GP_Logger.Info("Centrifugo", "open " + endpoint +
                                         " host=" + hostChannel +
                                         " state=" + stateChannel +
                                         " tokenLen=" + (token == null ? 0 : token.Length));

            var connected = new TaskCompletionSource<bool>();
            var hostReady = new TaskCompletionSource<bool>();
            var stateReady = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() =>
            {
                connected.TrySetCanceled();
                hostReady.TrySetCanceled();
                stateReady.TrySetCanceled();
            }))
            {
                _centrifuge = new NativeCentrifugo();
                _centrifuge.Connected += () =>
                {
                    GP_Logger.Info("Centrifugo", "connected");
                    connected.TrySetResult(true);
                };
                _centrifuge.Reconnecting += () =>
                {
                    _live = false;
                    _hostSub?.MarkUnsubscribed();
                    _stateSub?.MarkUnsubscribed();
                    NativeMainThread.Run(() => Reconnecting?.Invoke());
                };
                _centrifuge.Reconnected += () => NativeMainThread.Run(() => Reconnected?.Invoke());
                _centrifuge.Disconnected += reason => NativeMainThread.Run(() => Disconnected?.Invoke(reason));
                _hostSub = _centrifuge.Subscribe(hostChannel, hostToken);
                _stateSub = _centrifuge.Subscribe(stateChannel, stateToken);
                _hostSub.Publication += OnBytes;
                _stateSub.Publication += OnBytes;
                _hostSub.Subscribed += () =>
                {
                    GP_Logger.Info("Centrifugo", "subscribed host=" + hostChannel);
                    hostReady.TrySetResult(true);
                };
                _stateSub.Subscribed += () =>
                {
                    GP_Logger.Info("Centrifugo", "subscribed state=" + stateChannel);
                    stateReady.TrySetResult(true);
                };
                _centrifuge.Connect(endpoint, token);
                _hostSub.Subscribe();
                _stateSub.Subscribe();

                var timeout = Task.Delay(10000, cancellationToken);
                var ready = Task.WhenAll(connected.Task, hostReady.Task, stateReady.Task);
                var winner = await Task.WhenAny(ready, timeout);
                if (winner != ready)
                {
                    var pending = "";
                    if (!connected.Task.IsCompleted)
                        pending += " connected";
                    if (!hostReady.Task.IsCompleted)
                        pending += " hostSub";
                    if (!stateReady.Task.IsCompleted)
                        pending += " stateSub";
                    throw new TimeoutException("Centrifugo connection timeout:" + pending);
                }
                await ready;
                _live = true;
            }
        }

        public void MarkLive() => _live = true;

        public void Send(int type, string payloadJson, int senderId, int seq = -1) =>
            Publish(_stateSub, type, payloadJson, senderId, seq);

        public void SendToHost(int type, string payloadJson, int senderId, int seq = -1) =>
            Publish(_hostSub, type, payloadJson, senderId, seq);

        void Publish(NativeSubscription sub, int type, string payloadJson, int senderId, int seq)
        {
            if (sub == null || !_live)
                return;
            var json = "{\"t\":" + type +
                       ",\"s\":" + senderId +
                       ",\"ts\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() +
                       ",\"p\":" + (string.IsNullOrEmpty(payloadJson) ? "null" : payloadJson);
            if (seq >= 0)
                json += ",\"sq\":" + seq;
            json += "}";
            var bytes = Encoding.UTF8.GetBytes(json);
            var reportSendError = type == NativeMultiplayerWire.StateUpdate ||
                                  type == NativeMultiplayerWire.StateDelta ||
                                  type == NativeMultiplayerWire.GlobalStateUpdate ||
                                  type == NativeMultiplayerWire.GlobalStateDelta;
            sub.Publish(bytes, ok =>
            {
                if (!ok && reportSendError)
                {
                    GP_Logger.Info("Centrifugo", "sendState failed t=" + type);
                    GP_Multiplayer.NativeEmit("error:sendState",
                        new GP_Data("{\"message\":\"publish failed\",\"t\":" + type + "}"));
                }
            });
        }

        void OnBytes(byte[] data)
        {
            if (data == null || data.Length == 0)
                return;
            var text = Encoding.UTF8.GetString(data);
            var type = GpJson.GetInt(text, "t");
            var sender = GpJson.GetInt(text, "s");
            var seq = ReadSeq(text);
            var payload = GpJson.GetObject(text, "p") ?? "{}";
            var timestamp = GpJson.GetLong(text, "ts");
            if (timestamp <= 0)
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            NoteWire(type, sender);
            Message?.Invoke(new NativeMultiplayerWireMessage
            {
                Type = type,
                SenderId = sender,
                Seq = seq,
                Timestamp = timestamp,
                Payload = payload
            });
        }

        static int ReadSeq(string json)
        {
            if (!GpJson.TryGetString(json, "sq", out var raw) || string.IsNullOrEmpty(raw))
                return -1;
            return int.TryParse(raw, System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : -1;
        }

        void NoteWire(int type, int sender)
        {
            var bit = 1 << Math.Min(type, 16);
            if ((_loggedWire & bit) != 0)
                return;
            _loggedWire |= bit;
            GP_Logger.Info("Wire", "t=" + type + " from=" + sender);
        }

        public static string EnsureProtobufEndpoint(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint))
                endpoint = NativeCore.CentrifugeWs;
#if UNITY_WEBGL && !UNITY_EDITOR
            return endpoint.Replace("?format=protobuf", "").Replace("&format=protobuf", "");
#else
            if (endpoint.IndexOf("format=", StringComparison.OrdinalIgnoreCase) >= 0)
                return endpoint;
            return endpoint + (endpoint.IndexOf('?') >= 0 ? "&" : "?") + "format=protobuf";
#endif
        }

        public void Dispose()
        {
            _live = false;
            try { _hostSub?.Unsubscribe(); } catch { /* ignore */ }
            try { _stateSub?.Unsubscribe(); } catch { /* ignore */ }
            try { _centrifuge?.Dispose(); } catch { /* ignore */ }
            _hostSub = null;
            _stateSub = null;
            _centrifuge = null;
            Message = null;
            Reconnecting = null;
            Reconnected = null;
            Disconnected = null;
        }
    }
}
