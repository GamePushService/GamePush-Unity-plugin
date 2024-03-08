using System.Collections.Generic;
using UnityEngine;
using GamePush;
using Examples.Console;
using UnityEngine.UI;
using TMPro;

namespace Examples.Variables
{

    public class Variables : MonoBehaviour
    {
        [SerializeField] private Button _fetchButton;
        [SerializeField] private Button _checkButton;
        [SerializeField] private Button _fetchPlatformButton;
        [SerializeField] private Button _checkPlatformButton;
        [SerializeField] private TMP_InputField _optionsInput;

        private void OnEnable()
        {
            _fetchButton.onClick.AddListener(Fetch);
            _checkButton.onClick.AddListener(Check);
            _fetchPlatformButton.onClick.AddListener(FetchPlatformVariables);
            _checkPlatformButton.onClick.AddListener(IsPlatformFetchAvailable);
        }

        private void OnDisable()
        {
            _fetchButton.onClick.RemoveListener(Fetch);
            _checkButton.onClick.RemoveListener(Check);
            _fetchPlatformButton.onClick.RemoveListener(FetchPlatformVariables);
            _checkPlatformButton.onClick.RemoveListener(IsPlatformFetchAvailable);
        }

        public void Fetch() => GP_Variables.Fetch(OnFetchSuccess, OnFetchError);
        public void Check()
        {
            ConsoleUI.Instance.Log("VARIABLES: GET INT " + GP_Variables.GetInt("number"));
            ConsoleUI.Instance.Log("VARIABLES: GET FLOAT " + GP_Variables.GetFloat("float"));
            ConsoleUI.Instance.Log("VARIABLES: GET BOOL " + GP_Variables.GetBool("bool"));
            ConsoleUI.Instance.Log("VARIABLES: GET STRING " + GP_Variables.GetString("string"));

            ConsoleUI.Instance.Log("VARIABLES: GET IMAGE " + GP_Variables.GetImage("image"));
            ConsoleUI.Instance.Log("VARIABLES: GET FILE " + GP_Variables.GetFile("file"));
        }

        public void IsPlatformFetchAvailable()
        {
            ConsoleUI.Instance.Log("Platform Fetch Available: " + GP_Variables.IsPlatformVariablesAvailable());
        }

        public void FetchPlatformVariables()
        {
            ConsoleUI.Instance.Log("Fetch Platform Variables");

            if (_optionsInput.text != "")
            {
                ConsoleUI.Instance.Log("options: " + _optionsInput.text);
                GP_Variables.FetchPlatformVariables(_optionsInput.text, OnPlatformVariablesFetchSuccess, OnPlatformVariablesFetchError);
                
            }
            else
            {
                ConsoleUI.Instance.Log("no options");
                GP_Variables.FetchPlatformVariables(OnPlatformVariablesFetchSuccess, OnPlatformVariablesFetchError);
            }
                
        }

        private void OnFetchSuccess(List<VariablesData> variables)
        {
            for (int i = 0; i < variables.Count; i++)
            {
                ConsoleUI.Instance.Log("VARIABLE KEY: " + variables[i].key);
                ConsoleUI.Instance.Log("VARIABLE TYPE: " + variables[i].type);
                ConsoleUI.Instance.Log("VARIABLE VALUE: " + variables[i].value);
            }
        }

        private void OnFetchError() => ConsoleUI.Instance.Log("FETCH: ERROR");

        private void OnPlatformVariablesFetchSuccess(Dictionary<string, string> variables)
        {
            ConsoleUI.Instance.Log("TEST1: " + variables["TEST1"]);
            ConsoleUI.Instance.Log("TEST2: " + variables["TEST2"]);
        }

        private void OnPlatformVariablesFetchError(string err) => ConsoleUI.Instance.Log("FETCH ERROR: " + err);
    }

}