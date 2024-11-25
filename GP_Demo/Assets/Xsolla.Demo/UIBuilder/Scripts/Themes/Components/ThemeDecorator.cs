using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public abstract class ThemeDecorator<TComp> : MonoBehaviour where TComp : Component
	{
		[SerializeField] private string _propertyId;

		[SerializeField] private TComp _comp;

		[SerializeField] private DecoratorPointerOverrider _pointerOverrider;

		public string PropertyId
		{
			get => _propertyId;
			set => _propertyId = value;
		}

		public TComp Comp
		{
			get => _comp;
			set => _comp = value;
		}

		public DecoratorPointerOverrider PointerOverrider
		{
			get => _pointerOverrider;
			set => _pointerOverrider = value;
		}

		protected abstract IEnumerable<IUIItem> Props { get; }

		protected void Awake()
		{
			PointerOverrider.ApplyAction = ValidateAndApplyProperty;
			PointerOverrider.ResetAction = theme => ValidateAndApplyProperty(theme, PropertyId);
		}

		protected void OnEnable()
		{
			PointerOverrider.OnEnable();
			ThemesLibrary.CurrentChanged += ApplyTheme;

			ApplyTheme(ThemesLibrary.Current);
		}

		protected void OnDisable()
		{
			PointerOverrider.OnDisable();
			ThemesLibrary.CurrentChanged -= ApplyTheme;
		}

		protected void Reset()
		{
			if (!_comp)
			{
				_comp = GetComponent<TComp>();
			}

			if (PointerOverrider == null)
			{
				PointerOverrider = new DecoratorPointerOverrider();
			}
		}

		protected void OnValidate()
		{
			ApplyTheme(ThemesLibrary.Current);
		}

		protected abstract void ApplyProperty(Theme theme, string id);

		private void ApplyTheme(Theme theme)
		{
			if (Comp && theme != null)
			{
				ValidatePropertyId(Props);
				ApplyProperty(theme, PropertyId);
				PointerOverrider.ValidatePropertyId(Props);
			}
		}

		private void ValidateAndApplyProperty(Theme theme, string propertyId)
		{
			if (!Comp)
			{
				Debug.LogWarning("Decorator comp is missing", this);
				return;
			}

			if (theme == null)
			{
				Debug.LogWarning("Theme is null", this);
				return;
			}

			ApplyProperty(theme, propertyId);
		}

		private void ValidatePropertyId<T>(IEnumerable<T> properties) where T : IUIItem
		{
			if (string.IsNullOrEmpty(PropertyId))
			{
				var prop = properties.FirstOrDefault();
				PropertyId = prop != null ? prop.Id : string.Empty;
			}
		}
	}
}