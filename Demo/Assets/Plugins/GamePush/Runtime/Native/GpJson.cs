using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GamePush.Native
{
    public static class GpJson
    {
        public static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            var sb = new StringBuilder(value.Length + 8);
            foreach (var c in value)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '"': sb.Append("\\\""); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    default:
                        if (c < 0x20)
                        {
                            sb.Append("\\u");
                            sb.Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        }
                        else
                            sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        public static string Quote(string value) => "\"" + Escape(value ?? "") + "\"";

        public static object Parse(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            var i = 0;
            SkipWs(json, ref i);
            if (i >= json.Length)
                return null;
            return ParseValue(json, ref i);
        }

        public static Dictionary<string, object> ParseObject(string json)
        {
            TryParseObject(json, out var obj);
            return obj;
        }

        public static bool TryParseObject(string json, out Dictionary<string, object> obj)
        {
            obj = Parse(json) as Dictionary<string, object>;
            return obj != null;
        }

        public static bool TryGetString(string json, string key, out string value)
        {
            value = null;
            if (!TryGetNode(json, key, out var node) || node is Dictionary<string, object> || node is List<object>)
                return false;
            if (node == null)
                return true;
            value = NodeToPlainString(node);
            return true;
        }

        public static int GetInt(string json, string key, int fallback = 0)
        {
            if (!TryGetNode(json, key, out var node) || node == null)
                return fallback;
            return ToInt(node, fallback);
        }

        public static float GetFloat(string json, string key, float fallback = 0)
        {
            if (!TryGetNode(json, key, out var node) || node == null)
                return fallback;
            return ToFloat(node, fallback);
        }

        public static long GetLong(string json, string key, long fallback = 0)
        {
            if (!TryGetNode(json, key, out var node) || node == null)
                return fallback;
            return ToLong(node, fallback);
        }

        public static bool GetBool(string json, string key, bool fallback = false)
        {
            if (!TryGetNode(json, key, out var node) || node == null)
                return fallback;
            if (node is bool flag)
                return flag;
            var raw = NodeToPlainString(node);
            return raw == "true" || raw == "True";
        }

        public static string GetObject(string json, string key)
        {
            if (!TryGetNode(json, key, out var node) || node == null)
                return null;
            if (node is Dictionary<string, object> || node is List<object>)
                return Stringify(node);
            if (node is string text)
                return Quote(text);
            return null;
        }

        public static bool IsEmptyObject(string json)
        {
            if (string.IsNullOrEmpty(json))
                return true;
            var parsed = Parse(json);
            if (parsed == null)
                return true;
            if (parsed is Dictionary<string, object> obj)
                return obj.Count == 0;
            return false;
        }

        public static bool TryGetRaw(string json, string key, out string raw)
        {
            raw = null;
            if (!TryGetNode(json, key, out var node))
                return false;
            raw = Stringify(node);
            return true;
        }

        public static string MergeDelta(string state, string delta)
        {
            if (IsEmptyObject(delta))
                return IsEmptyObject(state) ? "{}" : state;
            if (IsEmptyObject(state))
                return delta;
            var deltaNode = Parse(delta);
            if (deltaNode is List<object>)
                return delta;
            var stateObj = Parse(state) as Dictionary<string, object>;
            var deltaObj = deltaNode as Dictionary<string, object>;
            if (stateObj == null)
                return delta;
            if (deltaObj == null)
                return state;
            return Stringify(MergeObjects(stateObj, deltaObj));
        }

        public static string CalculateDelta(string prev, string next)
        {
            if (string.Equals(prev, next, StringComparison.Ordinal))
                return null;
            if (IsEmptyObject(next) && IsEmptyObject(prev))
                return null;
            if (IsEmptyObject(prev))
                return next;
            var delta = CalculateDeltaNode(Parse(prev), Parse(next));
            return delta == null ? null : Stringify(delta);
        }

        public static bool HasPartialChanges(string partial, string existing)
        {
            if (string.Equals(partial, existing, StringComparison.Ordinal))
                return false;
            if (IsEmptyObject(partial))
                return false;
            return HasPartialChangesNode(Parse(partial), Parse(existing));
        }

        static bool HasPartialChangesNode(object partial, object existing)
        {
            if (ReferenceEquals(partial, existing))
                return false;
            if (partial == null || existing == null)
                return !ReferenceEquals(partial, existing);
            if (partial is List<object> || existing is List<object>)
                return !string.Equals(Stringify(partial), Stringify(existing), StringComparison.Ordinal);
            if (!(partial is Dictionary<string, object> partialObj) ||
                !(existing is Dictionary<string, object> existingObj))
                return !Equals(partial, existing);
            foreach (var pair in partialObj)
            {
                existingObj.TryGetValue(pair.Key, out var current);
                if (HasPartialChangesNode(pair.Value, current))
                    return true;
            }
            return false;
        }

        public static List<string> GetObjectArray(string json, string key)
        {
            if (TryGetNode(json, key, out var node) && node is List<object> list)
                return NodesToArrayItems(list);
            return SplitArray(GetObject(json, key));
        }

        public static List<string> ObjectKeys(string json)
        {
            var keys = new List<string>();
            if (!TryParseObject(json, out var obj))
                return keys;
            keys.AddRange(obj.Keys);
            return keys;
        }

        public static List<string> SplitArray(string arrayJson)
        {
            var parsed = Parse(arrayJson) as List<object>;
            return parsed == null ? new List<string>() : NodesToArrayItems(parsed);
        }

        public static string ReplaceKey(string json, string from, string to)
        {
            if (string.IsNullOrEmpty(json))
                return json;
            return json.Replace("\"" + from + "\"", "\"" + to + "\"");
        }

        public static string SortedStringifyObject(Dictionary<string, object> source)
        {
            return StringifyObject(source, true);
        }

        public static string Stringify(object value)
        {
            return Stringify(value, false);
        }

        public static double ToDouble(object value, double fallback = 0)
        {
            switch (value)
            {
                case null:
                    return fallback;
                case bool flag:
                    return flag ? 1 : 0;
                case int i:
                    return i;
                case long l:
                    return l;
                case float f:
                    return f;
                case double d:
                    return d;
                case string text:
                    return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                        ? parsed
                        : fallback;
                default:
                    return fallback;
            }
        }

        static string Stringify(object value, bool sortKeys)
        {
            if (value == null)
                return "null";
            switch (value)
            {
                case string text:
                    return Quote(text);
                case bool flag:
                    return flag ? "true" : "false";
                case int number:
                    return number.ToString(CultureInfo.InvariantCulture);
                case float f:
                    return f.ToString("R", CultureInfo.InvariantCulture);
                case double d:
                    if (Math.Abs(d - Math.Round(d)) < 1e-9 && Math.Abs(d) < long.MaxValue)
                        return ((long)Math.Round(d)).ToString(CultureInfo.InvariantCulture);
                    return d.ToString("R", CultureInfo.InvariantCulture);
                case long l:
                    return l.ToString(CultureInfo.InvariantCulture);
                case GpRawJson raw:
                    return string.IsNullOrEmpty(raw.Json) ? "{}" : raw.Json;
                case Dictionary<string, object> obj:
                    return StringifyObject(obj, sortKeys);
                case IList<string> strings:
                    {
                        var sb = new StringBuilder();
                        sb.Append('[');
                        for (var i = 0; i < strings.Count; i++)
                        {
                            if (i > 0) sb.Append(',');
                            sb.Append(Quote(strings[i]));
                        }
                        sb.Append(']');
                        return sb.ToString();
                    }
                case List<object> list:
                    {
                        var sb = new StringBuilder();
                        sb.Append('[');
                        for (var i = 0; i < list.Count; i++)
                        {
                            if (i > 0) sb.Append(',');
                            sb.Append(Stringify(list[i], sortKeys));
                        }
                        sb.Append(']');
                        return sb.ToString();
                    }
                default:
                    if (value is Array array)
                    {
                        var sb = new StringBuilder();
                        sb.Append('[');
                        for (var i = 0; i < array.Length; i++)
                        {
                            if (i > 0) sb.Append(',');
                            sb.Append(Stringify(array.GetValue(i), sortKeys));
                        }
                        sb.Append(']');
                        return sb.ToString();
                    }
                    return Quote(value.ToString());
            }
        }

        static string StringifyObject(Dictionary<string, object> source, bool sortKeys)
        {
            if (source == null)
                return "{}";
            var keys = new List<string>(source.Keys);
            if (sortKeys)
                keys.Sort(StringComparer.Ordinal);
            var sb = new StringBuilder();
            sb.Append('{');
            for (var i = 0; i < keys.Count; i++)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append(Quote(keys[i]));
                sb.Append(':');
                sb.Append(Stringify(source[keys[i]], sortKeys));
            }
            sb.Append('}');
            return sb.ToString();
        }

        static Dictionary<string, object> MergeObjects(
            Dictionary<string, object> state, Dictionary<string, object> delta)
        {
            var map = new Dictionary<string, object>(state);
            foreach (var pair in delta)
            {
                if (IsDeleteSentinel(pair.Value))
                {
                    map.Remove(pair.Key);
                    continue;
                }
                if (pair.Value is Dictionary<string, object> deltaChild
                    && map.TryGetValue(pair.Key, out var current)
                    && current is Dictionary<string, object> stateChild)
                    map[pair.Key] = MergeObjects(stateChild, deltaChild);
                else
                    map[pair.Key] = pair.Value;
            }
            return map;
        }

        static object CalculateDeltaNode(object prev, object next)
        {
            if (EqualsNode(prev, next))
                return null;
            if (next is List<object> || prev is List<object>)
                return EqualsNode(prev, next) ? null : next;
            var nextObj = next as Dictionary<string, object>;
            var prevObj = prev as Dictionary<string, object>;
            if (nextObj == null || prevObj == null)
                return next;

            var delta = new Dictionary<string, object>();
            foreach (var pair in nextObj)
            {
                prevObj.TryGetValue(pair.Key, out var prevValue);
                var field = CalculateDeltaNode(prevValue, pair.Value);
                if (field != null)
                    delta[pair.Key] = field;
            }
            foreach (var pair in prevObj)
            {
                if (!nextObj.ContainsKey(pair.Key))
                    delta[pair.Key] = "__gp_del__";
            }
            return delta.Count == 0 ? null : delta;
        }

        static bool EqualsNode(object left, object right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (left == null || right == null)
                return false;
            if (left is Dictionary<string, object> || right is Dictionary<string, object>
                || left is List<object> || right is List<object>)
                return string.Equals(Stringify(left), Stringify(right), StringComparison.Ordinal);
            return string.Equals(NodeToPlainString(left), NodeToPlainString(right), StringComparison.Ordinal);
        }

        static bool IsDeleteSentinel(object value)
        {
            return value is string text && text == "__gp_del__";
        }

        static bool TryGetNode(string json, string key, out object node)
        {
            node = null;
            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(key))
                return false;
            if (!TryParseObject(json, out var obj))
                return false;
            return obj.TryGetValue(key, out node);
        }

        static List<string> NodesToArrayItems(List<object> list)
        {
            var items = new List<string>(list.Count);
            foreach (var item in list)
            {
                if (item is string text)
                    items.Add(text);
                else if (item is Dictionary<string, object> || item is List<object>)
                    items.Add(Stringify(item));
                else
                    items.Add(item == null ? "null" : NodeToPlainString(item));
            }
            return items;
        }

        static string NodeToPlainString(object node)
        {
            switch (node)
            {
                case string text:
                    return text;
                case bool flag:
                    return flag ? "true" : "false";
                case int i:
                    return i.ToString(CultureInfo.InvariantCulture);
                case long l:
                    return l.ToString(CultureInfo.InvariantCulture);
                case float f:
                    return f.ToString(CultureInfo.InvariantCulture);
                case double d:
                    return d.ToString(CultureInfo.InvariantCulture);
                default:
                    return node.ToString();
            }
        }

        static int ToInt(object node, int fallback)
        {
            switch (node)
            {
                case int i:
                    return i;
                case long l:
                    return (int)l;
                case float f:
                    return (int)f;
                case double d:
                    return (int)d;
                case string text:
                    return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                        ? parsed
                        : fallback;
                default:
                    return fallback;
            }
        }

        static float ToFloat(object node, float fallback)
        {
            return (float)ToDouble(node, fallback);
        }

        static long ToLong(object node, long fallback)
        {
            switch (node)
            {
                case int i:
                    return i;
                case long l:
                    return l;
                case float f:
                    return (long)f;
                case double d:
                    return (long)d;
                case string text:
                    return long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                        ? parsed
                        : fallback;
                default:
                    return fallback;
            }
        }

        static object ParseValue(string json, ref int i)
        {
            SkipWs(json, ref i);
            if (i >= json.Length)
                return null;
            var c = json[i];
            if (c == '{')
                return ParseObjectValue(json, ref i);
            if (c == '[')
                return ParseArray(json, ref i);
            if (c == '"')
            {
                i++;
                return ReadQuoted(json, i, out i);
            }
            if (Match(json, ref i, "null"))
                return null;
            if (Match(json, ref i, "true"))
                return true;
            if (Match(json, ref i, "false"))
                return false;
            return ParseNumber(json, ref i);
        }

        static Dictionary<string, object> ParseObjectValue(string json, ref int i)
        {
            var obj = new Dictionary<string, object>();
            i++;
            while (i < json.Length)
            {
                SkipWs(json, ref i);
                if (i >= json.Length)
                    break;
                if (json[i] == '}')
                {
                    i++;
                    break;
                }
                if (json[i] == ',')
                {
                    i++;
                    continue;
                }
                if (json[i] != '"')
                    break;
                i++;
                var key = ReadQuoted(json, i, out i);
                SkipWs(json, ref i);
                if (i >= json.Length || json[i] != ':')
                    break;
                i++;
                obj[key] = ParseValue(json, ref i);
            }
            return obj;
        }

        static List<object> ParseArray(string json, ref int i)
        {
            var list = new List<object>();
            i++;
            while (i < json.Length)
            {
                SkipWs(json, ref i);
                if (i >= json.Length)
                    break;
                if (json[i] == ']')
                {
                    i++;
                    break;
                }
                if (json[i] == ',')
                {
                    i++;
                    continue;
                }
                list.Add(ParseValue(json, ref i));
            }
            return list;
        }

        static object ParseNumber(string json, ref int i)
        {
            var start = i;
            if (i < json.Length && (json[i] == '-' || json[i] == '+'))
                i++;
            var isFloat = false;
            while (i < json.Length)
            {
                var c = json[i];
                if (c >= '0' && c <= '9')
                {
                    i++;
                    continue;
                }
                if (c == '.' || c == 'e' || c == 'E' || c == '+' || c == '-')
                {
                    isFloat = isFloat || c == '.' || c == 'e' || c == 'E';
                    i++;
                    continue;
                }
                break;
            }
            var raw = json.Substring(start, i - start);
            if (!isFloat && long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var whole))
                return whole;
            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                return number;
            return raw;
        }

        static bool Match(string json, ref int i, string token)
        {
            if (i + token.Length > json.Length)
                return false;
            if (string.Compare(json, i, token, 0, token.Length, StringComparison.Ordinal) != 0)
                return false;
            i += token.Length;
            return true;
        }

        static void SkipWs(string json, ref int i)
        {
            while (i < json.Length && char.IsWhiteSpace(json[i]))
                i++;
        }

        static string ReadQuoted(string json, int start, out int end)
        {
            var sb = new StringBuilder();
            var i = start;
            while (i < json.Length)
            {
                var c = json[i];
                if (c == '\\' && i + 1 < json.Length)
                {
                    i++;
                    var escaped = json[i];
                    switch (escaped)
                    {
                        case '"':
                        case '\\':
                        case '/':
                            sb.Append(escaped);
                            break;
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'u':
                            if (i + 4 < json.Length)
                            {
                                var hex = json.Substring(i + 1, 4);
                                if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                                        out var code))
                                    sb.Append((char)code);
                                i += 4;
                            }
                            break;
                        default:
                            sb.Append(escaped);
                            break;
                    }
                    i++;
                    continue;
                }
                if (c == '"')
                {
                    end = i + 1;
                    return sb.ToString();
                }
                sb.Append(c);
                i++;
            }
            end = json.Length;
            return sb.ToString();
        }
    }

    public sealed class GpRawJson
    {
        public string Json;

        public GpRawJson(string json)
        {
            Json = json;
        }
    }
}
