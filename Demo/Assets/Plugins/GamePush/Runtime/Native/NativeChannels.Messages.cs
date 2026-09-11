using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GamePush;

namespace GamePush.Native
{
    public sealed class NativeChatMessage
    {
        public string id = "";
        public int channelId;
        public int authorId;
        public string text = "";
        public string createdAt = "";
        public string authorName = "";
        public string authorAvatar = "";
        public string json = "{}";
    }

    public sealed class NativeChatPage
    {
        public List<NativeChatMessage> items = new List<NativeChatMessage>();
        public string json = "[]";
        public bool more;
    }

    public enum NativeChatScope
    {
        Channel,
        Personal,
        Feed
    }

    public static partial class NativeChannels
    {
        public static void FetchMessages(int channelId, string tags, int limit, int offset, bool more)
        {
            NativeRun.Go(async () =>
            {
                var page = await LoadMessages(NativeChatScope.Channel, channelId, tags, limit, offset);
                NativeMainThread.Run(() => GP_Channels.NativeFireFetchMessages(page, more));
            }, () => GP_Channels.NativeFireFetchMessagesError(more), "channels");
        }

        public static void FetchPersonalMessages(int playerId, string tags, int limit, int offset, bool more)
        {
            NativeRun.Go(async () =>
            {
                var page = await LoadMessages(NativeChatScope.Personal, playerId, tags, limit, offset);
                NativeMainThread.Run(() => GP_Channels.NativeFireFetchPersonalMessages(page, more));
            }, () => GP_Channels.NativeFireFetchPersonalMessagesError(more), "channels");
        }

        public static void FetchFeedMessages(int playerId, string tags, int limit, int offset, bool more)
        {
            NativeRun.Go(async () =>
            {
                var page = await LoadMessages(NativeChatScope.Feed, playerId, tags, limit, offset);
                NativeMainThread.Run(() => GP_Channels.NativeFireFetchFeedMessages(page, more));
            }, () => GP_Channels.NativeFireFetchFeedMessagesError(more), "channels");
        }

        /// <summary>
        /// The hosted chat joins the player into the channel before reading it, exactly like
        /// gp.channels.openChat does. Without this the API answers access_denied to a guest.
        /// </summary>
        public static void EnsureJoined(int channelId, Action onDone, Action<string> onError)
        {
            NativeRun.Go(async () =>
            {
                var json = await NativeCore.Client.Fetch(NativeQueries.JoinChannel,
                    new Dictionary<string, object> { ["channelId"] = channelId });
                var result = GpJson.GetObject(json, "result") ?? json;
                if (GpJson.TryGetString(result, "__typename", out var typeName) && typeName == "Problem")
                {
                    var message = GpJson.TryGetString(result, "message", out var text) ? text : "problem";
                    // A returning player is already a member; that is the expected steady state.
                    if (message != "already_in_channel")
                        throw new Exception(message);
                }
                NativeMainThread.Run(() => onDone?.Invoke());
            }, error => onError?.Invoke(error), "channels");
        }

        public static void FetchMessagesForOverlay(NativeChatScope scope, int target, string tags, int limit,
            int offset, Action<NativeChatPage> onDone, Action<string> onError)
        {
            NativeRun.Go(async () =>
            {
                var page = await LoadMessages(scope, target, tags, limit, offset);
                NativeMainThread.Run(() => onDone?.Invoke(page));
            }, error => onError?.Invoke(error), "channels");
        }

        public static void SendMessage(NativeChatScope scope, int target, string text, string tags,
            Action<NativeChatMessage> onDone = null, Action<string> onError = null)
        {
            NativeRun.Go(async () =>
            {
                var input = new Dictionary<string, object> { ["text"] = text ?? "" };
                switch (scope)
                {
                    case NativeChatScope.Personal:
                        input["playerId"] = target;
                        break;
                    case NativeChatScope.Feed:
                        input["playerId"] = target;
                        break;
                    default:
                        input["channelId"] = target;
                        break;
                }
                var tagList = SplitTags(tags);
                if (tagList.Count > 0)
                    input["tags"] = tagList;

                var query = scope == NativeChatScope.Personal
                    ? NativeQueries.SendPersonalMessage
                    : scope == NativeChatScope.Feed
                        ? NativeQueries.SendFeedMessage
                        : NativeQueries.SendMessage;

                var json = await NativeCore.Client.Fetch(query, input);
                NativeRun.ThrowIfProblem(json);
                var raw = GpJson.GetObject(json, "result");
                var message = ParseMessage(raw);
                NativeMainThread.Run(() =>
                {
                    GP_Channels.NativeFireSendMessage(raw);
                    onDone?.Invoke(message);
                });
            }, error =>
            {
                GP_Channels.NativeFireSendMessageError();
                onError?.Invoke(error);
            }, "channels");
        }

        public static void EditMessage(string messageId, string text)
        {
            NativeRun.Go(async () =>
            {
                var json = await NativeCore.Client.Fetch(NativeQueries.EditMessage, new Dictionary<string, object>
                {
                    ["messageId"] = messageId ?? "",
                    ["text"] = text ?? ""
                });
                NativeRun.ThrowIfProblem(json);
                var raw = GpJson.GetObject(json, "result");
                NativeMainThread.Run(() => GP_Channels.NativeFireEditMessage(raw));
            }, () => GP_Channels.NativeFireEditMessageError(), "channels");
        }

        public static void DeleteMessage(string messageId)
        {
            NativeRun.Go(async () =>
            {
                var json = await NativeCore.Client.Fetch(NativeQueries.DeleteMessage, new Dictionary<string, object>
                {
                    ["messageId"] = messageId ?? ""
                });
                NativeRun.ThrowIfProblem(json);
                NativeMainThread.Run(() => GP_Channels.NativeFireDeleteMessage());
            }, () => GP_Channels.NativeFireDeleteMessageError(), "channels");
        }

        public static void DeleteChannel(int channelId)
        {
            Simple(NativeQueries.DeleteChannel, new Dictionary<string, object> { ["channelId"] = channelId },
                () => GP_Channels.NativeFireDeleteChannel(), () => GP_Channels.NativeFireDeleteChannelError());
        }

        public static void CancelJoin(int channelId)
        {
            Simple(NativeQueries.CancelJoinChannel, new Dictionary<string, object> { ["channelId"] = channelId },
                () => GP_Channels.NativeFireCancelJoin(channelId), () => GP_Channels.NativeFireCancelJoinError());
        }

        public static void Kick(int channelId, int playerId)
        {
            Simple(NativeQueries.KickFromChannel,
                new Dictionary<string, object> { ["channelId"] = channelId, ["playerId"] = playerId },
                () => GP_Channels.NativeFireKick(channelId, playerId), () => GP_Channels.NativeFireKickError());
        }

        public static void Mute(int channelId, int playerId, int seconds, string unmuteAt)
        {
            var input = new Dictionary<string, object> { ["channelId"] = channelId, ["playerId"] = playerId };
            if (!string.IsNullOrEmpty(unmuteAt))
                input["unmuteAt"] = unmuteAt;
            else if (seconds > 0)
                input["unmuteAt"] = DateTime.UtcNow.AddSeconds(seconds).ToString("o");
            Simple(NativeQueries.MutePlayerInChannel, input,
                () => GP_Channels.NativeFireMute(channelId, playerId, unmuteAt),
                () => GP_Channels.NativeFireMuteError());
        }

        public static void Unmute(int channelId, int playerId)
        {
            Simple(NativeQueries.UnmutePlayerInChannel,
                new Dictionary<string, object> { ["channelId"] = channelId, ["playerId"] = playerId },
                () => GP_Channels.NativeFireUnmute(channelId, playerId), () => GP_Channels.NativeFireUnmuteError());
        }

        public static void SendInvite(int channelId, int playerId)
        {
            Simple(NativeQueries.SendInviteToChannel,
                new Dictionary<string, object> { ["channelId"] = channelId, ["playerId"] = playerId },
                () => GP_Channels.NativeFireSendInvite(channelId, playerId),
                () => GP_Channels.NativeFireSendInviteError());
        }

        public static void CancelInvite(int channelId, int playerId)
        {
            Simple(NativeQueries.CancelInviteToChannel,
                new Dictionary<string, object> { ["channelId"] = channelId, ["playerId"] = playerId },
                () => GP_Channels.NativeFireCancelInvite(channelId, playerId),
                () => GP_Channels.NativeFireCancelInviteError());
        }

        public static void AcceptInvite(int channelId)
        {
            Simple(NativeQueries.AcceptInviteToChannel, new Dictionary<string, object> { ["channelId"] = channelId },
                () => GP_Channels.NativeFireAcceptInvite(channelId), () => GP_Channels.NativeFireAcceptInviteError());
        }

        public static void RejectInvite(int channelId)
        {
            Simple(NativeQueries.RejectInviteToChannel, new Dictionary<string, object> { ["channelId"] = channelId },
                () => GP_Channels.NativeFireRejectInvite(channelId), () => GP_Channels.NativeFireRejectInviteError());
        }

        public static void AcceptJoinRequest(int channelId, int playerId)
        {
            Simple(NativeQueries.AcceptJoinRequest,
                new Dictionary<string, object> { ["channelId"] = channelId, ["playerId"] = playerId },
                () => GP_Channels.NativeFireAcceptJoinRequest(channelId, playerId),
                () => GP_Channels.NativeFireAcceptJoinRequestError());
        }

        public static void RejectJoinRequest(int channelId, int playerId)
        {
            Simple(NativeQueries.RejectJoinRequest,
                new Dictionary<string, object> { ["channelId"] = channelId, ["playerId"] = playerId },
                () => GP_Channels.NativeFireRejectJoinRequest(channelId, playerId),
                () => GP_Channels.NativeFireRejectJoinRequestError());
        }

        public static void FetchChannelInvites(int channelId, int limit, int offset, bool more)
        {
            NativeRun.Go(async () =>
            {
                var json = await NativeCore.Client.Fetch(NativeQueries.FetchChannelInvites,
                    new Dictionary<string, object>
                    {
                        ["channelId"] = channelId,
                        ["limit"] = limit > 0 ? limit : 50,
                        ["offset"] = offset
                    });
                NativeRun.ThrowIfProblem(json);
                var items = GpJson.GetObject(GpJson.GetObject(json, "result"), "items") ?? "[]";
                NativeMainThread.Run(() => GP_Channels.NativeFireFetchChannelInvites(new GP_Data(items), more));
            }, () => GP_Channels.NativeFireFetchChannelInvitesError(more), "channels");
        }

        public static void FetchJoinRequests(int channelId, int limit, int offset, bool more)
        {
            NativeRun.Go(async () =>
            {
                var json = await NativeCore.Client.Fetch(NativeQueries.FetchJoinRequests,
                    new Dictionary<string, object>
                    {
                        ["channelId"] = channelId,
                        ["limit"] = limit > 0 ? limit : 50,
                        ["offset"] = offset
                    });
                NativeRun.ThrowIfProblem(json);
                var items = GpJson.GetObject(GpJson.GetObject(json, "result"), "items") ?? "[]";
                NativeMainThread.Run(() => GP_Channels.NativeFireFetchJoinRequests(new GP_Data(items), more));
            }, () => GP_Channels.NativeFireFetchJoinRequestsError(more), "channels");
        }

        static void Simple(string query, Dictionary<string, object> input, Action onDone, Action onError)
        {
            NativeRun.Go(async () =>
            {
                var json = await NativeCore.Client.Fetch(query, input);
                NativeRun.ThrowIfProblem(json);
                NativeMainThread.Run(onDone);
            }, onError, "channels");
        }

        static async Task<NativeChatPage> LoadMessages(NativeChatScope scope, int target, string tags, int limit,
            int offset)
        {
            var input = new Dictionary<string, object>
            {
                ["limit"] = limit > 0 ? limit : 50,
                ["offset"] = offset
            };
            if (scope == NativeChatScope.Channel)
                input["channelId"] = target;
            else
                input["playerId"] = target;

            var tagList = SplitTags(tags);
            if (tagList.Count > 0)
                input["tags"] = tagList;

            var query = scope == NativeChatScope.Personal
                ? NativeQueries.FetchPersonalMessages
                : scope == NativeChatScope.Feed
                    ? NativeQueries.FetchFeedMessages
                    : NativeQueries.FetchChannelMessages;

            var json = await NativeCore.Client.Fetch(query, input);
            NativeRun.ThrowIfProblem(json);
            var items = GpJson.GetObjectArray(GpJson.GetObject(json, "result"), "items");

            var page = new NativeChatPage();
            var sb = new StringBuilder();
            sb.Append('[');
            for (var i = 0; i < items.Count; i++)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append(items[i]);
                page.items.Add(ParseMessage(items[i]));
            }
            sb.Append(']');
            page.json = sb.ToString();
            page.more = items.Count >= (limit > 0 ? limit : 50);
            return page;
        }

        internal static NativeChatMessage ParseMessage(string json)
        {
            if (string.IsNullOrEmpty(json))
                return new NativeChatMessage();
            var message = new NativeChatMessage
            {
                id = Str(json, "id"),
                channelId = GpJson.GetInt(json, "channelId"),
                authorId = GpJson.GetInt(json, "authorId"),
                text = Str(json, "text"),
                createdAt = Str(json, "createdAt"),
                json = json
            };
            // "player" is an opaque scalar carrying the author's public state.
            var player = GpJson.GetObject(json, "player");
            if (!string.IsNullOrEmpty(player))
            {
                message.authorName = Str(player, "name");
                message.authorAvatar = Str(player, "avatar");
            }
            return message;
        }

        internal static List<string> SplitTags(string tags)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(tags))
                return list;
            foreach (var part in tags.Split(','))
            {
                var trimmed = part.Trim();
                if (trimmed.Length > 0)
                    list.Add(trimmed);
            }
            return list;
        }

        static string Str(string json, string key) =>
            GpJson.TryGetString(json, key, out var value) ? value ?? "" : "";
    }
}
