using UnityEngine;

namespace Xsolla.Demo
{
	public class FriendStatusLineUI : MonoBehaviour
	{
		[SerializeField] private GameObject RequestAcceptedObject = default;
		[SerializeField] private GameObject RequestDeclinedObject = default;

		private void Start()
		{
			ClearMessage();
		}

		public void ClearMessage()
		{
			if(RequestAcceptedObject != null)
				RequestAcceptedObject.SetActive(false);
			if(RequestDeclinedObject != null)
				RequestDeclinedObject.SetActive(false);
		}

		public void EnableRequestAcceptedMessage()
		{
			ClearMessage();
			if(RequestAcceptedObject != null)
				RequestAcceptedObject.SetActive(true);
		}
	
		public void EnableRequestDeclinedMessage()
		{
			ClearMessage();
			if(RequestDeclinedObject != null)
				RequestDeclinedObject.SetActive(true);
		}
	}
}
