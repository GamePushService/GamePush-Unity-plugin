using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.UIBuilder;

namespace Xsolla.Demo
{
	public class SearchFriendsMenuController : MonoBehaviour
	{
		private const float SearchUsersTimeout = 2.0F;

		[SerializeField] private WidgetProvider userPrefabProvider = default;
		[SerializeField] private ItemContainer usersContainer = default;
		[SerializeField] private FriendSearchBox searchBox = default;

		[SerializeField] private GameObject[] SearchContent = default;
		[SerializeField] private GameObject EmptySearchResultContent = default;
		[SerializeField] private GameObject SocialContent = default;

		private List<string> _nicknames;

		private void Awake()
		{
			_nicknames = new List<string>();
		}

		void Start()
		{
			StartCoroutine(SearchUsersCoroutine(RefreshUsersContainer, StoreDemoPopup.ShowError));
			if (searchBox != null)
			{
				searchBox.SearchRequest += SearchboxInputFinished;
				searchBox.ClearSearchRequest += () => SearchboxInputFinished(null);
			}
		}

		private void SearchboxInputFinished(string text)
		{
			if(string.IsNullOrEmpty(text))
				SwitchUI(UIState.Social);
			else
				_nicknames.Add(text);
		}

		private void SwitchUI(UIState state)
		{
			foreach (var item in SearchContent)
				SetActive(item, state == UIState.Search);

			SetActive(SocialContent, state == UIState.Social);
			SetActive(EmptySearchResultContent, state == UIState.EmptySearchResult);
		}

		private void SetActive(GameObject gameObject, bool targetState)
		{
			if(gameObject != null && gameObject.activeSelf != targetState)
				gameObject.SetActive(targetState);
		}

		private IEnumerator SearchUsersCoroutine(Action<List<FriendModel>> onSuccess, Action<Error> onError = null)
		{
			while (true)
			{
				yield return new WaitUntil(() => _nicknames.Any());
				var nickname = _nicknames.Last();
				_nicknames.Clear();
				UserFriends.Instance.SearchUsersByNickname(nickname, onSuccess, onError);
				yield return new WaitForSeconds(SearchUsersTimeout);
			}
		}

		private void RefreshUsersContainer(List<FriendModel> users)
		{
			usersContainer.Clear();
			users.ForEach(u =>
			{
				var go = usersContainer.AddItem(userPrefabProvider.GetValue());
				go.GetComponent<FriendUI>().Initialize(u);
			});

			if (users.Count > 0)
				SwitchUI(UIState.Search);
			else
				SwitchUI(UIState.EmptySearchResult);
		}

		private enum UIState
		{
			Social, Search, EmptySearchResult
		}
	}
}
