using UnityEngine;
using GamePush;

namespace Examples.Init
{
    public class Init : MonoBehaviour
    {
        private void OnEnable()
        {
            GP_Init.OnReady += OnPluginReady;
        }

        private async void Start()
        {
            await GP_Init.Ready;
            Debug.Log($"Init Example: isReady {IsReady()}");
        }

        private bool IsReady() => GP_Init.isReady;

        private void OnPluginReady()
        {
            Debug.Log("Init Example: SDK ready");
        }
    }
}
