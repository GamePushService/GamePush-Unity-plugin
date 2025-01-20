using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Data;

namespace GamePush.UI
{
    public class AchievementGroupCell : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _title, _description;
        [SerializeField]
        private ProgressCounter _progressCounter;

        public void SetUp(AchievementsGroup group, int progress)
        {
            _title.text = group.name;
            _description.text = group.description;

            _progressCounter.SetProgress(progress, group.achievements.Count);
        }
    }
}
