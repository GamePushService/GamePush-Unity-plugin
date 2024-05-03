using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;

namespace GamePush.Services
{
    public class GameStateService : MonoBehaviour
    {
        public bool isFocus;
        
        void Update()
        {
            CheckGameFocus();
        }

        void CheckGameFocus()
        {
            if(!Application.isFocused)
            {
                isFocus = false;
                CoreSDK.game.SetPause(isFocus);
            }
            else
            {
                if (!isFocus)
                {
                    isFocus = true;
                    CoreSDK.game.SetPause(isFocus);
                }
            }
        }
    }
}
