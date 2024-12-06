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
        private bool isInit;

        void Start()
        {
            if (_scrollRect != null)
            {
                _scrollRect.onValueChanged.AddListener(OnScrollChanged);
            }
        }

        public LeaderboardCell Init(WithMe withMe, LeaderboardCell cell)
        {
            _playerBoardCell = cell;

            switch (withMe)
            {
                case WithMe.none:
                    break;
                case WithMe.first:
                    _playerShowCell = _playerShowCellTop;
                    break;
                case WithMe.last:
                    _playerShowCell = _playerShowCellBot;
                    break;
            }

            isInit = true;

            return _playerShowCell;
        }

        private void OnScrollChanged(Vector2 position)
        {
            if (!isInit) return;
            if (IsElementInView())
            {
                Debug.Log("Element is visible!");
                _playerShowCell.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Element is not visible.");
                _playerShowCell.gameObject.SetActive(true);
            }
        }

        private bool IsElementInView()
        {
            Rect viewportRect = GetWorldRect(_scrollRect.viewport);
            Rect elementRect = GetWorldRect(_playerBoardCell.GetComponent<RectTransform>());

            return viewportRect.Overlaps(elementRect);
        }

        private Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector3 bottomLeft = corners[0];
            Vector3 topRight = corners[2];

            return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
        }
    }
}
