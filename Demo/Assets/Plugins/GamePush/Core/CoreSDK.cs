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
        public static string targetPlatform { get; private set; }
        public static Platform platformType { get; private set; }
        public static string platformKey { get; private set; }

        private static AllConfigData configData = new AllConfigData();

        public static string currentLang = "EN";

        public static PlatformConfig platformConfig;

        #region Modules

        public static readonly GameModule Game = new GameModule();
        public static readonly PlayerModule Player = new PlayerModule();
        public static readonly PlatformModule Platform = new PlatformModule();
        public static readonly GameVariablesModule Variables = new GameVariablesModule();
        public static readonly AdsModule Ads = new AdsModule();
        public static readonly DeviceModule Device = new DeviceModule();
        public static readonly PaymentsModule Payments = new PaymentsModule();
        public static readonly LeaderboardModule Leaderboard = new LeaderboardModule();
        public static readonly SystemModule System = new SystemModule();
        public static readonly AppModule App = new AppModule();
        public static readonly SocialsModule Socials = new SocialsModule();
        public static readonly LanguageModule Language = new LanguageModule();
        public static readonly UniquesModule Uniques = new UniquesModule();
        public static readonly AchievementsModule Achievements = new AchievementsModule();
        public static readonly AnalyticsModule Analytics = new AnalyticsModule();
        public static readonly DocumentsModule Documents = new DocumentsModule();
        public static readonly GameCollectionsModule GameCollections = new GameCollectionsModule();
        public static readonly PlayersModule Players = new PlayersModule();
        public static readonly RewardsModule Rewards = new RewardsModule();
        public static readonly TriggersModule Triggers = new TriggersModule();
        public static readonly EventsModule Events = new EventsModule();

        #endregion

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
            DateTime.TryParseExact(date, formats, null, global::System.Globalization.DateTimeStyles.None, out DateTime dateTime);

            //return dateTime.ToString();
            return dateTime.ToString();
        }

        public static DateTime ConvertToDateTime(string time)
        {
            if (DateTime.TryParse(time, out var dateTime))
            {
                return dateTime;
            }
            else
                return DateTime.ParseExact(time, formats, null, global::System.Globalization.DateTimeStyles.None);
        }

        public static DateTime GetServerTime()
        {
            return serverTime;
        }

        public static void AddPlayTime(float time)
        {
            serverTime = serverTime.AddSeconds(time);
            Player.AddPlayTime(time);
        }

        public static void SetServerTime(string time)
        {
            serverTime = ConvertToDateTime(time);
        }
        #endregion

        #region Initialization
        public static async void Initialize()
        {
            int.TryParse(ProjectData.ID, out var id);
            SetProjectData(id, ProjectData.TOKEN);

            // InitModules();
#if UNITY_EDITOR || !UNITY_WEBGL
            await InitFetch();
#endif

            isInit = true;
            OnInit?.Invoke();
            await Task.Delay(1);
        }

        // private static void InitModules()
        // {
        //     Platform = new PlatformModule();
        //     Language = new LanguageModule();
        //     Game = new GameModule();
        //     Player = new PlayerModule();
        //     Variables = new GameVariablesModule();
        //     Ads = new AdsModule();
        //     Device = new DeviceModule();
        //     System = new SystemModule();
        //     App = new AppModule();
        //     Leaderboard = new LeaderboardModule();
        //     Uniques = new UniquesModule();
        //     Achievements = new AchievementsModule();
        //     Analytics = new AnalyticsModule();
        //     GameCollections = new GameCollectionsModule();
        //     Documents = new DocumentsModule();
        //     Players = new PlayersModule();
        //     Events = new EventsModule();
        //     Rewards = new RewardsModule();
        //     Triggers = new TriggersModule();
        //
        //     Payments = new PaymentsModule();
        //     Socials = new SocialsModule();
        // }

        private static async Task InitFetch()
        {
            await FetchCoreConfig();
            await Player.FetchPlayerConfig();
        }

        private static async Task FetchCoreConfig()
        {
            AllConfigData data = await DataFetcher.GetConfig();
            SetConfig(data);
        }

        private static void SetConfig(AllConfigData allData)
        {
            configData = allData;

            SetServerTime(allData.serverTime);
            currentLang = allData.config.lang;
            if (currentLang == "") currentLang = "en";
            Language.Init(currentLang);

            Player.Init(configData.playerFields);
            Variables.Init(configData.gameVariables);
            Platform.Init(configData.platformConfig);
            Ads.Init(configData.project.ads, configData.platformConfig);
            Payments.Init(configData.products, configData.platformConfig);
            App.Init(configData.project, configData.platformConfig);
            Socials.Init(configData.config);
            // Rewards.Init(configData.rewards);
            Triggers.Init(configData);
            Uniques.Init();
            Achievements.Init(configData);
            Events.Init(configData.events);
        }
        
        public static async Task FetchEditorConfig()
        {
            AllConfigData data = await DataFetcher.GetConfig();
            configData = data;
            platformConfig = configData.platformConfig;
        }

#endregion

        public static void SetAndroidPlatform(string platform)
            => targetPlatform = platform;

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

        public static AllConfigData GetConfig()
        {
            return configData;
        }

        public static T GetValueWithDefault<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }

}
