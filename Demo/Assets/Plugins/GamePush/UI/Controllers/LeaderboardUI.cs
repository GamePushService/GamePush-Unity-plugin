using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.UI
{
    public class LeaderboardUI : MonoBehaviour
    {
        [SerializeField]
        private float _startOffset = 400f;
        [SerializeField]
        private float _moveUpSpeed = 1500f;

        private void Start()
        {
            StartCoroutine(MoveUp());
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
        }

        public void Close()
        {
            OverlayCanvas.Controller.Close();
        }
    }

}
