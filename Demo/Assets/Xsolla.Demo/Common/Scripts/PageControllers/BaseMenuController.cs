using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Demo.Popup;

namespace Xsolla.Demo
{
	public abstract class BaseMenuController : MonoBehaviour
	{
		public static void AttachButtonCallback(SimpleButton button, Action callback)
		{
			if (button != null && callback != null)
			{
				button.onClick = callback;
				button.transform.parent.gameObject.SetActive(true);
			}
		}

		public static void AttachUrlToButton(SimpleButton button, string url)
		{
			if (!string.IsNullOrEmpty(url))
			{
				AttachButtonCallback(button, () => XsollaWebBrowser.Open(url, true));
			}
		}
	
		public static void SetMenuState(MenuState state, Func<bool> condition = null)
		{
			if (condition == null || condition.Invoke())
				DemoController.Instance.SetState(state);
			else
			{
				PopupFactory.Instance.CreateWaiting()
					.SetCloseCondition(condition)
					.SetCloseHandler(() => DemoController.Instance.SetState(state));
			}
		}
	}
}
