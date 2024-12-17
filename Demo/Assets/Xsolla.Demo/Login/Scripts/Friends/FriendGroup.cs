using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class FriendGroup : MonoBehaviour
	{
		[SerializeField] private Text userCounterText = default;

		public void SetUsersCount(int count)
		{
			if(userCounterText != null)
				userCounterText.text = count > 0 ?  $" ({count})" : string.Empty;
		}

		private void OnEnable()
		{
			if(userCounterText != null)
				userCounterText.gameObject.SetActive(true);
		}

		private void OnDisable()
		{
			if(userCounterText != null)
				userCounterText.gameObject.SetActive(false);
		}
	}
}
