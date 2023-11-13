using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;

public class ModuleTests : MonoBehaviour
{
    public void CanReview() => GP_App.CanReview();

    public void GameReady() => GP_Game.GameReady();

    public void Test1() => GP_Ads.ShowFullscreen();
}
