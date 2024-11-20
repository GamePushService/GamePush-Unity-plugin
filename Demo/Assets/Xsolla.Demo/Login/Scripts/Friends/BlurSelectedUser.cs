using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BlurSelectedUser : MonoBehaviour
	{
		public static event Action OnActivated;
		public static event Action OnDeactivated;

		[SerializeField] private GameObject blurMaskObject = default;
		[SerializeField] private ItemSelection blurTrigger = default;

		private void Start()
		{
			if (blurTrigger == null || blurMaskObject == null) return;
			blurTrigger.OnPointerEnterEvent += () =>
			{
				blurMaskObject.SetActive(true);
				OnActivated?.Invoke();
			};
			blurTrigger.OnPointerExitEvent += () =>
			{
				blurMaskObject.SetActive(false);
				OnDeactivated?.Invoke();
			};
		}
	}
}
