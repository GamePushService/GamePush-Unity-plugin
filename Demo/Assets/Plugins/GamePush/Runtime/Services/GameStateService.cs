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
            if (Input.GetKeyDown(KeyCode.P)) CoreSDK.player.Ping();
                //Debug.Log();
        }

        IEnumerator Ping()
        {
            //print(Time.time);
            yield return new WaitForSecondsRealtime(5);
            print($"Ping");
            CoreSDK.player.Ping();

            StartCoroutine(Ping());
        }

        IEnumerator PlayTimeCounter()
        {
            yield return new WaitForSecondsRealtime(1);
            CoreSDK.player.AddPlayTime(1);
            CoreSDK.AddServerTime(1);

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
