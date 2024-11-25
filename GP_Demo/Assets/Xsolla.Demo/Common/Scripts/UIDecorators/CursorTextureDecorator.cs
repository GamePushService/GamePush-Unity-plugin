using UnityEngine;
using UnityEngine.EventSystems;

namespace Xsolla.Demo
{
	public class CursorTextureDecorator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private Texture2D HoverCursorTexture = default;

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			CursorChanger.SetCursorTexture(HoverCursorTexture);
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			CursorChanger.ChangeBackToDefault();
		}
	}
}