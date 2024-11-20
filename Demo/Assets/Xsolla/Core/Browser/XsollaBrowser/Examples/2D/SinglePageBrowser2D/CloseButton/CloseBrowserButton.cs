using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Browser
{
	internal class CloseBrowserButton : MonoBehaviour
	{
		[SerializeField] private Button CloseButton;

		private void Start()
		{
			if (CloseButton != null)
			{
				CloseButton.onClick.AddListener(() => Destroy(gameObject, 0.01f));
			}
		}
	}
}