using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(Image))]
	public class SelectableArea : MonoBehaviour,
		IPointerEnterHandler, IPointerExitHandler
	{
		public event Action OnPointerEnterEvent;
		public event Action OnPointerExitEvent;

		public void OnPointerEnter(PointerEventData eventData)
		{
			OnPointerEnterEvent?.Invoke();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			OnPointerExitEvent?.Invoke();
		}
	}
}
