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
    }
}

