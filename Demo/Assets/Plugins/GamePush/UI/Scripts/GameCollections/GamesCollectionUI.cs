using System;
using System.Collections;
using System.Collections.Generic;
using GamePush.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePush.UI
{
    public class GamesCollectionUI : ModuleUI
    {
        [SerializeField] private float startOffset = 400f;
        [SerializeField] private float moveUpSpeed = 1500f;

        [Space] [SerializeField] private TMP_Text collectionTitle, collectionDescription;

        [Space] [SerializeField] private GameCell gameCell;
        [SerializeField] private RectTransform cellHolder;

        private GamesCollection _gamesCollection;
        private event Action OnGamesCollectionOpen;
        private event Action OnGamesCollectionClose;

        public void Show(GamesCollection gamesCollection, Action onGamesCollectionOpen, Action onGamesCollectionClose)
        {
            _gamesCollection = gamesCollection;

            OnGamesCollectionOpen = onGamesCollectionOpen;
            OnGamesCollectionClose = onGamesCollectionClose;

            StartCoroutine(MoveUp());

            ClearBoard();
            SetUpBoard();

            SetUpCells();
        }


        private void ClearBoard()
        {
            foreach (Transform child in cellHolder.GetComponentInChildren<Transform>())
            {
                Destroy(child.gameObject);
            }
        }

        private void SetUpBoard()
        {
            collectionTitle.text = _gamesCollection.Name;
            collectionDescription.text = _gamesCollection.Description;
            
            SetCellHolder(_gamesCollection.GamesData.Count);
        }

        private void SetCellHolder(int cells)
        {
            Rect holderRect = cellHolder.rect;
            Rect cellRect = gameCell.GetComponent<RectTransform>().rect;

            //ProgressInfo height + Cell height * Cell count + Group width * Group count
            holderRect.height = cellRect.height * cells + 50;
            cellHolder.sizeDelta = new Vector2(holderRect.width, holderRect.height);
        }

        private void SetUpCells()
        {
            foreach (GamePreview game in _gamesCollection.GamesData)
            {
                GameCell cell = Instantiate(gameCell, cellHolder);
                cell.SetUp(game);
            }
        }

        private IEnumerator MoveUp()
        {
            Vector3 endPos = transform.position;
            Vector3 startPos = endPos;
            startPos.y -= startOffset;

            transform.position = startPos;

            while (transform.position.y < endPos.y)
            {
                transform.Translate(Vector2.up * moveUpSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            OnGamesCollectionOpen?.Invoke();
        }

        public override void Close()
        {
            OnGamesCollectionClose?.Invoke();
            OverlayCanvas.Controller.Close();
        }
    }
}
