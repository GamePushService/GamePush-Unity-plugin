using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush;
using GamePush.Data;

namespace GamePush.UI
{
    public class OverlayCanvas : MonoBehaviour
    {
        [SerializeField]
        GameObject overlayHolder;
        [Space]
        [Header("[ UI panels ]")]
        [Space]
        [SerializeField]
        LeaderboardUI leaderboard;
        [SerializeField]
        SocialsUI socials;

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

            //CoreSDK.socials.
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

        public void OpenLeaderboard(RatingData data, GetOpenLeaderboardQuery query, Action onLeaderboardOpen = null, Action onLeaderboardClose = null)
        {
            overlayHolder.SetActive(true);
            LeaderboardUI leaderboardUI = Instantiate(leaderboard, overlayHolder.transform).GetComponent<LeaderboardUI>();
            leaderboardUI.Init(data, query, onLeaderboardOpen, onLeaderboardClose);

        }

        public void OpenSocials(ShareType type)
        {
            overlayHolder.SetActive(true);
            SocialsUI socialsUI = Instantiate(socials, overlayHolder.transform).GetComponent<SocialsUI>();
            socialsUI.Init(type);
        }
    }
}

