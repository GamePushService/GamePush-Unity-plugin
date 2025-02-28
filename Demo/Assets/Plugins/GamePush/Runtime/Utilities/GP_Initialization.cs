using UnityEngine;
using UnityEngine.SceneManagement;
using GamePush;
using GamePush.Data;
using GamePush.ConsoleController;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;
using System.Collections;

namespace GamePush.Initialization
{
    
    public class GP_Initialization
    {
        public static string VERSION = PluginData.SDK_VERSION;

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_UnityReady();
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Execute()
        {

#if !UNITY_EDITOR && UNITY_WEBGL
             GP_UnityReady();
#endif
            GameObject SDK = new GameObject();
            SDK.name = "GamePushSDK";
            UnityEngine.Object.DontDestroyOnLoad(SDK);

#if UNITY_EDITOR
            SDK.AddComponent<GP_ConsoleController>();
#endif
            SDK.AddComponent<GP_Logger>();

            SDK.AddComponent<GP_Init>();
            SetUpInitAwaiter();

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
            SDK.AddComponent<GP_Uniques>();
            SDK.AddComponent<GP_Storage>();
            SDK.AddComponent<GP_Windows>();

            if (ProjectData.AUTO_PAUSE_ON_ADS)
            {
                SDK.AddComponent<GP_PauseLogic>();
            }

            EndInit();
        }

        private static async void EndInit()
        {
            await EndInitTask();
        }

        private static async Task EndInitTask()
        {
            await GP_Init.Ready;

            GP_Logger.Info($"Plugin {VERSION}", "Initialize");
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
