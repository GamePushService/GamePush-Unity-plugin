using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;

public class PauseLogic : MonoBehaviour
{
    private static bool tempMute;
    private static bool gamePause;

    private static bool adPause;

    private void OnEnable()
    {
        GP_Game.OnPause += PauseGame;
        GP_Game.OnResume += UnpauseGame;

        GP_Ads.OnPreloaderStart += AdStart;
        GP_Ads.OnFullscreenStart += AdStart;
        GP_Ads.OnRewardedStart += AdStart;

        GP_Ads.OnAdsClose += AdClose;
    }

    private void OnDisable()
    {
        GP_Game.OnPause -= PauseGame;
        GP_Game.OnResume -= UnpauseGame;

        GP_Ads.OnPreloaderStart -= AdStart;
        GP_Ads.OnFullscreenStart -= AdStart;
        GP_Ads.OnRewardedStart -= AdStart;

        GP_Ads.OnAdsClose -= AdClose;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            UnpauseGame();
        else
            PauseGame();
    }

    private static void PauseGame()
    {
        if (gamePause) return;
        gamePause = true;
        GP_Logger.Log($"Game On Pause: {gamePause}");

        tempMute = AudioListener.pause;
        MusicOff();
        Time.timeScale = 0f;
    }

    private static void UnpauseGame()
    {
        if (!gamePause || adPause) return;
        gamePause = false;
        GP_Logger.Log($"Game On Pause: {gamePause}");

        if (!tempMute) MusicOn();
        Time.timeScale = 1;
    }

    private static void MusicOff() => AudioListener.pause = true;
    private static void MusicOn() => AudioListener.pause = false;


    private static void AdStart()
    {
        GP_Logger.Log($"Ad Start");

        adPause = true;
        PauseGame();
    }

    private static void AdClose(bool succes)
    {
        GP_Logger.Log($"Ad Close: {succes}");

        adPause = false;
        UnpauseGame();
    }
}
