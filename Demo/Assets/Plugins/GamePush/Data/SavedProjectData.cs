using System;

namespace GamePush.Data
{
    [System.Serializable]
    public class SavedProjectData
    {
        public int id;
        public string token;

        public bool showPreAd;
        public bool gameReadyAuto;

        public int buildPlatformId;

        public SavedProjectData(
            int id,
            string token,
            bool showPreAd = false,
            bool gameReadyAuto = false,
            int buildPlatform = 0)
        {
            this.id = id;
            this.token = token;
            this.showPreAd = showPreAd;
            this.gameReadyAuto = gameReadyAuto;
            this.buildPlatformId = buildPlatform;
        }
    }

}