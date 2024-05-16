using GamePush.Data;
using GamePush.Core;
using GamePush.Config;
using UnityEditor;
using UnityEngine;

namespace GamePush
{
    public static class CoreSDK
    {
        public static bool isInit { get; private set; }

        public static int projectId { get; private set; }
        public static string projectToken { get; private set; }

        private static AllData data = new AllData();

        public static GameModule game;
        public static GameVariables variables;

        public static void Initialize()
        {
            SavedDataSO config = Resources.Load<SavedDataSO>("GP_ConfigSO");

            var path = AssetDatabase.GetAssetPath(config.saveFile);
            var file = new System.IO.StreamReader(path);
            var json = file.ReadToEnd();
            file.Close();

            var savedProjectData = JsonUtility.FromJson<SavedProjectData>(json);
            SetProjectData(savedProjectData);
            FetchConfig();
        }

        public static void SetProjectData(SavedProjectData data)
        {
            int parseId;
            int.TryParse(data.id, out parseId);
            projectId = parseId;
            projectToken = data.token;
        }

        public static void SetProjectData(int id, string token)
        {
            projectId = id;
            projectToken = token;
        }

        public static void FetchConfig()
        {
            ConfigFetcher.GetConfig();
        }

        public static void SetConfig(AllData allData)
        {
            data = allData;

            game = new GameModule();
            variables = new GameVariables(data.gameVariables);
        }

        public static AllData GetConfig()
        {
            return data;
        }
    }

    public class DeviceModule
    {
        public bool isMobile;
    }


    [System.Serializable]
    public class SavedProjectData
    {
        public string id, token;

        public SavedProjectData(string id, string token)
        {
            this.id = id;
            this.token = token;
        }
    }

}
