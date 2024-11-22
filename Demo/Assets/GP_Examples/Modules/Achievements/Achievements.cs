using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

using GamePush;
using Examples.Console;

namespace Examples.Achievements
{
    public class Achievements : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _idOrTag;
        [Space]
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _unlockButton;
        [SerializeField] private Button _fetchButton;
        [SerializeField] private Button _hasButton;
        [SerializeField] private Button _setProgressButton;
        [SerializeField] private Button _getProgressButton;


        private void OnEnable()
        {
            _openButton.onClick.AddListener(Open);
            _unlockButton.onClick.AddListener(Unlock);
            _fetchButton.onClick.AddListener(Fetch);
            _hasButton.onClick.AddListener(Has);
            _setProgressButton.onClick.AddListener(SetProgress);
            _getProgressButton.onClick.AddListener(GetProgress);

            GP_Achievements.OnAchievementsFetch += OnFetchSuccess;
            GP_Achievements.OnAchievementsFetchError += OnFetchError;
            GP_Achievements.OnAchievementsFetchGroups += OnFetchGroups;
            GP_Achievements.OnAchievementsFetchPlayer += OnFetchPlayer;
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(Open);
            _unlockButton.onClick.RemoveListener(Unlock);
            _fetchButton.onClick.RemoveListener(Fetch);
            _hasButton.onClick.RemoveListener(Has);
            _setProgressButton.onClick.RemoveListener(SetProgress);
            _getProgressButton.onClick.RemoveListener(GetProgress);

            GP_Achievements.OnAchievementsFetch -= OnFetchSuccess;
            GP_Achievements.OnAchievementsFetchError -= OnFetchError;
            GP_Achievements.OnAchievementsFetchGroups -= OnFetchGroups;
            GP_Achievements.OnAchievementsFetchPlayer -= OnFetchPlayer;
        }

        public void Open() => GP_Achievements.Open(OnOpen, OnClose);
        public void Unlock() => GP_Achievements.Unlock(_idOrTag.text, OnUnlock, OnUnlockError);
        private void SetProgress() => GP_Achievements.SetProgress(_idOrTag.text, 25, OnPogress, OnProgressError);
        private void GetProgress() => ConsoleUI.Instance.Log("PROGRESS: " + GP_Achievements.GetProgress(_idOrTag.text));
        private void Has() => ConsoleUI.Instance.Log("HAS: " + GP_Achievements.Has(_idOrTag.text));
        private void Fetch() => GP_Achievements.Fetch();



        private void OnOpen() => ConsoleUI.Instance.Log("ON OPEN");
        private void OnClose() => ConsoleUI.Instance.Log("ON CLOSE");

        private void OnUnlock(string achievement) => ConsoleUI.Instance.Log("UNLCOK SUCCESS: " + achievement);
        private void OnUnlockError(string error) => ConsoleUI.Instance.Log("UNLOCK ERROR: " + error);

        private void OnPogress(string idOrTag) => ConsoleUI.Instance.Log("ON PROGRESS: SUCCESS: " + idOrTag);
        private void OnProgressError() => ConsoleUI.Instance.Log("PROGRESS: ERROR");

        private void OnFetchSuccess(List<AchievementsFetch> achievements)
        {
            ConsoleUI.Instance.Log("FETCH: SUCCESS");

            for (int i = 0; i < achievements.Count; i++)
            {
                ConsoleUI.Instance.Log("ID: " + achievements[i].id);
                ConsoleUI.Instance.Log("TAG: " + achievements[i].tag);
                ConsoleUI.Instance.Log("NAME: " + achievements[i].name);
                ConsoleUI.Instance.Log("DESCRIPTION: " + achievements[i].description);
                ConsoleUI.Instance.Log("ICON: " + achievements[i].icon);
                ConsoleUI.Instance.Log("ICON SMALL: " + achievements[i].iconSmall);
                ConsoleUI.Instance.Log("LOCKED ICON: " + achievements[i].lockedIcon);
                ConsoleUI.Instance.Log("LOCKED ICON SMALL: " + achievements[i].lockedIconSmall);
                ConsoleUI.Instance.Log("RARE: " + achievements[i].rare);
                ConsoleUI.Instance.Log("MAX PROGRESS: " + achievements[i].maxProgress);
                ConsoleUI.Instance.Log("PROGRESS STEP: " + achievements[i].progressStep);
                ConsoleUI.Instance.Log("LOCKED VISIBLE: " + achievements[i].lockedVisible);
                ConsoleUI.Instance.Log("LOCKED DESCRIPTION VISIBLE: " + achievements[i].lockedDescriptionVisible);
            }
        }

        private void OnFetchGroups(List<AchievementsFetchGroups> data)
        {
            ConsoleUI.Instance.Log("FETCH: GROUP: SUCCESS");

            for (int i = 0; i < data.Count; i++)
            {
                ConsoleUI.Instance.Log("ID: " + data[i].id);
                ConsoleUI.Instance.Log("TAG: " + data[i].tag);
                ConsoleUI.Instance.Log("NAME: " + data[i].name);
                ConsoleUI.Instance.Log("DESCRIPTION: " + data[i].description);

                for (int x = 0; x < data[i].achievements.Length; x++)
                {
                    ConsoleUI.Instance.Log("ACHIEVEMENTS COUNT: " + data[i].achievements[x]);
                }
            }
        }

        private void OnFetchPlayer(List<AchievementsFetchPlayer> data)
        {
            ConsoleUI.Instance.Log("FETCH: PLAYER ACHIEVEMENTS: SUCCESS");

            for (int i = 0; i < data.Count; i++)
            {
                ConsoleUI.Instance.Log("ACHIEVEMENT ID: " + data[i].achievementId);
                ConsoleUI.Instance.Log("CREATED AT: " + data[i].createdAt);
                ConsoleUI.Instance.Log("PROGRESS: " + data[i].progress);
                ConsoleUI.Instance.Log("UNLOCKED: " + data[i].unlocked);
            }
        }

        private void OnFetchError() => ConsoleUI.Instance.Log("FETCH: ERROR");
    }
}