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

        public void IsPaused() => GP_Game.IsPaused();

        public void Pause() => GP_Game.Pause(OnPause);
        public void Resume() => GP_Game.Resume(OnResume);

        public void GameReady() => GP_Game.GameReady();

        public void GamePlayStart() => GP_Game.GameplayStart();
        public void GamePlayStop() => GP_Game.GameplayStop();


        private void OnPause() => ConsoleUI.Instance.Log("GAME: ON PAUSE");
        private void OnResume() => ConsoleUI.Instance.Log("GAME: ON RESUME");
    }
}