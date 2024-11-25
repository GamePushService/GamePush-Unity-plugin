using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class FriendActionsButton : MonoBehaviour
	{
		[SerializeField] private GameObject actionPrefab = default;
		[SerializeField] private Transform actionContainer = default;

		private static FriendActionsButton _activeButton;

		private readonly List<GameObject> _actions = new List<GameObject>();

		private bool IsActive => actionContainer.gameObject.activeSelf;

		private void OnDestroy()
		{
			ClearActions();

			if (_activeButton != null && _activeButton.Equals(this))
				_activeButton = null;
		}

		private void Start()
		{
			if (actionContainer != null)
			{
				actionContainer.gameObject.SetActive(false);

				var btn = gameObject.GetComponent<SimpleButton>();
				if (btn != null)
					btn.onClick = ToggleActionContainer;
			}
		}

		private void ToggleActionContainer()
		{
			var newActiveValue = !IsActive;
			actionContainer.gameObject.SetActive(newActiveValue);

			if (!_activeButton)
			{
				_activeButton = this;
				return;
			}

			if (_activeButton.Equals(this))
				return;

			if (_activeButton.IsActive)
				_activeButton.ToggleActionContainer();

			_activeButton = this;
		}

		public void AddAction(string actionName, Action callback)
		{
			if (actionPrefab != null && actionContainer != null)
			{
				var go = Instantiate(actionPrefab, actionContainer);
				var button = go.GetComponent<SimpleTextButton>();
				button.Text = actionName;
				button.onClick = () =>
				{
					actionContainer.gameObject.SetActive(false);
					callback?.Invoke();
				};
				_actions.Add(go);
			}
			else
			{
				XDebug.LogError("In FriendActionsButton script `actionPrefab` or `actionContainer` = null!");
			}
		}

		public void ClearActions()
		{
			_actions.ForEach(Destroy);
			_actions.Clear();
		}
	}
}
