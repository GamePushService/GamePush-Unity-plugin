using UnityEngine;
using UnityEngine.UI;

using GamePush;
using Examples.Console;

namespace Examples.Documents
{
    public class Documents : MonoBehaviour
    {
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _fetchButton;

        private void OnEnable()
        {
            _openButton.onClick.AddListener(Open);
            _fetchButton.onClick.AddListener(Fetch);
        }
        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(Open);
            _fetchButton.onClick.RemoveListener(Fetch);
        }


        public void Open() => GP_Documents.Open(OnOpen, OnClose);
        private void OnOpen() => ConsoleUI.Instance.Log("On Documents: OPEN");
        private void OnClose() => ConsoleUI.Instance.Log("On Documents: CLOSE");

        public void Fetch() => GP_Documents.Fetch(OnFetchSuccess, OnFetchError);
        private void OnFetchSuccess(string privacy) => ConsoleUI.Instance.Log(privacy);
        private void OnFetchError() => ConsoleUI.Instance.Log("Documents Fetch: ERROR");
    }
}