using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;

public class ShowAdWhenAvailable : MonoBehaviour
{
    private void Start()
    {
        //Запуск таймера вызова рекламы
        StartCoroutine(CheckEverySec());
    }

    //Проверка доступности рекламы каждую секунду
    IEnumerator CheckEverySec()
    {
        yield return new WaitForSecondsRealtime(1f);
        if (GP_Ads.IsFullscreenAvailable())
        {
            GP_Ads.ShowFullscreen();
        }

        StartCoroutine(CheckEverySec());
    }


    void Update()
    {
        // Проверка доступности рекламы каждый кадр
        if (GP_Ads.IsFullscreenAvailable())
        {
            GP_Ads.ShowFullscreen();
        }
    }
}
