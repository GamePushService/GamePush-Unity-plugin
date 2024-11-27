using System;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public class GetScreenshotCommand : IBrowserCommand
	{
		private readonly Action<byte[]> Callback;

		public GetScreenshotCommand(Action<byte[]> callback)
		{
			Callback = callback;
		}

		public async Task Perform(IPage page)
		{
			var options = new ScreenshotOptions {
				Type = ScreenshotType.Jpeg,
				Quality = 100
			};

			var data = await page.ScreenshotDataAsync(options);
			Callback?.Invoke(data);
		}
	}
}