using System;

namespace GamePush.Data
{
    [System.Serializable]
    public class SavedProjectData
    {
        public string id, token;

        public SavedProjectData(string id, string token)
        {
            this.id = id;
            this.token = token;
        }
    }

}