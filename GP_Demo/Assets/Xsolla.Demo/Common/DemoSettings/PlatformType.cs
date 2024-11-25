namespace Xsolla.Demo
{
	public enum PlatformType
	{
		None,
		Xsolla,
		GooglePlay,
		AppStore,
		PlaystationNetwork,
		XboxLive,
		NintendoShop,
		Android_Standalone,
		iOS_Standalone,
		PC_Standalone,
		Android_Other,
		iOS_Other,
		PC_Other
	}

	public static class PlatformExtensions
	{
		public static string GetString(this PlatformType platform)
		{
			switch(platform) {
				case PlatformType.None: return string.Empty;
				case PlatformType.Xsolla: return "xsolla";
				case PlatformType.AppStore: return "app_store_ios";
				case PlatformType.GooglePlay: return "google_play";
				case PlatformType.NintendoShop: return "nintendo_shop";
				case PlatformType.PlaystationNetwork: return "playstation_network";
				case PlatformType.XboxLive: return "xbox_live";
				case PlatformType.Android_Standalone: return "android_standalone";
				case PlatformType.iOS_Standalone: return "ios_standalone";
				case PlatformType.PC_Standalone: return "pc_standalone";
				case PlatformType.Android_Other: return "android_other";
				case PlatformType.iOS_Other: return "ios_other";
				case PlatformType.PC_Other: return "pc_other";
				default: return "pc_other";
			}
		}
	}
}
