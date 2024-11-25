using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class HighlightElement : MonoBehaviour
	{
		[SerializeField] private string Id = default;
		[SerializeField] Color highlightColor = default;
		public string ID => Id;

		public void Highlight()
		{
			// in case highlighted object is a button any interaction with it should be disabled
			GetComponentInChildren<SimpleTextButton>()?.Lock();

			var image = GetComponentInChildren<Image>();
			if (image != null)
				image.color = highlightColor;

			var text = GetComponentInChildren<Text>();
			if (text != null)
				text.color = highlightColor;
		}
	}
}
