using System;
using UnityEngine.Serialization;

namespace GamePush.Data
{
    [System.Serializable]
    public class SavedProjectData
    {
        public int id;
        public string token;

        public bool showPreloadAd;
        public bool showStickyOnStart;
        public bool waitPluginReady;
        public bool autoPause;
        public bool adsStubs = true;
        public bool paymentsStubs = true;
        public bool sdkLive;
        public bool nativeDebugConsole = true;
        public bool nativeOverlays = true;
        public bool autoPauseOnOverlay = true;
        public string androidPlatform = "ANDROID";
        public string androidPlatformTag = "";
    }

}