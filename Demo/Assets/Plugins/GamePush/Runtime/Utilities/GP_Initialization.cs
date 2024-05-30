using UnityEngine;
using GamePush;
using GP_Utilities.Console;
using System.Threading.Tasks;
using System;

namespace GP_Utilities.Initialization
{
    public class GP_Initialization
    {
        static string VERSION = "v1.4.0";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Execute()
        {

            GameObject SDK = new GameObject();
            SDK.name = "GamePushSDK";
            UnityEngine.Object.DontDestroyOnLoad(SDK);


            Debug.Log("GamePush start init");
            SetUpInitAwaiter();

            SDK.AddComponent<GP_Init>();

            SDK.AddComponent<GP_ConsoleController>();
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

            Debug.Log($"GamePush plugin ready ({VERSION})");
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
