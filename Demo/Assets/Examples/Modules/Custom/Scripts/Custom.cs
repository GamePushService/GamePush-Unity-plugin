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
        public TMP_InputField nameInputField;
        public TMP_InputField argsInputField;

        public void CustomCall()
        {
            Debug.Log(nameInputField.text + " , " + argsInputField.text);
            GP_Custom.CustomCall(nameInputField.text, argsInputField.text == "" ? null : argsInputField.text);
            ConsoleUI.Instance.Log("Custom call");
        }

        public void CustomReturn()
        {
            Debug.Log(nameInputField.text + " , " + argsInputField.text);
            var result = GP_Custom.CustomReturn(nameInputField.text, argsInputField.text == "" ? null : argsInputField.text);
            if (result != null)
                ConsoleUI.Instance.Log("Custom return " + result.ToString());
        }

        public void CustomValue()
        {

        }

    }
}

