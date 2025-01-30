using System.Collections.Generic;

namespace GamePush.Data
{
    [System.Serializable]
    public class PlayersIdList
    {
        public List<int> idsList;
    }

    [System.Serializable]
    public class PlayersIdArray
    {
        public int[] idsArray;
    }
    
    public abstract class FetchPlayersInput
    {
        public List<int> ids { get; set; } = new();
    }

    public class FetchPlayersOutput
    {
        public List<PlayerOutput> players { get; set; } = new();
    }
}