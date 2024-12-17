using UnityEngine;

namespace Xsolla.Demo
{
	public class FixScriptForEmptyPrefab : MonoBehaviour
	{
		private void Start()
		{
			Destroy(this);
		}
	}
}
