using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Demo;

namespace Xsolla.Demo.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/ConfirmationPopup")]
	public class ConfirmationPopup : MonoBehaviour, IConfirmationPopup
	{
		[SerializeField] private Text Title = default;
		[SerializeField] private Text Message = default;
		[SerializeField] private SimpleTextButton ConfirmButton = default;
		[SerializeField] private Text ConfirmButtonText = default;
		[SerializeField] private SimpleTextButton CancelButton = default;
		[SerializeField] private Text CancelButtonText = default;

		private void Awake()
		{
			if (ConfirmButton != null)
			{
				ConfirmButton.onClick = () => Destroy(gameObject, 0.001F);
			}

			if (CancelButton != null)
			{
				CancelButton.onClick = () => Destroy(gameObject, 0.001F);
			}
		}

		IConfirmationPopup IConfirmationPopup.SetMessage(string messageText)
		{
			Message.text = messageText;
			return this;
		}

		IConfirmationPopup IConfirmationPopup.SetTitle(string titleText)
		{
			Title.text = titleText;
			return this;
		}

		IConfirmationPopup IConfirmationPopup.SetConfirmCallback(Action buttonPressed)
		{
			ConfirmButton.onClick = () =>
			{
				buttonPressed?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		IConfirmationPopup IConfirmationPopup.SetConfirmButtonText(string buttonText)
		{
			ConfirmButtonText.text = buttonText;
			return this;
		}

		IConfirmationPopup IConfirmationPopup.SetCancelCallback(Action buttonPressed)
		{
			CancelButton.onClick = () =>
			{
				buttonPressed?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		IConfirmationPopup IConfirmationPopup.SetCancelButtonText(string buttonText)
		{
			CancelButtonText.text = buttonText;
			return this;
		}
	}
}
