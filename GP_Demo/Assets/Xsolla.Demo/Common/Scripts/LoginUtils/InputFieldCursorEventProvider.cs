using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(InputField))]
	public class InputFieldCursorEventProvider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public static event Action OnCursorEnter;
		public static event Action OnCursorExit;

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => OnCursorEnter?.Invoke();
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => OnCursorExit?.Invoke();
	}
}
