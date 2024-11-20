#if (UNITY_EDITOR || UNITY_STANDALONE)
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Browser
{
	internal class Display2DBehaviour : MonoBehaviour
	{
		private IXsollaBrowserRender xsollaRender;
		public RawImage renderImage;

		public Vector2Int CurrentRenderSize { get; private set; }

		public event Action<int, int> ViewportChangedEvent;
		public event Action RedrawFrameCompleteEvent;
		public event Action FirstRedrawFrameCompleteEvent;

		private IEnumerator Start()
		{
			if (!GetComponentInParent<Canvas>())
			{
				XDebug.LogError("Canvas not found. This browser for 2D project.");
				Destroy(gameObject);
				yield break;
			}

			renderImage = gameObject.GetComponent<RawImage>();
			SetOpacity(0);

			yield return new WaitWhile(() => !GetComponent<XsollaBrowser>());
			var xsollaBrowser = GetComponent<XsollaBrowser>();

			yield return new WaitWhile(() => xsollaBrowser.Render == null);
			xsollaRender = xsollaBrowser.Render;
		}

		private void OnDestroy()
		{
			StopRedraw();
		}

		public void StartRedraw(int width, int height)
		{
			if (xsollaRender == null)
				return;

			StopRedraw();
			xsollaRender.SetResolution(width, height, OnRenderResolutionChanged);
		}

		private void StopRedraw()
		{
			if (xsollaRender != null)
				StopAllCoroutines();
		}

		private void SetOpacity(float opacity)
		{
			if (renderImage == null)
				return;

			var color = renderImage.color;
			color.a = opacity;
			renderImage.color = color;
		}

		private void OnRenderResolutionChanged(int width, int height)
		{
			CurrentRenderSize = new Vector2Int(width, height);
			ViewportChangedEvent?.Invoke(width, height);
			StartCoroutine(RedrawCoroutine());
		}

		private IEnumerator RedrawCoroutine()
		{
			var isFirstRedraw = true;
			while (enabled)
			{
				yield return ActionExtensions.WaitMethod<Texture2D>(
					xsollaRender.To,
					texture => {
						if (renderImage != null && texture != null)
						{
							SetOpacity(1.0f);
							renderImage.texture = texture;
						}

						RedrawFrameCompleteEvent?.Invoke();
						if (isFirstRedraw)
						{
							FirstRedrawFrameCompleteEvent?.Invoke();
							isFirstRedraw = false;
						}
					});
			}
		}
	}
}
#endif