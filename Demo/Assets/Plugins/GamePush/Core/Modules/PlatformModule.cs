using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;

namespace GamePush.Core
{
    public class PlatformModule
    {
        public SyncStorageType prefferedSyncType;
        public bool alwaysSyncPublicFields;


        public void Init(PlatformConfig config)
        {
            SetPrefferedSync(config.progressSaveFormat);
            alwaysSyncPublicFields = config.alwaysSyncPublicFields;
        }

        private void SetPrefferedSync(string type)
        {
            if (type == ProgressSaveFormat.Local)
                prefferedSyncType = SyncStorageType.local;

            if (type == ProgressSaveFormat.Cloud)
                prefferedSyncType = SyncStorageType.cloud;

            if (type == ProgressSaveFormat.Platform)
                prefferedSyncType = SyncStorageType.platform;
        }
    }
}
