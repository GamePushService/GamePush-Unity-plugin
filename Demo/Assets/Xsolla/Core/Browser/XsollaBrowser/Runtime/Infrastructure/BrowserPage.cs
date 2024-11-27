using System;
using PuppeteerSharp;
using UnityEngine;

namespace Xsolla.XsollaBrowser
{
	public class BrowserPage
	{
		private bool _isFullScreen;
		private string _url = "about:blank";

		private readonly CommandsProcessor CommandsProcessor;
		private readonly MainThreadExecutor MainThreadExecutor;
		private readonly MainThreadLogger MainThreadLogger;

		public BrowserPage(CommandsProcessor commandsProcessor, MainThreadExecutor mainThreadExecutor, MainThreadLogger mainThreadLogger)
		{
			CommandsProcessor = commandsProcessor;
			MainThreadExecutor = mainThreadExecutor;
			MainThreadLogger = mainThreadLogger;
		}

		public Vector2Int ViewportSize { get; private set; }
		public Vector2Int WindowSize { get; set; }
		public Vector2Int FullScreenSize { get; set; }

		public string Url
		{
			get => _url;
			set
			{
				_url = value;
				OnUrlChangedAction?.Invoke(_url);
			}
		}

		public bool IsFullScreen
		{
			get => _isFullScreen;
			set
			{
				_isFullScreen = value;

				var size = _isFullScreen
					? FullScreenSize
					: WindowSize;

				SetRenderSize(size);
			}
		}

		public Action<string> OnUrlChangedAction { get; set; }
		public Action<string, Action> AlertDialogCommand { get; set; }
		public Action<string, Action, Action> ConfirmDialogCommand { get; set; }

		public void SetRenderSize(Vector2Int size)
		{
			var command = new SetViewportCommand(
				size,
				2,
				MainThreadExecutor,
				() => ViewportSize = size);

			CommandsProcessor.AddCommand(command);
		}

		public void GoToUrl(string url, Action callback = null)
		{
			var command = new GoToUrlCommand(
				url,
				MainThreadExecutor,
				callback);

			CommandsProcessor.AddCommand(command);
		}

		public void Click(Vector2 point)
		{
			CommandsProcessor.AddCommand(new ClickCommand(point));
		}

		public void MoveCursorAsync(Vector2 point)
		{
			CommandsProcessor.AddCommand(new MoveCursorCommand(point));
		}

		public void ScrollWheel(Vector2 offset)
		{
			CommandsProcessor.AddCommand(new ScrollWheelCommand(offset));
		}

		public void SendCharacterAsync(string text)
		{
			CommandsProcessor.AddCommand(new SendCharacterCommand(text));
		}

		public void PressKeyAsync(string key)
		{
			CommandsProcessor.AddCommand(new PressKeyCommand(key));
		}

		public void DownKeyAsync(string key)
		{
			CommandsProcessor.AddCommand(new DownKeyCommand(key));
		}

		public void UpKeyAsync(string key)
		{
			CommandsProcessor.AddCommand(new UpKeyCommand(key));
		}

		public void GetScreenshotDataAsync(Action<byte[]> callback)
		{
			CommandsProcessor.AddCommand(new GetScreenshotCommand(callback));
		}

		public void GetUrlAsync(Action<string> callback)
		{
			CommandsProcessor.AddCommand(new GetUrlCommand(callback));
		}

		public void GetPage(Action<IPage> callback)
		{
			CommandsProcessor.AddCommand(new GetPageCommand(callback));
		}
	}
}