using System.Threading.Tasks;
using PuppeteerSharp;
using UnityEngine;

namespace Xsolla.XsollaBrowser
{
	public class ScrollWheelCommand : IBrowserCommand
	{
		private readonly Vector2 Offset;

		public ScrollWheelCommand(Vector2 offset)
		{
			Offset = offset;
		}

		public Task Perform(IPage page)
		{
			return page.Mouse.WheelAsync((decimal) Offset.x, (decimal) Offset.y);
		}
	}
}