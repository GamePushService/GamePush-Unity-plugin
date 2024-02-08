using System.Collections.Generic;
using UnityEngine;
using GamePush;
using Examples.Console;
using UnityEngine.UI;

namespace Examples.Variables
{

    public class Variables : MonoBehaviour
    {
        [SerializeField] private Button _fetchButton;
        [SerializeField] private Button _checkButton;

        private void OnEnable()
        {
            _fetchButton.onClick.AddListener(Fetch);
            _checkButton.onClick.AddListener(Check);
        }
        private void OnDisable()
        {
            _fetchButton.onClick.RemoveListener(Fetch);
            _checkButton.onClick.RemoveListener(Check);
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
    }

}