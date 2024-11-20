#if (UNITY_EDITOR || UNITY_STANDALONE)
using System.Collections;
using UnityEngine;

namespace Xsolla.Core.Browser
{
	internal class Preloader2DBehaviour : MonoBehaviour
	{
		private int lastProgress;
		private object progressLocker;
		private GameObject preloaderObject;
		private XsollaBrowser xsollaBrowser;
		private GameObject prefab;

		private void Awake()
		{
			progressLocker = new object();
			lastProgress = 0;

			xsollaBrowser = GetComponent<XsollaBrowser>();
			xsollaBrowser.FetchingBrowserEvent += OnBrowserFetchingEvent;

			StartCoroutine(PreloaderInstantiateCoroutine());
		}

		private void OnDestroy()
		{
			xsollaBrowser.FetchingBrowserEvent -= OnBrowserFetchingEvent;
			StopAllCoroutines();

			if (preloaderObject)
			{
				Destroy(preloaderObject);
				preloaderObject = null;
			}

			progressLocker = null;
		}

		public void SetPrefab(GameObject obj)
		{
			prefab = obj;
		}

		private void OnBrowserFetchingEvent(int progress)
		{
			lock (progressLocker)
			{
				if (lastProgress >= progress)
					return;

				XDebug.Log($"Update[%]: {lastProgress} => {progress}");
				lastProgress = progress;

				StartCoroutine(PreloaderCoroutine(progress));
			}
		}

		private IEnumerator PreloaderCoroutine(int progress)
		{
			yield return new WaitForEndOfFrame();

			if (preloaderObject == null)
				yield break;

			if (progress < 99)
				preloaderObject.GetComponent<PreloaderScript>().SetPercent((int) progress);
			else
				preloaderObject.GetComponent<PreloaderScript>().SetText(string.Empty);
		}

		private IEnumerator PreloaderInstantiateCoroutine()
		{
			yield return new WaitWhile(() => prefab == null);
			preloaderObject = Instantiate(prefab, transform);
		}
	}
}
#endif