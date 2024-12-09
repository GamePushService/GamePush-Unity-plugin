using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GamePush.UI
{
    public class LeaderboardViewport : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;

        [SerializeField]
        private LeaderboardCell _playerShowCellTop;
        [SerializeField]
        private LeaderboardCell _playerShowCellBot;

        private LeaderboardCell _playerShowCell;
        public LeaderboardCell _playerBoardCell;

        private bool _isInit;
        private bool _inTop;
        private WithMe _withMe;

        void Start()
        {
            if (_scrollRect != null)
            {
                _scrollRect.onValueChanged.AddListener(OnScrollChanged);
            }
        }

        public LeaderboardCell Init(WithMe withMe, bool inTop, LeaderboardCell cell)
        {
            //if (inTop) return null;

            _playerBoardCell = cell;
            _inTop = inTop;
            _withMe = withMe;

            switch (withMe)
            {
                case WithMe.none:
                    _playerShowCell = null;
                    break;
                case WithMe.first:
                    _playerShowCell = _playerShowCellTop;
                    break;
                case WithMe.last:
                    _playerShowCell = _playerShowCellBot;
                    break;
            }

            _isInit = true;

            return _playerShowCell;
        }

        public async void InitPlayerCells(LeaderboardUI leaderboard, PlayerRatingState playerState, bool isMyPlayer)
        {
            await _playerShowCellTop.Init(leaderboard, playerState, isMyPlayer);
            await _playerShowCellBot.Init(leaderboard, playerState, isMyPlayer);
        }

        private void OnScrollChanged(Vector2 position)
        {
            if(!_isInit || _playerShowCell == null) return;

            if (IsElementInView())
                DectiveCell();
            else
                ActiveCell();
        }

        private void DectiveCell()
        {
            _playerShowCell.gameObject.SetActive(false);
        }

        private void ActiveCell()
        {
           if (_inTop)
                _playerShowCell = IsElementAbove() ? _playerShowCellTop : _playerShowCellBot;
           


            _playerShowCell.gameObject.SetActive(true);
        }

        private bool IsElementInView()
        {
            Rect viewportRect = GetWorldRect(_scrollRect.viewport);
            Rect elementRect = GetWorldRect(_playerBoardCell.GetComponent<RectTransform>());

            Rect viewAdjust = GetAdjustedRect(viewportRect, elementRect);

            return viewAdjust.Overlaps(elementRect);
        }

        private bool IsElementAbove()
        {
            Rect viewportRect = GetWorldRect(_scrollRect.viewport);
            Rect elementRect = GetWorldRect(_playerBoardCell.GetComponent<RectTransform>());
            Rect viewAdjust = GetAdjustedRect(viewportRect, elementRect);

            return IsRectAbove(elementRect, viewAdjust);
        }

        public static bool IsRectAbove(Rect rect1, Rect rect2)
        {
            return rect1.yMin >= rect2.yMax;
        }

        private Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector3 bottomLeft = corners[0];
            Vector3 topRight = corners[2];

            return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
        }

        public static Rect GetAdjustedRect(Rect originalRect, Rect referenceRect)
        {
            float padding = referenceRect.height;

            float newHeight = originalRect.height - referenceRect.height * 2;

            Rect newRect = new Rect(
                originalRect.x,                        
                originalRect.y + padding,            
                originalRect.width,
                newHeight
            );

            return newRect;
        }
    }
}
