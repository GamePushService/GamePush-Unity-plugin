using TMPro;
using UnityEngine;

namespace Examples.Console
{
    public class ConsoleUI : MonoBehaviour
    {
        #region Singleton
        public static ConsoleUI Instance;
        private void Awake() => Instance = this;
        #endregion

        [SerializeField] private TMP_Text _console;

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