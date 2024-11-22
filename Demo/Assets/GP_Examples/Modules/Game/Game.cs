using UnityEngine;
using GamePush;
using UnityEngine.UI;

using Examples.Console;

namespace Examples.Game
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _isPausedButton;
        [SerializeField] private Button _gameReadyButton;
        [SerializeField] private Button _gamePlayStartButton;
        [SerializeField] private Button _gamePlayStopButton;
        [SerializeField] private Button _happyTimeButton;
       

        private void OnEnable()
        {
            _pauseButton.onClick.AddListener(Pause);
            _resumeButton.onClick.AddListener(Resume);
            _isPausedButton.onClick.AddListener(IsPaused);
            _gameReadyButton.onClick.AddListener(GameReady);
            _gamePlayStartButton.onClick.AddListener(GamePlayStart);
            _gamePlayStopButton.onClick.AddListener(GamePlayStop);
            _happyTimeButton.onClick.AddListener(HappyTime);

            GP_Game.OnPause += OnPause;
            GP_Game.OnResume += OnResume;
        }

        private void OnDisable()
        {
            _pauseButton.onClick.RemoveListener(Pause);
            _resumeButton.onClick.RemoveListener(Resume);
            _isPausedButton.onClick.RemoveListener(IsPaused);
            _gameReadyButton.onClick.RemoveListener(GameReady);
            _gamePlayStartButton.onClick.RemoveListener(GamePlayStart);
            _gamePlayStopButton.onClick.RemoveListener(GamePlayStop);
            _happyTimeButton.onClick.RemoveListener(HappyTime);

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

        public void HappyTime()
        {
            GP_Game.HappyTime();
            ConsoleUI.Instance.Log("HAPPY TIME!!!");
        }


        private void OnPause() => ConsoleUI.Instance.Log("GAME: ON PAUSE");
        private void OnResume() => ConsoleUI.Instance.Log("GAME: ON RESUME");
    }
}