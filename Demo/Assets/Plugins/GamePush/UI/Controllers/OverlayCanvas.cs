using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        private void Start()
        {
            //OpenLeaderboard();
        }

        public void Close()
        {
            overlayHolder.SetActive(false);
            foreach(Transform child in overlayHolder.GetComponentInChildren<Transform>())
            {
                Destroy(child.gameObject);
            }
        }

        public void OpenLeaderboard()
        {
            overlayHolder.SetActive(true);
            Instantiate(leaderboard, overlayHolder.transform);
        }
    }
}

