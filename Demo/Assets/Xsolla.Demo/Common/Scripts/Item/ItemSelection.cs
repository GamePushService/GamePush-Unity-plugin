using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Xsolla.Demo
{
	public class ItemSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public event Action OnPointerEnterEvent;
		public event Action OnPointerExitEvent;

		[SerializeField] private GameObject itemSelectionImage = default;
		[SerializeField] private List<SelectableArea> someArea = default;
		private int _counter;
		private static ItemSelection _selectedItem;

		void Start()
		{
			if (itemSelectionImage != null)
				itemSelectionImage.SetActive(false);
			if (someArea != null && someArea.Any())
			{
				someArea.ForEach(s => s.OnPointerEnterEvent += EnableSelection);
				someArea.ForEach(s => s.OnPointerExitEvent += DisableSelection);
			}
		}

		private void DisableSelectedItem()
		{
			while (_selectedItem != null && !_selectedItem.Equals(this))
			{
				_selectedItem.OnPointerExit(null);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			DisableSelectedItem();
			EnableSelection();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			DisableSelection();
		}

		private void EnableSelection()
		{
			_counter++;
			if (_counter <= 0) _counter = 1;
			if (itemSelectionImage == null || _counter > 1) return;
			_selectedItem = this;
			itemSelectionImage.SetActive(true);
			OnPointerEnterEvent?.Invoke();
		}

		private void DisableSelection()
		{
			_counter--;
			if (itemSelectionImage != null && _counter <= 0)
			{
				_selectedItem = null;
				OnPointerExitEvent?.Invoke();
				itemSelectionImage.SetActive(false);
			}
		}
	}
}
