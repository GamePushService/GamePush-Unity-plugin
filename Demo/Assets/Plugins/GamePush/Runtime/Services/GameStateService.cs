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
            CoreSDK.OnInit += StartCounters;
        }

        private void OnDisable()
        {
            CoreSDK.OnInit -= StartCounters;
            StopAllCoroutines();
        }

        private void StartCounters()
        {
            StartCoroutine(PlayTimeCounter());
            StartCoroutine(Ping());
        }

        private void Update()
        {
            TickHandle();
        }

        private void TickHandle()
        {
            CoreSDK.player.AutoSync();
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
            CoreSDK.player.IncrementFields();

            StartCoroutine(PlayTimeCounter());
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!CoreSDK.isInit) return;

            if (!focus && isFocus)
            {
                isFocus = false;
                OnFocusChange?.Invoke(isFocus);
                CoreSDK.game.SetAutoPause(true);
            }
            else if (focus && !isFocus)
            {
                isFocus = true;
                OnFocusChange?.Invoke(isFocus);
                CoreSDK.game.SetAutoPause(false);
            }
        }

    }
}
