using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.Services
{
    public class MobileAdsService : MonoBehaviour
    {
        private async void Start()
        {
            await GP_Init.Ready;
            CoreSDK.ads.LateInit();
        }
    }
}
