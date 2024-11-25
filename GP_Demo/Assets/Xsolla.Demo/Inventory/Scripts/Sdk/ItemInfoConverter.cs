using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public static class ItemInfoConverter
    {
		public static KeyValuePair<string, string>[] ConvertAttributes(StoreItemAttribute[] attributes)
		{
			if (attributes != null && attributes.Length > 0)
			{
				var result = new List<KeyValuePair<string, string>>(capacity: attributes.Length);

				foreach (var attribute in attributes)
				{
					var values = attribute.values;

					foreach (var value in values)
						result.Add(new KeyValuePair<string, string>(attribute.name, value.value));
				}

				return result.ToArray();
			}

			//else
			return (new KeyValuePair<string, string>[0]);
		}

		public static List<string> ConvertGroups(StoreItemGroup[] groups)
		{
			if (groups == null || groups.Length == 0)
				return new List<string>();

			var result = new List<string>(groups.Length);
			foreach (var group in groups)
				result.Add(group.name);

			return result;
		}
	}
}
