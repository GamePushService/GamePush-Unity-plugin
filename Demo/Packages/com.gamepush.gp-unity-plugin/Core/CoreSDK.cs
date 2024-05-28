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

        private static AllData data = new AllData();

        public static GameModule game;
        public static PlayerModule player;
        public static GameVariables variables;

        public static async void Initialize()
        {
            var savedProjectData = new SavedProjectData(ProjectData.ID, ProjectData.TOKEN);
            SetProjectData(savedProjectData);

            InitModules();
            await FetchData();

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

        public static async void FetchConfig()
        {
            await DataFetcher.GetConfig();
        }

        public static async Task FetchData()
        {
            await DataFetcher.GetConfig();

            SyncPlayerInput syncPlayerInput = new SyncPlayerInput();
            syncPlayerInput.playerState = player.GetPlayerState();
            syncPlayerInput.isFirstRequest = true;
            //syncPlayerInput.
            await DataFetcher.SyncPlayer(syncPlayerInput, false);
            /*
            if(player.GetPlayerDataCode() != null)
                await DataFetcher.GetPlayer();
            else
                await DataFetcher.SyncPlayer();
            */
        }

        public static void SetConfig(AllData allData)
        {
            data = allData;
            //player = new PlayerModule(data.playerFields);
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
