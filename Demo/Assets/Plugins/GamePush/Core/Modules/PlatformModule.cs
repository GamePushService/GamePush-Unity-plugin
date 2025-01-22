using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;

namespace GamePush.Core
{
    public class PlatformModule
    {
        public string Type { get; private set; }
        public string Tag { get; private set; }
        public string AppId { get; private set; }
        public string GameLink { get; private set; }
        public string ProgressSaveFormat { get; private set; }

        public SyncStorageType PrefferedSyncType { get; private set; }
        public bool AlwaysSyncPublicFields;

        public bool hasIntegratedAuth;
        public bool isLogoutAvailable;
        public bool isExternalLinksAllowed;
        public bool isSecretCodeAuthAvailable;
      
        public bool isSupportsImageUploading;
        public bool hasAccountLinkingFeature;
        public bool isSupportsRemoteVariables;
        public bool isSupportsCloudSaves;


        public void Init(PlatformConfig config)
        {
            Type = config.type;
            Tag = config.tag;
            AppId = config.appId;
            GameLink = config.gameLink;
            ProgressSaveFormat = config.progressSaveFormat;

            SetPrefferedSync(config.progressSaveFormat);
            AlwaysSyncPublicFields = config.alwaysSyncPublicFields;
            
#if UNITY_ANDROID
            hasIntegratedAuth = false;
            isLogoutAvailable = false;

            isExternalLinksAllowed = true;
            isSecretCodeAuthAvailable = true;

            isSupportsImageUploading = false;
            hasAccountLinkingFeature = false;
            isSupportsRemoteVariables = false;
            isSupportsCloudSaves = false;
#endif

        }


            private void SetPrefferedSync(string type)
        {
            PrefferedSyncType = type switch
            {
                GamePush.ProgressSaveFormat.Local => SyncStorageType.local,
                GamePush.ProgressSaveFormat.Cloud => SyncStorageType.cloud,
                GamePush.ProgressSaveFormat.Platform => SyncStorageType.platform,
                _ => SyncStorageType.cloud,
            };
        }
    }
}
