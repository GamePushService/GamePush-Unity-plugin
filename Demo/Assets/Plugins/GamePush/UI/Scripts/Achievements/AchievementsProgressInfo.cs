using UnityEngine;
using TMPro;

namespace GamePush.UI
{
    public class AchievementsProgressInfo : MonoBehaviour
    {
        [SerializeField]
        private ProgressCounter _progressCounter;
        [SerializeField]
        private ProgressBar _progressBar;
        [SerializeField]
        private TMP_Text _unlockedText;

        public void SetInfo(int unlocked, int all)
        {
            _progressCounter.SetProgress(unlocked, all);
            _progressBar.SetProgress(unlocked, all);

            _unlockedText.text = CoreSDK.language.localization.achievements.unlockedTotal;
        }
    }
}
