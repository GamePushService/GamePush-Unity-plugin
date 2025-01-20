using UnityEngine;
using TMPro;

namespace GamePush.UI
{
    public class ProgressCounter : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _counter;

        public void SetProgress(int progress, int maxValue)
        {
            SetValues(progress, maxValue);
        }

        private void SetValues(int firstNum, int secondNum)
        {
            _counter.text = firstNum.ToString() + " / " + secondNum.ToString();
        }
    }
}
