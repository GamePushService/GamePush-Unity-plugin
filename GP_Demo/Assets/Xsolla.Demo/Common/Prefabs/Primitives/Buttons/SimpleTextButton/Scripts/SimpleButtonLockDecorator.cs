using UnityEngine.EventSystems;

namespace Xsolla.Demo
{
	public class SimpleButtonLockDecorator : SimpleButton
	{
		bool _isLocked = false;

		public void Lock()
		{
			_isLocked = true;
		}

		public void Unlock()
		{
			_isLocked = false;
		}

		public bool IsLocked() => _isLocked;

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!IsLocked())
			{
				base.OnPointerDown(eventData);
			}
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			if (!IsLocked())
			{
				base.OnPointerUp(eventData);
			}
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (!IsLocked())
				base.OnPointerEnter(eventData);
			else
				base.RaiseOnCursorEnter();
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			if (!IsLocked())
				base.OnPointerExit(eventData);
			else
				base.RaiseOnCursorExit();
		}

		public override void OnDrag(PointerEventData eventData)
		{
			if (!IsLocked())
			{
				base.OnDrag(eventData);
			}
		}
	}
}
