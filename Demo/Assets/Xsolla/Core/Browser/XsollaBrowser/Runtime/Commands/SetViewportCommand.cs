using System;
using System.Threading.Tasks;
using PuppeteerSharp;
using UnityEngine;

namespace Xsolla.XsollaBrowser
{
	public class SetViewportCommand : IBrowserCommand
	{
		private readonly int Width;
		private readonly int Height;
		private readonly int DeviceScaleFactor;
		private readonly MainThreadExecutor MainThreadExecutor;
		private readonly Action Callback;

		public SetViewportCommand(Vector2Int size, int deviceScaleFactor, MainThreadExecutor mainThreadExecutor, Action callback)
		{
			Width = size.x;
			Height = size.y;
			DeviceScaleFactor = deviceScaleFactor;
			MainThreadExecutor = mainThreadExecutor;
			Callback = callback;
		}

		public async Task Perform(IPage page)
		{
			var options = new ViewPortOptions {
				Width = Width,
				Height = Height,
				DeviceScaleFactor = DeviceScaleFactor,
				IsLandscape = true
			};

			await page.SetViewportAsync(options);
			MainThreadExecutor.Enqueue(Callback);
		}
	}
}