using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class CartMenuButton : SimpleTextButton
	{
		[SerializeField] Text counterText = default;

		public string CounterText
		{
			get => counterText.text;
			set => counterText.text = value;
		}
	}
}
