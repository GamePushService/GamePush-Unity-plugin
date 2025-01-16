using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

namespace GamePush.Services
{
    public class MobileAdsService : MonoBehaviour
    {
        private Timer _timer;

        private async void Start()
        {
            await GP_Init.Ready;
            CoreSDK.Ads.CustomAdInit();
            StartTimer();
        }

        private void OnDestroy()
        {
            StopTimer();
        }

        public void StartTimer()
        {
            StopTimer();
            _timer = new Timer(3000);
            _timer.Elapsed += (sender, e) => CoreSDK.Ads.CheckLimitsExpired();
            _timer.AutoReset = true; // Повторное выполнение таймера
            _timer.Enabled = true;   // Запуск таймера
        }

        public void StopTimer()
        {
            if(_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
           
        }

    }
}
