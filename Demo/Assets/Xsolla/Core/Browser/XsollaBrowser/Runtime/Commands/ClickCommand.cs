using System.Threading.Tasks;
using PuppeteerSharp;
using UnityEngine;

namespace Xsolla.XsollaBrowser
{
	public class ClickCommand : IBrowserCommand
	{
		private readonly Vector2 Point;

		public ClickCommand(Vector2 point)
		{
			Point = point;
		}

		public Task Perform(IPage page)
		{
			return page.Mouse.ClickAsync((decimal) Point.x, (decimal) Point.y);
		}
	}
}