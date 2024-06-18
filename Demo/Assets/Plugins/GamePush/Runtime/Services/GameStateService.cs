using System;
using System.Collections.Generic;
using UnityEngine;
using GamePush;
using System.Collections;

namespace GamePush.Services
{
    public class GameStateService : MonoBehaviour
    {
        private bool isFocus;
        public event Action<bool> OnFocusChange;
        private float counterWaitTime = 1f;

        private void Start()
        {
            StartCoroutine(PlayTimeCounter());
            StartCoroutine(Ping());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void Update()
        {
            CheckGameFocus();
            if (Input.GetKeyDown(KeyCode.P)) print("Handle P");//CoreSDK.player.Ping();
                                                                //Debug.Log();
        }

        IEnumerator Ping()
        {
            yield return new WaitForSecondsRealtime(10);
            CoreSDK.player.Ping();

            //Debug.Log(CoreSDK.player.GetPlaytimeToday());
            StartCoroutine(Ping());
        }

        IEnumerator PlayTimeCounter()
        {
            yield return new WaitForSecondsRealtime(counterWaitTime);
            CoreSDK.AddPlayTime(counterWaitTime);

            StartCoroutine(PlayTimeCounter());
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
