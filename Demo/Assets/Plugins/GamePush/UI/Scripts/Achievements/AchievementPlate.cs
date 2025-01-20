using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Data;
using System.Threading.Tasks;
using GamePush.Tools;

namespace GamePush.UI
{
    public class AchievementPlate : ModuleUI
    {
        [Header("Text components")]
        [SerializeField]
        private TMP_Text _titleText;
        [SerializeField]
        private TMP_Text _descriptionText;
        [SerializeField]
        private TMP_Text _topInfoText, _botInfoText;

        [Header("Image components")]
        [SerializeField]
        private Image _achiveProgress;
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
        private float _moveSpeed = 1500f;
        [Space]
        [SerializeField]
        private RectTransform _topInfo;
        [SerializeField]
        private RectTransform _botInfo;

        public static string GetTranslate(string rare)
        {
            return rare switch
            {
                RareTypes.COMMON => CoreSDK.Language.localization.rare.COMMON,
                RareTypes.UNCOMMON => CoreSDK.Language.localization.rare.UNCOMMON,
                RareTypes.RARE => CoreSDK.Language.localization.rare.RARE,
                RareTypes.EPIC => CoreSDK.Language.localization.rare.EPIC,
                RareTypes.LEGENDARY => CoreSDK.Language.localization.rare.LEGENDARY,
                RareTypes.MYTHIC => CoreSDK.Language.localization.rare.MYTHIC,
                _ => CoreSDK.Language.localization.rare.COMMON
            };
        }

        private void Awake()
        {
            SetStartState();
        }

        public async Task SetUnlock(Achievement data)
        {
            string json = UtilityJSON.ToJson(data);
            print(json);

            _titleText.text = data.name;
            _descriptionText.text = data.description;

            _achievImage.color = Color.white;
            if (data.icon != "")
                await UtilityImage.DownloadImageAsync(GetAchievementIcon(data), _achievImage);

            _topInfoText.text = CoreSDK.Language.localization.achievements.unlocked;

            if(data.rare == RareTypes.COMMON)
            {
                foreach (Transform child in _botInfo.GetComponentsInChildren<Transform>())
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                _botInfoText.text = GetTranslate(data.rare);

                _achiveProgress.fillAmount = 1;
                _achiveProgress.color = RareTypes.GetColor(data.rare);
            }

            await ShowPlate();
        }

        public async Task SetProgress(Achievement data)
        {
            _titleText.text = data.name;
            _descriptionText.text = data.description;

            _achievImage.color = Color.white;
            if (data.icon != "")
                await UtilityImage.DownloadImageAsync(GetAchievementIcon(data), _achievImage);

            _topInfoText.text = CoreSDK.Language.localization.achievements.progress;

            if (data.rare == RareTypes.COMMON)
                data.rare = RareTypes.UNCOMMON;

            _botInfoText.text = data.progress + " / " + data.maxProgress;
            _achiveProgress.fillAmount = (float)data.progress / (float)data.maxProgress;
            _achiveProgress.color = RareTypes.GetColor(data.rare);

            await ShowPlate();
        }

        private static string GetAchievementIcon(Achievement achievement)
        {
            float DevicePixelRatio = CoreSDK.Device.DevicePixelRatio();
            if (DevicePixelRatio > 1)
            {
                return achievement.unlocked ? achievement.icon ?? achievement.lockedIcon : achievement.lockedIcon ?? achievement.icon;
            }
            else
            {
                return achievement.unlocked ? achievement.iconSmall ?? achievement.lockedIconSmall : achievement.lockedIconSmall ?? achievement.iconSmall;
            }
        }

        private async Task VerticalMove(RectTransform rect, float endY, float moveSpeed)
        {
            bool isUp = endY > rect.anchoredPosition.y;

            while (rect && isUp ? rect.anchoredPosition.y < endY : rect.anchoredPosition.y > endY)
            {
                float newY = isUp ?
                    rect.anchoredPosition.y + moveSpeed * Time.deltaTime :
                    rect.anchoredPosition.y - moveSpeed * Time.deltaTime;

                rect.SetLocalPositionAndRotation(new Vector2(rect.anchoredPosition.x, newY), rect.rotation);
                await Task.Delay(1);
            }
        }

        public async Task TestPlate()
        {
            await ShowPlate();
        }
            
        private async Task ShowPlate()
        {
            await MoveDown();
            await Task.Delay(200);
            await OpenUp();
            ChangeTextVisibility(true);
            await Task.Delay(200);
            await ShowWings();
            ChangeTextVisibility(false);
            await CloseUp();
            await MoveUp();
        }

        private void SetStartState()
        {
            Vector3 startPos = _backPlate.anchoredPosition;
            startPos.y = _startY;

            _backPlate.anchoredPosition = startPos;


            _backPlate.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _startSize);

            ChangeTextVisibility(false);
            _topInfo.gameObject.SetActive(false);
            _botInfo.gameObject.SetActive(false);
        }

        private async Task MoveDown() => await VerticalMove(_backPlate, _endY, _moveSpeed);
        private async Task MoveUp() => await VerticalMove(_backPlate, _startY, _moveSpeed);

        private async Task OpenUp()
        {
            while (_backPlate.rect.width < _endSize)
            {
                _backPlate.SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Horizontal,
                    _backPlate.rect.width + 2 * _moveSpeed * Time.deltaTime);
                await Task.Delay(1);
            }
        }

        private async Task CloseUp()
        {
            while (_backPlate.rect.width > _startSize)
            {
                _backPlate.SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Horizontal,
                    _backPlate.rect.width - 2 * _moveSpeed * Time.deltaTime);
                await Task.Delay(1);
            }
        }

        private void ChangeTextVisibility(bool isShown)
        {
            _titleText.gameObject.SetActive(isShown);
            _descriptionText.gameObject.SetActive(isShown);
        }

        private async Task ShowWings()
        {
            _topInfo.gameObject.SetActive(true);
            _botInfo.gameObject.SetActive(true);

            float moveTopToY = _topInfo.anchoredPosition.y;
            float moveBotToY = _botInfo.anchoredPosition.y;

            _topInfo.SetLocalPositionAndRotation(Vector2.zero, _topInfo.rotation);
            _botInfo.SetLocalPositionAndRotation(Vector2.zero, _botInfo.rotation);

            float showSpeed = 0.5f * _moveSpeed;

            Task moveTop1 = VerticalMove(_topInfo, moveTopToY, showSpeed);
            Task moveBot1 = VerticalMove(_botInfo, moveBotToY, showSpeed);

            await Task.WhenAll(moveTop1, moveBot1);

            await Task.Delay(1000);

            Task moveTop2 = VerticalMove(_botInfo, 0, showSpeed);
            Task moveBot2 = VerticalMove(_topInfo, 0, showSpeed);

            await Task.WhenAll(moveTop2, moveBot2);

            _topInfo.gameObject.SetActive(false);
            _botInfo.gameObject.SetActive(false);
        }

        

    }
}
