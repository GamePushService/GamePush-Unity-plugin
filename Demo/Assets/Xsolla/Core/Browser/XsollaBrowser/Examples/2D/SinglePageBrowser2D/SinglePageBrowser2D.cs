using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Browser
{
	internal class SinglePageBrowser2D : MonoBehaviour
	{
		[SerializeField] private Button CloseButton;
		[SerializeField] private Button FullscreenButton;
		[SerializeField] private Button BackButton;
		[SerializeField] private Vector2Int Viewport = new Vector2Int(1920, 1080);
		[SerializeField] private GameObject PreloaderPrefab;

#pragma warning disable CS0067
		public event Action<IXsollaBrowser> BrowserInitEvent;
		public event Action BrowserCloseRequest;
		public event Action ToggleFullscreenRequest;

		public event Action<string, Action> AlertDialogEvent;
		public event Action<string, Action, Action> ConfirmDialogEvent;
#pragma warning restore CS0067

#if UNITY_EDITOR || UNITY_STANDALONE
		private XsollaBrowser xsollaBrowser;
		private Display2DBehaviour display;
		private Keyboard2DBehaviour keyboard;
		private Mouse2DBehaviour mouse;
		private string urlBeforePopup;
		private Preloader2DBehaviour preloader;

		private void Awake()
		{
			CloseButton.onClick.AddListener(OnCloseButtonPressed);
			CloseButton.gameObject.SetActive(false);

			BackButton.onClick.AddListener(OnBackButtonPressed);
			BackButton.gameObject.SetActive(false);

			FullscreenButton.onClick.AddListener(OnFullscreenButtonPressed);
			FullscreenButton.gameObject.SetActive(false);

			xsollaBrowser = this.GetOrAddComponent<XsollaBrowser>();
			xsollaBrowser.LogEvent += s => XDebug.Log(s);

			xsollaBrowser.Launch
			(
				Viewport.x,
				Viewport.y,
				GetBrowserPlatform(),
				GetBrowserPath(),
				Constants.BROWSER_REVISION,
				Constants.CUSTOM_BROWSER_USER_AGENT
			);

			xsollaBrowser.Navigate.SetOnPopupListener(popupUrl =>
			{
				xsollaBrowser.Navigate.GetUrl(currentUrl =>
				{
					if (string.IsNullOrEmpty(urlBeforePopup))
					{
						urlBeforePopup = currentUrl;
					}
				});
				xsollaBrowser.Navigate.To(popupUrl, newUrl => { BackButton.gameObject.SetActive(true); });
			});

			xsollaBrowser.SetDialogHandler(HandleBrowserDialog);

			display = this.GetOrAddComponent<Display2DBehaviour>();
		}

		private IEnumerator Start()
		{
			yield return new WaitForEndOfFrame();

			preloader = gameObject.AddComponent<Preloader2DBehaviour>();
			preloader.SetPrefab(PreloaderPrefab);
			yield return new WaitWhile(() => xsollaBrowser.FetchingProgress < 100);

			display.StartRedraw(Viewport.x, Viewport.y);
			display.RedrawFrameCompleteEvent += DestroyPreloader;
			display.RedrawFrameCompleteEvent += EnableCloseButton;
			display.RedrawFrameCompleteEvent += EnableFullScreenButton;
			display.ViewportChangedEvent += (width, height) => XDebug.Log("Display viewport changed: " + width + "x" + height);

			mouse = this.GetOrAddComponent<Mouse2DBehaviour>();
			keyboard = this.GetOrAddComponent<Keyboard2DBehaviour>();
			keyboard.EscapePressed += OnKeyboardEscapePressed;
			BrowserInitEvent?.Invoke(xsollaBrowser);
		}
		
		private void OnDestroy()
		{
			StopAllCoroutines();

			if (mouse != null)
			{
				Destroy(mouse);
				mouse = null;
			}

			if (display != null)
			{
				Destroy(display);
				display = null;
			}

			if (keyboard != null)
			{
				keyboard.EscapePressed -= OnKeyboardEscapePressed;
				Destroy(keyboard);
				keyboard = null;
			}

			if (xsollaBrowser != null)
			{
				Destroy(xsollaBrowser);
				xsollaBrowser = null;
			}
		}

		public void SetViewport(Vector2Int viewport)
		{
			Viewport = viewport;
			if (display)
				display.StartRedraw(Viewport.x, Viewport.y);
		}

		public Vector2Int GetViewport()
		{
			return Viewport;
		}

		private string GetBrowserPlatform()
		{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#if UNITY_64
			return "Win64";
#else
			return "Win32";
#endif
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
			return "MacOS";
#endif

#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
			return "Linux";
#endif
		}

		private string GetBrowserPath()
		{
#if UNITY_EDITOR
			return Application.persistentDataPath;
#else
			if (XsollaSettings.PackInAppBrowserInBuild)
			{
				var path = Application.dataPath;
				path = System.IO.Directory.GetParent(path).FullName;
				return System.IO.Path.Combine(path, ".local-chromium");
			}
			else
			{
				return Application.persistentDataPath;
			}
#endif
		}

		private void DestroyPreloader()
		{
			display.RedrawFrameCompleteEvent -= DestroyPreloader;

			if (preloader != null)
			{
				Destroy(preloader, 0.001f);
				preloader = null;
			}
		}

		private void EnableCloseButton()
		{
			display.RedrawFrameCompleteEvent -= EnableCloseButton;
			CloseButton.gameObject.SetActive(true);
		}

		private void EnableFullScreenButton()
		{
			display.RedrawFrameCompleteEvent -= EnableFullScreenButton;
			FullscreenButton.gameObject.SetActive(true);
		}

		private void OnCloseButtonPressed()
		{
			XDebug.Log("`Close` button pressed");
			BrowserCloseRequest?.Invoke();
		}

		private void OnBackButtonPressed()
		{
			XDebug.Log("`Back` button pressed");
			xsollaBrowser.Navigate.Back(newUrl =>
			{
				if (newUrl.Equals(urlBeforePopup))
				{
					BackButton.gameObject.SetActive(false);
					urlBeforePopup = string.Empty;
				}
			});
		}

		private void OnFullscreenButtonPressed()
		{
			XDebug.Log("`Fullscreen` button pressed");
			ToggleFullscreenRequest?.Invoke();
		}

		private void OnKeyboardEscapePressed()
		{
			BrowserCloseRequest?.Invoke();
		}

		private void HandleBrowserDialog(XsollaBrowserDialog dialog)
		{
			switch (dialog.Type)
			{
				case XsollaBrowserDialogType.Alert:
					ShowSimpleAlertPopup(dialog);
					break;
				case XsollaBrowserDialogType.Prompt:
					CloseAlert(dialog);
					break;
				case XsollaBrowserDialogType.Confirm:
					ShowConfirmAlertPopup(dialog);
					break;
				case XsollaBrowserDialogType.BeforeUnload:
					CloseAlert(dialog);
					break;
				default:
					CloseAlert(dialog);
					break;
			}
		}

		private void ShowSimpleAlertPopup(XsollaBrowserDialog dialog)
		{
			AlertDialogEvent?.Invoke(dialog.Message, dialog.Accept);
		}

		private void ShowConfirmAlertPopup(XsollaBrowserDialog dialog)
		{
			ConfirmDialogEvent?.Invoke(dialog.Message, dialog.Accept, dialog.Dismiss);
		}

		private void CloseAlert(XsollaBrowserDialog dialog)
		{
			XDebug.Log("Browser alert was closed automatically");
			dialog.Accept();
		}
#endif
	}
}