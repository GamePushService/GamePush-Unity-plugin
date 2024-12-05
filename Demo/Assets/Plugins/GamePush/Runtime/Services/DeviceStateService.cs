using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Core;

namespace GamePush.Services
{
    //[ExecuteAlways]
    public class DeviceStateService : MonoBehaviour
    {
        private ScreenOrientation _currentOrientation;
        //private bool _isFullscreen;

        void SetPortrate(bool value)
        {
            if (CoreSDK.isInit)
                CoreSDK.device.SetPortrate(value);
        }
        private async void Start()
        {
            await GP_Init.Ready;
            SetOrientation(Screen.orientation);
        }

        private void Update()
        {
            if(Screen.orientation != _currentOrientation)
            {
                SetOrientation(Screen.orientation);
                _currentOrientation = Screen.orientation;
                CoreSDK.device.ChangeOrientation();
            }
        }

        void SetOrientation(ScreenOrientation orientation)
        {
            switch (orientation)
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
        }

        bool GetIsPortrate()
        {
            if (Screen.width < Screen.height)
                return true;

            return false;
        }

    }

}
