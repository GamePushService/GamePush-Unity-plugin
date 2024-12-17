using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public struct FetchOptions
	{
		public Platform Platform { get; set; }
		public Product Product { get; set; }

		public string Revision { get; set; }
		public string Path { get; set; }
	}
}