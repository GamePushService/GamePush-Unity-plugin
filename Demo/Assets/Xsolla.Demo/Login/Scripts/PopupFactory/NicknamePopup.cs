using System;
using UnityEngine;
using Xsolla.Demo;

namespace Xsolla.Demo.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/NicknamePopup")]
	public class NicknamePopup : MonoBehaviour, INicknamePopup
	{
		[SerializeField] SimpleButton ContinueButton = default;
		[SerializeField] SimpleButton CancelButton = default;
		[SerializeField] UserProfileEntryEditor Editor = default;

		private string UserInput { get; set; }

		private void Awake()
		{
			Editor.UserProfileEntryEdited += newValue => UserInput = newValue;
		}

		public INicknamePopup SetCallback(Action<string> nicknameCallback)
		{
			ContinueButton.onClick = () =>
			{
				nicknameCallback?.Invoke(UserInput);
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		public INicknamePopup SetCancelCallback(Action cancelCallback)
		{
			CancelButton.onClick = () =>
			{
				cancelCallback?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}
	}
}
