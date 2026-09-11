using System.Collections.Generic;

namespace GamePush.Native
{
    static class NativeHostSelector
    {
        const int SignificantPingDiff = 100;

        public static NativeMultiplayerPlayer SelectHost(List<NativeMultiplayerPlayer> players)
        {
            if (players == null || players.Count == 0)
                return null;
            if (players.Count == 1)
                return players[0];

            NativeMultiplayerPlayer best = players[0];
            for (var i = 1; i < players.Count; i++)
            {
                var candidate = players[i];
                if (BetterThan(candidate, best))
                    best = candidate;
            }
            return best;
        }

        public static bool ShouldMigrateHost(
            NativeMultiplayerPlayer currentHost,
            List<NativeMultiplayerPlayer> players)
        {
            var optimal = SelectHost(players);
            return optimal != null && currentHost != null && optimal.playerId != currentHost.playerId;
        }

        public static float CalculateSelfStability(int reconnectCount, int freezeCount, float pingJitterMs)
        {
            var reconnectPenalty = System.Math.Max(0, 1 - reconnectCount * 0.15f);
            var freezePenalty = System.Math.Max(0, 1 - freezeCount * 0.1f);
            var pingQuality = System.Math.Max(0, 1 - System.Math.Max(0, pingJitterMs - 20f) / 200f);
            var raw = System.Math.Max(0, System.Math.Min(1, reconnectPenalty * freezePenalty * pingQuality));
            return (float)(System.Math.Round(raw * 20) / 20);
        }

        public static bool IsPingSignificantlyBetter(int currentHostPing, int candidatePing) =>
            currentHostPing - candidatePing > SignificantPingDiff;

        public static bool IsPingSignificantlyWorse(int currentHostPing, int candidatePing) =>
            candidatePing - currentHostPing > SignificantPingDiff;

        static bool BetterThan(NativeMultiplayerPlayer a, NativeMultiplayerPlayer b)
        {
            if (a.connectionStability != b.connectionStability)
                return a.connectionStability > b.connectionStability;
            if (a.sessionDuration != b.sessionDuration)
                return a.sessionDuration > b.sessionDuration;
            if (a.ping != b.ping)
                return a.ping < b.ping;
            return false;
        }
    }
}
