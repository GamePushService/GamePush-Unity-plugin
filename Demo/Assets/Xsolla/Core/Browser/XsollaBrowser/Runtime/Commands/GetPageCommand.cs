using System;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public class GetPageCommand : IBrowserCommand
	{
		private readonly Action<IPage> Callback;

		public GetPageCommand(Action<IPage> callback)
		{
			Callback = callback;
		}

		public Task Perform(IPage page)
		{
			Callback?.Invoke(page);
			return Task.CompletedTask;
		}
	}
}