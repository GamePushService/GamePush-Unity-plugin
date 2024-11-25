using System.Threading.Tasks;
using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public class PressKeyCommand : IBrowserCommand
	{
		private readonly string Key;

		public PressKeyCommand(string key)
		{
			Key = key;
		}

		public Task Perform(IPage page)
		{
			return page.Keyboard.PressAsync(Key);
		}
	}
}