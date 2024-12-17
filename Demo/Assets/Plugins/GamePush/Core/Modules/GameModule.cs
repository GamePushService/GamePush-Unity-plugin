using System;
using GamePush;

namespace GamePush.Core
{
    public class GameModule
    {
        private bool isPaused;
        private bool isAutoPaused;
        private bool isGameplay;

        public bool gameReady = false;

        //private bool showLogs = true;

        public Action OnPause;
        public Action OnResume;

        public bool IsPaused()
        {
            bool gamePaused = isAutoPaused || isPaused;
            return gamePaused;
        }

        public void GamePause(Action onPauseCallback = null)
        {
            isPaused = true;
            if (isAutoPaused) return;

            onPauseCallback?.Invoke();
            OnPause?.Invoke();
        }

        public void GameResume(Action onResumeCallback = null)
        {
            isPaused = false;
            if (isAutoPaused) return;

            onResumeCallback?.Invoke();
            OnResume?.Invoke();
        }

        public bool IsAutoPaused() => isAutoPaused;

        public void SetAutoPause(bool isPause)
        {
            isAutoPaused = isPause;
            if (isPaused) return;

            if (isPause) OnPause?.Invoke();
            else OnResume?.Invoke();
        }

        public void GameReady() => gameReady = true;

        public void GameplayStart() => SetGameplay(true);
        public void GameplayStop() => SetGameplay(false);

        public void SetGameplay(bool gameplay) => isGameplay = gameplay;

        public void SetPause(bool pause) => isPaused = pause;

        public void HappyTime()
        {
            //Logger.Log("GAME:", "HAPPY TIME!!!");
        }
    }
}
