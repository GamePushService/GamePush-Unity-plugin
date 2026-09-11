using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GamePush;
using UnityEngine;

namespace GamePush.Native
{
    sealed class NativeMultiplayerSynchronizer
    {
        const float SnapshotRequestThrottle = 1f;

        public event Action SnapshotRequest;
        public event Action<int, string> PeerStateChanged;
        public event Action PlayersUpdated;
        public event Action<string> GlobalStateUpdated;

        readonly Dictionary<int, string> _playerStates = new Dictionary<int, string>();
        readonly Dictionary<int, string> _peerStates = new Dictionary<int, string>();
        readonly Dictionary<int, GP_InterpolationEngine> _peerEngines = new Dictionary<int, GP_InterpolationEngine>();
        readonly Dictionary<int, object> _lastPeerSample = new Dictionary<int, object>();
        readonly GP_InterpolationEngine _playersInterp = new GP_InterpolationEngine(20);
        readonly GP_InterpolationEngine _globalInterp = new GP_InterpolationEngine(20);

        string _globalState = "{}";
        string _myState = "{}";
        string _playerSchema = "{}";
        string _globalSchema = "{}";
        int _tickRate = 20;
        int _bufferMinMs = 100;
        int _bufferMaxMs = 300;
        int _lastPlayersSeq = -1;
        int _lastGlobalSeq = -1;
        float _lastSnapshotRequestAt;
        bool _playersHaveBase;
        string _lastFullPlayersJson;
        string _lastFullGlobalJson;
        object _previousInterpolatedPlayers;
        object _previousInterpolatedGlobal;
        int _seqGapCount;
        int _snapshotRequestCount;

        public IReadOnlyDictionary<int, string> PlayerStates => _playerStates;
        public string MyState => _myState;
        public string GlobalState => _globalState;
        public string PlayerSchema => _playerSchema;
        public string GlobalSchema => _globalSchema;
        public int TickRate => _tickRate;
        public int BufferMinMs => _bufferMinMs;
        public int BufferMaxMs => _bufferMaxMs;
        public GP_InterpolationEngine PlayersInterp => _playersInterp;

        public int GlobalTickDivider =>
            Math.Max(1, (int)Math.Round(_tickRate / 20f));

        public void SetMyState(string state) => _myState = string.IsNullOrEmpty(state) ? "{}" : state;

        public void SetPlayerState(int playerId, string state)
        {
            _playerStates[playerId] = string.IsNullOrEmpty(state) ? "{}" : state;
        }

        public bool HasPlayerState(int playerId) => _playerStates.ContainsKey(playerId);

        public bool TryGetPlayerState(int playerId, out string state) =>
            _playerStates.TryGetValue(playerId, out state);

        public void RemovePlayer(int playerId)
        {
            _playerStates.Remove(playerId);
            RemovePeerState(playerId);
        }

        public void RemovePeerState(int playerId)
        {
            _peerStates.Remove(playerId);
            _peerEngines.Remove(playerId);
            _lastPeerSample.Remove(playerId);
        }

        public void DefinePlayerSchema(string schema)
        {
            _playerSchema = schema ?? "{}";
            ApplySchemasToEngines();
        }

        public void DefineGlobalSchema(string schema)
        {
            _globalSchema = schema ?? "{}";
            ApplySchemasToEngines();
        }

        public void SetMode(int tickRate, int bufferMinMs, int bufferMaxMs)
        {
            _tickRate = Math.Max(1, tickRate);
            _bufferMinMs = bufferMinMs;
            _bufferMaxMs = bufferMaxMs;
            ApplySchemasToEngines();
        }

        public void SetGlobalState(string state)
        {
            _globalState = string.IsNullOrEmpty(state) ? "{}" : state;
        }

        public void ClearReceiveState()
        {
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
        }

        public void Clear()
        {
            _playerStates.Clear();
            _peerStates.Clear();
            _peerEngines.Clear();
            _lastPeerSample.Clear();
            _globalState = "{}";
            _myState = "{}";
            _seqGapCount = 0;
            _snapshotRequestCount = 0;
            ClearReceiveState();
        }

        public void HandleHostPlayersSnapshot(string payload, int seq, long timestamp)
        {
            ApplyPlayersMap(payload, keepLocalSelf: true);
            _playersHaveBase = true;
            _lastFullPlayersJson = payload;
            if (seq >= 0)
                _lastPlayersSeq = seq;
            PushPlayersBuffer(payload, timestamp);
            PlayersUpdated?.Invoke();
        }

        public void HandleHostPlayersDelta(string payload, int seq, long timestamp)
        {
            if (!_playersHaveBase || string.IsNullOrEmpty(_lastFullPlayersJson))
            {
                RequestSnapshot();
                return;
            }
            if (seq >= 0 && _lastPlayersSeq >= 0 && seq != _lastPlayersSeq + 1)
            {
                _seqGapCount++;
                _playersHaveBase = false;
                _lastFullPlayersJson = null;
                _lastPlayersSeq = -1;
                RequestSnapshot();
                return;
            }
            _lastFullPlayersJson = GpJson.MergeDelta(_lastFullPlayersJson, payload);
            ApplyPlayersMap(_lastFullPlayersJson, keepLocalSelf: true);
            if (seq >= 0)
                _lastPlayersSeq = seq;
            PushPlayersBuffer(_lastFullPlayersJson, timestamp);
            PlayersUpdated?.Invoke();
        }

        public void HandleHostGlobalSnapshot(string payload, int seq, long timestamp)
        {
            _globalState = string.IsNullOrEmpty(payload) ? "{}" : payload;
            _lastFullGlobalJson = _globalState;
            if (seq >= 0)
                _lastGlobalSeq = seq;
            PushGlobalBuffer(_globalState, timestamp);
            GlobalStateUpdated?.Invoke(_globalState);
        }

        public void HandleHostGlobalDelta(string payload, int seq, long timestamp)
        {
            if (string.IsNullOrEmpty(_lastFullGlobalJson) || _lastFullGlobalJson == "{}")
            {
                RequestSnapshot();
                return;
            }
            if (seq >= 0 && _lastGlobalSeq >= 0 && seq != _lastGlobalSeq + 1)
            {
                _seqGapCount++;
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
            GlobalStateUpdated?.Invoke(_globalState);
        }

        public void HandlePeerState(int senderId, string payload, long timestamp)
        {
            _peerStates.TryGetValue(senderId, out var prev);
            var incoming = GP_NativeSchema.FilterReadonly(payload, prev ?? "{}", _playerSchema);
            var merged = GpJson.MergeDelta(prev, incoming);
            _peerStates[senderId] = merged;
            var engine = GetOrCreatePeerEngine(senderId);
            var tree = GpJson.Parse(merged);
            if (tree != null)
                engine.AddStateWithGapFill(timestamp, tree, 1000.0 / Math.Max(1, _tickRate));
        }

        public void SamplePeerStates(double nowMs)
        {
            if (_peerEngines.Count == 0)
                return;
            foreach (var pair in _peerEngines)
            {
                var sampled = pair.Value.Interpolate(nowMs);
                if (sampled == null)
                    continue;
                _lastPeerSample.TryGetValue(pair.Key, out var previous);
                if (ReferenceEquals(sampled, previous))
                    continue;
                _lastPeerSample[pair.Key] = sampled;
                var json = GpJson.Stringify(sampled);
                PeerStateChanged?.Invoke(pair.Key, json);
            }
        }

        public bool SampleInterpolation(double nowMs)
        {
            var changed = false;
            var interpolated = _playersInterp.Interpolate(nowMs);
            if (interpolated != null && !ReferenceEquals(interpolated, _previousInterpolatedPlayers))
            {
                _previousInterpolatedPlayers = interpolated;
                if (ApplyInterpolatedPlayers(interpolated))
                {
                    PlayersUpdated?.Invoke();
                    changed = true;
                }
            }

            var interpolatedGlobal = _globalInterp.Interpolate(nowMs);
            if (interpolatedGlobal != null && !ReferenceEquals(interpolatedGlobal, _previousInterpolatedGlobal))
            {
                _previousInterpolatedGlobal = interpolatedGlobal;
                _globalState = GpJson.Stringify(interpolatedGlobal);
                GlobalStateUpdated?.Invoke(_globalState);
                changed = true;
            }
            return changed;
        }

        public string PlayersStateJson()
        {
            var sb = new StringBuilder();
            sb.Append("{\"players\":[");
            var first = true;
            foreach (var pair in _playerStates)
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

        public string BuildPlayersPayload()
        {
            var sb = new StringBuilder();
            sb.Append("{\"players\":{");
            var first = true;
            foreach (var pair in _playerStates)
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

        public string NetworkStatsJson(int ping) =>
            "{\"ping\":" + ping +
            ",\"bufferSize\":" + _playersInterp.BufferSize +
            ",\"bufferDelay\":" + _playersInterp.BufferDelay.ToString("0.###", CultureInfo.InvariantCulture) + "}";

        public string InterpolationStatsJson() =>
            "{\"seqGapCount\":" + _seqGapCount +
            ",\"snapshotRequestCount\":" + _snapshotRequestCount +
            ",\"bufferSize\":" + _playersInterp.BufferSize +
            ",\"bufferDelay\":" + _playersInterp.BufferDelay.ToString("0.###", CultureInfo.InvariantCulture) + "}";

        void ApplyPlayersMap(string payload, bool keepLocalSelf)
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
                if (keepLocalSelf && id == NativePlayer.Id && _playerStates.ContainsKey(id)
                    && !string.IsNullOrEmpty(_myState) && _myState != "{}")
                {
                    _playerStates[id] = _myState;
                    continue;
                }
                _playerStates[id] = state;
            }
        }

        bool ApplyInterpolatedPlayers(object interpolated)
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
                    _playerStates[id] = _myState;
                    continue;
                }
                var json = GpJson.Stringify(pair.Value);
                if (!_playerStates.TryGetValue(id, out var current)
                    || !string.Equals(current, json, StringComparison.Ordinal))
                    changed = true;
                _playerStates[id] = json;
            }
            return changed;
        }

        void PushPlayersBuffer(string payload, long timestamp)
        {
            var tree = GpJson.Parse(payload);
            if (tree == null)
                return;
            _playersInterp.AddStateWithGapFill(timestamp, tree, 1000.0 / Math.Max(1, _tickRate));
        }

        void PushGlobalBuffer(string payload, long timestamp)
        {
            var tree = GpJson.Parse(payload);
            if (tree == null)
                return;
            _globalInterp.AddStateWithGapFill(
                timestamp, tree, (1000.0 / Math.Max(1, _tickRate)) * GlobalTickDivider);
        }

        GP_InterpolationEngine GetOrCreatePeerEngine(int playerId)
        {
            if (_peerEngines.TryGetValue(playerId, out var engine))
                return engine;
            engine = new GP_InterpolationEngine(_tickRate);
            engine.SetSchema(GpJson.ParseObject(_playerSchema));
            engine.SetBufferLimits(_bufferMinMs, _bufferMaxMs);
            _peerEngines[playerId] = engine;
            return engine;
        }

        void ApplySchemasToEngines()
        {
            _playersInterp.SetTickRate(_tickRate);
            _globalInterp.SetTickRate(Math.Max(1, _tickRate / Math.Max(1, GlobalTickDivider)));
            _playersInterp.SetBufferLimits(_bufferMinMs, _bufferMaxMs);
            _globalInterp.SetBufferLimits(_bufferMinMs, _bufferMaxMs);
            _playersInterp.SetSchema(GpJson.ParseObject(_playerSchema));
            _globalInterp.SetSchema(GpJson.ParseObject(_globalSchema));
            foreach (var pair in _peerEngines)
            {
                pair.Value.SetTickRate(_tickRate);
                pair.Value.SetBufferLimits(_bufferMinMs, _bufferMaxMs);
                pair.Value.SetSchema(GpJson.ParseObject(_playerSchema));
            }
        }

        void RequestSnapshot()
        {
            var now = Time.realtimeSinceStartup;
            if (now - _lastSnapshotRequestAt < SnapshotRequestThrottle)
                return;
            _lastSnapshotRequestAt = now;
            _snapshotRequestCount++;
            SnapshotRequest?.Invoke();
        }
    }
}
