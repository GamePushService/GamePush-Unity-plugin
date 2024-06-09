using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;

namespace GamePush.Services
{
    public class GameStateService : MonoBehaviour
    {
        private bool isFocus;

        private void Update()
        {
            CheckGameFocus();
            if (Input.GetKeyDown(KeyCode.T))
                Debug.Log(CoreSDK.player.IsStub());
        }

        private void FixedUpdate()
        {
            CoreSDK.player.AddPlayTime(Time.fixedDeltaTime);
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
