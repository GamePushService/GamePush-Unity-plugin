using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(Toggle))]
	public class RememberMeToggleDecorator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private GameObject[] NormalObjects = default;
		[SerializeField] private GameObject[] HoverObjects = default;

		private bool _isSelected;

		private void Awake()
		{
			var toggle = GetComponent<Toggle>();
			toggle.onValueChanged.AddListener(OnToggleValueChanged);

			var selectedNow = toggle.isOn;
		
			SetObjectsState(selectedNow);
			_isSelected = selectedNow;
		}

		private void OnToggleValueChanged(bool newValue) => _isSelected = newValue;

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => SetObjectsState(isPointerEnter: true);
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => SetObjectsState(isPointerEnter: false);

		private void SetObjectsState(bool isPointerEnter)
		{
			if (_isSelected)
				return;

			foreach (var item in NormalObjects)
				item.SetActive(!isPointerEnter);

			foreach (var item in HoverObjects)
				item.SetActive(isPointerEnter);
		}
	}
}
