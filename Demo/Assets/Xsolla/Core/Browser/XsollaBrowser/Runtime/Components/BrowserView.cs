using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.XsollaBrowser
{
	public class BrowserView : MonoBehaviour, IBrowserHandler
	{
		[SerializeField] private Button CloseButton;
		[SerializeField] private Button FullscreenButton;

		public void Run(BrowserPage page, CancellationToken cancellationToken)
		{
			CloseButton.onClick.AddListener(OnCloseButtonClick);
			FullscreenButton.onClick.AddListener(OnFullscreenButtonClick);

			StartCoroutine(TrackFullScreenExitLoop(cancellationToken));
		}

		public void Stop()
		{
			CloseButton.onClick.RemoveListener(OnCloseButtonClick);
			FullscreenButton.onClick.RemoveListener(OnFullscreenButtonClick);

			StopAllCoroutines();
		}

		private IEnumerator TrackFullScreenExitLoop(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					if (XsollaWebBrowser.InAppBrowser.IsFullScreen)
						XsollaWebBrowser.InAppBrowser.SetFullscreenMode(false);
					else
						XsollaWebBrowser.Close();
				}

				yield return null;
			}
		}

		private static void OnCloseButtonClick()
		{
			XsollaWebBrowser.InAppBrowser.Close(0, true);
		}

		private static void OnFullscreenButtonClick()
		{
			XsollaWebBrowser.InAppBrowser.SetFullscreenMode(true);
		}
	}
}