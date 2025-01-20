using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePush.Data;
using TMPro;
using UnityEngine;
using GamePush;

namespace GamePush.UI
{
    public class AchievementsUI : ModuleUI
    {
        [SerializeField]
        private float _startOffset = 400f;
        [SerializeField]
        private float _moveUpSpeed = 1500f;

        [Space]
        [SerializeField]
        private TMP_Text _achievementsTitle;

        [Space]
        [SerializeField]
        private AchievementsProgressInfo _progressInfo;
        [SerializeField]
        private AchievementGroupCell _achievementsGroupCell;
        [SerializeField]
        private AchievementCell _achievementCell;
        [SerializeField]
        private RectTransform _cellHolder;


        private FetchPlayerAchievementsOutput _info;
        private AchievementsSettings _settings;

        private Dictionary<int, Achievement> _allAchievements;
        private Dictionary<int, PlayerAchievement> _allPlayerAchievements;
        private Dictionary<int, Achievement> _tempAchievements;

        private event Action _OnAchievementsOpen;
        private event Action _OnAchievementsClose;

        public void Init(FetchPlayerAchievementsOutput info, AchievementsSettings settings, Action onAchievementsOpen, Action onAchievementsClose)
        {
            _info = info;
            _settings = settings;
            _allAchievements = new Dictionary<int, Achievement>();
            _allPlayerAchievements = new Dictionary<int, PlayerAchievement>();

            
            foreach (PlayerAchievement achievement in _info.PlayerAchievements)
                _allPlayerAchievements[achievement.achievementId] = achievement;

            foreach (Achievement achievement in _info.Achievements)
            {
                int id = achievement.id;
                achievement.unlocked = _allPlayerAchievements.ContainsKey(id) ? _allPlayerAchievements[id].unlocked : false;
                achievement.progress = _allPlayerAchievements.ContainsKey(id) ? _allPlayerAchievements[id].progress : 0;
                _allAchievements[achievement.id] = achievement;
            }


            _OnAchievementsOpen = onAchievementsOpen;
            _OnAchievementsClose = onAchievementsClose;

            StartCoroutine(MoveUp());

            ClearBoard();
            SetUpBoard();

            SetUpCells();
        }


        private void ClearBoard()
        {
            foreach (Transform child in _cellHolder.GetComponentInChildren<Transform>())
            {
                Destroy(child.gameObject);
            }
        }

        private void SetUpBoard()
        {
            _achievementsTitle.text = CoreSDK.Language.localization.achievements.title;

            AchievementsProgressInfo progressInfo = Instantiate(_progressInfo, _cellHolder).GetComponent<AchievementsProgressInfo>();
            int all = _info.Achievements.Count;
            int unlocked = GetUnlockedAchievements();
            progressInfo.SetInfo(unlocked, all);

            SetCellHolder(_info.AchievementsGroups.Count, _info.Achievements.Count);
        }

        private void SetCellHolder(int groups, int cells)
        {
            Rect holderRect = _cellHolder.rect;
            Rect topInfoRect = _progressInfo.GetComponent<RectTransform>().rect;
            Rect cellRect = _achievementCell.GetComponent<RectTransform>().rect;
            Rect groupRect = _achievementsGroupCell.GetComponent<RectTransform>().rect;

            //ProgressInfo height + Cell height * Cell count + Group width * Group count
            holderRect.height = topInfoRect.height + cellRect.height * cells + groupRect.height * groups + 50;
            _cellHolder.sizeDelta = new Vector2(holderRect.width, holderRect.height);
        }

        private int GetUnlockedAchievements(List<int> ids = null)
        {
            int counter = 0;
            foreach (PlayerAchievement playerAchievement in _info.PlayerAchievements)
            {
                if (ids != null && !ids.Contains(playerAchievement.achievementId))
                    continue;

                if (playerAchievement.unlocked)
                    counter++;
            }

            return counter;
        }

        private void SetUpCells()
        {
            foreach(AchievementsGroup group in _info.AchievementsGroups)
            {
                AchievementGroupCell groupCell = Instantiate(_achievementsGroupCell, _cellHolder).GetComponent<AchievementGroupCell>();
                int unlocked = GetUnlockedAchievements(group.achievements);
                groupCell.SetUp(group, unlocked);

                foreach (int id in group.achievements)
                {
                    Achievement achievement = _allAchievements[id];

                    //Debug.Log(achievement.name + " | " + achievement.unlocked + " | " + achievement.progress);
                    if (!achievement.unlocked && !_settings.isLockedVisible)
                        continue;
                    if (!achievement.unlocked && !_settings.isLockedDescriptionVisible)
                        achievement.description = "";

                    AchievementCell achievementCell = Instantiate(_achievementCell, _cellHolder).GetComponent<AchievementCell>();
                    achievementCell.SetUp(achievement);
                }
            }
        }

        private IEnumerator MoveUp()
        {
            Vector3 endPos = transform.position;
            Vector3 startPos = endPos;
            startPos.y -= _startOffset;

            transform.position = startPos;

            while (transform.position.y < endPos.y)
            {
                transform.Translate(Vector2.up * _moveUpSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            _OnAchievementsOpen?.Invoke();
        }

        public void Close()
        {
            _OnAchievementsClose?.Invoke();
            OverlayCanvas.Controller.Close();
        }
    }
}
