using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush;

namespace GamePush.UI
{
    public class OverlayCanvas : MonoBehaviour
    {
        [SerializeField]
        GameObject overlayHolder;
        [SerializeField]
        GameObject leaderboard;

        public static OverlayCanvas Controller;

        private void Awake()
        {
            if (Controller == null)
                Controller = this;
            else
                Destroy(this);

            DontDestroyOnLoad(gameObject);
            overlayHolder.SetActive(false);
        }

        private void OnEnable()
        {
            CoreSDK.leaderboard.OpenLeaderboard += OpenLeaderboard;
            //CoreSDK.game.OnPause += Close;
        }

        private void OnDisable()
        {
            CoreSDK.leaderboard.OpenLeaderboard -= OpenLeaderboard;
            //CoreSDK.game.OnPause -= Close;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus) Close();
        }

        public void Close()
        {
            overlayHolder.SetActive(false);
            foreach(Transform child in overlayHolder.GetComponentInChildren<Transform>())
            {
                Destroy(child.gameObject);
            }
        }

        public void OpenLeaderboard(RatingData data, WithMe withMe, Action onLeaderboardOpen = null, Action onLeaderboardClose = null)
        {
            overlayHolder.SetActive(true);

            LeaderboardUI leaderboardUI = Instantiate(leaderboard, overlayHolder.transform).GetComponent<LeaderboardUI>();
            leaderboardUI.Init(data, withMe, onLeaderboardOpen, onLeaderboardClose);

        }
    }
}
