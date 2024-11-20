using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Demo;

namespace Xsolla.Demo.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/RedeemCouponPopup")]
	public class CouponRedeemPopup : MonoBehaviour, ICouponRedeemPopup
	{
		[SerializeField] private InputField CodeInputField = default;
		[SerializeField] private Text ErrorMessage = default;
		[SerializeField] private SimpleTextButtonDisableable RedeemButton = default;
		[SerializeField] private SimpleTextButton CancelButton = default;

		private void Awake()
		{
			RedeemButton.onClick = () => Destroy(gameObject, 0.001F);
			CancelButton.onClick = () => Destroy(gameObject, 0.001F);
		}

		public ICouponRedeemPopup SetRedeemCallback(Action<string> buttonPressed)
		{
			RedeemButton.onClick = () =>
			{
				ErrorMessage.gameObject.SetActive(false);
				buttonPressed?.Invoke(CodeInputField?.text);
			};
			return this;
		}

		public ICouponRedeemPopup SetCancelCallback(Action buttonPressed)
		{
			CancelButton.onClick = () =>
			{
				buttonPressed?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		public ICouponRedeemPopup ShowError()
		{
			ErrorMessage.gameObject.SetActive(true);
			return this;
		}

		public void Close()
		{
			Destroy(gameObject, 0.001F);
		}
	}
}
