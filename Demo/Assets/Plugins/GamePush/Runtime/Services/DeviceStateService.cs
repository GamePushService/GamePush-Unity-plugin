using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.Services
{
    [ExecuteAlways]
    public class DeviceStateService : MonoBehaviour
    {
        ScreenOrientation currentOrientation;


        bool isPortrate = CoreSDK.device.isPortrait;

        void SetPortrate(bool isSet) => isPortrate = isSet;

        private void Awake()
        {

        }

        private void Update()
        {
            if(Screen.orientation != currentOrientation)
            {
                switch(Screen.orientation)
                {
                    case ScreenOrientation.Portrait:
                        SetPortrate(true);
                        break;
                    case ScreenOrientation.PortraitUpsideDown:
                        SetPortrate(true);
                        break;
                    case ScreenOrientation.LandscapeLeft:
                        SetPortrate(false);
                        break;
                    case ScreenOrientation.LandscapeRight:
                        SetPortrate(false);
                        break;
                    case ScreenOrientation.AutoRotation:
                        SetPortrate(GetIsPortrate());
                        break;

                }
                currentOrientation = Screen.orientation;
            }

        }


        bool GetIsPortrate()
        {
            if (Screen.width < Screen.height)
                return true;

            return false;
        }

    }

}
