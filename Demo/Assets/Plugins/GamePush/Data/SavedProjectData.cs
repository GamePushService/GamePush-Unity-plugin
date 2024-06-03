using System;

namespace GamePush.Data
{
    [System.Serializable]
    public class SavedProjectData
    {
        public int id;
        public string token;

        public bool showPreAd;

        public int gameReadyDelay;

        public SavedProjectData(
            int id,
            string token,
            bool showPreAd = false,
            int gameReadyDelay = 0)
        {
            this.id = id;
            this.token = token;
            this.showPreAd = showPreAd;
            this.gameReadyDelay = gameReadyDelay;
        }
    }

}