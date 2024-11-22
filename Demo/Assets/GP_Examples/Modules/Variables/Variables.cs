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
        [SerializeField] private TMP_InputField _key1;
        [SerializeField] private TMP_InputField _value1;
        [SerializeField] private TMP_InputField _key2;
        [SerializeField] private TMP_InputField _value2;

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

            Dictionary<string, string> dict = GetDictionary();
            
            if (dict.Count > 0)
            {
                GP_Variables.FetchPlatformVariables(dict, OnPlatformVariablesFetchSuccess, OnPlatformVariablesFetchError);
                
            }
            else
            {
                GP_Variables.FetchPlatformVariables(OnPlatformVariablesFetchSuccess, OnPlatformVariablesFetchError);
            }
                
        }

        private Dictionary<string, string> GetDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (_key1.text != "") dict.TryAdd(_key1.text, _value1.text);
            if (_key2.text != "") dict.TryAdd(_key2.text, _value2.text);

            Debug.Log(dict.Count + " pairs");

            return dict;
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
            foreach(string key in variables.Keys)
            {
                ConsoleUI.Instance.Log(key + " : " + variables[key]);
            }
        }

        private void OnPlatformVariablesFetchError(string err) => ConsoleUI.Instance.Log("FETCH ERROR: " + err);
    }

}