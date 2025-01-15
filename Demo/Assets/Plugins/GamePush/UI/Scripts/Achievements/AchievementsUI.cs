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

        private event Action _OnAchievementsOpen;
        private event Action _OnAchievementsClose;

        public async void Init(FetchPlayerAchievementsOutput info, Action onAchievementsOpen, Action onAchievementsClose)
        {
            _info = info;

            _OnAchievementsOpen = onAchievementsOpen;
            _OnAchievementsClose = onAchievementsClose;

            StartCoroutine(MoveUp());

            ClearBoard();
            SetUpBoard();
            await SetUpCells();
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
            _achievementsTitle.text = CoreSDK.language.localization.achievements.title;
        }

        private async Task SetUpCells()
        {
            foreach(AchievementsGroup group in _info.AchievementsGroups)
            {
                Debug.Log(group.name);

                foreach(int id in group.achievements)
                {
                    Debug.Log(id);
                }
            }

            await Task.Delay(100);

            foreach(Achievement achievement in _info.Achievements)
            {
                Debug.Log(achievement.id);
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
