using UnityEngine;

using GamePush;

namespace Examples.Player.JSON
{
    public class SaveSystem
    {
        private const string PLAYER_DATA = "data";

        public void Save(SaveData data)
        {
            GP_Player.Set(PLAYER_DATA, JsonUtility.ToJson(data));
            GP_Player.Sync();
        }

        public SaveData Load()
        {
            string data = GP_Player.GetString(PLAYER_DATA);

            if (data == null || data == "")
                return new SaveData();

            return JsonUtility.FromJson<SaveData>(data);
        }
    }
}