using UnityEngine;
using UnityEngine.UI;
using Xsolla.UIBuilder;

namespace Xsolla.Demo
{
	public class SimpleTextButtonDisableable : SimpleTextButton
	{
		[Space]
		[SerializeField] private DecoratorPointerEvents DecoratorPointerEvents = default;

		[SerializeField] private Image DisableImage = default;
		[SerializeField] private SpriteProvider NormalSpriteProp = default;
		[SerializeField] private SpriteProvider DisabledSpriteProp = default;
		[SerializeField] private ColorProvider NormalImageColorProp = default;
		[SerializeField] private ColorProvider DisabledImageColorProp = default;

		[SerializeField] private Text DisableText = default;
		[SerializeField] private ColorProvider NormalTextColorProp = default;
		[SerializeField] private ColorProvider DisabledTextColorProp = default;

		public void Enable()
		{
			DisableImage.sprite = NormalSpriteProp.GetValue();
			DisableImage.color = NormalImageColorProp.GetValue();
			DisableText.color = NormalTextColorProp.GetValue();

			DecoratorPointerEvents.IsMute = false;
			Unlock();
		}

		public void Disable()
		{
			Lock();
			DecoratorPointerEvents.IsMute = true;

			DisableImage.sprite = DisabledSpriteProp.GetValue();
			DisableImage.color = DisabledImageColorProp.GetValue();
			DisableText.color = DisabledTextColorProp.GetValue();
		}
	}
}