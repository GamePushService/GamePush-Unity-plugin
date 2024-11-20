using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamePush;

public class XsollaTest : MonoBehaviour
{
    //public InputField UsernameInput;
    //public InputField EmailInputField;
    //public InputField PasswordInputField;

    // Start is called before the first frame update
    async void Start()
    {
        await GP_Init.Ready;
        //GP_Payments.Fetch();
    }



    public void Fetch()
    {
        GP_Payments.Fetch();
        //GP_Player.FetchFields();
    }

    public void Purchase()
    {
        GP_Payments.Purchase("test_1");
    }

    public void Login()
    {
        GP_Player.Login(OnSuccess, OnError);
    }

    private static void OnSuccess()
    {
        Debug.Log("Authorization successful");
    }

    private static void OnError(string error)
    {
        Debug.LogError($"Authorization failed. Error: {error}");
    }
}
