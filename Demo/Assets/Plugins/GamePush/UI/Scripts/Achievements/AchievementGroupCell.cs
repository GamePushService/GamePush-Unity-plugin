using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GamePush.UI
{
    public class AchievementGroupCell : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _title, _description;
        [SerializeField]
        private ProgressCounter _progressCounter;

        public void Init()
        {

        }
    }
}
