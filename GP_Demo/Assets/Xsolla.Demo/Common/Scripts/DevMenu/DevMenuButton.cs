using UnityEngine;
using UnityEngine.UI;
using Xsolla.UIBuilder;

namespace Xsolla.Core
{
	public class DevMenuButton : MonoBehaviour
	{
		[SerializeField] private Button Button = default;

		[SerializeField] private GameObject PagePrefab = default;

		private int ClicksCount;

		private void OnEnable()
		{
			Button.onClick.AddListener(OnButtonClick);
		}

		private void OnDisable()
		{
			Button.onClick.RemoveListener(OnButtonClick);
		}

		private void Start()
		{
			DevMenuPage.TryLoadUiTheme();
		}

		private void OnButtonClick()
		{
			ClicksCount++;

			if (ClicksCount >= 8)
			{
				ClicksCount = 0;

				var parent = GetComponentInParent<Canvas>().transform;
				var pageObj = Instantiate(PagePrefab, parent, false);
				pageObj.transform.SetAsLastSibling();
			}
		}
	}
}