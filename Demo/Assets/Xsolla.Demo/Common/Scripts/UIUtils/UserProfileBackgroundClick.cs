using UnityEngine;
using UnityEngine.EventSystems;

namespace Xsolla.Demo
{
    public class UserProfileBackgroundClick : MonoBehaviour, IPointerClickHandler
	{
		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			UserProfileEntryUI.RaiseUserEntryEditStarted(null);
		}
	}
}
