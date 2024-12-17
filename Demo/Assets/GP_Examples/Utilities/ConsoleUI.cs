using TMPro;
using UnityEngine;

namespace Examples.Console
{
    public class ConsoleUI : MonoBehaviour
    {
        #region Singleton
        public static ConsoleUI Instance;
        private void OnEnable()
        {
            Instance = this;
            GamePush.GP_Logger.OnLog += EventLog;
        }
        private void OnDisable()
        {
            Instance = null;
            GamePush.GP_Logger.OnLog -= EventLog;
        }
        #endregion

        [SerializeField] private TMP_Text _console;


        public void EventLog(string data)
        {
            _console.text += $"\n{data}";
        }

        public void Log(string data)
        {
            _console.text += $"\n{data}";
            Debug.Log(data);
        }

        public void Log(int data)
        {
            _console.text += $"\n{data}";
            Debug.Log(data);
        }

        public void Log(bool data)
        {
            _console.text += $"\n{data}";
            Debug.Log(data);
        }
    }


}