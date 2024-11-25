using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Demo;

namespace Xsolla.Demo.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/ErrorPopup")]
	public class ErrorPopup : MonoBehaviour, IErrorPopup
	{
		[SerializeField] private Text Title = default;
		[SerializeField] private Text Message = default;
		[SerializeField] private SimpleTextButton Button = default;
		[SerializeField] private Text ButtonText = default;

		protected void Awake()
		{
			if (Button != null)
			{
				Button.onClick = () => Destroy(gameObject, 0.001F);
			}
		}

		IErrorPopup IErrorPopup.SetButtonText(string buttonText)
		{
			ButtonText.text = buttonText;
			return this;
		}

		IErrorPopup IErrorPopup.SetCallback(Action buttonPressed)
		{
			Button.onClick = () =>
			{
				buttonPressed?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		IErrorPopup IErrorPopup.SetMessage(string messageText)
		{
			Message.text = messageText;
			return this;
		}

		IErrorPopup IErrorPopup.SetTitle(string titleText)
		{
			Title.text = titleText;
			return this;
		}
	}
}
