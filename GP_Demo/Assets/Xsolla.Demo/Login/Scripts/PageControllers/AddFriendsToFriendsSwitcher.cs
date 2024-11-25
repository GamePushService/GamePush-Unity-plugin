using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class AddFriendsToFriendsSwitcher : MonoBehaviour
	{
		[SerializeField] private GroupsController groupsController = default;

		private void Awake()
		{
			gameObject.AddComponent<FriendsMenuHelper>();
		}

		void Start()
		{
			if (groupsController != null)
			{
				FriendsMenuHelper.RefreshGroups(groupsController, FriendsMenuHelper.ADD_FRIENDS_GROUP);
				Action<string> groupChangedCallback = group =>
				{
					FriendsMenuHelper.RefreshGroups(groupsController, group);
					DemoController.Instance.SetState(MenuState.Friends);
				};
				groupsController.GroupSelectedEvent += _ => groupsController.GroupSelectedEvent -= groupChangedCallback;
				groupsController.GroupSelectedEvent += groupChangedCallback;
			}
		}
	}
}
