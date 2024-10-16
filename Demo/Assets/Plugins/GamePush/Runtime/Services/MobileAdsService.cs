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
            CoreSDK.ads.LateInit();
            StartTimer();
        }

        private void OnDestroy()
        {
            StopTimer();
        }


        public void StartTimer()
        {
            // Создание таймера с интервалом 1 секунда (1000 миллисекунд)
            _timer = new Timer(1000);
            _timer.Elapsed += (sender, e) => CoreSDK.ads.CheckLimitsExpired(false);
            _timer.AutoReset = true; // Повторное выполнение таймера
            _timer.Enabled = true;   // Запуск таймера
        }

        public void StopTimer()
        {
            _timer.Stop();
            _timer.Dispose();
        }

    }
}
