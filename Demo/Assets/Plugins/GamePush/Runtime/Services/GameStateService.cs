using System;
using System.Collections.Generic;
using UnityEngine;
using GamePush;

namespace GamePush.Services
{
    public class GameStateService : MonoBehaviour
    {
        private bool isFocus;
        public event Action<bool> OnFocusChange;

        private void Update()
        {
            CheckGameFocus();
            if (Input.GetKeyDown(KeyCode.P)) CoreSDK.player.Ping();
                //Debug.Log();
        }

        private void FixedUpdate()
        {
            CoreSDK.player.AddPlayTime(Time.fixedDeltaTime);
        }

        void CheckGameFocus()
        {
            if (!CoreSDK.isInit) return;

            if (!Application.isFocused && isFocus)
            {
                isFocus = false;
                OnFocusChange?.Invoke(isFocus);
                CoreSDK.game.SetAutoPause(true);
            }
            else if (Application.isFocused && !isFocus)
            {
                isFocus = true;
                OnFocusChange?.Invoke(isFocus);
                CoreSDK.game.SetAutoPause(false);
            }
        }
    }
}
