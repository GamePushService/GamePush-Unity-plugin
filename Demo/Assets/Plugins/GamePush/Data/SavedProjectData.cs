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
        public bool gameReadyAuto;
        public bool autoPause;
    }

}