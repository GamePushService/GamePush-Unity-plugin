using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class ValueCounterControl : MonoBehaviour
	{
		public SimpleTextButton increaseButton;
		public SimpleTextButton decreaseButton;
		public Text counterValueText;
		public float timeout = StoreConstants.DEFAULT_BUTTON_RATE_LIMIT_MS;

		private ValueCounter _counter;

		void Start()
		{
			_counter = gameObject.GetComponent<ValueCounter>();
			_counter.ValueChanged += Counter_ValueChanged;

			counterValueText.text = _counter.ToString();

			increaseButton.onClick = () => _counter++;
			decreaseButton.onClick = () => _counter--;

			increaseButton.RateLimitMs = timeout;
			decreaseButton.RateLimitMs = timeout;
		}

		private void Counter_ValueChanged(int value)
		{
			counterValueText.text = value.ToString();
		}
	}
}
