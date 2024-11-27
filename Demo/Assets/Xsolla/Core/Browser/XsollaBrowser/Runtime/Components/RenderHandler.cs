using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.XsollaBrowser
{
	public class RenderHandler : MonoBehaviour, IBrowserHandler
	{
		[SerializeField] private RectTransform ViewportRect;
		[SerializeField] private RawImage RenderImage;
		private BrowserPage Page;

		public void Run(BrowserPage page, CancellationToken cancellationToken)
		{
			Page = page;
			StartCoroutine(RenderLoop(cancellationToken));
		}

		public void Stop()
		{
			StopAllCoroutines();
		}

		private IEnumerator RenderLoop(CancellationToken cancellationToken)
		{
			var renderTexture = new Texture2D(2, 2);
			RenderImage.texture = renderTexture;

			byte[] screenshotData;

			while (!cancellationToken.IsCancellationRequested)
			{
				screenshotData = null;
				Page.GetScreenshotDataAsync(x => screenshotData = x);
				yield return new WaitUntil(() => screenshotData != null || cancellationToken.IsCancellationRequested);

				if (cancellationToken.IsCancellationRequested)
					yield break;

				renderTexture.LoadImage(screenshotData);

				var currentRenderSize = new Vector2(renderTexture.width, renderTexture.height) / 2;
				var targetRenderSize = Page.ViewportSize;
				if (currentRenderSize == targetRenderSize)
				{
					renderTexture.Apply();

					if (ViewportRect.sizeDelta != targetRenderSize)
						ViewportRect.sizeDelta = targetRenderSize;
				}

				yield return null;
			}
		}
	}
}