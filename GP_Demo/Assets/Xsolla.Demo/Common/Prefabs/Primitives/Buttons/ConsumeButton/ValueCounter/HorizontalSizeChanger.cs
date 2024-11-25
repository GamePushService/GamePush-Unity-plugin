using UnityEngine;

namespace Xsolla.Demo
{
	public class HorizontalSizeChanger : MonoBehaviour
	{
		public RectTransform.Edge edge;

		private RectTransform _rectTransform;

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		public void SetWidth(float value)
		{
			_rectTransform.SetInsetAndSizeFromParentEdge(edge, 0, value);
		}

		public float GetWidth()
		{
			return _rectTransform.rect.width;
		}
	}
}
