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

        public string buildPlatform;

        public SavedProjectData(
            int id,
            string token,
            bool showPreAd = false,
            bool gameReadyAuto = false,
            string buildPlatform = "NONE")
        {
            this.id = id;
            this.token = token;
            this.showPreAd = showPreAd;
            this.gameReadyAuto = gameReadyAuto;
            this.buildPlatform = buildPlatform;
        }
    }

}