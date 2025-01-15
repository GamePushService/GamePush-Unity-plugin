using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

namespace GamePush.UI
{
    public class AchievementCell : MonoBehaviour
    {
        [SerializeField]
        private Image _logo, _medal;
        [SerializeField]
        private TMP_Text _title, _description;
        [SerializeField]
        private ProgressBar _progressBar;
        [SerializeField]
        private ProgressCounter _progressCounter;

        public async Task Init()
        {
            await Task.Delay(1);
        }
    }
}
