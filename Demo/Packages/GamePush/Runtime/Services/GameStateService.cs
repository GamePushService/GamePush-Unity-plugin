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
            if (!CoreSDK.isInit) return;

            if (!Application.isFocused)
            {
                isFocus = false;
                CoreSDK.game.SetAutoPause(isFocus);
            }
            else if (!isFocus)
            {
                isFocus = true;
                CoreSDK.game.SetAutoPause(isFocus);
            }
        }


    }
}
