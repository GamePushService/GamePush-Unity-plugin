using UnityEngine;
using UnityEngine.SceneManagement;
using GamePush;

namespace Examples.Init
{
    public class Init : MonoBehaviour
    {
        private void OnEnable()
        {
            GP_Init.OnReady += OnPluginReady;
            GP_Init.OnError += OnPluginError;
        }

        private void OnDisable()
        {
            GP_Init.OnReady -= OnPluginReady;
            GP_Init.OnError -= OnPluginError;
        }

        private async void Start()
        {
            await GP_Init.Ready;
            GP_Logger.SystemLog("Init Example: isReady {IsReady()}");
            SceneManager.LoadScene(1);
        }

        private bool IsReady() => GP_Init.isReady;

        private void OnPluginReady()
        {
            GP_Logger.SystemLog("Init Example: SDK ready");
        }

        private void OnPluginError()
        {
            GP_Logger.SystemLog("Init Example: SDK error");
        }
    }
}
