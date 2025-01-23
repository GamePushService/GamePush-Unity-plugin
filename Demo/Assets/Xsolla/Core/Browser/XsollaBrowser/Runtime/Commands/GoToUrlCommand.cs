using System;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public class GoToUrlCommand : IBrowserCommand
	{
		private readonly string Url;
		private readonly MainThreadExecutor MainThreadExecutor;
		private readonly Action Callback;

		public GoToUrlCommand(string url, MainThreadExecutor mainThreadExecutor, Action callback)
		{
			Url = url;
			MainThreadExecutor = mainThreadExecutor;
			Callback = callback;
		}

		public async Task Perform(IPage page)
		{
			await page.GoToAsync(Url);
			MainThreadExecutor.Enqueue(Callback);
		}
	}
}