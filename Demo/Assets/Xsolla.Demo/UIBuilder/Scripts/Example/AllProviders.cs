using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.UIBuilder
{
	public class AllProviders : MonoBehaviour
	{
		[SerializeField] private Image Image;

		[SerializeField] private ColorProvider ColorProvider;

		[SerializeField] private SpriteProvider SpriteProvider;

		[Space]
		[SerializeField] private Text Text;

		[SerializeField] private FontProvider FontProvider;

		[Space]
		[SerializeField] private Transform WidgetContainer;

		[SerializeField] private WidgetProvider WidgetProvider;

		private void Start()
		{
			ThemesLibrary.CurrentChanged += Redraw;
			Redraw(ThemesLibrary.Current);
			Instantiate(WidgetProvider.GetValue(), WidgetContainer, false);
		}

		private void Redraw(Theme theme)
		{
			Image.color = ColorProvider.GetValue();
			Image.sprite = SpriteProvider.GetValue();

			Text.color = ColorProvider.GetValue();
			Text.font = FontProvider.GetValue();
		}
	}
}