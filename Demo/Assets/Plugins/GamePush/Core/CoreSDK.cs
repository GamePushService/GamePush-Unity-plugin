using GamePush.Data;
using GamePush.Core;
using System;
using System.Threading.Tasks;
using System.Xml;

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
        public static PlatformModule platform;
        public static GameVariables variables;
        public static AdsModule ads;

        #region ServerTime

        private static DateTime serverTime;

        private static string[] formats =
        {
            "MM/dd/yyyy HH:mm:ss",
            "dd.MM.yyyy HH:mm:ss"
        };

        public static string TestConvert()
        {
            string date = "20.06.2024 09:45:52";
            DateTime.TryParseExact(date, formats, null, System.Globalization.DateTimeStyles.None, out DateTime dateTime);

            //return dateTime.ToString();
            return dateTime.ToString();
        }

        private static async Task HandleSync()
        {
            await Task.Yield();
        }

        public static DateTime ConvertToDateTime(string time)
        {
            DateTime dateTime;
            if (DateTime.TryParse(time, out dateTime))
            {
                return dateTime;
            }
            else
                return DateTime.ParseExact(time, formats, null, System.Globalization.DateTimeStyles.None);
        }


        public static DateTime GetServerTime()
        {
            return serverTime;
        }

        public static void AddPlayTime(float time)
        {
            serverTime = serverTime.AddSeconds(time);
            player.AddPlayTime(time);
        }

        public static void SetServerTime(string time)
        {
            serverTime = ConvertToDateTime(time);
        }
        #endregion

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
            platform = new PlatformModule();
            ads = new AdsModule();
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

        public static async Task InitFetch()
        {
            await FetchCoreConfig();
            await player.FetchPlayerConfig();
        }

        public static async Task FetchEditorConfig()
        {
            AllConfigData data = await DataFetcher.GetConfig();
            configData = data;
        }

        public static async Task FetchCoreConfig()
        {
            AllConfigData data = await DataFetcher.GetConfig();
            SetConfig(data);
        }

        public static void SetConfig(AllConfigData allData)
        {
            configData = allData;

            SetServerTime(allData.serverTime);

            player.Init(configData.playerFields);
            variables.SetVariablesData(configData.gameVariables);
            platform.Init(configData.platformConfig);
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
