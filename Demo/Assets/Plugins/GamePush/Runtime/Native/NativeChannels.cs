using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GamePush;

namespace GamePush.Native
{
    public static partial class NativeChannels
    {
        public static void FetchChannels(FetchChannelsFilter filter)
        {
            Run(async () =>
            {
                var input = new Dictionary<string, object>
                {
                    ["limit"] = filter?.limit ?? 100,
                    ["offset"] = filter?.offset ?? 0,
                    ["onlyJoined"] = filter?.onlyJoined ?? false,
                    ["onlyOwned"] = filter?.onlyOwned ?? false
                };
                if (filter?.tags != null && filter.tags.Length > 0)
                    input["tags"] = filter.tags;
                if (filter?.ids != null && filter.ids.Length > 0)
                    input["ids"] = filter.ids;
                if (!string.IsNullOrEmpty(filter?.search))
                    input["search"] = filter.search;
                var json = await NativeCore.Client.Fetch(NativeQueries.FetchChannels, input);
                ThrowIfProblem(json);
                var result = GpJson.GetObject(json, "result") ?? json;
                var items = GpJson.GetObjectArray(result, "items");
                var list = new List<FetchChannelData>();
                foreach (var item in items)
                    list.Add(ParseChannel<FetchChannelData>(item));
                NativeMainThread.Run(() => GP_Channels.NativeFireFetchChannels(list, false));
            }, () => GP_Channels.NativeFireFetchChannelsError());
        }

        public static void FetchChannel(int channelId)
        {
            Run(async () =>
            {
                var json = await NativeCore.Client.Fetch(NativeQueries.FetchChannel, new Dictionary<string, object>
                {
                    ["channelId"] = channelId
                });
                ThrowIfProblem(json);
                var result = GpJson.GetObject(json, "result") ?? json;
                var data = ParseChannel<FetchChannelData>(result);
                NativeMainThread.Run(() => GP_Channels.NativeFireFetchChannel(data));
            }, () => GP_Channels.NativeFireFetchChannelError());
        }

        public static void CreateChannel(CreateChannelFilter filter)
        {
            Run(async () =>
            {
                var input = new Dictionary<string, object>
                {
                    ["template"] = filter.template.ToString(),
                    ["name"] = filter.name ?? "",
                    ["visible"] = filter.visible,
                    ["private"] = filter.ch_private
                };
                if (filter.capacity > 0) input["capacity"] = filter.capacity;
                if (!string.IsNullOrEmpty(filter.description)) input["description"] = filter.description;
                if (!string.IsNullOrEmpty(filter.password)) input["password"] = filter.password;
                if (filter.tags != null && filter.tags.Length > 0) input["tags"] = filter.tags;
                if (filter.ownerAcl != null) input["ownerAcl"] = Acl(filter.ownerAcl);
                if (filter.memberAcl != null) input["memberAcl"] = Acl(filter.memberAcl);
                if (filter.guestAcl != null) input["guestAcl"] = Acl(filter.guestAcl);
                var json = await NativeCore.Client.Fetch(NativeQueries.CreateChannel, input);
                ThrowIfProblem(json);
                var data = ParseChannel<CreateChannelData>(GpJson.GetObject(json, "result") ?? json, "createChannel");
                if (data.id <= 0)
                    throw new InvalidOperationException("createChannel returned an invalid channel id");
                NativeMainThread.Run(() => GP_Channels.NativeFireCreateChannel(data));
            }, () => GP_Channels.NativeFireCreateChannelError());
        }

        public static void UpdateChannel(string rawJson)
        {
            Run(async () =>
            {
                var input = new Dictionary<string, object>();
                var channelId = GpJson.GetInt(rawJson, "channelId");
                input["channelId"] = channelId;
                if (GpJson.TryGetString(rawJson, "name", out var name) && name != null) input["name"] = name;
                if (GpJson.TryGetString(rawJson, "description", out var description) && description != null)
                    input["description"] = description;
                if (rawJson.Contains("\"ownerId\"")) input["ownerId"] = GpJson.GetInt(rawJson, "ownerId");
                if (rawJson.Contains("\"capacity\"") && GpJson.GetInt(rawJson, "capacity") > 0)
                    input["capacity"] = GpJson.GetInt(rawJson, "capacity");
                if (rawJson.Contains("\"visible\"")) input["visible"] = GpJson.GetBool(rawJson, "visible");
                if (rawJson.Contains("\"password\""))
                    input["password"] = GpJson.TryGetString(rawJson, "password", out var password) ? password : "";
                var json = await NativeCore.Client.Fetch(NativeQueries.UpdateChannel, input);
                ThrowIfProblem(json);
                var data = ParseChannel<UpdateChannelData>(GpJson.GetObject(json, "result") ?? json);
                data.channelId = data.id;
                NativeMainThread.Run(() => GP_Channels.NativeFireUpdateChannel(data));
            }, () => GP_Channels.NativeFireUpdateChannelError());
        }

        public static void Join(int channelId, string password)
        {
            Run(async () =>
            {
                var input = new Dictionary<string, object> { ["channelId"] = channelId };
                if (!string.IsNullOrEmpty(password))
                    input["password"] = password;
                var json = await NativeCore.Client.Fetch(NativeQueries.JoinChannel, input);
                ThrowIfProblem(json);
                NativeMainThread.Run(() => GP_Channels.NativeFireJoinSuccess());
            }, () => GP_Channels.NativeFireJoinError());
        }

        public static void Leave(int channelId)
        {
            Run(async () =>
            {
                var json = await NativeCore.Client.Fetch(NativeQueries.LeaveChannel, new Dictionary<string, object>
                {
                    ["channelId"] = channelId
                });
                ThrowIfProblem(json);
                NativeMainThread.Run(() => GP_Channels.NativeFireLeaveSuccess());
            }, () => GP_Channels.NativeFireLeaveError());
        }

        public static void FetchMembers(FetchMembersFilter filter)
        {
            Run(async () =>
            {
                var input = new Dictionary<string, object>
                {
                    ["channelId"] = filter.channelId,
                    ["onlyOnline"] = filter.onlyOnline,
                    ["limit"] = filter.limit > 0 ? filter.limit : 100,
                    ["offset"] = filter.offset
                };
                if (!string.IsNullOrEmpty(filter.search))
                    input["search"] = filter.search;
                var json = await NativeCore.Client.Fetch(NativeQueries.FetchMembers, input);
                ThrowIfProblem(json);
                var result = GpJson.GetObject(json, "result");
                var players = GpJson.GetObjectArray(result, "players");
                var sb = new StringBuilder();
                sb.Append('[');
                for (var i = 0; i < players.Count; i++)
                {
                    if (i > 0) sb.Append(',');
                    var mapped = GpJson.ReplaceKey(players[i], "private", "ch_private");
                    sb.Append(mapped);
                }
                sb.Append(']');
                var payload = sb.ToString();
                NativeMainThread.Run(() => GP_Channels.NativeFireFetchMembers(new GP_Data(payload), false));
            }, () => GP_Channels.NativeFireFetchMembersError());
        }

        static Dictionary<string, object> Acl(object acl)
        {
            var json = JsonUtility.ToJson(acl);
            return new Dictionary<string, object>
            {
                ["canViewMessages"] = GpJson.GetBool(json, "canViewMessages", true),
                ["canAddMessage"] = GpJson.GetBool(json, "canAddMessage", true),
                ["canEditMessage"] = GpJson.GetBool(json, "canEditMessage"),
                ["canDeleteMessage"] = GpJson.GetBool(json, "canDeleteMessage"),
                ["canViewMembers"] = GpJson.GetBool(json, "canViewMembers", true),
                ["canInvitePlayer"] = GpJson.GetBool(json, "canInvitePlayer"),
                ["canKickPlayer"] = GpJson.GetBool(json, "canKickPlayer"),
                ["canAcceptJoinRequest"] = GpJson.GetBool(json, "canAcceptJoinRequest"),
                ["canMutePlayer"] = GpJson.GetBool(json, "canMutePlayer"),
                ["canSetValue"] = GpJson.GetBool(json, "canSetValue", true),
                ["canAddValue"] = GpJson.GetBool(json, "canAddValue", true),
                ["canSubtractValue"] = GpJson.GetBool(json, "canSubtractValue", true)
            };
        }

        static T ParseChannel<T>(string json, string operation = "channel") where T : new()
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException(operation + " returned an empty channel payload");
            var mapped = GpJson.ReplaceKey(json, "private", "ch_private");
            try
            {
                var value = JsonUtility.FromJson<T>(mapped);
                if (value == null)
                    throw new InvalidOperationException("JsonUtility returned null");
                return value;
            }
            catch (Exception exception)
            {
                GP_Logger.Error("Channels", operation + " response parse failed: " + exception.Message + " payload=" + json);
                throw;
            }
        }

        static void ThrowIfProblem(string json)
        {
            var result = GpJson.GetObject(json, "result") ?? json;
            if (GpJson.TryGetString(result, "__typename", out var typeName) && typeName == "Problem")
                throw new Exception(GpJson.TryGetString(result, "message", out var msg) ? msg : "channel_problem");
        }

        static void Run(Func<Task> work, Action onError)
        {
            async void Go()
            {
                try
                {
                    if (NativeCore.Client == null || !NativeCore.Ready)
                        throw new Exception("sdk_not_ready");
                    if (NativePlayer.Id <= 0)
                        throw new Exception("player_not_found");
                    await work();
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("[GamePush Native] channels: " + exception.Message);
                    NativeMainThread.Run(onError);
                }
            }
            Go();
        }
    }
}
