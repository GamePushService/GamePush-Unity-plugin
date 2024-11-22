using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;

public class ShowAdWhenAvailable : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(CheckEverySec());
    }

    IEnumerator CheckEverySec()
    {
        yield return new WaitForSecondsRealtime(1f);
        if (GP_Ads.IsFullscreenAvailable())
        {
            GP_Ads.ShowFullscreen();
        }

        StartCoroutine(CheckEverySec());
    }
}
