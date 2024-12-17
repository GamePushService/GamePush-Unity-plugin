namespace Xsolla.XsollaBrowser
{
	public struct LaunchOptions
	{
		public int ViewportWidth { get; set; }
		public int ViewportHeight { get; set; }

		public bool Headless { get; set; }
		public bool DevTools { get; set; }

		public string[] Args { get; set; }
	}
}