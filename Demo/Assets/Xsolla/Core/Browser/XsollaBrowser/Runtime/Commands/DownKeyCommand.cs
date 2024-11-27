using System.Threading.Tasks;
using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public class DownKeyCommand : IBrowserCommand
	{
		private readonly string Key;

		public DownKeyCommand(string key)
		{
			Key = key;
		}

		public Task Perform(IPage page)
		{
			return page.Keyboard.DownAsync(Key);
		}
	}
}