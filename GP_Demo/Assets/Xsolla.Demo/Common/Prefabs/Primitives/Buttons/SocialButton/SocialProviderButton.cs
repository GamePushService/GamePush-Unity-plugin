using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SocialProviderButton : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] private SocialProvider _socialProvider = default;

		public SocialProvider SocialProvider => _socialProvider;

		public Action OnClick { get; set; }

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			OnClick?.Invoke();
		}
	}
}