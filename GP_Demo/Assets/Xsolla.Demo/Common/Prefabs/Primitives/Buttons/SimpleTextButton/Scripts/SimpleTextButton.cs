using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class SimpleTextButton : SimpleButtonLockDecorator
	{
		[SerializeField] Text buttonText = default;

		public string Text
		{
			get => buttonText.text;
			set => buttonText.text = value;
		}
	}
}
