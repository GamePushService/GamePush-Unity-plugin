using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GamePush;
using Examples.Console;

namespace Examples.Experiments
{
    public class Experiments : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _experimentTag;
        [SerializeField] private TMP_InputField _experimentCohort;
        [Space]
        [SerializeField] private Button _buttonMap;
        [SerializeField] private Button _buttonHas;

        private void OnEnable()
        {
            _buttonMap.onClick.AddListener(Map);
            _buttonHas.onClick.AddListener(Has);
        }

        private void OnDisable()
        {
            _buttonMap.onClick.RemoveListener(Map);
            _buttonHas.onClick.RemoveListener(Has);
        }

        public void Map()
        {
            string map = GP_Experiments.Map();
            ConsoleUI.Instance.Log("Experiments:\n " + map);
        }

        public void Has()
        {
            bool has = GP_Experiments.Has(_experimentTag.text, _experimentCohort.text);
            ConsoleUI.Instance.Log($"Cohort {_experimentCohort.text} in experiment {_experimentTag.text}:\n {has}");
        }
    }
}

