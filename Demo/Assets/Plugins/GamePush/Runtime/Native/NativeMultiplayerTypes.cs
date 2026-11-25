using System;
using System.Collections.Generic;
using System.Text;

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

    // Owned by one join attempt, including temporary loss of authority in that room.
    sealed class NativeMultiplayerBuffer
    {
        const int MaxMessages = 128;
        const int MaxBytes = 256 * 1024;
        readonly Queue<Message> _messages = new Queue<Message>();
        int _bytes;
        public int ChannelId;
        public bool Holding;
        public bool PreparingHost;
        public string PlayerChanges;

        internal sealed class Message
        {
            public int ChannelId;
            public int Sender; // Zero denotes an outgoing message.
            public string Name;
            public string Data;
            public string Options;
            public long Timestamp;
            public int Size;

            public void ReportError(string reason)
            {
                var detail = "{\"channelId\":" + ChannelId + ",\"eventName\":" + GpJson.Quote(Name ?? "") +
                             ",\"data\":" + GpJson.Quote(Data ?? "null") +
                             ",\"options\":" + GpJson.Quote(Options ?? "") +
                             ",\"message\":" + GpJson.Quote(reason) + "}";
                var error = Sender == 0 ? "error:sendMessage" : "error:receiveMessage";
                NativeMainThread.Run(() => GP_Multiplayer.NativeEmit(error, new GP_Data(detail)));
            }
        }

        public bool HasMessages => _messages.Count != 0;

        public void Enqueue(Message message)
        {
            long size = (long)Encoding.UTF8.GetByteCount(message.Name ?? "") +
                        Encoding.UTF8.GetByteCount(message.Data ?? "") +
                        Encoding.UTF8.GetByteCount(message.Options ?? "");
            if (_messages.Count >= MaxMessages || size > MaxBytes - _bytes)
            {
                message.ReportError("queue_full");
                return;
            }
            message.Size = (int)size;
            _bytes += message.Size;
            _messages.Enqueue(message);
        }

        public Message Dequeue()
        {
            var message = _messages.Dequeue();
            _bytes -= message.Size;
            return message;
        }

        public void Cancel()
        {
            PlayerChanges = null;
            while (HasMessages)
            {
                var message = Dequeue();
                if (message.Sender == 0) message.ReportError("room_closed");
            }
        }
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
