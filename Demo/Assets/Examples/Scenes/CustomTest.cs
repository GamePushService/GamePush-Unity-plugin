using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;
using TMPro;

using Examples.Console;

public class CustomTest : MonoBehaviour
{
    public TMP_InputField nameInputField;

    public void CustomCall()
    {
        GP_Custom.Call(nameInputField.text);
        ConsoleUI.Instance.Log("Custom call " + nameInputField.text);
    }

    public void CustomReturn()
    {
        string result = GP_Custom.Return(nameInputField.text);
        if (result != null)
            ConsoleUI.Instance.Log("Custom return: " + result.ToString());
    }

    public void CustomAsyncReturn()
    {
        GP_Custom.AsyncReturn(nameInputField.text, null, OnAsyncReturn, OnAsyncError);
        ConsoleUI.Instance.Log("Custom async send");
    }

    private void OnAsyncReturn(string result)
    {
        ConsoleUI.Instance.Log("Custom async return: " + result);
    }

    private void OnAsyncError(string result)
    {
        ConsoleUI.Instance.Log("Custom async error: " + result);
    }
}
