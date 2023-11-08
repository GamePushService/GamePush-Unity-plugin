using UnityEngine;

using GamePush;

namespace Examples.Languages
{
    public class Languages : MonoBehaviour
    {
        public void Current() => GP_Language.Current();

        public void Change() => GP_Language.Change(Language.English, OnChange);

        private void OnChange(Language language) => Debug.Log("LANGUAGE : ON CHANGE: " + language);
    }
}