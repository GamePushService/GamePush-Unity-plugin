using GamePush.Data;
using GamePush.Core;

namespace GamePush
{
    public static class CoreSDK
    {
        public static int projectId = 5815;
        public static string token = "BL5y2Oqp3Wl622MDcOAWrd55hJJF08SD";

        private static AllData data = new AllData();

        public static GameModule game;
        public static GameVariables variables;

        public static void SetConfig(AllData allData)
        {
            data = allData;

            variables = new GameVariables();
            variables.SetData(data.gameVariables);
        }

        public static AllData GetConfig()
        {
            return data;
        }
    }


    public class GameModule
    {
        public bool isPaused;
        public bool isGameplay;
        public bool isGameStarted;

        public bool gameReady = false;

        public void GameReady()
        {
            gameReady = true;
        }

        public void HappyTime() { }
    }

   

    public class DeviceModule
    {
        public bool isMobile;
    }

}
