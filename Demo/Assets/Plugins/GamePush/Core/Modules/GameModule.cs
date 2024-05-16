using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.Core
{
    public class GameModule
    {
        private bool isPaused;
        private bool isAutoPaused;
        private bool isGameplay;
        //private bool isGameStarted;

        public bool gameReady = false;

        //private bool showLogs = true;

        public bool IsPaused()
        {
            Console.Log("GAME: IS PAUSED: ", isPaused.ToString());
            return isPaused;
        }

        public void GamePause(Action onPause = null)
        {
            isPaused = true;
            onPause?.Invoke();
            Console.Log("GAME: ", "PAUSE");
        }

        public void GameResume(Action onResume = null)
        {
            if (isAutoPaused) return;

            isPaused = false;
            onResume?.Invoke();
            Console.Log("GAME: ", "RESUME");
        }

        public bool IsAutoPaused() => isAutoPaused;

        public void SetAutoPause(bool isPause)
        {
            isAutoPaused = isPause;
            Console.Log("GAME: ", isPause ? "PAUSE" : "RESUME");
        }

        public void GameReady()
        {
            gameReady = true;
            Console.Log("GAME:", "READY");
        }

        public void GameplayStart() => SetGameplay(true);
        public void GameplayStop() => SetGameplay(false);

        public void SetGameplay(bool gameplay)
        {
            isGameplay = gameplay;
            Console.Log("GAMEPLAY: ", isGameplay ? "START" : "STOP");
        }

        public void SetPause(bool pause)
        {
            isPaused = pause;
        }

        public void HappyTime()
        {
            Console.Log("GAME:", "HAPPY TIME!!!");
        }
    }
}
