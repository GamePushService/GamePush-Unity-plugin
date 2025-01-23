using UnityEngine;

namespace Xsolla.XsollaBrowser
{
	public class MainThreadLogger
	{
		private const string LOG_PREFIX = "[Xsolla BROWSER]";

		private readonly MainThreadExecutor MainThreadExecutor;

		public MainThreadLogger(MainThreadExecutor mainThreadExecutor)
		{
			MainThreadExecutor = mainThreadExecutor;
		}

		public void Log(string message)
		{
			if (MainThreadExecutor)
				MainThreadExecutor.Enqueue(() => Debug.Log($"{LOG_PREFIX} {message}"));
		}
	}
}