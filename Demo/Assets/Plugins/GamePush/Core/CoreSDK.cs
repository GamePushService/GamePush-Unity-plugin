using GamePush.Data;
using GamePush.Core;
using System;
using System.Threading.Tasks;

namespace GamePush
{
    public static class CoreSDK
    {
        public static bool isInit { get; private set; }
        public static Action OnInit;

        public static int projectId { get; private set; }
        public static string projectToken { get; private set; }

        private static AllConfigData configData = new AllConfigData();

        public static GameModule game;
        public static PlayerModule player;
        public static GameVariables variables;

        private static DateTime serverTime;

        public static void AddServerTime(float time) => serverTime.AddSeconds(time);
        public static string GetServerTime() => configData.serverTime;

        public static async void Initialize()
        {
            int id;
            int.TryParse(ProjectData.ID, out id);
            SetProjectData(id, ProjectData.TOKEN);

            InitModules();
            await InitFetch();

            isInit = true;
            OnInit?.Invoke();
        }

        private static void InitModules()
        {
            game = new GameModule();
            player = new PlayerModule();
            variables = new GameVariables();
        }

        public static void SetProjectData(SavedProjectData data)
        {
            projectId = data.id;
            projectToken = data.token;
        }

        public static void SetProjectData(int id, string token)
        {
            projectId = id;
            projectToken = token;
        }

        public static async Task FetchCoreConfig()
        {
            AllConfigData data = await DataFetcher.GetConfig();
            SetConfig(data);
        }

        public static async Task InitFetch()
        {
            await FetchCoreConfig();

            await player.FetchPlayerConfig();
        }

        public static void SetConfig(AllConfigData allData)
        {
            configData = allData;

            serverTime = DateTime.Parse(configData.serverTime);
            player.Init(configData.playerFields);
            variables.SetVariablesData(configData.gameVariables);
        }

        public static AllConfigData GetConfig()
        {
            return configData;
            
        }

        public static T GetValueWithDefault<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }

    public class DeviceModule
    {
        public bool isMobile;
    }

}
