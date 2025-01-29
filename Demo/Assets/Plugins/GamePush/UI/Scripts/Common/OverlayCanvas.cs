using System;
using System.Collections.Generic;
using UnityEngine;
using GamePush;
using GamePush.Data;
using GamePush.Tools;
using System.Threading.Tasks;
using UnityEngine.Serialization;

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
        private ModuleUI leaderboard;
        [SerializeField]
        private ModuleUI socials;
        [SerializeField]
        private ModuleUI achievements;
        [SerializeField]
        private ModuleUI gamesCollection;
        [SerializeField]
        private ModuleUI document;

        [Space]
        [Header("[ Notification UI ]")]
        [SerializeField]
        AchievementPlate achievementPlate;

        public static OverlayCanvas Controller;
        private TaskQueue _taskQueue;

        private void Awake()
        {
            if (Controller == null)
                Controller = this;
            else
                Destroy(this);

            DontDestroyOnLoad(gameObject);
            overlayHolder.SetActive(false);

            _taskQueue = new TaskQueue();
        }

        private void OnEnable()
        {
            CoreSDK.Leaderboard.OpenLeaderboard += OpenLeaderboard;
            CoreSDK.Socials.OnOpenOverlay += OpenSocials;

            CoreSDK.Achievements.OnShowAcievementsList += OpenAchievements;
            CoreSDK.Achievements.OnShowAcievementUnlock += UnlockAchievement;
            CoreSDK.Achievements.OnShowAcievementProgress += SetProgressAchievement;

            CoreSDK.GameCollections.OnShowGamesCollection += OpenGamesCollection;
            CoreSDK.Documents.OnShowDocument += OpenDocument;
        }
        
        private void OnDisable()
        {
            CoreSDK.Leaderboard.OpenLeaderboard -= OpenLeaderboard;
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

        #region UI Tables
        private ModuleUI CreateUITable(ModuleUI UIcomponent)
        {
            Close();
            overlayHolder.SetActive(true);
            ModuleUI ui = Instantiate(UIcomponent, overlayHolder.transform).GetComponent<ModuleUI>();
            return ui;
        }

        private void OpenLeaderboard(RatingData data, GetOpenLeaderboardQuery query, Action onLeaderboardOpen = null, Action onLeaderboardClose = null)
        {
            LeaderboardUI leaderboardUI = (LeaderboardUI)CreateUITable(leaderboard);
            leaderboardUI.Show(data, query, onLeaderboardOpen, onLeaderboardClose);
        }

        private void OpenSocials(string title, string text, string url, string image, Action<bool> callback)
        {
            SocialsUI socialsUI = (SocialsUI)CreateUITable(socials);
            socialsUI.Show(title, text, url, image, callback);
        }

        private void OpenAchievements(FetchPlayerAchievementsOutput info, AchievementsSettings settings, 
            Action onAchievementsOpen = null, Action onAchievementsClose = null)
        {
            AchievementsUI achievementsUI = (AchievementsUI)CreateUITable(achievements);
            achievementsUI.Show(info, settings, onAchievementsOpen, onAchievementsClose);
        }

        private void OpenGamesCollection(GamesCollection collection, Action onCollectionOpen = null,
            Action onCollectionClose = null)
        {
            GamesCollectionUI gamesCollectionUI = (GamesCollectionUI)CreateUITable(gamesCollection); 
            gamesCollectionUI.Show(collection, onCollectionOpen, onCollectionClose);
        }
        
        private void OpenDocument(DocumentData documentData, Action onDocumentOpen = null,
            Action onDocumentClose = null)
        {
            DocumentUI documentUI = (DocumentUI)CreateUITable(document);
            documentUI.Show(documentData, onDocumentOpen, onDocumentClose);
        }

        #endregion

        #region UI Plates
        private async Task PlateAwait(Task plateShow, AchievementPlate plate)
        {
            await plateShow;
            if (plate != null && plate.gameObject != null)
            {
                Destroy(plate.gameObject);
            }
        }

        public void UnlockAchievement(Achievement achievement)
        {
            AchievementPlate achievementPlateUI = Instantiate(achievementPlate, transform);

            _taskQueue.Enqueue(async () =>
            {
                Task show = achievementPlateUI.SetUnlock(achievement);
                await PlateAwait(show, achievementPlateUI);
            });
        }

        public void SetProgressAchievement(Achievement achievement)
        {
            AchievementPlate achievementPlateUI = Instantiate(achievementPlate, transform);

            _taskQueue.Enqueue(async () =>
            {
                Task show = achievementPlateUI.SetProgress(achievement);
                await PlateAwait(show, achievementPlateUI);
            });
        }
        #endregion
        
    }
}

