using System.Collections;
using UnityEngine;

namespace GamePush.UI
{
    public class AchievementPlate : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _backPlate;
        [SerializeField]
        private float _startOffset = 300f;
        [SerializeField]
        private float _moveUpSpeed = 1500f;

        void Start()
        {
        
        }


        void ShowPlate()
        {
            Vector3 lastPos = _backPlate.anchoredPosition;

            Vector3 startPos = _backPlate.anchoredPosition;
            startPos.y += _startOffset;
            _backPlate.anchoredPosition = startPos;

            MoveDown(lastPos);
        }

        IEnumerator MoveDown(Vector3 moveToPos)
        {
            while (_backPlate.anchoredPosition.y > moveToPos.y)
            {
                _backPlate.Translate(Vector2.up * _moveUpSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

       
    }
}
