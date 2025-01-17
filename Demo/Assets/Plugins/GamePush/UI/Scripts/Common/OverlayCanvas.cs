using System;
using System.Collections.Generic;
using UnityEngine;
using GamePush;
using GamePush.Data;
using GamePush.Tools;
using System.Threading.Tasks;

namespace GamePush.UI
{
    public class OverlayCanvas : MonoBehaviour
    {
        [SerializeField]
        private GameObject overlayHolder;
        [Space]
        [Header("[ UI panels ]")]
        [Space]
        [SerializeField]
        private LeaderboardUI leaderboard;
        [SerializeField]
        private SocialsUI socials;
        [SerializeField]
        private AchievementsUI achievements;

        [Space]
        [Header("[ Notification UI ]")]
        [SerializeField]
        AchievementPlate achievementPlate;

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
            CoreSDK.Leaderboard.OpenLeaderboard += OpenLeaderboard;
            CoreSDK.Socials.OnOpenOverlay += OpenSocials;

            CoreSDK.Achievements.OnShowAcievementsList += OpenAchivements;
            CoreSDK.Achievements.OnShowAcievementUnlock += UnlockAchievement;
            CoreSDK.Achievements.OnShowAcievementProgress += SetProgressAchievement;
        }

        private void OnDisable()
        {
            CoreSDK.Leaderboard.OpenLeaderboard -= OpenLeaderboard;
        }

        private void OnApplicationFocus(bool focus)
        {
            //if (!focus) Close();
        }

        public void Close()
        {
            overlayHolder.SetActive(false);
            foreach(Transform child in overlayHolder.GetComponentInChildren<Transform>())
            {
                Destroy(child.gameObject);
            }
        }

        public ModuleUI CreateUITable(ModuleUI UIcomponent)
        {
            Close();
            overlayHolder.SetActive(true);
            ModuleUI ui = Instantiate(UIcomponent, overlayHolder.transform).GetComponent<ModuleUI>();
            return ui;
        }

        public ModuleUI CreateUINotif(ModuleUI UIcomponent)
        {
            ModuleUI ui = Instantiate(UIcomponent, transform).GetComponent<ModuleUI>();
            return ui;
        }

        public void OpenLeaderboard(RatingData data, GetOpenLeaderboardQuery query, Action onLeaderboardOpen = null, Action onLeaderboardClose = null)
        {
            LeaderboardUI leaderboardUI = (LeaderboardUI)CreateUITable(leaderboard);
            leaderboardUI.Init(data, query, onLeaderboardOpen, onLeaderboardClose);
        }

        public void OpenSocials(string title, string text, string url, string image, Action<bool> callback)
        {
            SocialsUI socialsUI = (SocialsUI)CreateUITable(socials);
            socialsUI.Init(title, text, url, image, callback);
        }

        public void OpenAchivements(FetchPlayerAchievementsOutput info, AchievementsSettings settings, Action onAchievementsOpen = null, Action onAchievementsClose = null)
        {
            AchievementsUI achievementsUI = (AchievementsUI)CreateUITable(achievements);
            achievementsUI.Init(info, settings, onAchievementsOpen, onAchievementsClose);
        }

        private TaskQueue taskQueue = new TaskQueue();

        public async Task PlateAwait(Task plateShow, GameObject plate)
        {
            await plateShow;
            Destroy(plate);
        }

        public void UnlockAchievement(Achievement achievement)
        {
            AchievementPlate achievementPlateUI = (AchievementPlate)CreateUINotif(achievementPlate);
            Task show = achievementPlateUI.SetUnlock(achievement);
            taskQueue.Enqueue(() => PlateAwait(show, achievementPlateUI.gameObject));

        }

        public void SetProgressAchievement(Achievement achievement)
        {
            AchievementPlate achievementPlateUI = (AchievementPlate)CreateUINotif(achievementPlate);
            Task show = achievementPlateUI.SetProgress(achievement);
            taskQueue.Enqueue(() => PlateAwait(show, achievementPlateUI.gameObject));
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.T))
        //    {
        //        AchievementPlate achievementPlateUI = (AchievementPlate)CreateUINotif(achievementPlate);
        //        achievementPlateUI.TestPlate();
        //    }
        //}
    }
}

