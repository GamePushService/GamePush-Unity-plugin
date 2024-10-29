using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.Core
{
    public class DeviceModule
    {
        public bool isMobile { get; private set; }
        public bool isPortrait { get; private set; }

        public void SetPortrate(bool value) => isPortrait = value;

        public event Action OnChangeOrientation;

        public DeviceModule()
        {
#if UNITY_ANDROID
            isMobile = true;
#endif
        }

        public void ChangeOrientation()
        {
            OnChangeOrientation?.Invoke();
        }

    }
}
