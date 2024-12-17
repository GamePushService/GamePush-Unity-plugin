using UnityEngine;
using GamePush;
using Examples.Console;
using UnityEngine.UI;

namespace Examples.GamesCollections
{
    public class GamesCollections : MonoBehaviour
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


        public void Open() => GP_GamesCollections.Open("ALL", OnOpen, OnClose);
        public void Fetch() => GP_GamesCollections.Fetch("ALL", OnFetchSuccess, OnFetchError);


        private void OnOpen() => Debug.Log("GAMES COLLECTION: ON OPEN");
        private void OnClose() => Debug.Log("GAMES COLLECTION: ON CLOSE");

        private void OnFetchSuccess(string idOrTag, GamesCollectionsFetchData collection)
        {
            ConsoleUI.Instance.Log("ID: " + collection.id);
            ConsoleUI.Instance.Log("TAG: " + collection.tag);
            ConsoleUI.Instance.Log("NAME: " + collection.name);
            ConsoleUI.Instance.Log("DESCRIPTION: " + collection.description);

            for (int i = 0; i < collection.games.Length; i++)
            {
                ConsoleUI.Instance.Log("GAME ID: " + collection.games[i].id);
                ConsoleUI.Instance.Log("GAME NAME: " + collection.games[i].name);
                ConsoleUI.Instance.Log("GAME DESCRIPTION: " + collection.games[i].description);
                ConsoleUI.Instance.Log("GAME ICON: " + collection.games[i].icon);
                ConsoleUI.Instance.Log("GAME URL: " + collection.games[i].url);
            }
        }
        private void OnFetchError() => Debug.Log("GAMES COLLECTION: FETCH ERROR");



    }
}