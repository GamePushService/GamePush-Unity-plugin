using UnityEngine;

namespace GamePush.Data
{
    //[CreateAssetMenu(fileName = "SavedDataSO", menuName = "GP_Settings/SavedDataSO")]
    public class SavedDataSO : ScriptableObject
    {
        public string fileName = "gamepush-guid.txt";
        public bool isLogsEnabled = true;


#if UNITY_EDITOR
        public TextAsset saveFile;
#endif
        public TextAsset projectData;
        public TextAsset jspreData;

    }

}
