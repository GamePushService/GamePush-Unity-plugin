using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class DecoratorPointerOverrider
	{
		[SerializeField] private DecoratorPointerEvents _pointerEvents;

		[SerializeField] private TransitionMode _transitionMode;

		[SerializeField] private bool _isOverrideHover;

		[SerializeField] private string _hoverPropertyId;

		[SerializeField] private bool _isOverridePress;

		[SerializeField] private string _pressPropertyId;

		public DecoratorPointerEvents PointerEvents
		{
			get => _pointerEvents;
			set => _pointerEvents = value;
		}

		public bool IsOverrideHover
		{
			get => _isOverrideHover && _transitionMode == TransitionMode.Override;
			set => _isOverrideHover = value;
		}

		public string HoverPropertyId
		{
			get => _hoverPropertyId;
			set => _hoverPropertyId = value;
		}

		public bool IsOverridePress
		{
			get => _isOverridePress && _transitionMode == TransitionMode.Override;
			set => _isOverridePress = value;
		}

		public string PressPropertyId
		{
			get => _pressPropertyId;
			set => _pressPropertyId = value;
		}

		public bool IsHover { get; set; }

		public bool IsPress { get; set; }

		public Action<Theme, string> ApplyAction { get; set; }

		public Action<Theme> ResetAction { get; set; }

		public void OnEnable()
		{
			if (PointerEvents)
			{
				PointerEvents.PointerEnter += OnPointerEnter;
				PointerEvents.PointerExit += OnPointerExit;
				PointerEvents.PointerDown += OnPointerDown;
				PointerEvents.PointerUp += OnPointerUp;
			}
		}

		public void OnDisable()
		{
			if (PointerEvents)
			{
				PointerEvents.PointerEnter -= OnPointerEnter;
				PointerEvents.PointerExit -= OnPointerExit;
				PointerEvents.PointerDown -= OnPointerDown;
				PointerEvents.PointerUp -= OnPointerUp;
			}
		}

		public void OnPointerEnter()
		{
			IsHover = true;

			if (IsOverrideHover)
			{
				ApplyAction?.Invoke(ThemesLibrary.Current, HoverPropertyId);
			}
		}

		public void OnPointerExit()
		{
			IsHover = false;

			if (!IsPress && (IsOverridePress || IsOverrideHover))
			{
				ResetAction?.Invoke(ThemesLibrary.Current);
			}
		}

		public void OnPointerDown()
		{
			IsPress = true;

			if (IsOverridePress)
			{
				ApplyAction?.Invoke(ThemesLibrary.Current, PressPropertyId);
			}
		}

		public void OnPointerUp()
		{
			IsPress = false;

			if (IsOverridePress || IsOverrideHover)
			{
				if (IsOverrideHover && IsHover)
				{
					ApplyAction?.Invoke(ThemesLibrary.Current, HoverPropertyId);
				}
				else
				{
					ResetAction?.Invoke(ThemesLibrary.Current);
				}
			}
		}

		public void ValidatePropertyId<T>(IEnumerable<T> properties) where T : IUIItem
		{
			ValidatePropertyId(properties, ref _hoverPropertyId);
			ValidatePropertyId(properties, ref _pressPropertyId);
		}

		private void ValidatePropertyId<T>(IEnumerable<T> properties, ref string id) where T : IUIItem
		{
			if (string.IsNullOrEmpty(id))
			{
				var property = properties.FirstOrDefault();
				id = property?.Id ?? string.Empty;
				id = property != null ? property.Id : string.Empty;
			}
		}
	}
}