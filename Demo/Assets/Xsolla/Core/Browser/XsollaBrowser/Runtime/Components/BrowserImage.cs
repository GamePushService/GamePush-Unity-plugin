using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Xsolla.XsollaBrowser
{
	public class BrowserImage : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public event Action PointerClick;
		public event Action PointerEnter;
		public event Action PointerExit;

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			PointerClick?.Invoke();
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			PointerEnter?.Invoke();
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			PointerExit?.Invoke();
		}
	}
}