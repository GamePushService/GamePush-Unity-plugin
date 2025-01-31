using System;
using System.Collections;
using System.Collections.Generic;

namespace GamePush.Core
{
    public class EventsModule
    {
        public event Action<PlayerEvents> OnEventJoin;
        public event Action<string> OnEventJoinError;
        
        public void Join(string idOrTag)
        {
            
        }

        public EventData[] List()
        {
            
        }

        public PlayerEvents[] ActiveList()
        {
            
        }

        public EventData GetEvent(string idOrTag)
        {
            
        }

        public bool IsActive(string idOrTag)
        {
            
        }

        public bool IsJoined(string idOrTag)
        {
            
        }
    }
}
