using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper
	{
		public void ImageRequest(string url, Action<Sprite> onComplete, Action<Error> onError)
		{
			StartCoroutine(PerformImage(url, onComplete, onError));
		}

		private IEnumerator PerformImage(string url, Action<Sprite> onComplete, Action<Error> onError)
		{
			yield return PerformImage(url, texture =>
			{
				var spriteRect = new Rect(0, 0, texture.width, texture.height);
				var pivot = new Vector2(0.5f, 0.5f);
				var sprite = Sprite.Create(texture, spriteRect, pivot);
				onComplete?.Invoke(sprite);
			}, onError);
		}

		private IEnumerator PerformImage(string url, Action<Texture2D> onComplete, Action<Error> onError)
		{
			var webRequest = UnityWebRequestTexture.GetTexture(url);
			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError));
		}
	}
}