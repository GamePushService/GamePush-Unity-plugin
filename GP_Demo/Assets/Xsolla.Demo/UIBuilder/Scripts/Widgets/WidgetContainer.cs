using UnityEngine;

namespace Xsolla.UIBuilder
{
	[SelectionBase]
	[AddComponentMenu("Xsolla/UI Builder/Widget Container", 100)]
	public class WidgetContainer : MonoBehaviour
	{
		[SerializeField] private string _propertyId;

		[SerializeField] private Transform _container;

		[SerializeField] private Transform _current;

		public string PropertyId
		{
			get => _propertyId;
			set => _propertyId = value;
		}

		public Transform Container
		{
			get => _container;
			set => _container = value;
		}

		public Transform Current
		{
			get => _current;
			set => _current = value;
		}

		protected void Reset()
		{
			if (!_container)
			{
				_container = transform;
			}
		}
	}
}