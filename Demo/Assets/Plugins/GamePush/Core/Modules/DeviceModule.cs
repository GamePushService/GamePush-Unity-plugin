using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.Core
{
    public class DeviceModule
    {
        public bool _isMobile { get; private set; }
        public bool _isPortrait { get; private set; }
        public bool _isFullscreen { get; private set; }

        public void SetPortrate(bool value) => _isPortrait = value;

        public event Action OnChangeOrientation;

        public event Action OnFullscreenOpen;
        public event Action OnFullscreenClose;
        public event Action OnFullscreenChange;

        public DeviceModule()
        {
#if UNITY_ANDROID
            isMobile = true;
#endif
            _isFullscreen = Screen.fullScreen;
        }

        public void ChangeOrientation()
        {
            OnChangeOrientation?.Invoke();
        }

        private void SetFullscreen(bool fullscreen)
        {
            if (_isFullscreen == fullscreen) return;

            _isFullscreen = fullscreen;
            Screen.fullScreen = fullscreen;
            OnFullscreenChange?.Invoke();

            if(fullscreen)
                OnFullscreenOpen?.Invoke();
            else
                OnFullscreenClose?.Invoke();
        }

        public void ToggleFullscreen() => SetFullscreen(!_isFullscreen);

        public void OpenFullscreen() => SetFullscreen(true);

        public void CloseFullscreen() => SetFullscreen(false);
    
    }
}
