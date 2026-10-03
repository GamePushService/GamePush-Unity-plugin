using System;

namespace GamePush.Native
{
    public sealed class NativeMultiplayerPlayer
    {
        public int playerId;
        public string name;
        public bool isHost;
        public int ping;
        public float connectionStability = 1;
        public int sessionDuration;
    }

    sealed class NativeMultiplayerWireMessage
    {
        public int Type;
        public int SenderId;
        public int Seq;
        public long Timestamp;
        public string Payload;
    }

    static class NativeMultiplayerWire
    {
        public const int StateUpdate = 1;
        public const int Heartbeat = 2;
        public const int HostMigration = 3;
        public const int PeerState = 4;
        public const int CustomEvent = 5;
        public const int GlobalStateUpdate = 6;
        public const int StateDelta = 7;
        public const int GlobalStateDelta = 8;
        public const int SnapshotRequest = 9;
    }
}
