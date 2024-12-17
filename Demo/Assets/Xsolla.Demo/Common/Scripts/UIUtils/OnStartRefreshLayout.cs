using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class OnStartRefreshLayout : MonoBehaviour
    {
		[SerializeField] private RectTransform LayoutRoot = default;

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(0.2f);
			RefreshLayout();
		}

		protected void RefreshLayout()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutRoot);
		}
	}
}
