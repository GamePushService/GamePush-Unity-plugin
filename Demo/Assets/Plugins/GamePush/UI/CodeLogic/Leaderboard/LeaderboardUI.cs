using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;
using TMPro;

namespace GamePush.UI
{
    public class LeaderboardUI : MonoBehaviour
    {
        [SerializeField]
        private float _startOffset = 400f;
        [SerializeField]
        private float _moveUpSpeed = 1500f;

        [Space]
        [SerializeField]
        private TMP_Text _leaderboardTitle;

        [Space]
        [SerializeField]
        private LeaderboardCell _boardCell;
        [SerializeField]
        private RectTransform _cellHolder;

        private RatingData _ratingData;

        private event Action _OnLeaderboardOpen;
        private event Action _OnLeaderboardClose;

        public void Init(RatingData ratingData, Action onLeaderboardOpen, Action onLeaderboardClose)
        {
            _ratingData = ratingData;
            _OnLeaderboardOpen = onLeaderboardOpen;
            _OnLeaderboardClose = onLeaderboardClose;

            SetUpBoard();
            SetUpCells();

            StartCoroutine(MoveUp());
        }

        private void SetUpBoard()
        {
            _leaderboardTitle.text = _ratingData.leaderboard.name;
        }

        private async void SetUpCells()
        {
            List<Dictionary<string, object>> places = _ratingData.players;

            SetCellHolder(places.Count);

            foreach (Dictionary<string, object> place in places)
            {
                PlayerRatingState playerRatingState = PlayerRatingState.FromDictionary(place);

                LeaderboardCell leaderboardCell = Instantiate(_boardCell, _cellHolder).GetComponent<LeaderboardCell>();

                await leaderboardCell.Init(playerRatingState);
            }
        }

        private void SetCellHolder(int cells)
        {
            Rect holderRect = _cellHolder.rect;

            // Cell width * Cell count + BotBar height
            holderRect.height = 150 * cells + 240;
            _cellHolder.sizeDelta = new Vector2(holderRect.width, holderRect.height);
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

            _OnLeaderboardOpen?.Invoke();
        }

        public void Close()
        {
            _OnLeaderboardClose?.Invoke();
            OverlayCanvas.Controller.Close();
        }
    }

}
