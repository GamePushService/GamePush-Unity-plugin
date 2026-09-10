using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using GamePush;

namespace GamePush.Native
{
    public static class NativePlayer
    {
        const string SecretPref = "gp.native.secret";
        const string GuestSecretPref = "gp.native.guestSecret";
        const string CredPref = "gp.native.credentials";
        static readonly Dictionary<string, string> State = new Dictionary<string, string>();
        static bool _first = true;

        public static int Id { get; private set; }
        public static string AuthToken { get; private set; } = "";
        public static string Credentials { get; private set; } = "";
        public static string SecretCode { get; private set; } = "";
        public static bool IsLoggedIn { get; private set; }

        // Raw "achievementsList" array from the last player response, consumed by NativeAchievements.
        public static string AchievementsJson { get; private set; } = "[]";

        public static void Adopt(int id, string name)
        {
            if (id > 0)
                Id = id;
            if (!string.IsNullOrEmpty(name))
                Set("name", name);
        }

        public static void AdoptSession(NativeJsSession session, GraphQLClient client)
        {
            if (session == null)
                return;
            Adopt(session.PlayerId, session.Name);
            if (!string.IsNullOrEmpty(session.Avatar))
                Set("avatar", session.Avatar);
            if (!string.IsNullOrEmpty(session.SecretCode))
            {
                SecretCode = session.SecretCode;
                PersistSecret();
            }
            else
                EnsureSecret();
            Credentials = "";
            IsLoggedIn = false;
            client?.SetPlayerData(AuthPayload());
        }

        public static async Task FetchExisting(GraphQLClient client)
        {
            if (client == null || Id <= 0)
                return;
            try
            {
                await FetchCurrent(client);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[GamePush Native] GetPlayer after JS adopt: " + exception.Message);
                client.SetPlayerData(AuthPayload());
                if (Id <= 0)
                    throw;
            }
        }

        public static void PrepareCredentials(GraphQLClient client)
        {
            EnsureSecret();
            Credentials = "";
            IsLoggedIn = false;
            client.SetPlayerData(AuthPayload());
        }

        public static async Task Bootstrap(GraphQLClient client)
        {
            var json = await client.Fetch(NativeQueries.SyncPlayer, new Dictionary<string, object>
            {
                ["playerState"] = CurrentState(),
                ["override"] = false,
                ["isFirstRequest"] = true
            }, new Dictionary<string, object> { ["withToken"] = true });
            _first = false;
            ApplyResult(json, client);
        }

        public static async Task Sync(bool forceOverride)
        {
            if (NativeCore.Client == null)
                return;
            var json = await NativeCore.Client.Fetch(NativeQueries.SyncPlayer, new Dictionary<string, object>
            {
                ["playerState"] = CurrentState(),
                ["override"] = forceOverride,
                ["isFirstRequest"] = false
            }, new Dictionary<string, object> { ["withToken"] = true });
            ApplyResult(json, NativeCore.Client);
            NativeMainThread.Run(() => GP_Player.NotifyNativeSync());
        }

        public static int GetInt(string key, int fallback = 0) =>
            int.TryParse(GetString(key), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : fallback;

        public static float GetFloat(string key, float fallback = 0)
        {
            return float.TryParse(GetString(key), NumberStyles.Float, CultureInfo.InvariantCulture, out var v)
                ? v
                : fallback;
        }

        public static bool GetBool(string key) => GetString(key) == "true" || GetString(key) == "True";

        public static string GetString(string key)
        {
            if (key == "id") return Id.ToString(CultureInfo.InvariantCulture);
            if (key == "credentials") return Credentials;
            if (key == "secretCode") return SecretCode;
            return State.TryGetValue(key, out var value) ? value : "";
        }

        public static void Set(string key, string value)
        {
            State[key ?? ""] = value ?? "";
            LastMutatedAt = NowSeconds();
        }

        public static void Add(string key, float value)
        {
            Set(key, (GetFloat(key) + value).ToString(CultureInfo.InvariantCulture));
        }

        public static void SetFlag(string key, bool value)
        {
            Set(key, value ? "true" : "false");
        }

        public static float LastMutatedAt { get; private set; }

        static float _autoSyncInterval;
        static bool _autoSyncEnabled;
        static float _autoSyncLastCheck;
        static float _autoSyncLastSyncedMutation;

        public static void EnableAutoSync(int interval)
        {
            if (interval <= 0)
                return;
            _autoSyncInterval = interval;
            _autoSyncEnabled = true;
            _autoSyncLastCheck = 0f;
            NativeMainThread.Ensure();
        }

        public static void DisableAutoSync()
        {
            _autoSyncEnabled = false;
        }

        public static void TickAutoSync(float now)
        {
            if (!_autoSyncEnabled || _autoSyncInterval <= 0)
                return;
            if (LastMutatedAt <= _autoSyncLastSyncedMutation)
                return;
            if (_autoSyncLastCheck > 0f && now - _autoSyncLastCheck < _autoSyncInterval)
                return;
            _autoSyncLastCheck = now;
            _autoSyncLastSyncedMutation = LastMutatedAt;
            _ = Sync(false);
        }

        static float NowSeconds() => Time.realtimeSinceStartup;

        public static bool IsStub()
        {
            foreach (var pair in State)
            {
                if (pair.Key == "name" || pair.Key == "avatar" || pair.Key.EndsWith(":timestamp"))
                    continue;
                if (!string.IsNullOrEmpty(pair.Value) && pair.Value != "0" && pair.Value != "false")
                    return false;
            }
            return true;
        }

        static Dictionary<string, object> CurrentState()
        {
            var state = new Dictionary<string, object>
            {
                ["id"] = Id,
                ["active"] = true,
                ["removed"] = false,
                ["test"] = false,
                ["name"] = GetString("name"),
                ["avatar"] = GetString("avatar"),
                ["score"] = GetFloat("score"),
                ["credentials"] = IsLoggedIn ? Credentials ?? "" : "",
                ["modifiedAt"] = NativeCore.ServerTime ?? ""
            };
            foreach (var pair in State)
            {
                if (!state.ContainsKey(pair.Key))
                    state[pair.Key] = pair.Value;
            }
            return state;
        }

        static Dictionary<string, object> AuthPayload()
        {
            return new Dictionary<string, object> { ["secretCode"] = SecretCode ?? "" };
        }

        static void EnsureSecret()
        {
            SecretCode = PlayerPrefs.GetString(SecretPref, "");
            if (!string.IsNullOrEmpty(SecretCode))
                return;
            SecretCode = Guid.NewGuid().ToString("N");
            PersistSecret();
        }

        static void PersistSecret()
        {
            PlayerPrefs.SetString(SecretPref, SecretCode ?? "");
            PlayerPrefs.Save();
        }

        static void SetCredentials(string cred)
        {
            Credentials = cred ?? "";
            if (string.IsNullOrEmpty(Credentials))
                PlayerPrefs.DeleteKey(CredPref);
            else
                PlayerPrefs.SetString(CredPref, Credentials);
            PlayerPrefs.Save();
        }

        static async Task FetchCurrent(GraphQLClient client)
        {
            var json = await client.Fetch(NativeQueries.GetPlayer, new Dictionary<string, object>
            {
                ["isFirstRequest"] = _first
            }, new Dictionary<string, object> { ["withToken"] = true });
            _first = false;
            ApplyResult(json, client);
        }

        static async Task<string> FetchLoginStatusCredentials(GraphQLClient client)
        {
            var json = await client.Fetch(NativeQueries.GetPlayerLoginStatus);
            var result = GpJson.GetObject(json, "result") ?? json;
            if (GpJson.TryGetString(result, "__typename", out var typeName) && typeName == "Problem")
                return "";
            return GpJson.TryGetString(result, "credentials", out var cred) ? cred ?? "" : "";
        }

        static void ApplyResult(string json, GraphQLClient client)
        {
            var result = GpJson.GetObject(json, "result") ?? json;
            if (GpJson.TryGetString(result, "__typename", out var typeName) && typeName == "Problem")
            {
                var msg = GpJson.TryGetString(result, "message", out var problem) ? problem : "player_problem";
                throw new Exception(msg + " " + result);
            }
            if (typeName == "PlayerSyncConflict")
                throw new Exception("player_sync_conflict");

            var previousId = Id;
            var state = ReadPlayerState(result);
            var newId = GpJson.GetInt(result, "id");
            if (state != null)
            {
                var stateId = GpJson.GetInt(state, "id");
                if (stateId > 0)
                    newId = stateId;
            }
            if (newId > 0 && newId != previousId)
                State.Clear();
            if (newId > 0)
                Id = newId;

            if (GpJson.TryGetString(result, "authToken", out var token) && !string.IsNullOrEmpty(token))
                AuthToken = token;
            if (GpJson.TryGetString(result, "token", out var sessionToken) && !string.IsNullOrEmpty(sessionToken))
                AuthToken = sessionToken;

            ApplyPlayerFields(result);
            if (state != null)
                ApplyPlayerFields(state);

            client.SetPlayerData(AuthPayload());
            if (Id <= 0)
                throw new Exception("player_id_missing " + result);
            IsLoggedIn = !string.IsNullOrEmpty(Credentials);
        }

        static void ApplyPlayerFields(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;
            if (GpJson.TryGetString(json, "credentials", out var cred))
                SetCredentials(cred ?? "");
            if (GpJson.TryGetString(json, "secretCode", out var secret) && !string.IsNullOrEmpty(secret))
            {
                SecretCode = secret;
                PersistSecret();
            }
            if (GpJson.TryGetString(json, "name", out var name) && name != null)
                Set("name", name);
            if (GpJson.TryGetString(json, "avatar", out var avatar) && avatar != null)
                Set("avatar", avatar);
            if (GpJson.TryGetRaw(json, "achievementsList", out var achievements) &&
                !string.IsNullOrEmpty(achievements) && achievements.TrimStart().StartsWith("["))
            {
                AchievementsJson = achievements;
                NativeAchievements.ApplyPlayerList(achievements);
            }
            foreach (var key in GpJson.ObjectKeys(json))
            {
                if (key == "state" || key == "stats" || key == "token" || key == "authToken" ||
                    key == "__typename" || key == "selected" || key == "achievementsList")
                    continue;
                if (GpJson.TryGetString(json, key, out var value) && value != null)
                    Set(key, value);
            }
        }

        static string ReadPlayerState(string result)
        {
            if (GpJson.TryGetRaw(result, "state", out var raw) && !string.IsNullOrEmpty(raw))
            {
                raw = raw.Trim();
                if (raw.StartsWith("{"))
                    return raw;
            }
            if (GpJson.TryGetString(result, "state", out var quoted) && !string.IsNullOrEmpty(quoted))
            {
                var inner = quoted.Trim();
                if (inner.StartsWith("{"))
                    return inner;
            }
            return null;
        }

        public static async Task<bool> LoginWithOauth(string token, string tokenType, string redirectUri)
        {
            if (NativeCore.Client == null || string.IsNullOrEmpty(token))
                return false;
            var guestId = Id;
            var guestSecret = SecretCode;
            var input = new Dictionary<string, object> { ["token"] = token };
            if (!string.IsNullOrEmpty(tokenType))
                input["tokenType"] = tokenType;
            if (!string.IsNullOrEmpty(redirectUri))
                input["redirectUri"] = redirectUri;
            var json = await NativeCore.Client.Fetch(NativeQueries.LoginPlayer, input);
            if (!IsSuccess(json))
                return false;

            try
            {
                await FetchCurrent(NativeCore.Client);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[GamePush Native] GetPlayer after login: " + exception.Message);
            }

            var statusCred = "";
            try
            {
                statusCred = await FetchLoginStatusCredentials(NativeCore.Client);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[GamePush Native] GetPlayerLoginStatus: " + exception.Message);
            }

            var switched = Id != guestId && Id > 0;
            if (!switched && !string.IsNullOrEmpty(statusCred))
                await SwitchToLoggedInPlayer(statusCred, guestId, guestSecret);

            if (Id != guestId && !string.IsNullOrEmpty(guestSecret))
            {
                PlayerPrefs.SetString(GuestSecretPref, guestSecret);
                PlayerPrefs.Save();
            }

            NativeMainThread.Run(() =>
            {
                GP_Player.NotifyNativeLogin(IsLoggedIn);
                if (IsLoggedIn)
                    GP_Player.NotifyNativeSync();
            });
            return IsLoggedIn;
        }

        static async Task SwitchToLoggedInPlayer(string credentials, int guestId, string guestSecret)
        {
            if (string.IsNullOrEmpty(guestSecret))
                guestSecret = SecretCode;
            PlayerPrefs.SetString(GuestSecretPref, guestSecret ?? "");
            PlayerPrefs.Save();

            SecretCode = "";
            SetCredentials(credentials);
            NativeCore.Client.SetPlayerData(AuthPayload());

            var playerState = new Dictionary<string, object>
            {
                ["id"] = 0,
                ["active"] = true,
                ["removed"] = false,
                ["test"] = false,
                ["name"] = "",
                ["avatar"] = "",
                ["score"] = 0,
                ["credentials"] = credentials,
                ["modifiedAt"] = NativeCore.ServerTime ?? ""
            };

            var json = await NativeCore.Client.Fetch(NativeQueries.SyncPlayer, new Dictionary<string, object>
            {
                ["playerState"] = playerState,
                ["override"] = false,
                ["isFirstRequest"] = true
            }, new Dictionary<string, object> { ["withToken"] = true });

            var result = GpJson.GetObject(json, "result") ?? json;
            if (GpJson.TryGetString(result, "__typename", out var typeName) && typeName == "PlayerSyncConflict")
            {
                var chosen = PickLoggedInConflictPlayer(result, credentials);
                if (chosen != null)
                {
                    json = await NativeCore.Client.Fetch(NativeQueries.SyncPlayer, new Dictionary<string, object>
                    {
                        ["playerState"] = chosen,
                        ["override"] = true,
                        ["isFirstRequest"] = true
                    }, new Dictionary<string, object> { ["withToken"] = true });
                }
            }

            ApplyResult(json, NativeCore.Client);
            if (Id == guestId)
                Debug.LogWarning("[GamePush Native] Login stayed on guest " + guestId);
        }

        static Dictionary<string, object> PickLoggedInConflictPlayer(string result, string credentials)
        {
            var players = GpJson.GetObjectArray(result, "players");
            if (players == null || players.Count == 0)
                return null;
            string best = null;
            foreach (var player in players)
            {
                if (string.IsNullOrEmpty(player))
                    continue;
                var cred = GpJson.TryGetString(player, "credentials", out var c) ? c : "";
                var name = GpJson.TryGetString(player, "name", out var n) ? n : "";
                if (!string.IsNullOrEmpty(credentials) && cred == credentials)
                {
                    best = player;
                    break;
                }
                if (!string.IsNullOrEmpty(name) && best == null)
                    best = player;
            }
            if (best == null)
                best = players[0];
            var state = new Dictionary<string, object>();
            foreach (var key in GpJson.ObjectKeys(best))
            {
                if (!GpJson.TryGetString(best, key, out var value) || value == null)
                    continue;
                if (key == "id" || key == "score")
                {
                    if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var asInt))
                        state[key] = asInt;
                    else if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var asFloat))
                        state[key] = asFloat;
                    else
                        state[key] = value;
                }
                else if (key == "active" || key == "removed" || key == "test")
                    state[key] = value == "true" || value == "True";
                else
                    state[key] = value;
            }
            if (!state.ContainsKey("credentials"))
                state["credentials"] = credentials ?? "";
            return state;
        }

        public static async Task<bool> LogoutRemote()
        {
            if (NativeCore.Client == null)
                return false;
            var json = await NativeCore.Client.Fetch(NativeQueries.LogoutPlayer);
            if (!IsSuccess(json))
                return false;

            SetCredentials("");
            IsLoggedIn = false;
            Id = 0;
            State.Clear();
            AuthToken = "";

            var guestSecret = PlayerPrefs.GetString(GuestSecretPref, "");
            SecretCode = string.IsNullOrEmpty(guestSecret) ? Guid.NewGuid().ToString("N") : guestSecret;
            PersistSecret();
            NativeCore.Client.SetPlayerData(AuthPayload());
            await Bootstrap(NativeCore.Client);
            NativeMainThread.Run(() =>
            {
                GP_Player.NotifyNativeLogout(true);
                GP_Player.NotifyNativeSync();
            });
            return true;
        }

        static bool IsSuccess(string json)
        {
            var result = GpJson.GetObject(json, "result") ?? json;
            if (GpJson.TryGetString(result, "__typename", out var typeName) && typeName == "Problem")
                return false;
            return GpJson.GetBool(result, "success");
        }
    }
}
