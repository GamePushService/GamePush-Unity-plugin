using System.Threading.Tasks;
using PuppeteerSharp;
using UnityEngine;

namespace Xsolla.XsollaBrowser
{
	public class MoveCursorCommand : IBrowserCommand
	{
		private readonly Vector2 Point;

		public MoveCursorCommand(Vector2 point)
		{
			Point = point;
		}

		public Task Perform(IPage page)
		{
			return page.Mouse.MoveAsync((decimal) Point.x, (decimal) Point.y);
		}
	}
}