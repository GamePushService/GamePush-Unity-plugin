using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    [System.Serializable]
    public class GamesCollectionsData
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public GamesData[] games;
    }

    [System.Serializable]
    public class GamesData
    {
        public int id;
        public string name;
        public string description;
        public string icon;
        public string url;
    }

    [System.Serializable]
    public class FetchPlayerGamesCollectionInput
    {
        public int? id;
        public string tag;
        public string urlFrom;
    }

    public class FetchPlayerGamesCollectionOutput : GamesCollectionOutput
    {
    }

    public class GamePreview
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        // public string Icon4x3 { get; set; }
        // public string Cover { get; set; }
        // public List<string> AlbumScreenshots { get; set; }
        // public List<string> PortraitScreenshots { get; set; }
    }

    public class ImageAsset
    {
        public List<ImageResource> Resources { get; set; }
    }

    public class ImageResource
    {
        public string Src { get; set; }
    }

    public class GameProject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public GameProjectAssets Assets { get; set; }
    }

    public class GameProjectAssets
    {
        public ImageAsset Icon { get; set; }
        // public ImageAsset Icon4x3 { get; set; }
        // public ImageAsset Cover { get; set; }
        // public List<ImageAsset> AlbumScreenshots { get; set; }
        // public List<ImageAsset> PortraitScreenshots { get; set; }
    }

    public class GamesCollection
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<GamePreview> GamesData { get; set; }
    }

    public class GamesCollectionOutput : GamesCollection
    {
        public new List<GameProject> Games { get; set; }
    }
}


