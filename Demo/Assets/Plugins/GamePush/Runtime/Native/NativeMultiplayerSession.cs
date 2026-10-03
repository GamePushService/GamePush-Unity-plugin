using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GamePush;
using UnityEngine;

namespace GamePush.Native
{
    sealed class NativeMultiplayerSession
    {
        public const float HeartbeatInterval = 1f;
        public const float HeartbeatTimeout = 4f;
        public const float HostHeartbeatTimeout = 5f;
        public const float HostDiscoverDelay = 1.25f;
        public const float HostRecheckInterval = 60f;
        public const float ReconnectGrace = 2.5f;

        public event Action<NativeMultiplayerPlayer, bool> PlayerJoined;
        public event Action<NativeMultiplayerPlayer> PlayerLeft;
        public event Action<int, int> HostChanged;
        public event Action<int, string, string> CustomEvent;
        public event Action<float> PageUnfrozen;
        public event Action<int> SnapshotRequested;

        readonly NativeMultiplayerTransport _transport;
        readonly Dictionary<int, NativeMultiplayerPlayer> _players = new Dictionary<int, NativeMultiplayerPlayer>();
        readonly Dictionary<int, float> _lastBeat = new Dictionary<int, float>();
        readonly Dictionary<int, float> _hostStabilityObservations = new Dictionary<int, float>();

        int _hostId;
        int _previousHostId;
        long _hostElectedAt;
        float _sessionStart;
        float _reconnectGraceUntil;
        float _lastHostRecheckTime;
        float _lastMigrationTime;
        float _hostAnnounceRetry1;
        float _hostAnnounceRetry2;
        float _lastTimeoutCheck;
        float _selfEchoLastAt;
        float _hostDiscoverUntil;
        int _selfReconnectCount;
        int _selfFreezeCount;
        float _selfPingJitterEma;
        int _selfPing;
        bool _active;
        float _nextHeartbeatAt;

        public bool IsActive => _active;
        public int HostId => _hostId;
        public bool IsHost => _active && _hostId == NativePlayer.Id && NativePlayer.Id > 0;
        public int SelfPing => _selfPing;
        public IReadOnlyDictionary<int, NativeMultiplayerPlayer> Players => _players;

        public NativeMultiplayerSession(NativeMultiplayerTransport transport)
        {
            _transport = transport;
        }

        public void Start()
        {
            _active = true;
            _sessionStart = Time.realtimeSinceStartup;
            _hostId = 0;
            _hostElectedAt = 0;
            _nextHeartbeatAt = 0f;
            _hostDiscoverUntil = _sessionStart + HostDiscoverDelay;
            AddSelf(true);
            SendHeartbeat();
            GP_Logger.Info("Multiplayer", "session started player=" + NativePlayer.Id + " awaiting host election");
            // Do not elect immediately. A joiner that claims host here rejects the
            // real host's snapshots (ShouldAcceptHostState) and never sees them.
            // The room creator is still elected as soon as the discover window
            // expires without a remote HostMigration.
        }

        public void Stop()
        {
            _active = false;
            _players.Clear();
            _lastBeat.Clear();
            _hostStabilityObservations.Clear();
            _hostId = 0;
            _hostElectedAt = 0;
            _nextHeartbeatAt = 0f;
            _hostDiscoverUntil = 0f;
            PlayerJoined = null;
            PlayerLeft = null;
            HostChanged = null;
            CustomEvent = null;
            PageUnfrozen = null;
            SnapshotRequested = null;
        }

        public void Tick(float now)
        {
            if (!_active || !_transport.IsLive)
                return;
            TryDiscoverHost(now);
            if (now < _nextHeartbeatAt)
                return;
            _nextHeartbeatAt = now + HeartbeatInterval;
            SendHeartbeat();
            CheckTimeouts(now);
            if (IsHost)
            {
                if (_hostAnnounceRetry1 > 0 && now >= _hostAnnounceRetry1) { _hostAnnounceRetry1 = 0; AnnounceHost(); }
                if (_hostAnnounceRetry2 > 0 && now >= _hostAnnounceRetry2) { _hostAnnounceRetry2 = 0; AnnounceHost(); }
            }
        }

        public void HandleMessage(NativeMultiplayerWireMessage message)
        {
            if (!_active || message == null || message.SenderId <= 0)
                return;
            _lastBeat[message.SenderId] = Time.realtimeSinceStartup;
            switch (message.Type)
            {
                case NativeMultiplayerWire.Heartbeat:
                    HandleHeartbeat(message);
                    break;
                case NativeMultiplayerWire.HostMigration:
                    if (message.SenderId != NativePlayer.Id)
                    {
                        var newHost = GpJson.GetInt(message.Payload, "newHostId");
                        var elected = GpJson.GetLong(message.Payload, "hostElectedAt");
                        ApplyHost(newHost, elected, true);
                    }
                    break;
                case NativeMultiplayerWire.SnapshotRequest:
                    if (IsHost && message.SenderId != NativePlayer.Id)
                        SnapshotRequested?.Invoke(message.SenderId);
                    break;
                case NativeMultiplayerWire.CustomEvent:
                    if (message.SenderId != NativePlayer.Id)
                        HandleCustom(message.SenderId, message.Payload);
                    break;
            }
        }

        public bool ShouldAcceptHostState(int senderId)
        {
            return _transport.IsLive && _hostId > 0 && senderId > 0 &&
                   senderId != NativePlayer.Id && senderId == _hostId;
        }

        public void HandleReconnect()
        {
            if (!_active)
                return;
            _reconnectGraceUntil = Time.realtimeSinceStartup + ReconnectGrace;
            _selfReconnectCount++;
            _selfEchoLastAt = 0f;
            TouchLastBeats();
            _lastHostRecheckTime = 0f;
            _hostDiscoverUntil = Time.realtimeSinceStartup + HostDiscoverDelay;
            SendHeartbeat();
        }

        void TryDiscoverHost(float now)
        {
            if (_hostId != 0 || NativePlayer.Id <= 0)
                return;
            if (_hostDiscoverUntil > 0f && now < _hostDiscoverUntil)
                return;
            if (now < _reconnectGraceUntil)
                return;
            ElectHost();
        }

        public void NoteTransportInterrupted()
        {
            if (!_active)
                return;
            GP_Logger.Warn("Centrifugo", "reconnecting");
            InvalidateHost();
            // Membership from before the interruption is not evidence of presence now.
            var previousPlayers = new List<NativeMultiplayerPlayer>(_players.Values);
            _players.Clear();
            _lastBeat.Clear();
            _hostStabilityObservations.Clear();
            AddSelf(false);
            foreach (var player in previousPlayers)
                if (player.playerId != NativePlayer.Id) PlayerLeft?.Invoke(player);
            _reconnectGraceUntil = Time.realtimeSinceStartup + ReconnectGrace;
            TouchLastBeats();
        }

        void InvalidateHost()
        {
            if (_hostId == 0) return;
            _previousHostId = _hostId;
            _hostId = 0;
            _hostElectedAt = 0;
            _hostAnnounceRetry1 = _hostAnnounceRetry2 = 0f;
            foreach (var player in _players.Values) player.isHost = false;
            HostChanged?.Invoke(_previousHostId, 0);
        }

        public void NoteFocusLost()
        {
            if (!_active)
                return;
            _reconnectGraceUntil = Time.realtimeSinceStartup + ReconnectGrace;
            TouchLastBeats();
        }

        public string ConnectedPlayersJson()
        {
            var sb = new StringBuilder();
            sb.Append('[');
            var first = true;
            foreach (var pair in _players)
            {
                if (!first) sb.Append(',');
                first = false;
                sb.Append(PlayerJson(pair.Value));
            }
            sb.Append(']');
            return sb.ToString();
        }

        public static string PlayerJson(NativeMultiplayerPlayer slot)
        {
            return "{\"playerId\":" + slot.playerId +
                   ",\"isHost\":" + (slot.isHost ? "true" : "false") +
                   ",\"ping\":" + slot.ping +
                   ",\"connectionStability\":" + slot.connectionStability.ToString("0.###", CultureInfo.InvariantCulture) +
                   ",\"sessionDuration\":" + slot.sessionDuration + "}";
        }

        public static MultiplayerConnectedPlayerData ToConnected(NativeMultiplayerPlayer slot)
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

        void HandleHeartbeat(NativeMultiplayerWireMessage message)
        {
            var payload = message.Payload;
            if (message.SenderId == NativePlayer.Id)
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
                if (_players.TryGetValue(NativePlayer.Id, out var self))
                    self.ping = _selfPing;
                return;
            }

            if (IsHost)
            {
                var observed = GpJson.GetFloat(payload, "hostStability", float.NaN);
                if (!float.IsNaN(observed))
                    _hostStabilityObservations[message.SenderId] = observed;
            }

            if (!_players.TryGetValue(message.SenderId, out var slot))
            {
                slot = new NativeMultiplayerPlayer
                {
                    playerId = message.SenderId,
                    name = GpJson.TryGetString(payload, "name", out var n) ? n : "",
                    connectionStability = Math.Max(0f, Math.Min(1f, GpJson.GetFloat(payload, "stability", 1f))),
                    ping = GpJson.GetInt(payload, "ping"),
                    sessionDuration = GpJson.GetInt(payload, "sessionDuration")
                };
                _players[message.SenderId] = slot;
                _lastBeat[message.SenderId] = Time.realtimeSinceStartup;
                PlayerJoined?.Invoke(slot, false);
                if (IsHost)
                {
                    AnnounceHost();
                    ScheduleHostAnnounceRetries();
                }
            }
            else
            {
                slot.ping = GpJson.GetInt(payload, "ping");
                slot.sessionDuration = GpJson.GetInt(payload, "sessionDuration");
                slot.connectionStability = GpJson.GetFloat(payload, "stability", slot.connectionStability);
                slot.isHost = message.SenderId == _hostId;
                // A reconnecting peer may still be in our membership map. It must
                // rediscover authority even when it did not trigger playerJoined.
                if (IsHost && GpJson.GetInt(payload, "hostId") != NativePlayer.Id)
                    AnnounceHost();
            }
        }

        void HandleCustom(int sender, string payload)
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
            CustomEvent?.Invoke(sender, eventName, data);
        }

        void SendHeartbeat()
        {
            RefreshSelfMetrics();
            _players.TryGetValue(NativePlayer.Id, out var self);
            var duration = self != null ? self.sessionDuration : 0;
            var stability = self != null
                ? self.connectionStability.ToString("0.##", CultureInfo.InvariantCulture)
                : "1";
            var payload = "{\"sessionDuration\":" + duration +
                          ",\"hostId\":" + _hostId +
                          ",\"sentAt\":" + (Time.realtimeSinceStartup * 1000f).ToString("0.###", CultureInfo.InvariantCulture) +
                          ",\"ping\":" + _selfPing +
                          ",\"name\":" + GpJson.Quote(NativePlayer.GetString("name")) +
                          ",\"stability\":" + stability;
            if (_hostId != 0 && _hostId != NativePlayer.Id &&
                _players.TryGetValue(_hostId, out var host))
            {
                payload += ",\"hostStability\":" +
                           host.connectionStability.ToString("0.##", CultureInfo.InvariantCulture);
            }
            payload += "}";
            _transport.Send(NativeMultiplayerWire.Heartbeat, payload, NativePlayer.Id);
        }

        void RefreshSelfMetrics()
        {
            if (!_players.TryGetValue(NativePlayer.Id, out var self))
                return;
            self.sessionDuration = (int)((Time.realtimeSinceStartup - _sessionStart) * 1000f);
            self.ping = _selfPing;
            self.connectionStability = NativeHostSelector.CalculateSelfStability(
                _selfReconnectCount, _selfFreezeCount, _selfPingJitterEma);
        }

        void ElectHost()
        {
            RefreshSelfMetrics();
            if (IsHost)
            {
                var aggregated = GetAggregatedHostStability();
                if (aggregated.HasValue && _players.TryGetValue(NativePlayer.Id, out var self))
                    self.connectionStability = aggregated.Value;
            }

            var list = new List<NativeMultiplayerPlayer>(_players.Values);
            if (list.Count == 0)
                return;

            if (!_players.TryGetValue(_hostId, out var currentHost))
            {
                var selected = NativeHostSelector.SelectHost(list);
                if (selected == null)
                    return;
                ApplyHost(selected.playerId, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), false);
                GP_Logger.Info("Multiplayer", "host elected player=" + selected.playerId + " candidates=" + list.Count);
                if (IsHost)
                {
                    AnnounceHost();
                    ScheduleHostAnnounceRetries();
                }
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

        float? GetAggregatedHostStability()
        {
            if (_hostStabilityObservations.Count == 0)
                return null;
            var min = 1f;
            foreach (var pair in _hostStabilityObservations)
            {
                if (pair.Value < min)
                    min = pair.Value;
            }
            return min;
        }

        void AnnounceHost(int newHostId = 0)
        {
            var hostId = newHostId > 0 ? newHostId : NativePlayer.Id;
            if (hostId <= 0)
                return;
            if (newHostId <= 0 && !IsHost)
                return;
            var payload = "{\"newHostId\":" + hostId +
                          ",\"hostElectedAt\":" + _hostElectedAt.ToString(CultureInfo.InvariantCulture) + "}";
            _transport.Send(NativeMultiplayerWire.HostMigration, payload, NativePlayer.Id);
        }

        void ScheduleHostAnnounceRetries()
        {
            var now = Time.realtimeSinceStartup;
            _hostAnnounceRetry1 = now + 0.5f;
            _hostAnnounceRetry2 = now + 1f;
        }

        void ApplyHost(int newHost, long electedAt, bool fromRemote)
        {
            if (newHost <= 0)
                return;
            if (_hostId == newHost && (electedAt <= 0 || electedAt <= _hostElectedAt))
                return;
            if (fromRemote && IsHost && newHost != NativePlayer.Id)
            {
                var keep = _hostElectedAt != 0 && (electedAt == 0 || _hostElectedAt < electedAt ||
                    _hostElectedAt == electedAt && NativePlayer.Id < newHost);
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
            var old = _hostId != 0 ? _hostId : _previousHostId;
            _hostId = newHost;
            _previousHostId = 0;
            _hostElectedAt = electedAt > 0 ? electedAt : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (!_players.ContainsKey(newHost))
            {
                var host = new NativeMultiplayerPlayer { playerId = newHost, isHost = true };
                _players[newHost] = host;
                _lastBeat[newHost] = Time.realtimeSinceStartup;
                PlayerJoined?.Invoke(host, false);
            }
            foreach (var pair in _players)
                pair.Value.isHost = pair.Value.playerId == newHost;
            HostChanged?.Invoke(old, newHost);
            GP_Logger.Info("Host", "old=" + old + " new=" + newHost +
                                   " self=" + NativePlayer.Id +
                                   " isHost=" + IsHost +
                                   " remote=" + fromRemote);
        }

        void CheckTimeouts(float now)
        {
            if (!_transport.IsLive || now < _reconnectGraceUntil)
                return;
            if (_lastTimeoutCheck > 0f && now - _lastTimeoutCheck > HeartbeatTimeout)
            {
                var frozenMs = (now - _lastTimeoutCheck) * 1000f;
                GP_Logger.Info("Multiplayer", "frozen " + frozenMs.ToString("0", CultureInfo.InvariantCulture) + "ms");
                var frozen = new List<int>(_lastBeat.Keys);
                for (var i = 0; i < frozen.Count; i++)
                    _lastBeat[frozen[i]] = now;
                _lastTimeoutCheck = now;
                _selfFreezeCount++;
                InvalidateHost();
                _hostDiscoverUntil = now + HostDiscoverDelay;
                _reconnectGraceUntil = now + ReconnectGrace;
                PageUnfrozen?.Invoke(frozenMs);
                return;
            }
            _lastTimeoutCheck = now;
            if (_hostId != 0 && _selfEchoLastAt > 0f && now - _selfEchoLastAt > HostHeartbeatTimeout)
            {
                GP_Logger.Info("Multiplayer", "self-echo delayed " +
                               ((now - _selfEchoLastAt) * 1000f).ToString("0", CultureInfo.InvariantCulture) +
                               "ms — skip peer timeouts");
                return;
            }
            var dead = new List<int>();
            foreach (var pair in _lastBeat)
            {
                if (pair.Key == NativePlayer.Id)
                    continue;
                var limit = pair.Key == _hostId ? HostHeartbeatTimeout : HeartbeatTimeout;
                if (now - pair.Value > limit)
                    dead.Add(pair.Key);
            }
            foreach (var id in dead)
                RemovePlayer(id);
            if (_hostId != 0 && !_players.ContainsKey(_hostId))
            {
                InvalidateHost();
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

        void RemovePlayer(int id)
        {
            if (!_players.TryGetValue(id, out var slot))
                return;
            if (id == _hostId) InvalidateHost();
            _players.Remove(id);
            _lastBeat.Remove(id);
            _hostStabilityObservations.Remove(id);
            PlayerLeft?.Invoke(slot);
        }

        void AddSelf(bool emitJoined)
        {
            var self = new NativeMultiplayerPlayer
            {
                playerId = NativePlayer.Id,
                name = NativePlayer.GetString("name"),
                connectionStability = 1
            };
            _players[self.playerId] = self;
            _lastBeat[self.playerId] = Time.realtimeSinceStartup;
            if (emitJoined)
                PlayerJoined?.Invoke(self, true);
        }

        void TouchLastBeats()
        {
            var now = Time.realtimeSinceStartup;
            var ids = new List<int>(_lastBeat.Keys);
            for (var i = 0; i < ids.Count; i++)
                _lastBeat[ids[i]] = now;
            _lastTimeoutCheck = now;
        }

    }
}
