using UnityEngine;
using GamePush.Services;
using GamePush.Tools;
using GamePush.Core;
using System.Threading.Tasks;

namespace GamePush.Initialization
{
    public class SDK_Initialization
    {
        static string VERSION = "v1.4.0";

        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Execute()
        {
            //PlayerPrefs.DeleteAll();
            GP_Logger.Log("Init plugin...");

            GameObject SDK = new GameObject();

            SDK.name = "GamePushSDK";

            Object.DontDestroyOnLoad(SDK);

            SDK.AddComponent<GP_Init>();
            SetUpInitAwaiter();

            CoreSDK.Initialize();

            SDK.AddComponent<GP_Achievements>();
            SDK.AddComponent<GP_Ads>();
            SDK.AddComponent<GP_Analytics>();
            SDK.AddComponent<GP_App>();
            SDK.AddComponent<GP_AvatarGenerator>();
            SDK.AddComponent<GP_Channels>();
            SDK.AddComponent<GP_Device>();
            SDK.AddComponent<GP_Documents>();
            SDK.AddComponent<GP_Files>();
            SDK.AddComponent<GP_Fullscreen>();
            SDK.AddComponent<GP_Game>();
            SDK.AddComponent<GP_GamesCollections>();
            SDK.AddComponent<GP_Language>();
            SDK.AddComponent<GP_Leaderboard>();
            SDK.AddComponent<GP_LeaderboardScoped>();
            SDK.AddComponent<GP_Payments>();
            SDK.AddComponent<GP_Platform>();
            SDK.AddComponent<GP_Player>();
            SDK.AddComponent<GP_Players>();
            SDK.AddComponent<GP_Server>();
            SDK.AddComponent<GP_Socials>();
            SDK.AddComponent<GP_System>();
            SDK.AddComponent<GP_Variables>();
            SDK.AddComponent<GP_Triggers>();
            SDK.AddComponent<GP_Events>();
            SDK.AddComponent<GP_Experiments>();
            SDK.AddComponent<GP_Segments>();
            SDK.AddComponent<GP_Rewards>();
            SDK.AddComponent<GP_Schedulers>();
            SDK.AddComponent<GP_Images>();
            SDK.AddComponent<GP_Custom>();
            

            SDK.AddComponent<GameStateService>();

            GP_Logger.Log($"plugin ready ({VERSION})");
        }

        private static void SetUpInitAwaiter()
        {
            TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();
            GP_Init.Ready = _tcs.Task;

            GP_Init.OnReady += () => {
                if (!_tcs.Task.IsCompleted)
                    _tcs.SetResult(true);
            };

            GP_Init.OnError += () => {
                if (!_tcs.Task.IsCompleted)
                    _tcs.SetResult(false);
            };
        }
    }
}