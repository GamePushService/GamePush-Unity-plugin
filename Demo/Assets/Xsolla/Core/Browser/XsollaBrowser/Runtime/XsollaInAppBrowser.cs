using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.XsollaBrowser
{
	public class XsollaInAppBrowser : MonoBehaviour, IInAppBrowser
	{
		[SerializeField] private GameObject LoaderViewRoot;
		[SerializeField] private GameObject PageViewRoot;
		[SerializeField] private Text FetchingProgressText;
		[SerializeField] private Canvas Canvas;
		[SerializeField] private GraphicRaycaster GraphicRaycaster;
		[SerializeField] private CanvasScaler CanvasScaler;

		private readonly List<Action> DeferredActions = new List<Action>();

		private BrowserDriver BrowserDriver;
		private BrowserPage BrowserPage;
		private IBrowserHandler[] Handlers;
		private CancellationTokenSource BrowserCancellationSource;
		private MainThreadExecutor MainThreadExecutor;
		private MainThreadLogger MainThreadLogger;
		private CommandsProcessor CommandsProcessor;

		public event Action OpenEvent;
		public event Action<BrowserCloseInfo> CloseEvent;
		public event Action<string> UrlChangeEvent;
		public event Action<string, Action> AlertDialogEvent;
		public event Action<string, Action, Action> ConfirmDialogEvent;
		public bool IsOpened { get; private set; }
		public bool IsFullScreen => BrowserPage != null && BrowserPage.IsFullScreen;

		private bool IsAlreadyClosing;
		private int LastFetchingProgress;

		private void Awake()
		{
			ToggleCanvas(false);
			LoaderViewRoot.SetActive(false);
			PageViewRoot.SetActive(false);
		}

		private void OnDestroy()
		{
			Close();
		}

		public void Open(string url)
		{
			OpenUrlAsync(url);
		}

		public void Close(float delay = 0, bool isManually = false)
		{
			if (IsAlreadyClosing)
				return;

			IsAlreadyClosing = true;

			if (Handlers != null)
			{
				foreach (var handler in Handlers)
				{
					handler.Stop();
				}
			}

			CloseEvent?.Invoke(new BrowserCloseInfo {
				isManually = isManually
			});

			BrowserCancellationSource?.Cancel();
			BrowserDriver?.Dispose();
			BrowserDriver = null;

			OpenEvent = null;
			CloseEvent = null;
			UrlChangeEvent = null;
			AlertDialogEvent = null;
			ConfirmDialogEvent = null;

			if (gameObject)
				Destroy(gameObject);
		}

		public void AddInitHandler(Action callback)
		{
			OpenEvent += callback;
		}

		public void AddCloseHandler(Action callback)
		{
			CloseEvent += _ => callback?.Invoke();
		}

		public void AddUrlChangeHandler(Action<string> callback)
		{
			UrlChangeEvent += callback;
		}

		public void UpdateSize(int width, int height)
		{
			var size = new Vector2Int(width, height);

			if (IsOpened)
				BrowserPage.WindowSize = size;
			else
				DeferredActions.Add(() => BrowserPage.WindowSize = size);

			if (!IsFullScreen && IsOpened)
				BrowserPage.SetRenderSize(size);
		}

		public void SetFullscreenMode(bool value)
		{
			if (IsOpened)
				BrowserPage.IsFullScreen = value;
			else
				DeferredActions.Add(() => BrowserPage.IsFullScreen = value);
		}

		private async void OpenUrlAsync(string url)
		{
			if (BrowserDriver == null)
			{
				MainThreadExecutor = gameObject.AddComponent<MainThreadExecutor>();
				MainThreadLogger = new MainThreadLogger(MainThreadExecutor);
				CommandsProcessor = new CommandsProcessor(MainThreadLogger);
				BrowserCancellationSource = new CancellationTokenSource();

				ToggleCanvas(true);
				ToggleLoader(true);

				var fetchOptions = BrowserSettings.GetFetchOptions();
				var launchOptions = BrowserSettings.GetLaunchOptions();
				await CreatedDriver(fetchOptions, launchOptions);

				CreatePage();
				SetupViewport(launchOptions);
				SetupComponents();

				BrowserPage.GoToUrl(url);
				await Task.Delay(1000);

				foreach (var action in DeferredActions)
				{
					action.Invoke();
				}
				DeferredActions.Clear();

				BrowserPage.GoToUrl(url, () => {
					ToggleLoader(false);
					IsOpened = true;
					OpenEvent?.Invoke();
				});
			}
			else
			{
				BrowserPage.GoToUrl(url);
			}
		}

		private Task CreatedDriver(FetchOptions fetchOptions, LaunchOptions launchOptions)
		{
			BrowserDriver = new BrowserDriver(
				fetchOptions,
				launchOptions,
				CommandsProcessor,
				MainThreadExecutor,
				MainThreadLogger,
				BrowserCancellationSource.Token,
				UpdateFetchingProgress,
				OpenExternalUrl);

			return BrowserDriver.Launch();
		}

		private void CreatePage()
		{
			BrowserPage = new BrowserPage(CommandsProcessor, MainThreadExecutor, MainThreadLogger) {
				OnUrlChangedAction = x => UrlChangeEvent?.Invoke(x),
				AlertDialogCommand = (message, accept) => AlertDialogEvent?.Invoke(message, accept),
				ConfirmDialogCommand = (message, accept, dismiss) => ConfirmDialogEvent?.Invoke(message, accept, dismiss)
			};
		}

		private void SetupViewport(LaunchOptions launchOptions)
		{
			var renderSize = new Vector2Int(
				launchOptions.ViewportWidth,
				launchOptions.ViewportHeight);

			BrowserPage.WindowSize = renderSize;
			BrowserPage.FullScreenSize = CalculateFullScreenRenderSize();
			CanvasScaler.referenceResolution = BrowserPage.FullScreenSize;

			BrowserPage.SetRenderSize(renderSize);
		}

		private void SetupComponents()
		{
			Handlers = GetComponentsInChildren<IBrowserHandler>();
			foreach (var handler in Handlers)
			{
				handler.Run(BrowserPage, BrowserCancellationSource.Token);
			}
		}

		private void ToggleCanvas(bool isActive)
		{
			Canvas.enabled = isActive;
			GraphicRaycaster.enabled = isActive;
			CanvasScaler.enabled = isActive;
		}

		private void ToggleLoader(bool isLoader)
		{
			LoaderViewRoot.SetActive(isLoader);
			PageViewRoot.SetActive(!isLoader);

			FetchingProgressText.gameObject.SetActive(false);
		}

		private void UpdateFetchingProgress(int progress)
		{
			MainThreadExecutor.Enqueue(() => {
				FetchingProgressText.gameObject.SetActive(true);

				if (LastFetchingProgress == progress)
					return;

				FetchingProgressText.text = $"Update[%]: {LastFetchingProgress} => {progress}";
				LastFetchingProgress = progress;

				if (progress == 100)
					FetchingProgressText.gameObject.SetActive(false);
			});
		}

		private void OpenExternalUrl(string url)
		{
			Application.OpenURL(url);
		}
		
		private static Vector2Int CalculateFullScreenRenderSize()
		{
			var renderSize = new Vector2Int(Screen.width, Screen.height);

			var factor = (float) renderSize.x / renderSize.y;
			var isWidthRule = renderSize.x > renderSize.y;

			if (isWidthRule)
			{
				renderSize.x = 1920;
				renderSize.y = (int) (renderSize.x / factor);
			}
			else
			{
				renderSize.y = 1080;
				renderSize.x = (int) (renderSize.y * factor);
			}

			return renderSize;
		}
	}
}