using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Demo;

namespace Xsolla.Demo.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/SuccessPopup")]
	public class SuccessPopup : MonoBehaviour, ISuccessPopup
	{
		[SerializeField] private Text title = default;
		[SerializeField] private Text message = default;
		[SerializeField] private SimpleTextButton button = default;
		[SerializeField] private Text buttonText = default;

		private void Awake()
		{
			if (button != null)
			{
				button.onClick = () => Destroy(gameObject, 0.001F);
			}
		}

		ISuccessPopup ISuccessPopup.SetButtonText(string text)
		{
			if (!string.IsNullOrEmpty(text))
				buttonText.text = text;
			return this;
		}

		ISuccessPopup ISuccessPopup.SetCallback(Action buttonPressed)
		{
			button.onClick = () =>
			{
				buttonPressed?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		ISuccessPopup ISuccessPopup.SetMessage(string messageText)
		{
			if (!string.IsNullOrEmpty(messageText))
				message.text = messageText;
			return this;
		}

		ISuccessPopup ISuccessPopup.SetTitle(string titleText)
		{
			if (!string.IsNullOrEmpty(titleText))
				title.text = titleText;
			return this;
		}
	}
}
