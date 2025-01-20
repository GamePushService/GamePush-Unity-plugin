using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using GamePush.Tools;
using GamePush.Data;

namespace GamePush.UI
{
    public class AchievementCell : MonoBehaviour
    {
        [SerializeField]
        private Image _logo, _medal;
        [SerializeField]
        private TMP_Text _title, _description;

        [SerializeField]
        private GameObject _subInfo;
        [SerializeField]
        private Image _rareDot;
        [SerializeField]
        private TMP_Text _rare, _unlocked;

        [SerializeField]
        private ProgressBar _progressBar;
        [SerializeField]
        private ProgressCounter _progressCounter;

        public async void SetUp(Achievement achievement)
        {
            _title.text = achievement.name;
            _description.text = achievement.description;
            if (achievement.description == "")
                _description.gameObject.SetActive(false);

            SetProgress(achievement);
            SetMedal(achievement);
            SetRare(achievement);
            SetUnlock(achievement);

            if (achievement.rare == RareTypes.COMMON && !achievement.unlocked)
                _subInfo.SetActive(false);

            await UtilityImage.DownloadImageAsync(achievement.icon, _logo);
        }

        private void SetProgress(Achievement achievement)
        {
            if (achievement.maxProgress != 0)
            {
                _progressBar.SetProgress(achievement.progress, achievement.maxProgress);
                _progressCounter.SetProgress(achievement.progress, achievement.maxProgress);
            }
            else
            {
                _progressBar.gameObject.SetActive(false);
                _progressCounter.gameObject.SetActive(false);
            }
        }

        private void SetMedal(Achievement achievement)
        {
            Color medalColor = Color.white;
            medalColor.a = achievement.unlocked ? 1 : 0.5f;
            _medal.color = medalColor;
        }

        private void SetRare(Achievement achievement)
        {
            if(achievement.rare == RareTypes.COMMON)
            {
                _rareDot.gameObject.SetActive(false);
                _rare.gameObject.SetActive(false);
                return;
            }

            string hexColor = RareTypes.GetColorHEX(achievement.rare);

            Color rareColor = UtilityImage.GetColorByHEX(hexColor);
            _rare.color = rareColor;
            _rareDot.color = rareColor;

            _rare.text = GetTranslate(achievement.rare);
        }

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

        private void SetUnlock(Achievement achievement)
        {
            _unlocked.text = "• " + CoreSDK.Language.localization.achievements.unlocked;

            _unlocked.gameObject.SetActive(achievement.unlocked);
        }

    }
}
