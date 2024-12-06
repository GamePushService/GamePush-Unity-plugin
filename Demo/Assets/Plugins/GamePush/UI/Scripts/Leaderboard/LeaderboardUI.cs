using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;
using TMPro;
using System.Threading.Tasks;

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
        private LeaderboardViewport _viewport;
        [SerializeField]
        private GameObject _botBar;
        [SerializeField]
        private GameObject _divider;
        [SerializeField]
        private RectTransform _cellHolder;

        private RatingData _ratingData;
        private WithMe _withMe;

        private event Action _OnLeaderboardOpen;
        private event Action _OnLeaderboardClose;

        public async void Init(RatingData ratingData, WithMe withMe, Action onLeaderboardOpen, Action onLeaderboardClose)
        {
            _ratingData = ratingData;
            _withMe = withMe;
            _OnLeaderboardOpen = onLeaderboardOpen;
            _OnLeaderboardClose = onLeaderboardClose;

            StartCoroutine(MoveUp());

            SetUpBoard();
            await SetUpCells();
            SetBotBar();
        }

        private void SetUpBoard()
        {
            _leaderboardTitle.text = _ratingData.leaderboard.name;
            ClearBoard();
        }

        private void ClearBoard()
        {
            foreach (Transform child in _cellHolder.GetComponentInChildren<Transform>())
            {
                Destroy(child.gameObject);
            }
        }

        private async Task SetUpCells()
        {
            List<Dictionary<string, object>> places = _ratingData.players;

            SetCellHolder(places.Count);

            int playerID = CoreSDK.player.GetID();
            int lastPlace = 0;

            foreach (Dictionary<string, object> place in places)
            {
                PlayerRatingState playerRatingState = PlayerRatingState.FromDictionary(place);

                bool isPlayer = playerID == playerRatingState.id;

                if (!isPlayer && lastPlace + 1 != playerRatingState.position)
                {
                    if(lastPlace != playerRatingState.position)
                        Instantiate(_divider, _cellHolder);
                }

                lastPlace = playerRatingState.position;

                LeaderboardCell leaderboardCell = Instantiate(_boardCell, _cellHolder).GetComponent<LeaderboardCell>();

                if (isPlayer)
                {
                    LeaderboardCell playerShowCell = _viewport.Init(_withMe, leaderboardCell);
                    if(playerShowCell)
                        await playerShowCell.Init(this, playerRatingState, isPlayer);
                }

                await leaderboardCell.Init(this, playerRatingState, isPlayer);
                
            }
        }

        //public void ShowPlayerCell(bool isShow)
        //{
        //    _playerBoardCell.gameObject.SetActive(isShow);
        //}

        //private bool IsNeedDivider()
        //{
        //    bool isNeedToShowDivider = false;
        //    bool isGapBetweenPlayers = (player?.position as number) + 1 !== players[index + 1]?.position && index !== players.length - 1;

        //    if (_ratingData.countOfPlayersAbove > 0)
        //    {
        //        isNeedToShowDivider = showNearest && _ratingData.countOfPlayersAbove >= showNearest && isGapBetweenPlayers;
        //    }

        //    return isNeedToShowDivider;
        //}

        private void SetCellHolder(int cells)
        {
            Rect holderRect = _cellHolder.rect;

            // Cell width * Cell count + BotBar height
            holderRect.height = 150 * cells + 300;
            _cellHolder.sizeDelta = new Vector2(holderRect.width, holderRect.height);
        }


        private void SetBotBar()
        {
            Instantiate(_botBar, _cellHolder);
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
