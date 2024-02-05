using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GamePush;
using Examples.Console;

namespace Examples.Triggers
{
    public class Triggers : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _triggerTagOrId;
        [SerializeField] private Button _triggersClaim;

        public void Claim()
        {
            GP_Triggers.Claim(_triggerTagOrId.text);
        }
    }
}
