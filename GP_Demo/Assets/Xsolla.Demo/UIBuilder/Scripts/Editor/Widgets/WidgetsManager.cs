using System;

namespace Xsolla.UIBuilder
{
	public static class WidgetsManager
	{
		public static void CreateWidget()
		{
			var prop = new WidgetProperty
			{
				Id = Guid.NewGuid().ToString(),
				Name = "New Widget"
			};

			WidgetsLibrary.Widgets.Add(prop);
		}

		public static void DeleteWidget(string id)
		{
			var prop = WidgetsLibrary.GetWidgetProperty(id);
			if (prop != null)
			{
				WidgetsLibrary.Widgets.Remove(prop);
			}
		}

		public static void ChangeWidgetName(string id, string name)
		{
			if (string.IsNullOrEmpty(id))
				throw new InvalidOperationException(nameof(id));

			if (string.IsNullOrEmpty(name))
				throw new InvalidOperationException(nameof(name));

			var prop = WidgetsLibrary.GetWidgetProperty(id);
			if (prop != null)
			{
				prop.Name = name;
			}
		}

		public static void ChangeWidgetsOrder(int oldIndex, int newIndex)
		{
			var list = WidgetsLibrary.Widgets;

			if (oldIndex < 0 || oldIndex >= list.Count)
				throw new ArgumentOutOfRangeException($"{nameof(oldIndex)}: {oldIndex}");

			if (newIndex < 0 || newIndex >= list.Count)
				throw new ArgumentOutOfRangeException($"{nameof(newIndex)}: {newIndex}");

			var item = list[oldIndex];
			list.RemoveAt(oldIndex);
			list.Insert(newIndex, item);
		}
	}
}