using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GamePush.Examples.Parity
{
    public class GP_ParityBridge : MonoBehaviour
    {
        private static readonly string[] ChannelsSimpleEvents =
        {
            "openChat",
            "closeChat",
            "join",
            "cancelJoin",
            "leave",
            "kick",
            "mute",
            "unmute",
            "sendInvite",
            "cancelInvite",
            "acceptInvite",
            "rejectInvite",
            "acceptJoinRequest",
            "rejectJoinRequest",
            "deleteChannel",
            "deleteMessage"
        };

        private static readonly string[] ChannelsDataEvents =
        {
            "createChannel",
            "error:createChannel",
            "updateChannel",
            "error:updateChannel",
            "event:updateChannel",
            "event:deleteChannel",
            "error:deleteChannel",
            "event:connect",
            "fetchChannel",
            "error:fetchChannel",
            "fetchPersonalChannel",
            "error:fetchPersonalChannel",
            "fetchFeedChannel",
            "error:fetchFeedChannel",
            "fetchChannels",
            "error:fetchChannels",
            "fetchMoreChannels",
            "error:fetchMoreChannels",
            "error:openChat",
            "error:openFeed",
            "event:join",
            "error:join",
            "event:joinRequest",
            "event:cancelJoin",
            "error:cancelJoin",
            "event:leave",
            "error:leave",
            "error:kick",
            "setValue",
            "error:setValue",
            "event:changeValue",
            "fetchMembers",
            "error:fetchMembers",
            "fetchMoreMembers",
            "error:fetchMoreMembers",
            "event:mute",
            "error:mute",
            "event:unmute",
            "error:unmute",
            "error:sendInvite",
            "event:invite",
            "event:cancelInvite",
            "error:cancelInvite",
            "event:acceptInvite",
            "error:acceptInvite",
            "event:rejectInvite",
            "error:rejectInvite",
            "fetchInvites",
            "error:fetchInvites",
            "fetchMoreInvites",
            "error:fetchMoreInvites",
            "fetchChannelInvites",
            "error:fetchChannelInvites",
            "fetchMoreChannelInvites",
            "error:fetchMoreChannelInvites",
            "fetchSentInvites",
            "error:fetchSentInvites",
            "fetchMoreSentInvites",
            "error:fetchMoreSentInvites",
            "error:acceptJoinRequest",
            "event:rejectJoinRequest",
            "error:rejectJoinRequest",
            "fetchJoinRequests",
            "error:fetchJoinRequests",
            "fetchMoreJoinRequests",
            "error:fetchMoreJoinRequests",
            "fetchSentJoinRequests",
            "error:fetchSentJoinRequests",
            "fetchMoreSentJoinRequests",
            "error:fetchMoreSentJoinRequests",
            "sendMessage",
            "error:sendMessage",
            "event:message",
            "editMessage",
            "event:editMessage",
            "error:editMessage",
            "event:deleteMessage",
            "error:deleteMessage",
            "fetchMessages",
            "error:fetchMessages",
            "fetchPersonalMessages",
            "error:fetchPersonalMessages",
            "fetchFeedMessages",
            "error:fetchFeedMessages",
            "fetchMoreMessages",
            "error:fetchMoreMessages",
            "fetchMorePersonalMessages",
            "error:fetchMorePersonalMessages",
            "fetchMoreFeedMessages",
            "error:fetchMoreFeedMessages"
        };

        private static readonly string[] MultiplayerSimpleEvents =
        {
            "becameHost",
            "becamePeer"
        };

        private static readonly string[] MultiplayerDataEvents =
        {
            "connect",
            "disconnect",
            "error:connect",
            "error:disconnect",
            "error:sendState",
            "playerJoined",
            "playerLeft",
            "playersUpdated",
            "customEvent",
            "hostMigrated"
        };

        private static readonly UnityEngine.Events.UnityAction<GP_Data> MultiplayerMessageProbeHandler = payload =>
        {
            _multiplayerMessageProbeHits++;
            _lastMultiplayerMessageProbeData = payload?.Data ?? "null";
        };

        private static readonly UnityEngine.Events.UnityAction<float> MultiplayerTickProbeHandler = delta =>
        {
            _multiplayerTickProbeHits++;
            _lastMultiplayerTickProbeDelta = delta;
        };

        private static bool _multiplayerMessageProbeEnabled;
        private static bool _multiplayerTickProbeEnabled;
        private static int _multiplayerMessageProbeHits;
        private static int _multiplayerTickProbeHits;
        private static string _lastMultiplayerMessageProbeData = "null";
        private static float _lastMultiplayerTickProbeDelta;

        [DllImport("__Internal")]
        private static extern void GP_Parity_Report(string data);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateBridge()
        {
            if (FindAnyObjectByType<GP_ParityBridge>() != null)
                return;

            GameObject instance = new GameObject(nameof(GP_ParityBridge));
            DontDestroyOnLoad(instance);
            instance.AddComponent<GP_ParityBridge>();
        }

        private void Awake()
        {
            SubscribeChannels();
            SubscribeMultiplayer();
        }

        private void Start()
        {
            Report("ready", "system", "ready", "{\"scene\":\"" + EscapeJsonLiteral(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name) + "\"}");
        }

        public void RunCommand(string json)
        {
            ParityCommand command;

            try
            {
                command = ParseCommand(json);
            }
            catch (Exception exception)
            {
                ReportError("invalidCommand", null, json, exception.Message);
                return;
            }

            if (command == null || string.IsNullOrEmpty(command.module) || string.IsNullOrEmpty(command.member))
            {
                ReportError("invalidCommand", null, json, "Command must contain module and member");
                return;
            }

            try
            {
                Type targetType = GetTargetType(command.module);
                object result = ExecuteCommand(targetType, command);
                ReportCommandResult(command, result);
            }
            catch (Exception exception)
            {
                string message = exception.InnerException?.Message ?? exception.Message;
                ReportError(command.member, command.module, json, message, command.requestId);
            }
        }

        private async void ReportCommandResult(ParityCommand command, object result)
        {
            try
            {
                if (result is Task task)
                {
                    await task;
                    result = ExtractTaskResult(task);
                }

                Report(
                    "result",
                    command.module,
                    command.member,
                    SerializeValue(result),
                    command.requestId);
            }
            catch (Exception exception)
            {
                string message = exception.InnerException?.Message ?? exception.Message;
                ReportError(command.member, command.module, null, message, command.requestId);
            }
        }

        private static object ExtractTaskResult(Task task)
        {
            if (task == null)
                return null;

            Type taskType = task.GetType();
            if (!taskType.IsGenericType)
                return null;

            PropertyInfo resultProperty = taskType.GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
            return resultProperty?.GetValue(task);
        }

        private static ParityCommand ParseCommand(string json)
        {
            JObject source = JObject.Parse(json);
            JArray args = source["args"] as JArray;
            string[] parsedArgs = null;

            if (args != null)
            {
                parsedArgs = new string[args.Count];

                for (int index = 0; index < args.Count; index++)
                {
                    JToken token = args[index];
                    parsedArgs[index] = token == null || token.Type == JTokenType.Null
                        ? null
                        : token.Type == JTokenType.String
                            ? token.Value<string>()
                            : token.ToString(Formatting.None);
                }
            }

            return new ParityCommand
            {
                requestId = source.Value<string>("requestId"),
                module = source.Value<string>("module"),
                member = source.Value<string>("member"),
                kind = source.Value<string>("kind"),
                args = parsedArgs
            };
        }

        public void Ping(string requestId)
        {
            Report("pong", "system", "ping", "\"ok\"", requestId);
        }

        private static Type GetTargetType(string module)
        {
            switch (module)
            {
                case "channels":
                    return typeof(GP_Channels);
                case "multiplayer":
                    return typeof(GP_Multiplayer);
                case "parity":
                    return typeof(GP_ParityBridge);
                default:
                    throw new InvalidOperationException("Unsupported module: " + module);
            }
        }

        private static object ExecuteCommand(Type targetType, ParityCommand command)
        {
            if (command.kind == "get")
            {
                PropertyInfo property = targetType.GetProperty(
                    command.member,
                    BindingFlags.Public | BindingFlags.Static);

                if (property == null)
                    throw new MissingMemberException(targetType.Name, command.member);

                return property.GetValue(null);
            }

            MethodInfo method = ResolveMethod(targetType, command.member, command.args);

            if (method == null)
                throw new MissingMethodException(targetType.Name, command.member);

            ParameterInfo[] parameters = method.GetParameters();
            object[] invokeArgs = new object[parameters.Length];

            for (int index = 0; index < parameters.Length; index++)
            {
                string rawArgument = command.args != null && index < command.args.Length
                    ? command.args[index]
                    : null;

                invokeArgs[index] = DeserializeArgument(parameters[index], rawArgument);
            }

            return method.Invoke(null, invokeArgs);
        }

        private static MethodInfo ResolveMethod(Type targetType, string member, string[] args)
        {
            int providedArgs = args?.Length ?? 0;
            MethodInfo bestMatch = null;
            int bestScore = int.MinValue;

            foreach (MethodInfo candidate in targetType.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (candidate.Name != member)
                    continue;

                ParameterInfo[] parameters = candidate.GetParameters();
                int requiredCount = 0;

                foreach (ParameterInfo parameter in parameters)
                {
                    if (!parameter.IsOptional)
                        requiredCount++;
                }

                if (providedArgs < requiredCount || providedArgs > parameters.Length)
                    continue;

                bool canHandle = true;

                for (int index = 0; index < providedArgs; index++)
                {
                    if (!CanDeserialize(parameters[index].ParameterType))
                    {
                        canHandle = false;
                        break;
                    }
                }

                if (!canHandle)
                    continue;

                int score = 0;
                for (int index = 0; index < providedArgs; index++)
                    score += GetDeserializationScore(parameters[index].ParameterType, args[index]);

                if (bestMatch == null ||
                    score > bestScore ||
                    (score == bestScore && parameters.Length < bestMatch.GetParameters().Length))
                {
                    bestMatch = candidate;
                    bestScore = score;
                }
            }

            return bestMatch;
        }

        private static int GetDeserializationScore(Type parameterType, string rawValue)
        {
            if (typeof(Delegate).IsAssignableFrom(parameterType))
                return 0;

            if (string.IsNullOrEmpty(rawValue))
                return parameterType.IsValueType ? 0 : 1;

            string trimmed = rawValue.Trim();
            bool isJsonObject = trimmed.StartsWith("{") && trimmed.EndsWith("}");
            bool isJsonArray = trimmed.StartsWith("[") && trimmed.EndsWith("]");
            bool isJsonContainer = isJsonObject || isJsonArray;

            if (parameterType == typeof(string))
                return isJsonContainer ? 1 : 4;

            if (parameterType == typeof(int))
                return int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out _) ? 5 : 0;

            if (parameterType == typeof(float))
                return float.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out _) ? 5 : 0;

            if (parameterType == typeof(bool))
                return rawValue == "true" || rawValue == "True" || rawValue == "false" || rawValue == "False" ? 5 : 0;

            if (parameterType == typeof(GP_Data))
                return isJsonContainer ? 5 : 2;

            if (parameterType.IsClass || parameterType.IsValueType)
                return isJsonContainer ? 6 : 1;

            return 0;
        }

        private static bool CanDeserialize(Type parameterType)
        {
            if (typeof(Delegate).IsAssignableFrom(parameterType))
                return false;

            return parameterType == typeof(string) ||
                   parameterType == typeof(int) ||
                   parameterType == typeof(float) ||
                   parameterType == typeof(bool) ||
                   parameterType == typeof(GP_Data) ||
                   parameterType.IsClass ||
                   parameterType.IsValueType;
        }

        private static object DeserializeArgument(ParameterInfo parameter, string rawValue)
        {
            if (rawValue == null)
            {
                if (parameter.IsOptional)
                    return parameter.DefaultValue;

                if (!parameter.ParameterType.IsValueType || Nullable.GetUnderlyingType(parameter.ParameterType) != null)
                    return null;
            }

            Type parameterType = parameter.ParameterType;

            if (parameterType == typeof(string))
                return rawValue;

            if (parameterType == typeof(int))
                return int.Parse(rawValue ?? "0", CultureInfo.InvariantCulture);

            if (parameterType == typeof(float))
                return float.Parse(rawValue ?? "0", CultureInfo.InvariantCulture);

            if (parameterType == typeof(bool))
                return rawValue == "true" || rawValue == "True";

            if (parameterType == typeof(GP_Data))
                return new GP_Data(string.IsNullOrEmpty(rawValue) ? "null" : rawValue);

            if (parameterType == typeof(ChannelStateValueQuery))
                return JsonConvert.DeserializeObject(rawValue, parameterType);

            if (string.IsNullOrEmpty(rawValue))
                return null;

            return JsonUtility.FromJson(rawValue, parameterType);
        }

        private static string SerializeValue(object value)
        {
            if (value == null)
                return "null";

            if (ReferenceEquals(value, GP_Undefined.Value))
                return "undefined";

            if (value is GP_Data data)
                return string.IsNullOrEmpty(data.Data) ? "null" : data.Data;

            if (value is bool booleanValue)
                return booleanValue ? "true" : "false";

            if (value is int || value is float || value is double || value is long)
                return Convert.ToString(value, CultureInfo.InvariantCulture);

            if (value is string stringValue)
                return "\"" + EscapeJsonLiteral(stringValue) + "\"";

            return JsonConvert.SerializeObject(value);
        }

        private void SubscribeChannels()
        {
            foreach (string eventName in ChannelsSimpleEvents)
            {
                string capturedEvent = eventName;
                GP_Channels.on(capturedEvent, _ => Report("event", "channels", capturedEvent, "null"));
            }

            foreach (string eventName in ChannelsDataEvents)
            {
                string capturedEvent = eventName;
                GP_Channels.on(capturedEvent, payload =>
                    Report("event", "channels", capturedEvent, payload?.Data ?? "null"));
            }
        }

        private void SubscribeMultiplayer()
        {
            foreach (string eventName in MultiplayerSimpleEvents)
            {
                string capturedEvent = eventName;
                GP_Multiplayer.on(capturedEvent, () => Report("event", "multiplayer", capturedEvent, "null"));
            }

            foreach (string eventName in MultiplayerDataEvents)
            {
                string capturedEvent = eventName;
                GP_Multiplayer.on(capturedEvent, payload =>
                    Report("event", "multiplayer", capturedEvent, payload?.Data ?? "null"));
            }

            GP_Multiplayer.onTick(delta =>
                Report(
                    "event",
                    "multiplayer",
                    "tick",
                    delta.ToString(CultureInfo.InvariantCulture)));
        }

        public static void resetProbes()
        {
            _multiplayerMessageProbeHits = 0;
            _multiplayerTickProbeHits = 0;
            _lastMultiplayerMessageProbeData = "null";
            _lastMultiplayerTickProbeDelta = 0f;
        }

        public static void enableMessageProbe()
        {
            if (_multiplayerMessageProbeEnabled)
                return;

            GP_Multiplayer.onMessage(MultiplayerMessageProbeHandler);
            _multiplayerMessageProbeEnabled = true;
        }

        public static void disableMessageProbe()
        {
            if (!_multiplayerMessageProbeEnabled)
                return;

            GP_Multiplayer.offMessage(MultiplayerMessageProbeHandler);
            _multiplayerMessageProbeEnabled = false;
        }

        public static void enableTickProbe()
        {
            if (_multiplayerTickProbeEnabled)
                return;

            GP_Multiplayer.onTick(MultiplayerTickProbeHandler);
            _multiplayerTickProbeEnabled = true;
        }

        public static void disableTickProbe()
        {
            if (!_multiplayerTickProbeEnabled)
                return;

            GP_Multiplayer.offTick(MultiplayerTickProbeHandler);
            _multiplayerTickProbeEnabled = false;
        }

        public static Task enableDefaultPlayerInitializer()
        {
            return GP_Multiplayer.setPlayerInitializer(CreateDefaultPlayerState);
        }

        public static Task enableDefaultPlayerInitializerAsync()
        {
            return GP_Multiplayer.setPlayerInitializer(CreateDefaultPlayerStateAsync);
        }

        public static Task disablePlayerInitializer()
        {
            return GP_Multiplayer.setPlayerInitializer((Func<int, MultiplayerConnectedPlayerData, GP_Data>)null);
        }

        public static GP_Data getProbeState()
        {
            string payload =
                "{\"messageProbeEnabled\":" + (_multiplayerMessageProbeEnabled ? "true" : "false") +
                ",\"messageProbeHits\":" + _multiplayerMessageProbeHits.ToString(CultureInfo.InvariantCulture) +
                ",\"lastMessage\":" + (_lastMultiplayerMessageProbeData ?? "null") +
                ",\"tickProbeEnabled\":" + (_multiplayerTickProbeEnabled ? "true" : "false") +
                ",\"tickProbeHits\":" + _multiplayerTickProbeHits.ToString(CultureInfo.InvariantCulture) +
                ",\"lastTickDelta\":" + _lastMultiplayerTickProbeDelta.ToString(CultureInfo.InvariantCulture) +
                "}";

            return new GP_Data(payload);
        }

        private static GP_Data CreateDefaultPlayerState(int playerId, MultiplayerConnectedPlayerData player)
        {
            string payload =
                "{\"playerId\":" + playerId.ToString(CultureInfo.InvariantCulture) +
                ",\"isHost\":" + (player?.isHost == true ? "true" : "false") +
                ",\"from\":\"initializer\"}";

            return new GP_Data(payload);
        }

        private static Task<GP_Data> CreateDefaultPlayerStateAsync(int playerId, MultiplayerConnectedPlayerData player)
        {
            return Task.FromResult(CreateDefaultPlayerState(playerId, player));
        }

        private static void ReportError(
            string member,
            string module,
            string commandJson,
            string message,
            string requestId = null)
        {
            string payload =
                "{\"message\":\"" + EscapeJsonLiteral(message) + "\",\"command\":\"" +
                EscapeJsonLiteral(commandJson ?? string.Empty) + "\"}";

            Report("error", module ?? "system", member ?? "unknown", payload, requestId);
        }

        private static void Report(
            string type,
            string module,
            string member,
            string dataJson,
            string requestId = null)
        {
            ParityEnvelope envelope = new ParityEnvelope
            {
                type = type,
                module = module,
                member = member,
                requestId = requestId ?? string.Empty,
                data = dataJson ?? "null"
            };

            string json = JsonUtility.ToJson(envelope);

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Parity_Report(json);
#else
            Debug.Log("[GP_PARITY] " + json);
#endif
        }

        private static string EscapeJsonLiteral(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        [Serializable]
        private class ParityCommand
        {
            public string requestId;
            public string module;
            public string member;
            public string kind;
            public string[] args;
        }

        [Serializable]
        private class ParityEnvelope
        {
            public string type;
            public string module;
            public string member;
            public string requestId;
            public string data;
        }
    }
}
