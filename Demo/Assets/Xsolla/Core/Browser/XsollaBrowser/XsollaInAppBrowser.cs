using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Browser
{
#if !(UNITY_EDITOR || UNITY_STANDALONE)
	internal class XsollaInAppBrowser : MonoBehaviour { }
#else
	internal class XsollaInAppBrowser : MonoBehaviour, IInAppBrowser
	{
		[SerializeField] private GameObject BrowserPrefab;
		[SerializeField] private bool IsDontDestroyOnLoad;
		private GameObject BrowserObject;
		private SinglePageBrowser2D SinglePageBrowser;
		private XsollaBrowser XsollaBrowser;

		private CanvasScaler CanvasScaler;
		private Vector2 ScalerOriginalReferenceSize;
		private Vector2Int FramedModeDisplaySize;
		private Vector2Int FullscreenModeDisplaySize;
		private Display2DBehaviour Display2DBehaviour;

		public event Action OpenEvent;
		public event Action<BrowserCloseInfo> CloseEvent;
		public event Action<string> UrlChangeEvent;

		public event Action<string, Action> AlertDialogEvent;
		public event Action<string, Action, Action> ConfirmDialogEvent;

		public bool IsOpened => BrowserObject;

		public bool IsFullScreen { get; private set; }

		public void Close(float delay = 0f, bool isManually = false)
		{
			if (SinglePageBrowser)
			{
				SinglePageBrowser.BrowserCloseRequest -= OnBrowserCloseRequest;
				SinglePageBrowser.AlertDialogEvent -= OnAlertDialogEvent;
				SinglePageBrowser.ConfirmDialogEvent -= OnConfirmDialogEvent;
				SinglePageBrowser.ToggleFullscreenRequest -= OnToggleFullscreenRequest;
			}

			if (XsollaBrowser && XsollaBrowser.Navigate != null)
				XsollaBrowser.Navigate.UrlChangedEvent -= OnUrlChanged;

			if (BrowserObject)
				Destroy(BrowserObject, delay);

			var info = new BrowserCloseInfo {
				isManually = isManually
			};
			CloseEvent?.Invoke(info);

			OpenEvent = null;
			CloseEvent = null;
			UrlChangeEvent = null;
			AlertDialogEvent = null;
			ConfirmDialogEvent = null;

			IsFullScreen = false;
		}

		public void Open(string url)
		{
			if (!IsOpened)
				CreateBrowser();

			XsollaBrowser.Navigate.To(url);
			OpenEvent?.Invoke();
		}

		public void AddCloseHandler(Action action)
		{
			if (IsOpened)
				CloseEvent += _ => action?.Invoke();
		}

		public void AddUrlChangeHandler(Action<string> callback)
		{
			if (IsOpened)
				XsollaBrowser.Navigate.UrlChangedEvent += (browser, url) => callback?.Invoke(url);
		}

		public void AddInitHandler(Action callback)
		{
			if (IsOpened)
				SinglePageBrowser.BrowserInitEvent += browser => callback?.Invoke();
		}

		public void UpdateSize(int width, int height)
		{
			if (!IsFullScreen)
			{
				FramedModeDisplaySize = new Vector2Int(width, height);
			}

			if (IsOpened)
				SinglePageBrowser.SetViewport(new Vector2Int(width, height));
		}

		public void SetFullscreenMode(bool isFullscreen)
		{
			IsFullScreen = isFullscreen;

			if (isFullscreen)
			{
				FramedModeDisplaySize = SinglePageBrowser.GetViewport();
				ScalerOriginalReferenceSize = CanvasScaler.referenceResolution;

				var displaySize = new Vector2Int(Screen.width, Screen.height);
				if (displaySize.x > 1920 || displaySize.y > 1080)
				{
					var ratio = (float) displaySize.x / displaySize.y;
					var isWidthRule = displaySize.x > displaySize.y;
					if (isWidthRule)
					{
						displaySize.x = 1920;
						displaySize.y = (int) (displaySize.x / ratio);
					}
					else
					{
						displaySize.y = 1080;
						displaySize.x = (int) (displaySize.y * ratio);
					}
				}

				FullscreenModeDisplaySize = displaySize;
				UpdateSize(displaySize.x, displaySize.y);
			}
			else
			{
				UpdateSize(FramedModeDisplaySize.x, FramedModeDisplaySize.y);
			}
		}

		private void CreateBrowser()
		{
			BrowserObject = Instantiate(BrowserPrefab);
			BrowserObject.name = "XsollaInAppBrowser";
			if (IsDontDestroyOnLoad)
				DontDestroyOnLoad(BrowserObject);

			SinglePageBrowser = BrowserObject.GetComponentInChildren<SinglePageBrowser2D>();
			SinglePageBrowser.BrowserCloseRequest += OnBrowserCloseRequest;
			SinglePageBrowser.AlertDialogEvent += OnAlertDialogEvent;
			SinglePageBrowser.ConfirmDialogEvent += OnConfirmDialogEvent;
			SinglePageBrowser.ToggleFullscreenRequest += OnToggleFullscreenRequest;

			XsollaBrowser = BrowserObject.GetComponentInChildren<XsollaBrowser>();
			XsollaBrowser.Navigate.UrlChangedEvent += OnUrlChanged;

			Display2DBehaviour = BrowserObject.GetComponentInChildren<Display2DBehaviour>();
			Display2DBehaviour.FirstRedrawFrameCompleteEvent += () => {
				if (IsFullScreen)
				{
					CanvasScaler.referenceResolution = FullscreenModeDisplaySize;
					Display2DBehaviour.renderImage.rectTransform.sizeDelta = FullscreenModeDisplaySize;
				}
				else
				{
					CanvasScaler.referenceResolution = ScalerOriginalReferenceSize;
					Display2DBehaviour.renderImage.rectTransform.sizeDelta = FramedModeDisplaySize;
				}
			};

			CanvasScaler = BrowserObject.GetComponent<CanvasScaler>();
			FramedModeDisplaySize = SinglePageBrowser.GetViewport();
			ScalerOriginalReferenceSize = CanvasScaler.referenceResolution;
		}

		private void OnToggleFullscreenRequest()
		{
			SetFullscreenMode(!IsFullScreen);
		}

		private void OnBrowserCloseRequest()
		{
			if (IsFullScreen)
				SetFullscreenMode(false);
			else
				Close(0, true);
		}

		private void OnUrlChanged(IXsollaBrowser browser, string url)
		{
			UrlChangeEvent?.Invoke(url);
		}

		private void OnAlertDialogEvent(string message, Action acceptAction)
		{
			AlertDialogEvent?.Invoke(message, acceptAction);
		}

		private void OnConfirmDialogEvent(string message, Action acceptAction, Action cancelAction)
		{
			ConfirmDialogEvent?.Invoke(message, acceptAction, cancelAction);
		}
	}
#endif
}