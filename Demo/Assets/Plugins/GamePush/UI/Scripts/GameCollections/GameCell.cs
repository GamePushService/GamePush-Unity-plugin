using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using GamePush.Tools;
using GamePush.Data;
using GamePush.Localization;

namespace GamePush.UI
{
    public class GameCell : MonoBehaviour
    {
        [SerializeField]
        private Image logo;
        [SerializeField]
        private TMP_Text title, description;
        
        private Button _button;
        private GamePreview _project;
        
        public async void SetUp(GamePreview game)
        {
            _project = game;
            title.text = game.Name;
            description.text = game.Description;

            _button = GetComponent<Button>();
            _button.onClick.AddListener(OpenGame);
        
            string iconLink = UtilityImage.ReplaceWebpWithPng(game.Icon);
            await UtilityImage.DownloadImageAsync(iconLink, logo);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OpenGame()
        {
            if (_project.Url != "")
            {
                Application.OpenURL(_project.Url);
            }
        }
    }
}
