using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GamePush.UI
{
    public class AchievementPlate : MonoBehaviour
    {
        [Header("Text components")]
        [SerializeField]
        private TMP_Text _titleText;
        [SerializeField]
        private TMP_Text _descriptionText;
        [SerializeField]
        private TMP_Text _topInfoText;
        [SerializeField]
        private TMP_Text _botInfoText;

        [Header("Image component")]
        [SerializeField]
        private Image _achievImage;

        [Space]
        [Header("Moving parts")]
        [SerializeField]
        private RectTransform _backPlate;
        [SerializeField]
        private float _startSize = 200, _endSize = 800;
        [SerializeField]
        private float _startY = -300f, _endY = 350f;
        [SerializeField]
        private float _moveUpSpeed = 1500f;
        [Space]
        [SerializeField]
        private RectTransform _topInfo;
        [SerializeField]
        private RectTransform _botInfo;

        void Start()
        {
            
        }


        void SetUnlock()
        {
            //_topInfo.text = CoreSDK.language.localization.achievements.
        }

        void SetProgress()
        {

        }

        void ShowPlate()
        {
            Vector3 endPos = _backPlate.anchoredPosition;
            endPos.y = _endY;

            Vector3 startPos = _backPlate.anchoredPosition;
            startPos.y = _startY;

            _backPlate.anchoredPosition = startPos;
            MoveDown(endPos);
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
