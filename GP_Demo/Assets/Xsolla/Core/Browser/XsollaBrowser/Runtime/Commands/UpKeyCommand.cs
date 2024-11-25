using System.Threading.Tasks;
using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public class UpKeyCommand : IBrowserCommand
	{
		private readonly string Key;

		public UpKeyCommand(string key)
		{
			Key = key;
		}

		public Task Perform(IPage page)
		{
			return page.Keyboard.UpAsync(Key);
		}
	}
}