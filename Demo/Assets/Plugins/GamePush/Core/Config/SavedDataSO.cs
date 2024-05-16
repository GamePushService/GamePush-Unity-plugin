using UnityEngine;

//[CreateAssetMenu(fileName = "SavedDataSO", menuName = "GP_Settings/SavedDataSO")]
public class SavedDataSO : ScriptableObject
{
    public string fileName = "gamepush-guid.txt";
    public bool isLogsEnabled = true;

    public TextAsset saveFile;
/*
#if UNITY_EDITOR
    public TextAsset editorOnlySaveFile;
#endif
*/
}
