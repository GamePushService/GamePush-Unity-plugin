using System;
using System.Collections;
using System.Collections.Generic;

namespace GamePush.Core
{
    public class PlayersModule
    {
        public event Action<GP_Data> OnFetchSuccess;
        public event Action OnFetchError;
        
        public void Fetch(string jsonIds)
        {

            
        }
    }
}
