using System;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public class GetUrlCommand : IBrowserCommand
	{
		private readonly Action<string> Callback;

		public GetUrlCommand(Action<string> callback)
		{
			Callback = callback;
		}

		public Task Perform(IPage page)
		{
			Callback?.Invoke(page.Url);
			return Task.CompletedTask;
		}
	}
}