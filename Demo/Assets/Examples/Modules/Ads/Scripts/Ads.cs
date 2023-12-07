using UnityEngine;
using UnityEngine.UI;

using GamePush;
using Examples.Console;

namespace Examples.Ads
{
    public class Ads : MonoBehaviour
    {
        [SerializeField] private Button _showFullscreenButton;
        [SerializeField] private Button _showRewardedButton;
        [SerializeField] private Button _showPreloaderButton;
        [SerializeField] private Button _showStickyButton;
        [SerializeField] private Button _closeStickyButton;
        [SerializeField] private Button _refreshStickyButton;
        [SerializeField] private Button _isButton;


        private void OnEnable()
        {
            _showFullscreenButton.onClick.AddListener(ShowFullscreen);
            _showRewardedButton.onClick.AddListener(ShowRewarded);
            _showPreloaderButton.onClick.AddListener(ShowPreloader);
            _showStickyButton.onClick.AddListener(ShowSticky);
            _closeStickyButton.onClick.AddListener(CloseSticky);
            _refreshStickyButton.onClick.AddListener(RefreshSticky);
            _isButton.onClick.AddListener(Is_Parametrs);

            GP_Ads.OnAdsStart += OnAdsStart;
            GP_Ads.OnAdsClose += OnAdsClose;

            GP_Ads.OnStickyStart += OnStickyStart;
            GP_Ads.OnStickyClose += OnStickyClose;
            GP_Ads.OnStickyRefresh += OnStickyRefresh;
            GP_Ads.OnStickyRender += OnStickyRender;
        }

        private void OnDisable()
        {
            _showFullscreenButton.onClick.RemoveListener(ShowFullscreen);
            _showRewardedButton.onClick.RemoveListener(ShowRewarded);
            _showPreloaderButton.onClick.RemoveListener(ShowPreloader);
            _showStickyButton.onClick.RemoveListener(ShowSticky);
            _closeStickyButton.onClick.RemoveListener(CloseSticky);
            _refreshStickyButton.onClick.RemoveListener(RefreshSticky);
            _isButton.onClick.RemoveListener(Is_Parametrs);

            GP_Ads.OnAdsStart -= OnAdsStart;
            GP_Ads.OnAdsClose -= OnAdsClose;

            GP_Ads.OnStickyStart -= OnStickyStart;
            GP_Ads.OnStickyClose -= OnStickyClose;
            GP_Ads.OnStickyRefresh -= OnStickyRefresh;
            GP_Ads.OnStickyRender -= OnStickyRender;
        }


        public void ShowFullscreen() => GP_Ads.ShowFullscreen(OnFullscreenStart, OnFullscreenClose);

        public void ShowPreloader() => GP_Ads.ShowPreloader(OnPreloaderStart, OnPreloaderClose);

        public void ShowRewarded() => GP_Ads.ShowRewarded("COINS", OnRewardedReward, OnRewardedStart, OnRewardedClose);

        public void ShowSticky() => GP_Ads.ShowSticky();
        public void RefreshSticky() => GP_Ads.RefreshSticky();
        public void CloseSticky() => GP_Ads.CloseSticky();

        public void Is_Parametrs()
        {
            ConsoleUI.Instance.Log("IS ADBLOCK ENABLED: " + GP_Ads.IsAdblockEnabled());

            ConsoleUI.Instance.Log("-----");

            ConsoleUI.Instance.Log("IS FULLSCREEN AVAILABLE: " + GP_Ads.IsFullscreenAvailable());
            ConsoleUI.Instance.Log("IS PRELOADER AVAILABLE: " + GP_Ads.IsPreloaderAvailable());
            ConsoleUI.Instance.Log("IS REWARDED AVAILABLE: " + GP_Ads.IsRewardedAvailable());
            ConsoleUI.Instance.Log("IS STICKY AVAILABLE: " + GP_Ads.IsStickyAvailable());

            ConsoleUI.Instance.Log("-----");

            ConsoleUI.Instance.Log("IS FULLSCREEN PLAYING: " + GP_Ads.IsFullscreenPlaying());
            ConsoleUI.Instance.Log("IS PRELOADER PLAYING: " + GP_Ads.IsPreloaderPlaying());
            ConsoleUI.Instance.Log("IS REWARDED PLAYING: " + GP_Ads.IsRewardPlaying());
            ConsoleUI.Instance.Log("IS STICKY PLAYING: " + GP_Ads.IsStickyPlaying());

            ConsoleUI.Instance.Log("-----");

            ConsoleUI.Instance.Log("Is Countdown Overlay Enabled: " + GP_Ads.IsCountdownOverlayEnabled());
            ConsoleUI.Instance.Log("Is Rewarded Failed Overlay Enabled: " + GP_Ads.IsRewardedFailedOverlayEnabled());
            ConsoleUI.Instance.Log("Can Show Fullscreen Before Gameplay: " + GP_Ads.CanShowFullscreenBeforeGamePlay());
        }


        private void OnAdsStart() => ConsoleUI.Instance.Log("ON ADS: START");
        private void OnAdsClose(bool success) => ConsoleUI.Instance.Log("ON ADS: CLOSE");

        private void OnFullscreenStart() => ConsoleUI.Instance.Log("ON FULLSCREEN START");
        private void OnFullscreenClose(bool success) => ConsoleUI.Instance.Log("ON FULLSCREEN CLOSE");

        private void OnPreloaderStart() => ConsoleUI.Instance.Log("ON PRELOADER: START");
        private void OnPreloaderClose(bool success) => ConsoleUI.Instance.Log("ON PRELOADER: CLOSE");

        private void OnRewardedStart() => ConsoleUI.Instance.Log("ON REWARDED: START");
        private void OnRewardedReward(string value)
        {
            if (value == "COINS")
                ConsoleUI.Instance.Log("ON REWARDED: +150 COINS");

            if (value == "GEMS")
                ConsoleUI.Instance.Log("ON REWARDED: +5 GEMS");
        }
        private void OnRewardedClose(bool success) => ConsoleUI.Instance.Log("ON REWARDED: CLOSE");

        private void OnStickyStart() => ConsoleUI.Instance.Log("ON STICKY: START");
        private void OnStickyClose() => ConsoleUI.Instance.Log("ON STICKY: CLOSE");
        private void OnStickyRender() => ConsoleUI.Instance.Log("ON STICKY: RENDER");
        private void OnStickyRefresh() => ConsoleUI.Instance.Log("ON STICKY: REFRESH");
    }
}