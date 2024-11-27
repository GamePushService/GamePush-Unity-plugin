using System.Collections;
using System.Threading;
using UnityEngine;

namespace Xsolla.XsollaBrowser
{
	public class NavigationHandler : MonoBehaviour, IBrowserHandler
	{
		private BrowserPage Page;

		public void Run(BrowserPage page, CancellationToken cancellationToken)
		{
			Page = page;
			StartCoroutine(TrackUrlChangeLoop(cancellationToken));
		}

		public void Stop()
		{
			StopAllCoroutines();
		}

		private IEnumerator TrackUrlChangeLoop(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				string url = null;
				Page.GetUrlAsync(x => url = x);
				yield return new WaitUntil(() => url != null);

				if (cancellationToken.IsCancellationRequested)
					yield break;

				if (!string.Equals(Page.Url, url))
					Page.Url = url;

				yield return new WaitForSeconds(0.5f);
			}
		}
	}
}