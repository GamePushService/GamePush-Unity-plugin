#if (UNITY_EDITOR || UNITY_STANDALONE)
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Xsolla.Core.Browser
{
	internal class Mouse2DBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IScrollHandler
	{
		private Canvas canvas;
		private RectTransform selfRectTransform;

		private IXsollaBrowserKeyboardInput browserKeyboard;
		private IXsollaBrowserMouseInput browserMouse;
		private Display2DBehaviour displayBehaviour;

		private Coroutine mouseMovementCoroutine;

		private void Awake()
		{
			canvas = GetComponentInParent<Canvas>();
			selfRectTransform = transform as RectTransform;

			var xsollaBrowser = GetComponent<XsollaBrowser>();
			browserKeyboard = xsollaBrowser.Input.Keyboard;
			browserMouse = xsollaBrowser.Input.Mouse;

			displayBehaviour = GetComponent<Display2DBehaviour>();
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (mouseMovementCoroutine != null)
			{
				StopCoroutine(mouseMovementCoroutine);
			}

			mouseMovementCoroutine = StartCoroutine(MouseMovementCoroutine());
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			if (mouseMovementCoroutine != null)
			{
				StopCoroutine(mouseMovementCoroutine);
				mouseMovementCoroutine = null;
			}
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			var mousePosition = CalculateBrowserMousePosition(eventData.position);
			browserMouse.Click(mousePosition, pos => XDebug.Log("Click handled by: " + pos));
		}

		void IScrollHandler.OnScroll(PointerEventData eventData)
		{
			var key = eventData.scrollDelta.y > 0
				? KeysConverter.Convert(KeyCode.UpArrow)
				: KeysConverter.Convert(KeyCode.DownArrow);

			browserKeyboard.KeyDown(key);
		}

		private IEnumerator MouseMovementCoroutine()
		{
			var lastPosition = Vector2.zero;
			while (enabled)
			{
				yield return Display2DInitializationCoroutine();
				yield return null;

				var mousePosition = CalculateBrowserMousePosition(InputProxy.MousePosition);
				yield return MouseMovementCoroutine(lastPosition, mousePosition, pos => lastPosition = pos);
			}
		}

		private IEnumerator Display2DInitializationCoroutine()
		{
			yield return new WaitWhile(() => displayBehaviour.CurrentRenderSize.x == 0 || displayBehaviour.CurrentRenderSize.y == 0);
		}

		private IEnumerator MouseMovementCoroutine(Vector2 lastPosition, Vector2 mousePosition, Action<Vector2> callback = null)
		{
			if ((mousePosition - lastPosition).sqrMagnitude > 1.0f)
			{
				yield return ActionExtensions.WaitMethod(
					browserMouse.MoveTo,
					mousePosition,
					pos => callback?.Invoke(pos)
				);
			}
			else
			{
				yield return null;
			}
		}

		private Vector2 CalculateBrowserMousePosition(Vector3 inputMousePosition)
		{
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(selfRectTransform, inputMousePosition, canvas.worldCamera, out var point))
			{
				point += selfRectTransform.sizeDelta * 0.5f; // Transform's anchors in the center of the rect
				point.y = displayBehaviour.CurrentRenderSize.y - point.y; // Browser's axis are differs from Unity's axis
				return point;
			}

			XDebug.LogWarning("You try get mouse position, but mouse not over canvas");
			return Vector2.zero;
		}
	}
}
#endif