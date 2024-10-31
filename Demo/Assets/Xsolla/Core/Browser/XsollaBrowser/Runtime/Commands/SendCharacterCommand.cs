using System.Threading.Tasks;
using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public class SendCharacterCommand : IBrowserCommand
	{
		private readonly string Character;

		public SendCharacterCommand(string character)
		{
			Character = character;
		}

		public Task Perform(IPage page)
		{
			return page.Keyboard.SendCharacterAsync(Character);
		}
	}
}