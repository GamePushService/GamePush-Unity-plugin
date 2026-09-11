using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using GamePush;

namespace GamePush.Native
{
    public static class NativeMultiplayer
    {
        const float SnapshotResponseThrottle = 0.25f;

        static NativeMultiplayerTransport _transport;
        static NativeMultiplayerSynchronizer _synchronizer;
        static NativeMultiplayerStateManager _stateManager;
        static NativeMultiplayerSession _session;
        static NativePump _pump;

        static bool _connected;
        static bool _connecting;
        static int _channelId;
        static float _nextTick;
        static string _pendingPeerState;
        static bool _pendingPeerStateFlushed = true;
        static bool _peerActiveLastTick;
        static float _lastSnapshotResponseAt;
        static string _pendingPlayerSchema = "{}";
        static string _pendingGlobalSchema = "{}";
        static string _pendingMode = "smooth";

        public static bool IsConnected => _connected;
        public static bool IsHost => _session != null && _session.IsHost;
        public static int TickRate =>
            _synchronizer != null
                ? _synchronizer.TickRate
                : string.Equals(_pendingMode, "fast", StringComparison.OrdinalIgnoreCase) ? 60 : 20;

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
                await OpenSession(result, cancellationToken);
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
                await OpenSession(result, cancellationToken);
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

        static async Task OpenSession(string credentials, CancellationToken cancellationToken)
        {
            _transport = new NativeMultiplayerTransport();
            _synchronizer = new NativeMultiplayerSynchronizer();
            _synchronizer.DefinePlayerSchema(_pendingPlayerSchema);
            _synchronizer.DefineGlobalSchema(_pendingGlobalSchema);
            ApplyPendingMode(false);
            _stateManager = new NativeMultiplayerStateManager(_transport, _synchronizer);
            _session = new NativeMultiplayerSession(_transport);
            WireModules();
            await _transport.Connect(credentials, cancellationToken);
        }

        static void WireModules()
        {
            _transport.Reconnecting += () => _session?.NoteTransportInterrupted();
            _transport.Reconnected += HandleTransportReconnect;
            _transport.Disconnected += HandleTransportDrop;
            _transport.Message += HandleWire;
            _synchronizer.SnapshotRequest += () =>
            {
                if (_connected && _session != null && !_session.IsHost)
                    _transport.SendToHost(NativeMultiplayerWire.SnapshotRequest, "{}", NativePlayer.Id);
            };
            _synchronizer.PeerStateChanged += (playerId, state) =>
            {
                if (_session == null || !_session.IsHost)
                    return;
                _synchronizer.TryGetPlayerState(playerId, out var existing);
                if (!GpJson.HasPartialChanges(state, existing ?? "{}"))
                    return;
                _synchronizer.SetPlayerState(playerId, GpJson.MergeDelta(existing, state));
                GP_Multiplayer.NativeEmit("playersUpdated", new GP_Data(_synchronizer.PlayersStateJson()));
            };
            _synchronizer.PlayersUpdated += () =>
                GP_Multiplayer.NativeEmit("playersUpdated", new GP_Data(_synchronizer.PlayersStateJson()));
            _synchronizer.GlobalStateUpdated += state =>
                GP_Multiplayer.NativeEmit("globalStateUpdated", new GP_Data(state));
            _session.PlayerJoined += (player, isSelf) =>
            {
                if (_session.IsHost)
                    InitMissingPlayers();
                var joined = "{\"player\":" + NativeMultiplayerSession.PlayerJson(player) +
                             ",\"isSelf\":" + (isSelf ? "true" : "false") + "}";
                GP_Multiplayer.NativeEmit("playerJoined", new GP_Data(joined));
            };
            _session.PlayerLeft += player =>
            {
                _synchronizer.RemovePlayer(player.playerId);
                GP_Multiplayer.NativeEmit("playerLeft", new GP_Data(NativeMultiplayerSession.PlayerJson(player)));
                if (_session != null && _session.IsHost)
                    _stateManager.SendFullSnapshot();
            };
            _session.HostMigrated += (oldHost, newHost) =>
                GP_Multiplayer.NativeEmit("hostMigrated",
                    new GP_Data("{\"oldHost\":" + oldHost + ",\"newHost\":" + newHost + "}"));
            _session.BecameHost += () =>
            {
                _stateManager.ResetSendState();
                _stateManager.StartSending();
                InitMissingPlayers();
                GP_Multiplayer.NativeEmit("becameHost");
            };
            _session.BecamePeer += () =>
            {
                _stateManager.StopSending();
                _stateManager.ResetSendState();
                GP_Multiplayer.NativeEmit("becamePeer");
            };
            _session.CustomEvent += EmitCustom;
            _session.PageUnfrozen += frozenMs =>
            {
                GP_Logger.Info("Multiplayer", "Resyncing state after " +
                               frozenMs.ToString("0", CultureInfo.InvariantCulture) + "ms freeze");
                if (_session != null && _session.IsHost)
                {
                    _stateManager.ResetSendState();
                    _stateManager.SendFullSnapshot();
                }
                else if (!string.IsNullOrEmpty(_synchronizer.MyState) && _synchronizer.MyState != "{}")
                {
                    _pendingPeerState = _synchronizer.MyState;
                    _pendingPeerStateFlushed = false;
                    FlushPendingPeerState();
                }
            };
            _session.SnapshotRequested += _ =>
            {
                if (_session == null || !_session.IsHost)
                    return;
                var now = Time.realtimeSinceStartup;
                if (now - _lastSnapshotResponseAt < SnapshotResponseThrottle)
                    return;
                _lastSnapshotResponseAt = now;
                _stateManager.SendFullSnapshot();
            };
        }

        static MultiplayerConnectResultData FinishConnect()
        {
            _nextTick = 0;
            _pendingPeerState = null;
            _pendingPeerStateFlushed = true;
            _peerActiveLastTick = false;
            _lastSnapshotResponseAt = 0f;
            _synchronizer.SetPlayerState(NativePlayer.Id, _synchronizer.MyState);
            _session.Start();
            EnsurePump();
            Application.runInBackground = true;
            _connected = true;
            _connecting = false;
            var payload = new GP_Data("{\"success\":true}");
            NativeMainThread.Run(() => GP_Multiplayer.NativeEmit("connect", payload));
            GP_Logger.Info("Multiplayer", "connected channel=" + _channelId + " player=" + NativePlayer.Id +
                                          " tickRate=" + TickRate +
                                          " globalDivider=" + _synchronizer.GlobalTickDivider);
            return new MultiplayerConnectResultData { success = true };
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
            _pendingPlayerSchema = schema ?? "{}";
            _synchronizer?.DefinePlayerSchema(_pendingPlayerSchema);
        }

        public static void DefineGlobalSchema(string schema)
        {
            _pendingGlobalSchema = schema ?? "{}";
            _synchronizer?.DefineGlobalSchema(_pendingGlobalSchema);
        }

        public static void SetMode(string mode)
        {
            _pendingMode = mode ?? "smooth";
            ApplyPendingMode(_connected);
        }

        static void ApplyPendingMode(bool restartHost)
        {
            var fast = string.Equals(_pendingMode, "fast", StringComparison.OrdinalIgnoreCase);
            var tickRate = fast ? 60 : 20;
            var bufferMin = fast ? 50 : 100;
            var bufferMax = fast ? 100 : 300;
            _synchronizer?.SetMode(tickRate, bufferMin, bufferMax);
            _nextTick = 0;
            if (restartHost && _session != null && _session.IsHost)
            {
                _stateManager.ResetSendState();
                _stateManager.StartSending();
                _stateManager.SendFullSnapshot();
            }
        }

        public static void SetPlayerState(string state)
        {
            if (!_connected || _synchronizer == null)
                return;
            var incoming = string.IsNullOrEmpty(state) ? "{}" : state;
            incoming = GP_NativeSchema.FilterReadonly(incoming, _synchronizer.MyState, _synchronizer.PlayerSchema);
            if (!GpJson.HasPartialChanges(incoming, _synchronizer.MyState))
                return;
            var merged = GpJson.MergeDelta(_synchronizer.MyState, incoming);
            _synchronizer.SetMyState(merged);
            _synchronizer.SetPlayerState(NativePlayer.Id, merged);
            if (_session != null && _session.IsHost)
                GP_Multiplayer.NativeEmit("playersUpdated", new GP_Data(_synchronizer.PlayersStateJson()));
            else
            {
                _pendingPeerState = string.IsNullOrEmpty(_pendingPeerState)
                    ? incoming
                    : GpJson.MergeDelta(_pendingPeerState, incoming);
                _pendingPeerStateFlushed = false;
            }
        }

        public static void SetGlobalState(string state)
        {
            if (_session == null || !_session.IsHost || _synchronizer == null)
                return;
            var incoming = string.IsNullOrEmpty(state) ? "{}" : state;
            incoming = GP_NativeSchema.FilterReadonly(incoming, _synchronizer.GlobalState, _synchronizer.GlobalSchema);
            _synchronizer.SetGlobalState(GpJson.MergeDelta(_synchronizer.GlobalState, incoming));
        }

        public static void SendMessage(string eventName, string data, string options)
        {
            if (!_connected || _transport == null)
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
            _transport.Send(NativeMultiplayerWire.CustomEvent, payload, NativePlayer.Id);
            if (echo || target == NativePlayer.Id.ToString(CultureInfo.InvariantCulture))
                EmitCustom(NativePlayer.Id, eventName, data);
        }

        public static void NotifyInitializerChanged()
        {
            if (_connected && _session != null && _session.IsHost)
                InitMissingPlayers();
        }

        public static string ConnectedPlayersJson() =>
            _session != null ? _session.ConnectedPlayersJson() : "[]";

        public static string PlayersStateJson() =>
            _synchronizer != null ? _synchronizer.PlayersStateJson() : "{\"players\":[]}";

        public static string MyStateJson() => _synchronizer != null ? _synchronizer.MyState : "{}";
        public static string GlobalStateJson() => _synchronizer != null ? _synchronizer.GlobalState : "{}";

        public static string NetworkStatsJson() =>
            _synchronizer != null
                ? _synchronizer.NetworkStatsJson(_session != null ? _session.SelfPing : 0)
                : "{\"ping\":0,\"bufferSize\":0,\"bufferDelay\":0}";

        public static string InterpolationStatsJson() =>
            _synchronizer != null
                ? _synchronizer.InterpolationStatsJson()
                : "{\"seqGapCount\":0,\"snapshotRequestCount\":0,\"bufferSize\":0,\"bufferDelay\":0}";

        public static string RuntimeCapabilitiesJson() =>
            "{\"connect\":true,\"disconnect\":true,\"setPlayerState\":true,\"setGlobalState\":true,\"sendMessage\":true,\"hostMigrationEvents\":true}";

        public static void Tick(float now)
        {
            if (!_connected || _synchronizer == null || _session == null)
                return;
            _session.Tick(now);
            if (!_session.IsHost)
                _synchronizer.SampleInterpolation(now * 1000.0);
            if (now < _nextTick)
                return;
            var dt = 1f / Math.Max(1, TickRate);
            _nextTick = now + dt;
            if (_session.IsHost)
            {
                _synchronizer.SetPlayerState(NativePlayer.Id, _synchronizer.MyState);
                _stateManager.TickHostSend();
            }
            else
                FlushPendingPeerState();
            GP_Multiplayer.NativeEmitTick(dt * 1000f);
        }

        static void FlushPendingPeerState()
        {
            if (_session == null || _session.IsHost || _stateManager == null)
            {
                _peerActiveLastTick = false;
                return;
            }
            if (_session.HostId <= 0)
                return;
            if (_pendingPeerStateFlushed || _pendingPeerState == null)
            {
                if (_peerActiveLastTick)
                {
                    _peerActiveLastTick = false;
                    _stateManager.SendMyStateToHost("{}");
                }
                return;
            }
            _stateManager.SendMyStateToHost(_pendingPeerState);
            _pendingPeerState = null;
            _pendingPeerStateFlushed = true;
            _peerActiveLastTick = true;
        }

        static void HandleWire(NativeMultiplayerWireMessage message)
        {
            if (_session == null || _synchronizer == null)
                return;
            if (message == null || message.SenderId <= 0)
            {
                GP_Logger.Warn("Multiplayer", "Dropped wire message with invalid senderId");
                return;
            }
            _session.HandleMessage(message);
            switch (message.Type)
            {
                case NativeMultiplayerWire.StateUpdate:
                    if (_session.ShouldAcceptHostState(message.SenderId))
                        _synchronizer.HandleHostPlayersSnapshot(message.Payload, message.Seq, message.Timestamp);
                    break;
                case NativeMultiplayerWire.StateDelta:
                    if (_session.ShouldAcceptHostState(message.SenderId))
                        _synchronizer.HandleHostPlayersDelta(message.Payload, message.Seq, message.Timestamp);
                    break;
                case NativeMultiplayerWire.PeerState:
                    if (_session.IsHost && message.SenderId != NativePlayer.Id && message.SenderId > 0)
                        _synchronizer.HandlePeerState(message.SenderId, message.Payload, message.Timestamp);
                    break;
                case NativeMultiplayerWire.GlobalStateUpdate:
                    if (_session.ShouldAcceptHostState(message.SenderId))
                        _synchronizer.HandleHostGlobalSnapshot(message.Payload, message.Seq, message.Timestamp);
                    break;
                case NativeMultiplayerWire.GlobalStateDelta:
                    if (_session.ShouldAcceptHostState(message.SenderId))
                        _synchronizer.HandleHostGlobalDelta(message.Payload, message.Seq, message.Timestamp);
                    break;
            }
        }

        static void HandleTransportReconnect()
        {
            if (!_connected)
                return;
            _transport?.MarkLive();
            _stateManager?.StopSending();
            _stateManager?.ResetSendState();
            _synchronizer?.ClearReceiveState();
            _session?.HandleReconnect();
        }

        static void HandleTransportDrop(string reason)
        {
            if (!_connected)
                return;
            GP_Logger.Error("Centrifugo", "dropped: " + (reason ?? "disconnected"));
            Cleanup(false);
            GP_Multiplayer.NativeEmit("disconnect",
                new GP_Data("{\"reason\":\"" + GpJson.Escape(reason ?? "disconnected") + "\"}"));
        }

        static void InitMissingPlayers()
        {
            _ = InitMissingPlayersAsync();
        }

        static async Task InitMissingPlayersAsync()
        {
            if (_session == null || _synchronizer == null || !_session.IsHost)
                return;
            var missing = new List<KeyValuePair<int, NativeMultiplayerPlayer>>();
            foreach (var pair in _session.Players)
            {
                if (pair.Key == NativePlayer.Id)
                    continue;
                if (_synchronizer.HasPlayerState(pair.Key))
                    continue;
                missing.Add(pair);
            }
            var changed = false;
            foreach (var pair in missing)
            {
                var state = await GP_Multiplayer.NativeInitPlayerAsync(
                    pair.Key, NativeMultiplayerSession.ToConnected(pair.Value));
                if (!string.IsNullOrEmpty(state) && state != "null")
                {
                    _synchronizer.SetPlayerState(pair.Key, state);
                    changed = true;
                }
            }
            if (changed && _session != null && _session.IsHost)
                _stateManager.SendFullSnapshot();
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

        static void Cleanup(bool emit)
        {
            _connected = false;
            _connecting = false;
            _pendingPeerState = null;
            _pendingPeerStateFlushed = true;
            _peerActiveLastTick = false;
            _lastSnapshotResponseAt = 0f;
            try { _session?.Stop(); } catch { /* ignore */ }
            try { _stateManager?.Cleanup(); } catch { /* ignore */ }
            try { _synchronizer?.Clear(); } catch { /* ignore */ }
            try { _transport?.Dispose(); } catch { /* ignore */ }
            _session = null;
            _stateManager = null;
            _synchronizer = null;
            _transport = null;
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

        static void NoteFocusLost() => _session?.NoteFocusLost();

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
