using System;
using GamePush;

namespace GamePush.Native
{
    sealed class NativeMultiplayerStateManager
    {
        readonly NativeMultiplayerTransport _transport;
        readonly NativeMultiplayerSynchronizer _synchronizer;

        string _lastSentPlayers;
        string _lastSentGlobal;
        bool _playersActiveLastTick;
        bool _globalActiveLastTick;
        int _playersSeq;
        int _globalSendSeq;
        int _tickCounter;
        bool _sending;

        public NativeMultiplayerStateManager(
            NativeMultiplayerTransport transport,
            NativeMultiplayerSynchronizer synchronizer)
        {
            _transport = transport;
            _synchronizer = synchronizer;
        }

        public void StartSending()
        {
            _sending = true;
        }

        public void StopSending()
        {
            _sending = false;
        }

        public void ResetSendState()
        {
            _lastSentPlayers = null;
            _lastSentGlobal = null;
            _playersSeq = 0;
            _globalSendSeq = 0;
            _playersActiveLastTick = false;
            _globalActiveLastTick = false;
            _tickCounter = 0;
        }

        public void Cleanup()
        {
            StopSending();
            ResetSendState();
        }

        public void TickHostSend()
        {
            if (!_sending || !_transport.IsLive)
                return;
            _synchronizer.SamplePeerStates(UnityEngine.Time.realtimeSinceStartup * 1000.0);
            SendHostPlayers(false);
            if (_tickCounter % _synchronizer.GlobalTickDivider == 0)
                SendHostGlobal(false);
            _tickCounter++;
        }

        public void SendFullSnapshot()
        {
            if (!_transport.IsLive)
                return;
            _lastSentPlayers = null;
            _lastSentGlobal = null;
            SendHostPlayers(true);
            SendHostGlobal(true);
        }

        public void SendMyStateToHost(string state)
        {
            if (!_transport.IsLive)
                return;
            _transport.SendToHost(NativeMultiplayerWire.PeerState, state ?? "{}", NativePlayer.Id);
        }

        void SendHostPlayers(bool forceSnapshot)
        {
            if (!_transport.IsLive)
                return;
            var current = _synchronizer.BuildPlayersPayload();
            if (forceSnapshot || string.IsNullOrEmpty(_lastSentPlayers))
            {
                _playersSeq++;
                _transport.Send(NativeMultiplayerWire.StateUpdate, current, NativePlayer.Id, _playersSeq);
                _lastSentPlayers = current;
                _playersActiveLastTick = true;
                return;
            }

            var delta = GpJson.CalculateDelta(_lastSentPlayers, current);
            if (delta == null)
            {
                if (!_playersActiveLastTick)
                    return;
                _playersActiveLastTick = false;
                _playersSeq++;
                _transport.Send(NativeMultiplayerWire.StateDelta, "{}", NativePlayer.Id, _playersSeq);
                return;
            }
            _playersSeq++;
            _transport.Send(NativeMultiplayerWire.StateDelta, delta, NativePlayer.Id, _playersSeq);
            _lastSentPlayers = current;
            _playersActiveLastTick = true;
        }

        void SendHostGlobal(bool forceSnapshot)
        {
            if (!_transport.IsLive)
                return;
            var current = string.IsNullOrEmpty(_synchronizer.GlobalState) ? "{}" : _synchronizer.GlobalState;
            if (forceSnapshot || string.IsNullOrEmpty(_lastSentGlobal))
            {
                _globalSendSeq++;
                _transport.Send(NativeMultiplayerWire.GlobalStateUpdate, current, NativePlayer.Id, _globalSendSeq);
                _lastSentGlobal = current;
                _globalActiveLastTick = true;
                return;
            }

            var delta = GpJson.CalculateDelta(_lastSentGlobal, current);
            if (delta == null)
            {
                if (!_globalActiveLastTick)
                    return;
                _globalActiveLastTick = false;
                _globalSendSeq++;
                _transport.Send(NativeMultiplayerWire.GlobalStateDelta, "{}", NativePlayer.Id, _globalSendSeq);
                return;
            }
            _globalSendSeq++;
            _transport.Send(NativeMultiplayerWire.GlobalStateDelta, delta, NativePlayer.Id, _globalSendSeq);
            _lastSentGlobal = current;
            _globalActiveLastTick = true;
        }
    }
}
