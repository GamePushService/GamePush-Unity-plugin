using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class VirtualCurrencyBalanceUI : MonoBehaviour
	{
		[SerializeField] public Image Image = default;
		[SerializeField] public Text Text = default;

		public void Initialize(ItemModel item)
		{
			if (Image != null)
			{
				if (!string.IsNullOrEmpty(item.ImageUrl))
				{
					ImageLoader.LoadSprite(item.ImageUrl, sprite =>
					{
						if (Image /*still*/ != null)
							Image.sprite = sprite;
					});
				}
				else
				{
					XDebug.LogError($"Item with sku = '{item.Sku}' without image!");
				}
			}
			else
			{
				XDebug.LogWarning($"Your Virtual Currency with sku = `{item.Sku}` created without Image component!");
			}
		}

		public void SetBalance(int balance)
		{
			if (Text)
				Text.text = balance.ToString();
		}
	}
}