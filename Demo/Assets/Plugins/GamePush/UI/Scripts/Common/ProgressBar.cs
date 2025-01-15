using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GamePush.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private TMP_Text _progressPercent;

        public void SetProgress(float progress, float maxValue)
        {
            float sliderValue = progress / maxValue;
            _slider.value = sliderValue;
            _progressPercent.text = (sliderValue * 100).ToString() + "%";
        }
    }
}
