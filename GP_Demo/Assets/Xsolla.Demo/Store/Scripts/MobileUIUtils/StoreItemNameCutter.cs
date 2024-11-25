using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(StoreItemUI))]
    public class StoreItemNameCutter : MonoBehaviour
    {
		[SerializeField] private Text NameText = default;
		[SerializeField] private int NameLimit = default;
		[SerializeField] private string Suffix = "";

		private void Awake()
		{
			GetComponent<StoreItemUI>().OnInitialized += CutTheName;
		}

		private void CutTheName(CatalogItemModel itemModel)
		{
			var itemName = itemModel.Name;
			
			if (itemName.Length > NameLimit)
			{
				var cuttedName = itemName.Substring(0, NameLimit);
				var result = $"{cuttedName}{Suffix}";
				NameText.text = result;
			}
		}
	}
}
