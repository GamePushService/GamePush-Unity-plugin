using System.Collections;
using System.Threading;
using PuppeteerSharp;
using UnityEngine;

namespace Xsolla.XsollaBrowser
{
	public class DialogHandler : MonoBehaviour, IBrowserHandler
	{
		private BrowserPage Page;
		private IPage BrowserPage;

		public void Run(BrowserPage page, CancellationToken cancellationToken)
		{
			Page = page;
			StartCoroutine(SubscribeOnPage(cancellationToken));
		}

		public void Stop()
		{
			if (BrowserPage != null)
				BrowserPage.Dialog -= OnDialogAppears;

			StopAllCoroutines();
		}

		private IEnumerator SubscribeOnPage(CancellationToken cancellationToken)
		{
			Page.GetPage(x => BrowserPage = x);
			yield return new WaitUntil(() => BrowserPage != null);

			if (!cancellationToken.IsCancellationRequested)
				BrowserPage.Dialog += OnDialogAppears;
		}

		private void OnDialogAppears(object _, DialogEventArgs args)
		{
			var dialog = args.Dialog;
			switch (dialog.DialogType)
			{
				case DialogType.Alert:
					Page.AlertDialogCommand?.Invoke(dialog.Message, () => dialog.Accept());
					break;
				case DialogType.Confirm:
					Page.ConfirmDialogCommand?.Invoke(dialog.Message, () => dialog.Accept(), () => dialog.Dismiss());
					break;
				default:
					dialog.Accept();
					break;
			}
		}
	}
}