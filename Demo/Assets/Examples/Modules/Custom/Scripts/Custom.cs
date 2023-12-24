using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;
using TMPro;

using Examples.Console;

namespace Examples.Custom
{
    public class Custom : MonoBehaviour
    {
        public TMP_InputField inputField;

        public void CustomCall1()
        {
            GP_Custom.CustomCall1(inputField.text);
            ConsoleUI.Instance.Log("Custom call1");
        }

        public void CustomCall2()
        {
            GP_Custom.CustomCall2(inputField.text);
            ConsoleUI.Instance.Log("Custom call2");
        }

        public void CustomCall3()
        {
            var result = GP_Custom.CustomCall3(inputField.text);
            if(result != null)
                ConsoleUI.Instance.Log("Custom call3 " + result.ToString());
        }

        public void CustomCall4()
        {
            var result = GP_Custom.CustomCall4(inputField.text);
            if (result != null)
                ConsoleUI.Instance.Log("Custom call4 " + result.ToString());
        }
    }
}

