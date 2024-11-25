using UnityEngine.EventSystems;

namespace Xsolla.Demo
{
	public interface ISimpleButton :
		IPointerDownHandler,
		IPointerEnterHandler,
		IPointerExitHandler,
		IPointerUpHandler,
		IDragHandler
	{	}
}