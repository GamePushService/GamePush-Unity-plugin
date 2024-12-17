using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class WidgetProvider : UIPropertyProvider<WidgetProperty, GameObject>
	{
		protected override IEnumerable<WidgetProperty> Props => WidgetsLibrary.Widgets;
	}
}