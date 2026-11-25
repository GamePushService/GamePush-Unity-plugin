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
        bool _socketReady;
        bool _everReady;
        int _loggedWire;
        string _credentials;
        CancellationTokenSource _recovery;

        public bool IsLive => _live && _socketReady && _centrifuge != null &&
                              _hostSub?.IsSubscribed == true && _stateSub?.IsSubscribed == true;

        public async Task Connect(string credentialsJson, CancellationToken cancellationToken)
        {
            NativeMainThread.Ensure();
            _credentials = credentialsJson;
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
            var dropped = new TaskCompletionSource<string>();
            var ready = Task.WhenAll(connected.Task, hostReady.Task, stateReady.Task);
            using (cancellationToken.Register(() =>
            {
                connected.TrySetCanceled();
                hostReady.TrySetCanceled();
                stateReady.TrySetCanceled();
            }))
            {
                var client = _centrifuge = new NativeCentrifugo();
                client.Connected += () =>
                {
                    if (!ReferenceEquals(_centrifuge, client)) return;
                    GP_Logger.Info("Centrifugo", "connected");
                    _socketReady = true;
                    connected.TrySetResult(true);
                    UpdateReadiness();
                };
                client.Reconnecting += () =>
                {
                    if (ReferenceEquals(_centrifuge, client))
                        MarkReconnecting();
                };
                client.Reconnected += () =>
                {
                    if (!ReferenceEquals(_centrifuge, client)) return;
                    _socketReady = true;
                    UpdateReadiness();
                };
                client.Disconnected += reason =>
                {
                    if (!ReferenceEquals(_centrifuge, client)) return;
                    _live = false;
                    _socketReady = false;
                    if (ready.IsCompletedSuccessfully)
                        Disconnected?.Invoke(reason);
                    else
                        dropped.TrySetResult(reason);
                };
                _hostSub = client.Subscribe(hostChannel, hostToken);
                _stateSub = client.Subscribe(stateChannel, stateToken);
                _hostSub.Publication += OnPublication;
                _stateSub.Publication += OnPublication;

                void OnPublication(byte[] data)
                {
                    if (ReferenceEquals(_centrifuge, client))
                        OnBytes(data);
                }

                _hostSub.Subscribed += () =>
                {
                    if (!ReferenceEquals(_centrifuge, client)) return;
                    GP_Logger.Info("Centrifugo", "subscribed host=" + hostChannel);
                    hostReady.TrySetResult(true);
                    UpdateReadiness();
                };
                _stateSub.Subscribed += () =>
                {
                    if (!ReferenceEquals(_centrifuge, client)) return;
                    GP_Logger.Info("Centrifugo", "subscribed state=" + stateChannel);
                    stateReady.TrySetResult(true);
                    UpdateReadiness();
                };
                client.Connect(endpoint, token);
                _hostSub.Subscribe();
                _stateSub.Subscribe();

                var timeout = Task.Delay(10000, cancellationToken);
                var winner = await Task.WhenAny(ready, timeout, dropped.Task);
                cancellationToken.ThrowIfCancellationRequested();
                if (winner == dropped.Task)
                    throw new InvalidOperationException(await dropped.Task);
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
                UpdateReadiness();
            }
        }

        void MarkReconnecting()
        {
            bool wasLive = _live;
            _live = false;
            _socketReady = false;
            _hostSub?.MarkUnsubscribed();
            _stateSub?.MarkUnsubscribed();
            if (wasLive)
                Reconnecting?.Invoke();
        }

        public void RecoverStalledConnection()
        {
            if (!IsLive || _recovery != null)
                return;

            _recovery = new CancellationTokenSource();
            _ = RecoverConnection(_recovery);
        }

        async Task RecoverConnection(CancellationTokenSource recovery)
        {
            try
            {
                MarkReconnecting();
                recovery.Token.ThrowIfCancellationRequested();
                CloseConnection();
                await Connect(_credentials, recovery.Token);
            }
            catch (Exception exception)
            {
                if (!recovery.IsCancellationRequested)
                    Disconnected?.Invoke(exception.Message);
            }
            finally
            {
                _recovery = null;
                recovery.Dispose();
            }
        }

        void UpdateReadiness()
        {
            if (_live || !_socketReady || _hostSub?.IsSubscribed != true ||
                _stateSub?.IsSubscribed != true) return;
            _live = true;
            bool reconnect = _everReady;
            _everReady = true;
            if (reconnect) Reconnected?.Invoke();
        }

        public void Send(int type, string payloadJson, int senderId, int seq = -1) =>
            Publish(_stateSub, type, payloadJson, senderId, seq);

        public void SendToHost(int type, string payloadJson, int senderId, int seq = -1) =>
            Publish(_hostSub, type, payloadJson, senderId, seq);

        public void SendCustom(string payloadJson, int senderId, Action<string> onError) =>
            Publish(_stateSub, NativeMultiplayerWire.CustomEvent, payloadJson, senderId, -1, onError);

        void Publish(NativeSubscription sub, int type, string payloadJson, int senderId, int seq, Action<string> onError = null)
        {
            var reportSendError = type == NativeMultiplayerWire.PeerState ||
                                  type == NativeMultiplayerWire.StateUpdate ||
                                  type == NativeMultiplayerWire.StateDelta ||
                                  type == NativeMultiplayerWire.GlobalStateUpdate ||
                                  type == NativeMultiplayerWire.GlobalStateDelta;
            if (sub == null || !IsLive || !sub.IsSubscribed)
            {
                onError?.Invoke("not_sent");
                if (reportSendError)
                {
                    GP_Logger.Warn("Centrifugo", "publish skipped: transport not live t=" + type);
                    GP_Multiplayer.NativeEmit("error:sendState",
                        new GP_Data("{\"message\":\"transport not ready\",\"t\":" + type + "}"));
                }
                return;
            }
            var json = "{\"t\":" + type +
                       ",\"s\":" + senderId +
                       ",\"ts\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() +
                       ",\"p\":" + (string.IsNullOrEmpty(payloadJson) ? "null" : payloadJson);
            if (seq >= 0)
                json += ",\"sq\":" + seq;
            json += "}";
            var bytes = Encoding.UTF8.GetBytes(json);
            if (onError == null && !reportSendError)
            {
                sub.PublishWithResult(bytes, null);
                return;
            }
            sub.PublishWithResult(bytes, error =>
            {
                if (error == null) return;
                onError?.Invoke(error);
                if (reportSendError && IsLive && (ReferenceEquals(sub, _stateSub) || ReferenceEquals(sub, _hostSub)))
                {
                    GP_Multiplayer.NativeEmit("error:sendState",
                        new GP_Data("{\"message\":" + GpJson.Quote(error) + ",\"t\":" + type + "}"));
                }
            });
        }

        void OnBytes(byte[] data)
        {
            if (data == null || data.Length == 0 || (!IsLive && _stateSub?.IsSubscribed != true))
                return;
            var text = Encoding.UTF8.GetString(data);
            var type = GpJson.GetInt(text, "t");
            // The room channel may deliver an event before the host subscription is ready.
            if (!IsLive && type != NativeMultiplayerWire.CustomEvent) return;
            var sender = GpJson.GetInt(text, "s");
            var seq = ReadSeq(text);
            var payload = GpJson.GetObject(text, "p") ?? "{}";
            var timestamp = GpJson.GetLong(text, "ts");
            if (timestamp <= 0)
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            else
            {
                var nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (timestamp > nowMs + 60000L)
                {
                    GP_Logger.Warn("Wire", "future timestamp clamped type=" + type);
                    timestamp = nowMs;
                }
            }
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

        void CloseConnection()
        {
            _live = false;
            _socketReady = false;
            var client = _centrifuge;
            _centrifuge = null;
            try { _hostSub?.Unsubscribe(); } catch { /* ignore */ }
            try { _stateSub?.Unsubscribe(); } catch { /* ignore */ }
            try { client?.Dispose(); } catch { /* ignore */ }
            _hostSub = null;
            _stateSub = null;
        }

        public void Dispose()
        {
            _recovery?.Cancel();
            CloseConnection();
            _credentials = null;
            Message = null;
            Reconnecting = null;
            Reconnected = null;
            Disconnected = null;
        }
    }
}
