using System;
using System.Collections.Generic;

namespace GamePush.Native
{
    public sealed class GP_InterpolationEngine
    {
        const int MaxBufferSize = 30;
        const float DefaultBufferTime = 150f;
        const float MaxExtrapolationTime = 100f;
        const float HardSnapThresholdMs = 250f;
        const float MaxSlewRate = 0.05f;
        const float SlewRangeMs = 200f;
        const float MaxFrameDtMs = 100f;
        const float JitterSmoothing = 0.1f;
        const int MaxFieldDecisionCache = 1000;

        struct BufferedState
        {
            public double Timestamp;
            public object State;
        }

        readonly List<BufferedState> _buffer = new List<BufferedState>();
        readonly Dictionary<string, bool> _fieldDecisionCache = new Dictionary<string, bool>();
        Dictionary<string, object> _schema;
        float _adaptiveBufferTime = DefaultBufferTime;
        float _minBufferTime = 100f;
        float _maxBufferTime = 300f;
        float _jitterEstimate;
        int _tickRate;
        object _lastValidState;
        object _lastOutputState;
        object _lastRawInput;
        double _lastFrameTime;
        bool _hasOutput;
        double _timeSinceRawChange;
        double? _playbackTime;
        double? _lastFrameAt;
        double _lastStatsTimestamp;
        double _lastStatsReceivedAt;
        bool _hasStatsRef;

        public GP_InterpolationEngine(int tickRate)
        {
            _tickRate = Math.Max(1, tickRate);
        }

        public int BufferSize => _buffer.Count;

        public float BufferDelay
        {
            get
            {
                if (_buffer.Count < 2)
                    return 0f;
                return (float)(_buffer[_buffer.Count - 1].Timestamp - _buffer[0].Timestamp);
            }
        }

        public void SetSchema(Dictionary<string, object> schema)
        {
            _schema = schema;
            _fieldDecisionCache.Clear();
        }

        public void SetTickRate(int tickRate)
        {
            _tickRate = Math.Max(1, tickRate);
        }

        public void SetBufferLimits(float min, float max)
        {
            _minBufferTime = min;
            _maxBufferTime = max;
            _adaptiveBufferTime = Math.Min(max, Math.Max(min, _adaptiveBufferTime));
        }

        public void AddStateWithGapFill(double timestamp, object state, double tickIntervalMs)
        {
            if (_buffer.Count > 0)
            {
                var last = _buffer[_buffer.Count - 1];
                var gap = timestamp - last.Timestamp;
                if (gap > tickIntervalMs * 1.5)
                    InsertState(timestamp - tickIntervalMs, last.State, false);
            }
            InsertState(timestamp, state, true);
        }

        public void AddState(double timestamp, object state) => InsertState(timestamp, state, true);

        public object Interpolate(double currentTime)
        {
            if (_buffer.Count == 0)
                return _lastValidState;

            var newestTimestamp = _buffer[_buffer.Count - 1].Timestamp;
            var target = newestTimestamp - _adaptiveBufferTime;
            var stopConfirmed = _buffer.Count >= 2
                                && ReferenceEquals(_buffer[_buffer.Count - 1].State, _buffer[_buffer.Count - 2].State);
            var renderTime = AdvancePlaybackClock(currentTime, target, newestTimestamp, stopConfirmed);
            CleanOldStates(renderTime);
            if (_buffer.Count == 0)
                return _lastValidState;
            if (_buffer.Count == 1)
            {
                _lastValidState = _buffer[0].State;
                return SmoothOutput(_buffer[0].State, currentTime);
            }

            FindInterpolationStates(renderTime, out var prev, out var next);
            if (prev == null && next == null)
                return _lastValidState;
            if (prev == null)
            {
                _lastValidState = next.Value.State;
                return SmoothOutput(next.Value.State, currentTime);
            }
            if (next == null)
            {
                var extrapolated = Extrapolate(prev.Value, renderTime);
                _lastValidState = extrapolated;
                return SmoothOutput(extrapolated, currentTime);
            }

            var result = InterpolateBetween(prev.Value, next.Value, renderTime);
            _lastValidState = result;
            return SmoothOutput(result, currentTime);
        }

        public void Clear()
        {
            _buffer.Clear();
            _lastValidState = null;
            _lastOutputState = null;
            _lastRawInput = null;
            _lastFrameTime = 0;
            _hasOutput = false;
            _timeSinceRawChange = 0;
            _adaptiveBufferTime = DefaultBufferTime;
            _jitterEstimate = 0;
            _hasStatsRef = false;
            _playbackTime = null;
            _lastFrameAt = null;
            _fieldDecisionCache.Clear();
        }

        void InsertState(double timestamp, object state, bool updateStats)
        {
            var receivedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (updateStats)
            {
                if (_hasStatsRef)
                {
                    var expectedDelta = timestamp - _lastStatsTimestamp;
                    var maxNormalDelta = (1000.0 / _tickRate) * 3;
                    if (expectedDelta > 0 && expectedDelta <= maxNormalDelta)
                    {
                        var actualDelta = receivedAt - _lastStatsReceivedAt;
                        var jitter = Math.Abs(actualDelta - expectedDelta);
                        _jitterEstimate = _jitterEstimate * (1f - JitterSmoothing) + (float)jitter * JitterSmoothing;
                        _adaptiveBufferTime = Math.Min(_maxBufferTime,
                            Math.Max(_minBufferTime, _jitterEstimate * 2f + 50f));
                    }
                }
                _lastStatsTimestamp = timestamp;
                _lastStatsReceivedAt = receivedAt;
                _hasStatsRef = true;
            }

            var entry = new BufferedState { Timestamp = timestamp, State = state };
            var low = 0;
            var high = _buffer.Count;
            while (low < high)
            {
                var mid = (low + high) >> 1;
                if (_buffer[mid].Timestamp < timestamp)
                    low = mid + 1;
                else
                    high = mid;
            }
            _buffer.Insert(low, entry);
            if (_buffer.Count > MaxBufferSize)
                _buffer.RemoveAt(0);
        }

        double AdvancePlaybackClock(double currentTime, double target, double newestTimestamp, bool stopConfirmed)
        {
            if (_playbackTime == null || _lastFrameAt == null)
            {
                _playbackTime = target;
                _lastFrameAt = currentTime;
                return target;
            }

            var dt = Math.Max(0, Math.Min(currentTime - _lastFrameAt.Value, MaxFrameDtMs));
            _lastFrameAt = currentTime;
            var diff = target - _playbackTime.Value;
            if (Math.Abs(diff) > HardSnapThresholdMs)
                _playbackTime = target;
            else
            {
                var correctionRate = Math.Max(-MaxSlewRate, Math.Min(MaxSlewRate, (float)(diff / SlewRangeMs)));
                _playbackTime += dt * (1 + correctionRate);
            }

            var maxCursor = stopConfirmed ? newestTimestamp : newestTimestamp + MaxExtrapolationTime;
            if (_playbackTime > maxCursor)
                _playbackTime = maxCursor;
            return _playbackTime.Value;
        }

        void CleanOldStates(double renderTime)
        {
            while (_buffer.Count > 2)
            {
                if (_buffer[1].Timestamp <= renderTime)
                {
                    _lastValidState = _buffer[0].State;
                    _buffer.RemoveAt(0);
                }
                else
                    break;
            }
        }

        void FindInterpolationStates(double renderTime, out BufferedState? prev, out BufferedState? next)
        {
            prev = null;
            next = null;
            for (var i = 0; i < _buffer.Count; i++)
            {
                var state = _buffer[i];
                if (state.Timestamp <= renderTime)
                    prev = state;
                else
                {
                    next = state;
                    break;
                }
            }
        }

        object InterpolateBetween(BufferedState prev, BufferedState next, double renderTime)
        {
            var timeDiff = next.Timestamp - prev.Timestamp;
            if (timeDiff == 0)
                return prev.State;
            var t = (renderTime - prev.Timestamp) / timeDiff;
            if (t <= 0)
                return prev.State;
            if (t >= 1)
                return next.State;
            return InterpolateState(prev.State, next.State, t, "");
        }

        object Extrapolate(BufferedState last, double renderTime)
        {
            var timeSinceLastUpdate = renderTime - last.Timestamp;
            if (timeSinceLastUpdate <= 0 || timeSinceLastUpdate >= MaxExtrapolationTime)
                return last.State;
            var lastIndex = -1;
            for (var i = 0; i < _buffer.Count; i++)
            {
                if (_buffer[i].Timestamp == last.Timestamp && ReferenceEquals(_buffer[i].State, last.State))
                {
                    lastIndex = i;
                    break;
                }
            }
            if (lastIndex > 0)
            {
                var prev = _buffer[lastIndex - 1];
                var dt = last.Timestamp - prev.Timestamp;
                if (dt > 0)
                {
                    var t = 1 + timeSinceLastUpdate / dt;
                    return ExtrapolateState(prev.State, last.State, t, "");
                }
            }
            return last.State;
        }

        object InterpolateState(object prevState, object nextState, double t, string parentPath)
        {
            if (prevState == null || nextState == null)
                return nextState ?? prevState;
            if (!(nextState is Dictionary<string, object> nextObj))
            {
                if (nextState is List<object> nextList && prevState is List<object> prevList)
                    return InterpolateList(prevList, nextList, t, parentPath);
                return nextState;
            }
            if (!(prevState is Dictionary<string, object> prevObj))
                return nextState;

            var result = new Dictionary<string, object>();
            foreach (var pair in nextObj)
            {
                prevObj.TryGetValue(pair.Key, out var prevValue);
                var fieldPath = ChildFieldPath(parentPath, pair.Key);
                var nextValue = pair.Value;
                var should = ShouldInterpolateField(fieldPath, nextValue);
                if (should && IsNumber(nextValue) && IsNumber(prevValue))
                {
                    var prevNum = GpJson.ToDouble(prevValue);
                    var nextNum = GpJson.ToDouble(nextValue);
                    result[pair.Key] = IsAngle(pair.Key)
                        ? InterpolateAngle(prevNum, nextNum, t)
                        : prevNum + (nextNum - prevNum) * t;
                }
                else if (nextValue is Dictionary<string, object> || nextValue is List<object>)
                    result[pair.Key] = InterpolateState(prevValue, nextValue, t, fieldPath);
                else
                    result[pair.Key] = nextValue;
            }
            return result;
        }

        List<object> InterpolateList(List<object> prevList, List<object> nextList, double t, string parentPath)
        {
            var result = new List<object>(nextList.Count);
            for (var i = 0; i < nextList.Count; i++)
            {
                var prevValue = i < prevList.Count ? prevList[i] : null;
                result.Add(InterpolateState(prevValue, nextList[i], t, parentPath));
            }
            return result;
        }

        object ExtrapolateState(object prevState, object lastState, double t, string parentPath)
        {
            if (prevState == null || lastState == null)
                return lastState ?? prevState;
            if (!(lastState is Dictionary<string, object> lastObj))
                return lastState;
            var prevObj = prevState as Dictionary<string, object>;
            var result = new Dictionary<string, object>();
            foreach (var pair in lastObj)
            {
                object prevValue = null;
                prevObj?.TryGetValue(pair.Key, out prevValue);
                var fieldPath = ChildFieldPath(parentPath, pair.Key);
                if (ShouldInterpolateField(fieldPath, pair.Value) && IsNumber(pair.Value) && IsNumber(prevValue))
                {
                    var lastNum = GpJson.ToDouble(pair.Value);
                    var prevNum = GpJson.ToDouble(prevValue);
                    var velocity = lastNum - prevNum;
                    var max = Math.Abs(velocity) * 2;
                    var extrapolated = lastNum + velocity * (t - 1);
                    result[pair.Key] = Math.Max(lastNum - max, Math.Min(lastNum + max, extrapolated));
                }
                else if (pair.Value is Dictionary<string, object>)
                    result[pair.Key] = ExtrapolateState(prevValue, pair.Value, t, fieldPath);
                else
                    result[pair.Key] = pair.Value;
            }
            return result;
        }

        object SmoothOutput(object rawState, double currentTime)
        {
            if (!_hasOutput)
            {
                _lastOutputState = rawState;
                _lastFrameTime = currentTime;
                _lastRawInput = rawState;
                _timeSinceRawChange = 0;
                _hasOutput = true;
                return rawState;
            }

            var deltaTime = currentTime - _lastFrameTime;
            if (deltaTime <= 0)
                return _lastOutputState;
            if (ReferenceEquals(rawState, _lastRawInput))
                _timeSinceRawChange += deltaTime;
            else
            {
                _lastRawInput = rawState;
                _timeSinceRawChange = 0;
            }

            var expectedTickInterval = 1000.0 / _tickRate;
            if (_buffer.Count >= 2)
            {
                var sampleCount = Math.Min(3, _buffer.Count);
                var startIdx = _buffer.Count - sampleCount;
                var totalInterval = _buffer[_buffer.Count - 1].Timestamp - _buffer[startIdx].Timestamp;
                expectedTickInterval = totalInterval / (sampleCount - 1);
            }

            if (_timeSinceRawChange >= expectedTickInterval)
            {
                _lastOutputState = rawState;
                _lastFrameTime = currentTime;
                return rawState;
            }

            var smoothFactor = Math.Min(1, (deltaTime * 2) / expectedTickInterval);
            if (smoothFactor >= 1)
            {
                _lastOutputState = rawState;
                _lastFrameTime = currentTime;
                return rawState;
            }

            var smoothed = LerpState(_lastOutputState, rawState, smoothFactor, "");
            _lastOutputState = smoothed;
            _lastFrameTime = currentTime;
            return smoothed;
        }

        object LerpState(object from, object to, double t, string parentPath)
        {
            if (from == null || to == null)
                return to ?? from;
            if (!(to is Dictionary<string, object> toObj) || !(from is Dictionary<string, object> fromObj))
                return to;
            var result = new Dictionary<string, object>();
            foreach (var pair in toObj)
            {
                fromObj.TryGetValue(pair.Key, out var fromValue);
                var fieldPath = ChildFieldPath(parentPath, pair.Key);
                if (ShouldInterpolateField(fieldPath, pair.Value) && IsNumber(pair.Value) && IsNumber(fromValue))
                {
                    var toNum = GpJson.ToDouble(pair.Value);
                    var fromNum = GpJson.ToDouble(fromValue);
                    var diff = toNum - fromNum;
                    result[pair.Key] = Math.Abs(diff) < 0.001 ? toNum : fromNum + diff * t;
                }
                else if (pair.Value is Dictionary<string, object>)
                    result[pair.Key] = LerpState(fromValue, pair.Value, t, fieldPath);
                else
                    result[pair.Key] = pair.Value;
            }
            return result;
        }

        bool ShouldInterpolateField(string fieldPath, object value)
        {
            if (_schema == null)
                return IsNumber(value);
            if (!IsNumber(value))
                return false;
            if (_fieldDecisionCache.TryGetValue(fieldPath, out var cached))
                return cached;
            var schemaField = GetSchemaField(fieldPath);
            var decision = true;
            if (schemaField != null && schemaField.TryGetValue("interpolate", out var flag) && flag is bool interp)
                decision = interp;
            if (_fieldDecisionCache.Count >= MaxFieldDecisionCache)
                _fieldDecisionCache.Clear();
            _fieldDecisionCache[fieldPath] = decision;
            return decision;
        }

        Dictionary<string, object> GetSchemaField(string fieldPath)
        {
            if (_schema == null || string.IsNullOrEmpty(fieldPath))
                return null;
            var parts = fieldPath.Split('.');
            var result = FindFieldByParts(_schema, parts);
            if (result != null)
                return result;
            for (var i = 1; i < parts.Length; i++)
            {
                var partial = new string[parts.Length - i];
                Array.Copy(parts, i, partial, 0, partial.Length);
                result = FindFieldByParts(_schema, partial);
                if (result != null)
                    return result;
            }
            return null;
        }

        static Dictionary<string, object> FindFieldByParts(Dictionary<string, object> schema, string[] parts)
        {
            object current = schema;
            foreach (var part in parts)
            {
                if (part == "interpolate")
                    return null;
                if (!(current is Dictionary<string, object> obj) || !obj.TryGetValue(part, out current))
                    return null;
            }
            return current as Dictionary<string, object>;
        }

        static string ChildFieldPath(string parentPath, string key)
        {
            if (key.Length > 0 && key[0] >= '0' && key[0] <= '9' && long.TryParse(key, out _))
                return parentPath;
            return string.IsNullOrEmpty(parentPath) ? key : parentPath + "." + key;
        }

        static bool IsNumber(object value)
        {
            return value is int || value is long || value is float || value is double;
        }

        static bool IsAngle(string key)
        {
            var lower = key.ToLowerInvariant();
            return lower.IndexOf("rotation", StringComparison.Ordinal) >= 0
                   || lower.IndexOf("angle", StringComparison.Ordinal) >= 0;
        }

        static double InterpolateAngle(double prev, double next, double t)
        {
            prev = ((prev % 360) + 360) % 360;
            next = ((next % 360) + 360) % 360;
            var diff = next - prev;
            if (diff > 180) diff -= 360;
            else if (diff < -180) diff += 360;
            return prev + diff * t;
        }
    }
}
