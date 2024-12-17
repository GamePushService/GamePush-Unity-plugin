using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class CartGroupUI : MonoBehaviour, IGroup
	{
		private const string CART_GROUP_NAME = "CART";

		[SerializeField]
		CartMenuButton menuButton = default;

		int _counter;
	
		void Awake()
		{
			UserCart.Instance.AddItemEvent += item => IncreaseCounter();
			UserCart.Instance.RemoveItemEvent += item => DecreaseCounter();
			UserCart.Instance.UpdateItemEvent += (item, deltaValue) => ChangeCounter(deltaValue);
			UserCart.Instance.ClearCartEvent += ResetCounter;

			menuButton.onClick = () => OnGroupClick?.Invoke(CART_GROUP_NAME);
		}

		public string Id { get; set; }

		public string Name
		{
			get { return menuButton.Text; }
			set { menuButton.Text = value; }
		}

		public Action<string> OnGroupClick { get; set; }

		public void Select(bool _)
		{
			//menuButton.Select();
		}

		public void Deselect()
		{
			//menuButton.Deselect();
		}
	
		public bool IsSelected()
		{
			return false;
			//return menuButton.IsSelected;
		}

		private void SetCounter(int newValue)
		{
			_counter = newValue;
			menuButton.CounterText = _counter.ToString();
		}

		private void IncreaseCounter(int value = 1)
		{
			SetCounter(_counter + 1);
		}

		private void DecreaseCounter(int value = 1)
		{
			SetCounter(_counter - 1);
		}

		private void ResetCounter()
		{
			SetCounter(0);
		}

		private void ChangeCounter(int deltaValue)
		{
			SetCounter(_counter + deltaValue);
		}
	}
}
