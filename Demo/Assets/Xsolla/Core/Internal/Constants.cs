namespace Xsolla.Core
{
	public static class Constants
	{
		public const string SDK_VERSION = "2.4.1";

		public const string DEFAULT_PROJECT_ID = "77640";
		public const string DEFAULT_LOGIN_ID = "026201e3-7e40-11ea-a85b-42010aa80004";
		public const int DEFAULT_OAUTH_CLIENT_ID = 57;
		public const string DEFAULT_REDIRECT_URL = "https://login.xsolla.com/api/blank";
		public const string DEFAULT_WEB_STORE_URL = "https://sitebuilder.xsolla.com/game/sdk-web-store/";

		public const string WEB_BROWSER_RESOURCE_PATH = "XsollaWebBrowser";
		public const string BROWSER_REVISION = "1069273";
		public const string CUSTOM_BROWSER_USER_AGENT = null;

		public const float WEB_SOCKETS_PING_INTERVAL = 25;
		public const float WEB_SOCKETS_PING_LIMIT = 600;
		public const float SHORT_POLLING_INTERVAL = 3;
		public const float SHORT_POLLING_LIMIT = 600;
	}
}