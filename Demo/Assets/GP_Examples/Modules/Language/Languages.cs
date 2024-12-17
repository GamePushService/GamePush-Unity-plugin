using UnityEngine;

using GamePush;

namespace Examples.Languages
{
    public class Languages : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("Language: " + Current());
        }

        public string Current() => GP_Language.Current().ToString();

        public void Change() => GP_Language.Change(Language.English, OnChange);

        private void OnChange(Language language) => Debug.Log("LANGUAGE : ON CHANGE: " + language);
    }
}