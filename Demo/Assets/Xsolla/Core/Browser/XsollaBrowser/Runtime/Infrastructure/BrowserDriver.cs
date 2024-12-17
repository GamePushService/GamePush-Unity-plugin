using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Xsolla.XsollaBrowser
{
	public class BrowserDriver : IDisposable
	{
		private readonly FetchOptions FetchOptions;
		private readonly LaunchOptions LaunchOptions;
		private readonly CommandsProcessor CommandsProcessor;
		private readonly MainThreadExecutor MainThreadExecutor;
		private readonly MainThreadLogger MainThreadLogger;
		private readonly CancellationToken CancellationToken;
		private readonly Action<int> FetchProgressAction;
		private readonly Action<string> OpenExternalUrlAction;

		private Thread Thread;
		private IBrowser Browser;
		private bool IsLaunched;

		public BrowserDriver(
			FetchOptions fetchOptions,
			LaunchOptions launchOptions,
			CommandsProcessor commandsProcessor,
			MainThreadExecutor mainThreadExecutor,
			MainThreadLogger mainThreadLogger,
			CancellationToken cancellationToken,
			Action<int> fetchProgressAction,
			Action<string> openExternalUrlAction)
		{
			FetchOptions = fetchOptions;
			LaunchOptions = launchOptions;
			CommandsProcessor = commandsProcessor;
			MainThreadExecutor = mainThreadExecutor;
			MainThreadLogger = mainThreadLogger;
			CancellationToken = cancellationToken;
			FetchProgressAction = fetchProgressAction;
			OpenExternalUrlAction = openExternalUrlAction;
		}

		public void Dispose()
		{
			Thread?.Join();
		}

		public async Task Launch()
		{
			IsLaunched = false;
			Thread = new Thread(DriverThread);
			Thread.Start();

			while (IsLaunched == false)
			{
				await Task.Delay(50);
			}
		}

		private async void DriverThread()
		{
			MainThreadLogger.Log("Browser driver thread started");
			Thread.Sleep(50);
			MainThreadLogger.Log("Start fetching browser");

			var fetcherOptions = new BrowserFetcherOptions {
				Product = FetchOptions.Product,
				Platform = FetchOptions.Platform,
				Path = FetchOptions.Path
			};

			var fetcher = new BrowserFetcher(fetcherOptions);
			fetcher.DownloadProgressChanged += OnDownloadProgressChanged;
			await fetcher.DownloadAsync(FetchOptions.Revision);
			fetcher.DownloadProgressChanged -= OnDownloadProgressChanged;

			MainThreadLogger.Log("Finish fetching browser");
			Thread.Sleep(50);
			MainThreadLogger.Log("Start launching browser");

			var launchOptions = new PuppeteerSharp.LaunchOptions {
				Product = FetchOptions.Product,
				Headless = LaunchOptions.Headless,
				Devtools = LaunchOptions.DevTools,
				ExecutablePath = fetcher.GetExecutablePath(FetchOptions.Revision),
				Args = LaunchOptions.Args,
				DefaultViewport = new ViewPortOptions {
					Width = LaunchOptions.ViewportWidth,
					Height = LaunchOptions.ViewportHeight
				}
			};

#pragma warning disable CS1701
			Browser = await Puppeteer.LaunchAsync(launchOptions);
#pragma warning restore CS1701

			MainThreadLogger.Log("Finish launching browser");
			Thread.Sleep(50);

			var pages = await Browser.PagesAsync();
			var page = pages.Length > 0
				? pages[0]
				: await Browser.NewPageAsync();

			Thread.Sleep(50);
			MainThreadLogger.Log("Start processing commands");
			IsLaunched = true;

			Browser.TargetCreated += OnBrowserOnTargetCreated;

			while (!CancellationToken.IsCancellationRequested)
			{
				try
				{
					await CommandsProcessor.ProcessCommands(page, CancellationToken);
				}
				catch (TaskCanceledException)
				{
					break;
				}
			}

			Browser.TargetCreated += OnBrowserOnTargetCreated;

			MainThreadLogger.Log("Browser driver thread was cancelled");

			MainThreadLogger.Log("Closing browser page");
			await page.CloseAsync();
			page.Dispose();

			MainThreadLogger.Log("Closing browser");
			await Browser.CloseAsync();
			Browser.Dispose();

			MainThreadLogger.Log("Browser driver thread finished");
		}

		private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			FetchProgressAction?.Invoke(e.ProgressPercentage);
		}

		private async void OnBrowserOnTargetCreated(object sender, TargetChangedArgs e)
		{
			if (e.Target.Type != TargetType.Page)
				return;

			if (Browser == null)
				return;

			if (CancellationToken.IsCancellationRequested)
				return;

			var allPages = await Browser.PagesAsync();
			if (CancellationToken.IsCancellationRequested)
				return;

			if (allPages.Length <= 1)
				return;

			var newPage = await e.Target.PageAsync();
			if (CancellationToken.IsCancellationRequested)
				return;

			MainThreadExecutor.Enqueue(() => OpenExternalUrlAction?.Invoke(newPage.Url));
			await newPage.CloseAsync();
		}
	}
}