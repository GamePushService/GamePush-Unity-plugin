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
        GP_Player.Login();
    }

    public void Register()
    {
        //var username = UsernameInput.text;
        //var email = EmailInputField.text;
        //var password = PasswordInputField.text;

        // Call the user registration method
        // Pass credentials and callback functions for success and error cases
       
    }
}
