using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class WidgetsLibrary : ScriptableObject
	{
		[HideInInspector]
		[SerializeField] private List<WidgetProperty> _widgets = new List<WidgetProperty>();

		public static List<WidgetProperty> Widgets
		{
			get => Instance._widgets;
			set => Instance._widgets = value;
		}

		public static WidgetProperty GetWidgetProperty(string id)
		{
			return Widgets.FirstOrDefault(x => x.Id == id);
		}

		#region Singleton

		private const string AssetPath = "UIBuilder/WidgetsLibrary";

		private static WidgetsLibrary _instance;

		public static WidgetsLibrary Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = Resources.Load<WidgetsLibrary>(AssetPath);
					Debug.Assert(_instance, $"Can't load instance of WidgetsLibrary by path: {AssetPath}");
				}

				return _instance;
			}
		}

		#endregion
	}
}