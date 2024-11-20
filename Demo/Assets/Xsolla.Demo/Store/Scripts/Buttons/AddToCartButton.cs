using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Xsolla.Demo
{
	public partial class AddToCartButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IDragHandler
	{
		bool _isClickInProgress;
	
		bool _isSelected;

		public Action<bool> onClick;

		public void Select(bool bSelect)
		{
			_isSelected = bSelect;
		}
	
		public void OnDrag(PointerEventData eventData)
		{
		}
	
		public void OnPointerEnter(PointerEventData eventData)
		{
			OnCursorEnter?.Invoke();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			OnCursorExit?.Invoke();
			_isClickInProgress = false;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_isClickInProgress = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (_isClickInProgress)
			{
				_isSelected = !_isSelected;
				
				onClick?.Invoke(_isSelected);
			}
		
			_isClickInProgress = false;
		}	
		
	}
}
