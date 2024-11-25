using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class CartMenuButtonCounter : MonoBehaviour
	{
		[SerializeField] private CartMenuButton button = default;
		[SerializeField] private Text counterText = default;

		private int Counter
		{
			get => int.Parse(counterText.text);
			set => counterText.text = value.ToString();
		}

		private void Start()
		{
			Counter = UserCart.Instance.GetItems().Sum(i => i.Quantity);
			UserCart.Instance.AddItemEvent += IncreaseCounter;
			UserCart.Instance.RemoveItemEvent += DecreaseCounter;
			UserCart.Instance.UpdateItemEvent += UpdateCounter;
			UserCart.Instance.ClearCartEvent += ResetCounter;
			button.onClick = () => DemoController.Instance.SetState(MenuState.Cart);
		}

		private void OnDestroy()
		{
			if (!UserCart.IsExist) return;
			UserCart.Instance.AddItemEvent -= IncreaseCounter;
			UserCart.Instance.RemoveItemEvent -= DecreaseCounter;
			UserCart.Instance.UpdateItemEvent -= UpdateCounter;
			UserCart.Instance.ClearCartEvent -= ResetCounter;
		}

		private void IncreaseCounter(UserCartItem item)
		{
			Counter++;
		}

		private void DecreaseCounter(UserCartItem item)
		{
			Counter--;
		}

		private void UpdateCounter(UserCartItem item, int delta)
		{
			Counter += delta;
		}

		private void ResetCounter()
		{
			Counter = 0;
		}
	}
}
