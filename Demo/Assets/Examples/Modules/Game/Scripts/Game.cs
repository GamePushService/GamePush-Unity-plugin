using UnityEngine;
using GamePush;
using Examples.Console;

namespace Examples.Game
{
    public class Game : MonoBehaviour
    {
        private void OnEnable()
        {
            GP_Game.OnPause += OnPause;
            GP_Game.OnResume += OnResume;
        }

        private void OnDisable()
        {
            GP_Game.OnPause -= OnPause;
            GP_Game.OnResume -= OnResume;
        }

        public void IsPaused()
        {
            bool isPaused = GP_Game.IsPaused();
            ConsoleUI.Instance.Log($"GAME IS PAUSED: {isPaused}");
        }

        public void Pause() => GP_Game.Pause(OnPause);
        public void Resume() => GP_Game.Resume(OnResume);

        public void GameReady()
        {
            GP_Game.GameReady();
            ConsoleUI.Instance.Log("GAME: READY");
        }

        public void GamePlayStart()
        {
            GP_Game.GameplayStart();
            ConsoleUI.Instance.Log("GAME: GAMEPLAY START");
        }

        public void GamePlayStop()
        {
            GP_Game.GameplayStop();
            ConsoleUI.Instance.Log("GAME: GAMEPLAY STOP");
        }


        private void OnPause() => ConsoleUI.Instance.Log("GAME: ON PAUSE");
        private void OnResume() => ConsoleUI.Instance.Log("GAME: ON RESUME");
    }
}