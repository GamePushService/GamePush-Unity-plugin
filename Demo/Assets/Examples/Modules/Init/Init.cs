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
            OnPluginReady();
        }

        private void CheckReady()
        {
            if (GP_Init.isReady)
            {
                OnPluginReady();
            }
        }

        private void OnPluginReady()
        {
            Debug.Log("Init Example: Plugin ready");
        }
    }
}
