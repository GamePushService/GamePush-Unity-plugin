using System;
using System.Collections;
using GamePush.Data;
using TMPro;
using UnityEngine;

namespace GamePush.UI
{
    public class DocumentUI : ModuleUI
    {
        [SerializeField]
        private float _startOffset = 400f;
        [SerializeField]
        private float _moveUpSpeed = 1200f;
        [Space] 
        [SerializeField] 
        private TMP_Text documentTitle, documentBody;
        
        private DocumentData _documentData;
        private event Action OnDocumentOpen;
        private event Action OnDocumentClose;

        
        public void Show(DocumentData data, Action onDocumentOpen, Action onDocumentClose)
        {
            _documentData = data;

            OnDocumentOpen = onDocumentOpen;
            OnDocumentClose = onDocumentClose;

            StartCoroutine(MoveUp());

            SetUpPanel();
        }
        
        private void SetUpPanel()
        {
            documentTitle.text = CoreSDK.Language.localization.documents.PLAYER_PRIVACY_POLICY.title;
            documentBody.text = _documentData.content;
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

            OnDocumentOpen?.Invoke();
        }
        
        public override void Close()
        {
            OnDocumentClose?.Invoke();
            OverlayCanvas.Controller.Close();
        }
    }
}
