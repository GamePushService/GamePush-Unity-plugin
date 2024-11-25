using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Xsolla.Core
{
	public static class ImageLoader
	{
		private static readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

		public static void LoadSprite(string url, Action<Sprite> callback)
		{
			if (string.IsNullOrEmpty(url))
				throw new ArgumentNullException(nameof(url));

			if (Sprites.ContainsKey(url) && Sprites[url] != null)
			{
				callback?.Invoke(Sprites[url]);
				return;
			}

			if (!Sprites.ContainsKey(url))
			{
				Sprites[url] = null;

				WebRequestHelper.Instance.ImageRequest(
					url,
					sprite => Sprites[url] = sprite,
					error => XDebug.LogError(error.errorMessage));
			}

			CoroutinesExecutor.Run(WaitSprite(url, callback));
		}

		private static IEnumerator WaitSprite(string url, Action<Sprite> callback)
		{
			yield return new WaitUntil(() => Sprites[url] != null);
			callback?.Invoke(Sprites[url]);
		}
	}
}