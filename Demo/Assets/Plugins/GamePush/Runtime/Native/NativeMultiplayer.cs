using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using GamePush;

namespace GamePush.Native
{
    public static class NativeMultiplayer
    {
        const int StateUpdate = 1;
        const int Heartbeat = 2;
        const int HostMigration = 3;
        const int PeerState = 4;
        const int CustomEvent = 5;
        const int GlobalStateUpdate = 6;
        const int StateDelta = 7;
        const int GlobalStateDelta = 8;
        const int SnapshotRequest = 9;
        const float HeartbeatInterval = 1f;
        const float HeartbeatTimeout = 3f;
        const float HostHeartbeatTimeout = 1.5f;
        const float HostRecheckInterval = 60f;
        const int GlobalTargetSendRate = 20;

        static readonly Dictionary<int, PlayerSlot> Players = new Dictionary<int, PlayerSlot>();
        static readonly Dictionary<int, string> PlayerStates = new Dictionary<int, string>();
        static readonly Dictionary<int, float> LastBeat = new Dictionary<int, float>();

        static NativeCentrifugo _centrifuge;
        static NativeSubscription _hostSub;
        static NativeSubscription _stateSub;
        static bool _connected;
        static bool _connecting;
        static int _channelId;
        static int _hostId;
        static long _hostElectedAt;
        static float _sessionStart;
        static int _tickRate = 20;
        static int _bufferMinMs = 100;
        static int _bufferMaxMs = 300;
        static bool _transportLive;
        static float _reconnectGraceUntil;
        static string _globalState = "{}";
        static string _myState = "{}";
        static string _playerSchema = "{}";
        static string _globalSchema = "{}";
        static NativePump _pump;
        static Timer _heartbeatClock;
        static int _heartbeatQueued;
        static float _nextTick;
        static int _playersSeq;
        static float _lastHostRecheckTime;
        static float _lastMigrationTime;
        static float _hostAnnounceRetry1;
        static float _hostAnnounceRetry2;
        static int _selfReconnectCount;
        static int _selfFreezeCount;
        static float _selfPingJitterEma;
        static int _selfPing;
        static float _selfEchoLastAt;
        static int _tickCounter;
        static int _globalSendSeq;
        static int _loggedWire;
        static string _lastSentPlayers;
        static string _lastSentGlobal;
        static bool _playersActiveLastTick;
        static bool _globalActiveLastTick;
        static int _lastPlayersSeq = -1;
        static int _lastGlobalSeq = -1;
        static float _lastSnapshotRequestAt;
        static bool _playersHaveBase;
        static string _lastFullPlayersJson;
        static string _lastFullGlobalJson;
        static readonly GP_InterpolationEngine _playersInterp = new GP_InterpolationEngine(20);
        static readonly GP_InterpolationEngine _globalInterp = new GP_InterpolationEngine(20);
        static object _previousInterpolatedPlayers;
        static object _previousInterpolatedGlobal;

        public static bool IsConnected => _connected;
        public static bool IsHost => _connected && _hostId == NativePlayer.Id && NativePlayer.Id > 0;
        public static int TickRate => _tickRate;

        public static async Task<MultiplayerConnectResultData> Connect(
            MultiplayerChannelQuery query, CancellationToken cancellationToken)
        {
            if (_connecting)
                throw new InvalidOperationException("Multiplayer operation is already in progress");
            if (_connected)
                throw new InvalidOperationException("Already connected to a room");

            _connecting = true;
            try
            {
                _channelId = query != null ? query.channelId : 0;
                var json = await NativeCore.Client.Fetch(NativeQueries.ConnectMultiplayer,
                    new Dictionary<string, object> { ["channelId"] = _channelId });
                cancellationToken.ThrowIfCancellationRequested();
                var result = GpJson.GetObject(json, "result") ?? json;
                if (GpJson.TryGetString(result, "__typename", out var typeName) && typeName == "Problem")
                    throw new Exception(GpJson.TryGetString(result, "message", out var msg) ? msg : "connect_problem");
                await OpenTransport(result, cancellationToken);
                return FinishConnect();
            }
            catch (Exception exception)
            {
                _connecting = false;
                Cleanup(false);
                NativeMainThread.Run(() => GP_Multiplayer.NativeEmit("error:connect",
                    new GP_Data("{\"message\":\"" + GpJson.Escape(exception.Message) + "\"}")));
                NativeMainThread.Run(() => GP_Logger.Error("Multiplayer", exception.Message));
                throw;
            }
        }

        public static async Task<MultiplayerConnectResultData> ConnectWithTransport(
            string json, CancellationToken cancellationToken)
        {
            if (_connecting)
                throw new InvalidOperationException("Multiplayer operation is already in progress");
            if (_connected)
                throw new InvalidOperationException("Already connected to a room");

            _connecting = true;
            try
            {
                var result = GpJson.GetObject(json, "result") ?? json;
                if (GpJson.TryGetString(result, "__typename", out var typeName) && typeName == "Problem")
                    throw new Exception(GpJson.TryGetString(result, "message", out var msg) ? msg : "connect_problem");
                var channelId = GpJson.GetInt(result, "channelId");
                if (channelId > 0)
                    _channelId = channelId;
                var playerId = GpJson.GetInt(result, "playerId");
                var playerName = GpJson.TryGetString(result, "playerName", out var name) ? name : "";
                if (playerId > 0)
                    NativePlayer.Adopt(playerId, playerName);
                if (NativePlayer.Id <= 0)
                    throw new Exception("Play2Web player id missing");
                await OpenTransport(result, cancellationToken);
                return FinishConnect();
            }
            catch (Exception exception)
            {
                _connecting = false;
                Cleanup(false);
                NativeMainThread.Run(() => GP_Multiplayer.NativeEmit("error:connect",
                    new GP_Data("{\"message\":\"" + GpJson.Escape(exception.Message) + "\"}")));
                NativeMainThread.Run(() => GP_Logger.Error("Multiplayer", exception.Message));
                throw;
            }
        }

        static async Task OpenTransport(string result, CancellationToken cancellationToken)
        {
            NativeMainThread.Ensure();
            var connection = GpJson.GetObject(result, "connection");
            var hostInfo = GpJson.GetObject(result, "hostSubscription");
            var stateInfo = GpJson.GetObject(result, "stateSubscription");
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
                _centrifuge.Reconnecting += () => NativeMainThread.Run(HandleTransportInterrupted);
                _centrifuge.Reconnected += () => NativeMainThread.Run(HandleTransportReconnect);
                _centrifuge.Disconnected += reason =>
                {
                    if (_connected)
                        NativeMainThread.Run(() => HandleTransportDrop(reason));
                };
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
            }
        }

        static MultiplayerConnectResultData FinishConnect()
        {
            Players.Clear();
            PlayerStates.Clear();
            LastBeat.Clear();
            _hostId = 0;
            _hostElectedAt = 0;
            _sessionStart = Time.realtimeSinceStartup;
            _nextTick = 0;
            _lastHostRecheckTime = 0f;
            _lastMigrationTime = 0f;
            _hostAnnounceRetry1 = 0f;
            _hostAnnounceRetry2 = 0f;
            _selfReconnectCount = 0;
            _selfFreezeCount = 0;
            _selfPingJitterEma = 0f;
            _selfPing = 0;
            _selfEchoLastAt = 0f;
            _tickCounter = 0;
            _playersSeq = 0;
            _globalSendSeq = 0;
            _loggedWire = 0;
            _transportLive = true;
            _reconnectGraceUntil = 0f;
            _lastSentPlayers = null;
            _lastSentGlobal = null;
            _playersActiveLastTick = false;
            _globalActiveLastTick = false;
            _lastPlayersSeq = -1;
            _lastGlobalSeq = -1;
            _lastSnapshotRequestAt = 0f;
            _playersHaveBase = false;
            _lastFullPlayersJson = null;
            _lastFullGlobalJson = null;
            _previousInterpolatedPlayers = null;
            _previousInterpolatedGlobal = null;
            _playersInterp.Clear();
            _globalInterp.Clear();
            ApplySchemasToEngines();
            AddSelf();
            EnsurePump();
            StartHeartbeatClock();
            Application.runInBackground = true;
            _connected = true;
            _connecting = false;
            var payload = new GP_Data("{\"success\":true}");
            NativeMainThread.Run(() => GP_Multiplayer.NativeEmit("connect", payload));
            GP_Logger.Info("Multiplayer", "connected channel=" + _channelId + " player=" + NativePlayer.Id +
                                          " tickRate=" + _tickRate +
                                          " globalDivider=" + GlobalTickDivider);
            return new MultiplayerConnectResultData { success = true };
        }

        static string EnsureProtobufEndpoint(string endpoint)
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

        public static void CloseLocal() => Cleanup(false);

        public static async Task Disconnect(MultiplayerChannelQuery query, CancellationToken cancellationToken)
        {
            var channelId = query != null ? query.channelId : _channelId;
            try
            {
                Cleanup(false);
                if (NativeCore.Client != null && channelId > 0)
                {
                    await NativeCore.Client.Fetch(NativeQueries.DisconnectMultiplayer,
                        new Dictionary<string, object> { ["channelId"] = channelId });
                }
                cancellationToken.ThrowIfCancellationRequested();
                NativeMainThread.Run(() => GP_Multiplayer.NativeEmit("disconnect",
                    new GP_Data("{\"reason\":\"User disconnected\"}")));
            }
            catch (Exception exception)
            {
                Cleanup(false);
                NativeMainThread.Run(() => GP_Multiplayer.NativeEmit("error:disconnect",
                    new GP_Data("{\"message\":\"" + GpJson.Escape(exception.Message) + "\"}")));
                throw;
            }
        }

        public static void DefinePlayerSchema(string schema)
        {
            _playerSchema = schema ?? "{}";
            ApplySchemasToEngines();
        }

        public static void DefineGlobalSchema(string schema)
        {
            _globalSchema = schema ?? "{}";
            ApplySchemasToEngines();
        }

        public static void SetMode(string mode)
        {
            if (string.Equals(mode, "fast", StringComparison.OrdinalIgnoreCase))
            {
                _tickRate = 60;
                _bufferMinMs = 50;
                _bufferMaxMs = 100;
            }
            else
            {
                _tickRate = 20;
                _bufferMinMs = 100;
                _bufferMaxMs = 300;
            }
            _tickCounter = 0;
            ApplySchemasToEngines();
        }

        public static void SetPlayerState(string state)
        {
            var incoming = string.IsNullOrEmpty(state) ? "{}" : state;
            incoming = GP_NativeSchema.FilterReadonly(incoming, _myState, _playerSchema);
            _myState = GpJson.MergeDelta(_myState, incoming);
            PlayerStates[NativePlayer.Id] = _myState;
        }

        public static void SetGlobalState(string state)
        {
            if (!IsHost)
                return;
            var incoming = string.IsNullOrEmpty(state) ? "{}" : state;
            incoming = GP_NativeSchema.FilterReadonly(incoming, _globalState, _globalSchema);
            _globalState = GpJson.MergeDelta(_globalState, incoming);
        }

        public static void SendMessage(string eventName, string data, string options)
        {
            if (!_connected)
                return;
            var target = "all";
            var echo = false;
            if (!string.IsNullOrEmpty(options) && options != "undefined")
            {
                if (int.TryParse(options, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                    target = options;
                else
                {
                    if (GpJson.TryGetString(options, "target", out var t) && !string.IsNullOrEmpty(t))
                        target = t;
                    echo = GpJson.GetBool(options, "echo");
                }
            }
            var payload = "{\"eventName\":" + GpJson.Quote(eventName ?? "") +
                          ",\"data\":" + (string.IsNullOrEmpty(data) ? "null" : data) +
                          ",\"target\":" + GpJson.Quote(target) + "}";
            Publish(CustomEvent, payload, false);
            if (echo || target == NativePlayer.Id.ToString(CultureInfo.InvariantCulture))
                EmitCustom(NativePlayer.Id, eventName, data);
        }

        public static string ConnectedPlayersJson()
        {
            var sb = new StringBuilder();
            sb.Append('[');
            var first = true;
            foreach (var pair in Players)
            {
                if (!first) sb.Append(',');
                first = false;
                sb.Append(PlayerJson(pair.Value));
            }
            sb.Append(']');
            return sb.ToString();
        }

        public static string PlayersStateJson()
        {
            var sb = new StringBuilder();
            sb.Append("{\"players\":[");
            var first = true;
            foreach (var pair in PlayerStates)
            {
                if (!first) sb.Append(',');
                first = false;
                sb.Append("{\"playerId\":");
                sb.Append(GpJson.Quote(pair.Key.ToString(CultureInfo.InvariantCulture)));
                sb.Append(",\"state\":");
                sb.Append(GpJson.Quote(pair.Value ?? "{}"));
                sb.Append('}');
            }
            sb.Append("]}");
            return sb.ToString();
        }

        public static string MyStateJson() => _myState;
        public static string GlobalStateJson() => _globalState;

        public static string NetworkStatsJson() =>
            "{\"ping\":" + _selfPing +
            ",\"bufferSize\":" + _playersInterp.BufferSize +
            ",\"bufferDelay\":" + _playersInterp.BufferDelay.ToString("0.###", CultureInfo.InvariantCulture) + "}";

        public static string RuntimeCapabilitiesJson() =>
            "{\"connect\":true,\"disconnect\":true,\"setPlayerState\":true,\"setGlobalState\":true,\"sendMessage\":true,\"hostMigrationEvents\":true}";

        static int GlobalTickDivider =>
            Math.Max(1, (int)Math.Round(_tickRate / (float)GlobalTargetSendRate));

        public static void Tick(float now)
        {
            if (!_connected)
                return;
            if (!IsHost)
                SampleInterpolation(now * 1000.0);
            if (now >= _nextTick)
            {
                var dt = 1f / Math.Max(1, _tickRate);
                _nextTick = now + dt;
                if (IsHost)
                {
                    PlayerStates[NativePlayer.Id] = _myState;
                    SendHostPlayers();
                    if (_tickCounter % GlobalTickDivider == 0)
                        SendHostGlobal();
                    _tickCounter++;
                }
                else if (!string.IsNullOrEmpty(_myState) && _myState != "{}")
                    Publish(PeerState, _myState, true);
                GP_Multiplayer.NativeEmitTick(dt * 1000f);
            }
        }

        static void OnHeartbeatClock()
        {
            if (!_connected)
                return;
            var now = Time.realtimeSinceStartup;
            SendHeartbeat();
            CheckTimeouts(now);
            if (IsHost)
            {
                if (_hostAnnounceRetry1 > 0 && now >= _hostAnnounceRetry1)
                {
                    _hostAnnounceRetry1 = 0;
                    AnnounceHost();
                }
                if (_hostAnnounceRetry2 > 0 && now >= _hostAnnounceRetry2)
                {
                    _hostAnnounceRetry2 = 0;
                    AnnounceHost();
                }
            }
        }

        static void EnqueueHeartbeatClock()
        {
            if (Interlocked.Exchange(ref _heartbeatQueued, 1) == 1)
                return;
            NativeMainThread.Run(() =>
            {
                Interlocked.Exchange(ref _heartbeatQueued, 0);
                OnHeartbeatClock();
            });
        }

        static void StartHeartbeatClock()
        {
            StopHeartbeatClock();
            _heartbeatClock = new Timer(_ => EnqueueHeartbeatClock(), null,
                TimeSpan.Zero, TimeSpan.FromSeconds(HeartbeatInterval));
        }

        static void StopHeartbeatClock()
        {
            try { _heartbeatClock?.Dispose(); }
            catch { /* ignore */ }
            _heartbeatClock = null;
            Interlocked.Exchange(ref _heartbeatQueued, 0);
        }

        static string BuildPlayersPayload()
        {
            var sb = new StringBuilder();
            sb.Append("{\"players\":{");
            var first = true;
            foreach (var pair in PlayerStates)
            {
                if (!first)
                    sb.Append(',');
                first = false;
                sb.Append(GpJson.Quote(pair.Key.ToString(CultureInfo.InvariantCulture)));
                sb.Append(':');
                sb.Append(string.IsNullOrEmpty(pair.Value) ? "{}" : pair.Value);
            }
            sb.Append("}}");
            return sb.ToString();
        }

        static void SendHostPlayers()
        {
            if (!IsHost || !_connected || !_transportLive)
                return;
            var current = BuildPlayersPayload();
            int type;
            string payload;
            if (string.IsNullOrEmpty(_lastSentPlayers))
            {
                type = StateUpdate;
                payload = current;
                _playersActiveLastTick = true;
            }
            else
            {
                var delta = GpJson.CalculateDelta(_lastSentPlayers, current);
                if (delta == null)
                {
                    if (!_playersActiveLastTick)
                        return;
                    _playersActiveLastTick = false;
                    Publish(StateDelta, "{}");
                    return;
                }
                type = StateDelta;
                payload = delta;
                _playersActiveLastTick = true;
            }
            Publish(type, payload);
            _lastSentPlayers = current;
        }

        static void SendHostGlobal()
        {
            if (!IsHost || !_connected || !_transportLive)
                return;
            var current = string.IsNullOrEmpty(_globalState) ? "{}" : _globalState;
            int type;
            string payload;
            if (string.IsNullOrEmpty(_lastSentGlobal))
            {
                type = GlobalStateUpdate;
                payload = current;
                _globalActiveLastTick = true;
            }
            else
            {
                var delta = GpJson.CalculateDelta(_lastSentGlobal, current);
                if (delta == null)
                {
                    if (!_globalActiveLastTick)
                        return;
                    _globalActiveLastTick = false;
                    Publish(GlobalStateDelta, "{}");
                    return;
                }
                type = GlobalStateDelta;
                payload = delta;
                _globalActiveLastTick = true;
            }
            Publish(type, payload);
            _lastSentGlobal = current;
        }

        static void SendHeartbeat()
        {
            RefreshSelfMetrics();
            Players.TryGetValue(NativePlayer.Id, out var self);
            var duration = self != null ? self.sessionDuration : 0;
            var stability = self != null
                ? self.connectionStability.ToString("0.##", CultureInfo.InvariantCulture)
                : "1";
            var payload = "{\"sessionDuration\":" + duration +
                          ",\"sentAt\":" + (Time.realtimeSinceStartup * 1000f).ToString("0.###", CultureInfo.InvariantCulture) +
                          ",\"ping\":" + _selfPing + ",\"name\":" + GpJson.Quote(NativePlayer.GetString("name")) +
                          ",\"stability\":" + stability + "}";
            Publish(Heartbeat, payload, false);
        }

        static void RefreshSelfMetrics()
        {
            if (!Players.TryGetValue(NativePlayer.Id, out var self))
                return;
            self.sessionDuration = (int)((Time.realtimeSinceStartup - _sessionStart) * 1000f);
            self.ping = _selfPing;
            self.connectionStability = NativeHostSelector.CalculateSelfStability(
                _selfReconnectCount, _selfFreezeCount, _selfPingJitterEma);
        }

        static void ElectHost()
        {
            RefreshSelfMetrics();
            var list = new List<PlayerSlot>(Players.Values);
            if (list.Count == 0)
                return;

            if (!Players.TryGetValue(_hostId, out var currentHost))
            {
                var selected = NativeHostSelector.SelectHost(list);
                if (selected == null)
                    return;
                ApplyHost(selected.playerId, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), false);
                if (IsHost)
                    AnnounceHost();
                return;
            }

            if (!NativeHostSelector.ShouldMigrateHost(currentHost, list))
                return;
            var next = NativeHostSelector.SelectHost(list);
            if (next == null || next.playerId == currentHost.playerId)
                return;
            if (_lastMigrationTime > 0 && Time.realtimeSinceStartup - _lastMigrationTime < HostRecheckInterval)
                return;
            if (NativeHostSelector.IsPingSignificantlyWorse(currentHost.ping, next.ping))
                return;
            if (!NativeHostSelector.IsPingSignificantlyBetter(currentHost.ping, next.ping)
                && next.connectionStability <= currentHost.connectionStability)
                return;
            var wasHost = IsHost;
            ApplyHost(next.playerId, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), false);
            _lastMigrationTime = Time.realtimeSinceStartup;
            if (wasHost)
                AnnounceHost(next.playerId);
        }

        internal static string BuildHostMigrationPayload(int newHostId, long electedAt)
        {
            return "{\"newHostId\":" + newHostId +
                   ",\"hostElectedAt\":" + electedAt.ToString(CultureInfo.InvariantCulture) + "}";
        }

        static void AnnounceHost(int newHostId = 0)
        {
            var hostId = newHostId > 0 ? newHostId : NativePlayer.Id;
            if (hostId <= 0)
                return;
            if (newHostId <= 0 && !IsHost)
                return;
            Publish(HostMigration, BuildHostMigrationPayload(hostId, _hostElectedAt), false);
        }

        static void ScheduleHostAnnounceRetries()
        {
            var now = Time.realtimeSinceStartup;
            _hostAnnounceRetry1 = now + 0.5f;
            _hostAnnounceRetry2 = now + 1f;
        }

        static void ApplyHost(int newHost, long electedAt, bool fromRemote)
        {
            if (newHost <= 0)
                return;
            if (_hostId == newHost)
                return;
            if (fromRemote && IsHost && newHost != NativePlayer.Id)
            {
                var keep = _hostElectedAt != 0 && (electedAt == 0 || _hostElectedAt <= electedAt);
                if (keep)
                {
                    GP_Logger.Info("Host", "conflict keep self=" + NativePlayer.Id +
                                           " elected=" + _hostElectedAt + " vs " + electedAt);
                    AnnounceHost();
                    return;
                }
                GP_Logger.Info("Host", "conflict yield to=" + newHost +
                                       " elected=" + electedAt + " vs " + _hostElectedAt);
            }
            var old = _hostId;
            var wasHost = IsHost;
            _hostId = newHost;
            _hostElectedAt = electedAt > 0 ? electedAt : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            foreach (var pair in Players)
                pair.Value.isHost = pair.Value.playerId == newHost;
            if (old != 0)
            {
                GP_Multiplayer.NativeEmit("hostMigrated",
                    new GP_Data("{\"oldHost\":" + old + ",\"newHost\":" + newHost + "}"));
            }
            if (IsHost && !wasHost)
                GP_Multiplayer.NativeEmit("becameHost");
            else if (!IsHost && wasHost)
                GP_Multiplayer.NativeEmit("becamePeer");
            if (wasHost && !IsHost)
            {
                _lastSentPlayers = null;
                _lastSentGlobal = null;
            }
            if (IsHost)
            {
                InitMissingPlayers();
                _lastSentPlayers = null;
                _lastSentGlobal = null;
                _tickCounter = 0;
                SendHostPlayers();
                SendHostGlobal();
            }
            GP_Logger.Info("Host", "old=" + old + " new=" + newHost +
                                   " self=" + NativePlayer.Id +
                                   " isHost=" + IsHost +
                                   " remote=" + fromRemote);
        }

        static void InitMissingPlayers()
        {
            _ = InitMissingPlayersAsync();
        }

        static async Task InitMissingPlayersAsync()
        {
            var missing = new List<KeyValuePair<int, PlayerSlot>>();
            foreach (var pair in Players)
            {
                if (pair.Key == NativePlayer.Id)
                    continue;
                if (PlayerStates.ContainsKey(pair.Key))
                    continue;
                missing.Add(pair);
            }
            var changed = false;
            foreach (var pair in missing)
            {
                var state = await GP_Multiplayer.NativeInitPlayerAsync(pair.Key, ToConnected(pair.Value));
                if (!string.IsNullOrEmpty(state) && state != "null")
                {
                    PlayerStates[pair.Key] = state;
                    changed = true;
                }
            }
            if (changed && IsHost)
            {
                _lastSentPlayers = null;
                SendHostPlayers();
            }
        }

        static float _lastTimeoutCheck;

        static void CheckTimeouts(float now)
        {
            if (!_transportLive || now < _reconnectGraceUntil)
                return;
            if (_lastTimeoutCheck > 0f && now - _lastTimeoutCheck > HeartbeatTimeout)
            {
                var frozenMs = (now - _lastTimeoutCheck) * 1000f;
                GP_Logger.Info("Multiplayer", "frozen " + frozenMs.ToString("0", CultureInfo.InvariantCulture) + "ms");
                var frozen = new List<int>(LastBeat.Keys);
                for (var i = 0; i < frozen.Count; i++)
                    LastBeat[frozen[i]] = now;
                _lastTimeoutCheck = now;
                _selfFreezeCount++;
                if (IsHost)
                {
                    _lastSentPlayers = null;
                    _lastSentGlobal = null;
                    SendHostPlayers();
                    SendHostGlobal();
                }
                return;
            }
            _lastTimeoutCheck = now;
            if (_selfEchoLastAt > 0f && now - _selfEchoLastAt > HostHeartbeatTimeout)
            {
                GP_Logger.Info("Multiplayer", "self-echo delayed " +
                               ((now - _selfEchoLastAt) * 1000f).ToString("0", CultureInfo.InvariantCulture) +
                               "ms — skip timeouts");
                return;
            }
            var dead = new List<int>();
            foreach (var pair in LastBeat)
            {
                if (pair.Key == NativePlayer.Id)
                    continue;
                var limit = pair.Key == _hostId ? HostHeartbeatTimeout : HeartbeatTimeout;
                if (now - pair.Value > limit)
                    dead.Add(pair.Key);
            }
            foreach (var id in dead)
                RemovePlayer(id);
            if (_hostId != 0 && !Players.ContainsKey(_hostId))
            {
                _hostId = 0;
                ElectHost();
            }
            if (_hostId == 0 && now >= _reconnectGraceUntil && now - _sessionStart > HeartbeatInterval * 3f)
                ElectHost();
            if (_hostId != 0 && IsHost && now - _lastHostRecheckTime > HostRecheckInterval)
            {
                _lastHostRecheckTime = now;
                ElectHost();
            }
        }

        static void OnBytes(byte[] data)
        {
            if (data == null || data.Length == 0)
                return;
            var text = Encoding.UTF8.GetString(data);
            HandleWire(text);
        }

        static void HandleWire(string json)
        {
            var type = GpJson.GetInt(json, "t");
            var sender = GpJson.GetInt(json, "s");
            var seq = ReadSeq(json);
            var payload = GpJson.GetObject(json, "p") ?? "{}";
            var timestamp = GpJson.GetLong(json, "ts");
            if (timestamp <= 0)
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            LastBeat[sender] = Time.realtimeSinceStartup;
            NoteWire(type, sender);
            switch (type)
            {
                case Heartbeat:
                    HandleHeartbeat(sender, payload);
                    break;
                case HostMigration:
                    if (sender != NativePlayer.Id)
                    {
                        var newHost = GpJson.GetInt(payload, "newHostId");
                        var elected = GpJson.GetLong(payload, "hostElectedAt");
                        ApplyHost(newHost, elected, true);
                    }
                    break;
                case StateUpdate:
                    if (!IsHost && (_hostId == 0 || sender == _hostId))
                        ApplyPlayersSnapshot(payload, seq, timestamp);
                    break;
                case StateDelta:
                    if (!IsHost && (_hostId == 0 || sender == _hostId))
                        ApplyPlayersDelta(payload, seq, timestamp);
                    break;
                case PeerState:
                    if (IsHost && sender != NativePlayer.Id && sender > 0)
                    {
                        PlayerStates.TryGetValue(sender, out var current);
                        var incoming = GP_NativeSchema.FilterReadonly(payload, current, _playerSchema);
                        PlayerStates[sender] = GpJson.MergeDelta(current, incoming);
                        GP_Multiplayer.NativeEmit("playersUpdated", new GP_Data(PlayersStateJson()));
                    }
                    break;
                case GlobalStateUpdate:
                    if (!IsHost && (_hostId == 0 || sender == _hostId))
                        ApplyGlobalSnapshot(payload, seq, timestamp);
                    break;
                case GlobalStateDelta:
                    if (!IsHost && (_hostId == 0 || sender == _hostId))
                        ApplyGlobalDelta(payload, seq, timestamp);
                    break;
                case SnapshotRequest:
                    if (IsHost && sender != NativePlayer.Id)
                    {
                        _lastSentPlayers = null;
                        _lastSentGlobal = null;
                        SendHostPlayers();
                        SendHostGlobal();
                    }
                    break;
                case CustomEvent:
                    if (sender != NativePlayer.Id)
                        HandleCustom(sender, payload);
                    break;
            }
        }

        static void NoteWire(int type, int sender)
        {
            var bit = 1 << Math.Min(type, 16);
            if ((_loggedWire & bit) != 0)
                return;
            _loggedWire |= bit;
            GP_Logger.Info("Wire", "t=" + type + " from=" + sender);
        }

        static void HandleHeartbeat(int sender, string payload)
        {
            if (sender == NativePlayer.Id)
            {
                _selfEchoLastAt = Time.realtimeSinceStartup;
                var sentAt = GpJson.GetFloat(payload, "sentAt", -1f);
                if (sentAt >= 0f)
                {
                    var raw = Math.Max(0, (int)Math.Round(Time.realtimeSinceStartup * 1000f - sentAt));
                    if (_selfPing != 0)
                    {
                        var jitter = Math.Abs(raw - _selfPing);
                        _selfPingJitterEma = _selfPingJitterEma * 0.8f + jitter * 0.2f;
                    }
                    _selfPing = _selfPing == 0 ? raw : (int)Math.Round(_selfPing * 0.7f + raw * 0.3f);
                }
                if (Players.TryGetValue(NativePlayer.Id, out var self))
                    self.ping = _selfPing;
                return;
            }

            if (!Players.TryGetValue(sender, out var slot))
            {
                slot = new PlayerSlot
                {
                    playerId = sender,
                    name = GpJson.TryGetString(payload, "name", out var n) ? n : "",
                    connectionStability = Math.Max(0f, Math.Min(1f, GpJson.GetFloat(payload, "stability", 1f))),
                    ping = GpJson.GetInt(payload, "ping"),
                    sessionDuration = GpJson.GetInt(payload, "sessionDuration")
                };
                Players[sender] = slot;
                LastBeat[sender] = Time.realtimeSinceStartup;
                var joined = "{\"player\":" + PlayerJson(slot) + ",\"isSelf\":" +
                             (sender == NativePlayer.Id ? "true" : "false") + "}";
                GP_Multiplayer.NativeEmit("playerJoined", new GP_Data(joined));
                if (IsHost)
                {
                    InitMissingPlayers();
                    AnnounceHost();
                    ScheduleHostAnnounceRetries();
                }
            }
            else
            {
                slot.ping = GpJson.GetInt(payload, "ping");
                slot.sessionDuration = GpJson.GetInt(payload, "sessionDuration");
                slot.connectionStability = GpJson.GetFloat(payload, "stability", slot.connectionStability);
                slot.isHost = sender == _hostId;
            }
        }

        static void HandleCustom(int sender, string payload)
        {
            var eventName = GpJson.TryGetString(payload, "eventName", out var name) ? name : "";
            var target = GpJson.TryGetString(payload, "target", out var t) ? t : "all";
            if (target != "all" && target != NativePlayer.Id.ToString(CultureInfo.InvariantCulture))
                return;
            var data = GpJson.GetObject(payload, "data");
            if (string.IsNullOrEmpty(data))
            {
                if (GpJson.TryGetString(payload, "data", out var raw) && !string.IsNullOrEmpty(raw))
                    data = raw;
                else
                    data = "null";
            }
            else if (data.Length >= 2 && data[0] == '"')
            {
                if (GpJson.TryGetString(payload, "data", out var raw) && raw != null)
                    data = raw;
            }
            EmitCustom(sender, eventName, data);
        }

        static void EmitCustom(int sender, string eventName, string data)
        {
            var json = "{\"eventName\":" + GpJson.Quote(eventName ?? "") +
                       ",\"senderId\":" + GpJson.Quote(sender.ToString(CultureInfo.InvariantCulture)) +
                       ",\"data\":" + GpJson.Quote(data ?? "null") +
                       ",\"timestamp\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "}";
            GP_Logger.Info("Wire", "t=5 event=" + eventName + " from=" + sender);
            GP_Multiplayer.NativeEmit("customEvent", new GP_Data(json));
        }

        static int ReadSeq(string json)
        {
            if (!GpJson.TryGetString(json, "sq", out var raw) || string.IsNullOrEmpty(raw))
                return -1;
            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : -1;
        }

        static void RequestSnapshot()
        {
            var now = Time.realtimeSinceStartup;
            if (now - _lastSnapshotRequestAt < 1f)
                return;
            _lastSnapshotRequestAt = now;
            Publish(SnapshotRequest, "{}", true);
        }

        static void ApplyPlayersSnapshot(string payload, int seq, long timestamp)
        {
            var map = GpJson.GetObject(payload, "players") ?? payload;
            var keys = GpJson.ObjectKeys(map);
            foreach (var key in keys)
            {
                if (!int.TryParse(key, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                    continue;
                var state = GpJson.GetObject(map, key);
                if (string.IsNullOrEmpty(state))
                    continue;
                if (id == NativePlayer.Id && PlayerStates.ContainsKey(id)
                    && !string.IsNullOrEmpty(_myState) && _myState != "{}")
                    continue;
                PlayerStates[id] = state;
            }
            _playersHaveBase = true;
            _lastFullPlayersJson = payload;
            if (seq >= 0)
                _lastPlayersSeq = seq;
            PushPlayersBuffer(payload, timestamp);
            GP_Multiplayer.NativeEmit("playersUpdated", new GP_Data(PlayersStateJson()));
        }

        static void ApplyPlayersDelta(string payload, int seq, long timestamp)
        {
            if (!_playersHaveBase || string.IsNullOrEmpty(_lastFullPlayersJson))
            {
                RequestSnapshot();
                return;
            }
            if (seq >= 0 && _lastPlayersSeq >= 0 && seq != _lastPlayersSeq + 1)
            {
                _playersHaveBase = false;
                _lastFullPlayersJson = null;
                _lastPlayersSeq = -1;
                RequestSnapshot();
                return;
            }
            _lastFullPlayersJson = GpJson.MergeDelta(_lastFullPlayersJson, payload);
            var map = GpJson.GetObject(_lastFullPlayersJson, "players") ?? _lastFullPlayersJson;
            var keys = GpJson.ObjectKeys(map);
            foreach (var key in keys)
            {
                if (!int.TryParse(key, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                    continue;
                var state = GpJson.GetObject(map, key);
                if (string.IsNullOrEmpty(state))
                    continue;
                if (id == NativePlayer.Id)
                {
                    PlayerStates[id] = _myState;
                    continue;
                }
                PlayerStates[id] = state;
            }
            if (seq >= 0)
                _lastPlayersSeq = seq;
            PushPlayersBuffer(_lastFullPlayersJson, timestamp);
            GP_Multiplayer.NativeEmit("playersUpdated", new GP_Data(PlayersStateJson()));
        }

        static void ApplyGlobalSnapshot(string payload, int seq, long timestamp)
        {
            _globalState = string.IsNullOrEmpty(payload) ? "{}" : payload;
            _lastFullGlobalJson = _globalState;
            if (seq >= 0)
                _lastGlobalSeq = seq;
            PushGlobalBuffer(_globalState, timestamp);
            GP_Multiplayer.NativeEmit("globalStateUpdated", new GP_Data(_globalState));
        }

        static void ApplyGlobalDelta(string payload, int seq, long timestamp)
        {
            if (string.IsNullOrEmpty(_lastFullGlobalJson) || _lastFullGlobalJson == "{}")
            {
                RequestSnapshot();
                return;
            }
            if (seq >= 0 && _lastGlobalSeq >= 0 && seq != _lastGlobalSeq + 1)
            {
                _globalState = "{}";
                _lastFullGlobalJson = null;
                _lastGlobalSeq = -1;
                RequestSnapshot();
                return;
            }
            var merged = GpJson.MergeDelta(_lastFullGlobalJson, payload);
            if (seq >= 0)
                _lastGlobalSeq = seq;
            if (string.Equals(merged, _lastFullGlobalJson, StringComparison.Ordinal))
                return;
            _lastFullGlobalJson = merged;
            _globalState = merged;
            PushGlobalBuffer(merged, timestamp);
            GP_Multiplayer.NativeEmit("globalStateUpdated", new GP_Data(_globalState));
        }

        static void PushPlayersBuffer(string payload, long timestamp)
        {
            var tree = GpJson.Parse(payload);
            if (tree == null)
                return;
            _playersInterp.AddStateWithGapFill(timestamp, tree, 1000.0 / Math.Max(1, _tickRate));
        }

        static void PushGlobalBuffer(string payload, long timestamp)
        {
            var tree = GpJson.Parse(payload);
            if (tree == null)
                return;
            _globalInterp.AddStateWithGapFill(
                timestamp, tree, (1000.0 / Math.Max(1, _tickRate)) * GlobalTickDivider);
        }

        static void ApplySchemasToEngines()
        {
            _playersInterp.SetTickRate(_tickRate);
            _globalInterp.SetTickRate(_tickRate);
            _playersInterp.SetBufferLimits(_bufferMinMs, _bufferMaxMs);
            _globalInterp.SetBufferLimits(_bufferMinMs, _bufferMaxMs);
            _playersInterp.SetSchema(GpJson.ParseObject(_playerSchema));
            _globalInterp.SetSchema(GpJson.ParseObject(_globalSchema));
        }

        static void SampleInterpolation(double nowMs)
        {
            var interpolated = _playersInterp.Interpolate(nowMs);
            if (interpolated != null && !ReferenceEquals(interpolated, _previousInterpolatedPlayers))
            {
                _previousInterpolatedPlayers = interpolated;
                if (ApplyInterpolatedPlayers(interpolated))
                    GP_Multiplayer.NativeEmit("playersUpdated", new GP_Data(PlayersStateJson()));
            }

            var interpolatedGlobal = _globalInterp.Interpolate(nowMs);
            if (interpolatedGlobal != null && !ReferenceEquals(interpolatedGlobal, _previousInterpolatedGlobal))
            {
                _previousInterpolatedGlobal = interpolatedGlobal;
                _globalState = GpJson.Stringify(interpolatedGlobal);
                GP_Multiplayer.NativeEmit("globalStateUpdated", new GP_Data(_globalState));
            }
        }

        static bool ApplyInterpolatedPlayers(object interpolated)
        {
            Dictionary<string, object> players = null;
            if (interpolated is Dictionary<string, object> root)
            {
                if (root.TryGetValue("players", out var node))
                    players = node as Dictionary<string, object>;
                if (players == null)
                    players = root;
            }
            if (players == null)
                return false;
            var changed = false;
            foreach (var pair in players)
            {
                if (!int.TryParse(pair.Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                    continue;
                if (id == NativePlayer.Id)
                {
                    PlayerStates[id] = _myState;
                    continue;
                }
                var json = GpJson.Stringify(pair.Value);
                if (!PlayerStates.TryGetValue(id, out var current)
                    || !string.Equals(current, json, StringComparison.Ordinal))
                    changed = true;
                PlayerStates[id] = json;
            }
            return changed;
        }

        static void Publish(int type, string payloadJson, bool toHost = false, Action<bool> onDone = null)
        {
            var sub = toHost ? _hostSub : _stateSub;
            if (sub == null || !_connected || !_transportLive)
            {
                onDone?.Invoke(false);
                return;
            }
            var json = "{\"t\":" + type +
                       ",\"s\":" + NativePlayer.Id +
                       ",\"ts\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() +
                       ",\"p\":" + (string.IsNullOrEmpty(payloadJson) ? "null" : payloadJson);
            if (type == StateUpdate || type == StateDelta)
            {
                _playersSeq++;
                json += ",\"sq\":" + _playersSeq;
            }
            else if (type == GlobalStateUpdate || type == GlobalStateDelta)
            {
                _globalSendSeq++;
                json += ",\"sq\":" + _globalSendSeq;
            }
            json += "}";
            var bytes = Encoding.UTF8.GetBytes(json);
            var reportSendError = type == StateUpdate || type == StateDelta ||
                                  type == GlobalStateUpdate || type == GlobalStateDelta;
            sub.Publish(bytes, ok =>
            {
                if (!ok && reportSendError)
                {
                    GP_Logger.Info("Centrifugo", "sendState failed t=" + type);
                    GP_Multiplayer.NativeEmit("error:sendState",
                        new GP_Data("{\"message\":\"publish failed\",\"t\":" + type + "}"));
                }
                onDone?.Invoke(ok);
            });
        }

        static void AddSelf()
        {
            var self = new PlayerSlot
            {
                playerId = NativePlayer.Id,
                name = NativePlayer.GetString("name"),
                connectionStability = 1
            };
            Players[self.playerId] = self;
            LastBeat[self.playerId] = Time.realtimeSinceStartup;
            PlayerStates[self.playerId] = _myState;
        }

        static void RemovePlayer(int id)
        {
            if (!Players.TryGetValue(id, out var slot))
                return;
            Players.Remove(id);
            LastBeat.Remove(id);
            PlayerStates.Remove(id);
            GP_Multiplayer.NativeEmit("playerLeft", new GP_Data(PlayerJson(slot)));
        }

        static void HandleTransportInterrupted()
        {
            if (!_connected)
                return;
            GP_Logger.Warn("Centrifugo", "reconnecting");
            _reconnectGraceUntil = Time.realtimeSinceStartup + 2.5f;
            TouchLastBeats();
        }

        static void HandleTransportReconnect()
        {
            if (!_connected)
                return;
            var wasHost = IsHost;
            _transportLive = true;
            _hostId = 0;
            _hostElectedAt = 0;
            foreach (var pair in Players)
                pair.Value.isHost = false;
            _reconnectGraceUntil = Time.realtimeSinceStartup + 2.5f;
            _selfReconnectCount++;
            TouchLastBeats();
            _lastSentPlayers = null;
            _lastSentGlobal = null;
            _lastHostRecheckTime = 0f;
            _tickCounter = 0;
            _playersHaveBase = false;
            _lastFullPlayersJson = null;
            _lastFullGlobalJson = null;
            _lastPlayersSeq = -1;
            _lastGlobalSeq = -1;
            _previousInterpolatedPlayers = null;
            _previousInterpolatedGlobal = null;
            _playersInterp.Clear();
            _globalInterp.Clear();
            ApplySchemasToEngines();
            if (wasHost)
                GP_Multiplayer.NativeEmit("becamePeer");
            GP_Logger.Info("Centrifugo", "reconnected host-reset self=" + NativePlayer.Id);
        }

        static void TouchLastBeats()
        {
            var now = Time.realtimeSinceStartup;
            var ids = new List<int>(LastBeat.Keys);
            for (var i = 0; i < ids.Count; i++)
                LastBeat[ids[i]] = now;
            _lastTimeoutCheck = now;
        }

        static void NoteFocusLost()
        {
            if (!_connected)
                return;
            _reconnectGraceUntil = Time.realtimeSinceStartup + 2.5f;
            TouchLastBeats();
        }

        static void HandleTransportDrop(string reason)
        {
            GP_Logger.Error("Centrifugo", "dropped: " + (reason ?? "disconnected"));
            Cleanup(false);
            GP_Multiplayer.NativeEmit("disconnect",
                new GP_Data("{\"reason\":\"" + GpJson.Escape(reason ?? "disconnected") + "\"}"));
        }

        static void Cleanup(bool emit)
        {
            _connected = false;
            _connecting = false;
            StopHeartbeatClock();
            try { _hostSub?.Unsubscribe(); } catch { /* ignore */ }
            try { _stateSub?.Unsubscribe(); } catch { /* ignore */ }
            try { _centrifuge?.Dispose(); } catch { /* ignore */ }
            _hostSub = null;
            _stateSub = null;
            _centrifuge = null;
            Players.Clear();
            PlayerStates.Clear();
            LastBeat.Clear();
            _hostId = 0;
            _hostElectedAt = 0;
            _loggedWire = 0;
            _transportLive = false;
            _reconnectGraceUntil = 0f;
            _lastSentPlayers = null;
            _lastSentGlobal = null;
            _playersActiveLastTick = false;
            _globalActiveLastTick = false;
            _lastPlayersSeq = -1;
            _lastGlobalSeq = -1;
            _lastSnapshotRequestAt = 0f;
            _playersHaveBase = false;
            _lastFullPlayersJson = null;
            _lastFullGlobalJson = null;
            _previousInterpolatedPlayers = null;
            _previousInterpolatedGlobal = null;
            _playersInterp.Clear();
            _globalInterp.Clear();
            _lastHostRecheckTime = 0f;
            _lastMigrationTime = 0f;
            _hostAnnounceRetry1 = 0f;
            _hostAnnounceRetry2 = 0f;
            _selfReconnectCount = 0;
            _selfFreezeCount = 0;
            _selfPingJitterEma = 0f;
            _selfPing = 0;
            _selfEchoLastAt = 0f;
            _tickCounter = 0;
            _globalState = "{}";
            if (_pump != null)
            {
                UnityEngine.Object.Destroy(_pump);
                _pump = null;
            }
            if (emit)
                GP_Multiplayer.NativeEmit("disconnect", new GP_Data("{\"reason\":\"cleanup\"}"));
        }

        static void EnsurePump()
        {
            if (_pump != null)
                return;
            _pump = NativeMainThread.Instance.gameObject.GetComponent<NativePump>()
                    ?? NativeMainThread.Instance.gameObject.AddComponent<NativePump>();
        }

        static string PlayerJson(PlayerSlot slot)
        {
            return "{\"playerId\":" + slot.playerId +
                   ",\"isHost\":" + (slot.isHost ? "true" : "false") +
                   ",\"ping\":" + slot.ping +
                   ",\"connectionStability\":" + slot.connectionStability.ToString("0.###", CultureInfo.InvariantCulture) +
                   ",\"sessionDuration\":" + slot.sessionDuration + "}";
        }

        static MultiplayerConnectedPlayerData ToConnected(PlayerSlot slot)
        {
            return new MultiplayerConnectedPlayerData
            {
                playerId = slot.playerId,
                isHost = slot.isHost,
                ping = slot.ping,
                connectionStability = slot.connectionStability,
                sessionDuration = slot.sessionDuration
            };
        }

        public sealed class PlayerSlot
        {
            public int playerId;
            public string name;
            public bool isHost;
            public int ping;
            public float connectionStability = 1;
            public int sessionDuration;
        }

        sealed class NativePump : MonoBehaviour
        {
            void OnEnable() => Application.runInBackground = true;

            void Update() => Tick(Time.realtimeSinceStartup);

            void OnApplicationFocus(bool hasFocus)
            {
                if (!hasFocus)
                    NoteFocusLost();
            }

            void OnApplicationPause(bool paused)
            {
                if (paused)
                    NoteFocusLost();
            }
        }
    }
}
