using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace GamePush.Services
{
    public class GameStateService : MonoBehaviour
    {
        private bool isFocus;
        public event Action<bool> OnFocusChange;
        private float tickTime = 0.5f;

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
            StartCoroutine(Ping());
            StartCoroutine(Tick());
        }


        IEnumerator Tick()
        {
            yield return new WaitForSecondsRealtime(tickTime);

            CoreSDK.AddPlayTime(tickTime);
            CoreSDK.Player.IncrementFields();
            CoreSDK.Player.AutoSync();

            StartCoroutine(Tick());
        }

        IEnumerator Ping()
        {
            yield return new WaitForSecondsRealtime(10);
            CoreSDK.Player.Ping();

            //Debug.Log(CoreSDK.player.GetPlaytimeToday());
            StartCoroutine(Ping());
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!CoreSDK.isInit) return;

            if (!focus && isFocus)
            {
                isFocus = false;
                OnFocusChange?.Invoke(isFocus);
                CoreSDK.Game.SetAutoPause(true);
            }
            else if (focus && !isFocus)
            {
                isFocus = true;
                OnFocusChange?.Invoke(isFocus);
                CoreSDK.Game.SetAutoPause(false);
            }
        }

    }
}
