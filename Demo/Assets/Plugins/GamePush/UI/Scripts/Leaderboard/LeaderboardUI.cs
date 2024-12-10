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
        private GetOpenLeaderboardQuery _query;

        private event Action _OnLeaderboardOpen;
        private event Action _OnLeaderboardClose;

        public async void Init(RatingData ratingData, GetOpenLeaderboardQuery query, Action onLeaderboardOpen, Action onLeaderboardClose)
        {
            _ratingData = ratingData;
            _query = query;
            _OnLeaderboardOpen = onLeaderboardOpen;
            _OnLeaderboardClose = onLeaderboardClose;

            //print(ratingData.players.Count);
            //print(ratingData.topPlayers.Count);

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

            int holderCells = places.Count;
            bool inTop = CheckInTop();

            Enum.TryParse(_query.withMe, out WithMe withMe);

            int playerID = CoreSDK.player.GetID();
            int lastPlace = 0;

            //if (withMe == WithMe.first && inTop)
            //{
            //    Instantiate(_boardCell, _cellHolder).GetComponent<LeaderboardCell>().InitEmpty();
            //    holderCells++;
            //}

            SetCellHolder(holderCells);

            foreach (Dictionary<string, object> place in places)
            {
                PlayerRatingState playerRatingState = PlayerRatingState.FromDictionary(place);

                bool isPlayer = playerID == playerRatingState.id;

                bool needDivider =
                    !isPlayer &&
                    _query.showNearest > 0 &&
                    playerRatingState.position > _query.limit &&
                    _ratingData.countOfPlayersAbove >= _query.showNearest &&
                    lastPlace != playerRatingState.position &&
                    lastPlace + 1 != playerRatingState.position;

                if (needDivider)
                {
                    Instantiate(_divider, _cellHolder);
                }

                lastPlace = playerRatingState.position;

                LeaderboardCell leaderboardCell = Instantiate(_boardCell, _cellHolder).GetComponent<LeaderboardCell>();

                if (isPlayer)
                {
                    LeaderboardCell playerShowCell = _viewport.Init(withMe, inTop, leaderboardCell);
                    if(playerShowCell)
                        _viewport.InitPlayerCells(this, playerRatingState, isPlayer);
                }

                await leaderboardCell.Init(this, playerRatingState, isPlayer);
                
            }
        }

        private bool CheckInTop()
        {
            foreach(Dictionary<string, object> gamer in _ratingData.players)
            {
                if (gamer["id"] == _ratingData.player["id"])
                    return true;
            }
            return false;

            //var playerPosition = _ratingData.player["position"];
            //return Convert.ToInt32(playerPosition) <= _query.limit;
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
            Rect cellRect = _boardCell.GetComponent<RectTransform>().rect;
            Rect botBarRect = _botBar.GetComponent<RectTransform>().rect;
            // Cell width * Cell count + BotBar height
            holderRect.height = cellRect.height * cells + botBarRect.height + 50;
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
