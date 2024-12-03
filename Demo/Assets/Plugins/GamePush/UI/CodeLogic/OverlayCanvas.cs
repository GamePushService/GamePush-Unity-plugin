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
            
        }

        public void Close()
        {
            overlayHolder.SetActive(false);
            foreach(Transform child in overlayHolder.GetComponentInChildren<Transform>())
            {
                Destroy(child.gameObject);
            }
        }

        public void OpenLeaderboard(RatingData data, Action onLeaderboardOpen = null, Action onLeaderboardClose = null)
        {
            overlayHolder.SetActive(true);

            LeaderboardUI leaderboardUI = Instantiate(leaderboard, overlayHolder.transform).GetComponent<LeaderboardUI>();
            leaderboardUI.Init(data, onLeaderboardOpen, onLeaderboardClose);

        }
    }
}

