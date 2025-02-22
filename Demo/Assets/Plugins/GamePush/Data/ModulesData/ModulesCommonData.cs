namespace GamePush
{
    public enum ModuleName
    {
        None,
        Init,
        Achievements,
        Ads,
        Analytics,
        App,
        AvatarGenerator,
        Channels,
        Custom,
        Device,
        Documents,
        Events,
        Experiments,
        Files,
        Fullscreen,
        Game,
        GamesCollections,
        Images,
        Language,
        Leaderboard,
        LeaderboardScoped,
        Payments,
        Platform,
        Player,
        Players,
        Rewards,
        Schedulers,
        Segments,
        Server,
        Socials,
        System,
        Variables,
        Triggers,
        Uniques,
        Storage
    }
    
    [System.Serializable]
    public class BonusData
    {
        public string type;
        public int id;
    }
    
    // [System.Serializable]
    // public class Condition
    // {
    //     public string type;
    //     public string key;
    //     public string @operator;
    //     public List<string> value;
    // }
}
