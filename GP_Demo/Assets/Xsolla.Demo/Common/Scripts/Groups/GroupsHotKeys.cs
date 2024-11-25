using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class GroupsHotKeys : MonoBehaviour
	{
		public Action ArrowUpKeyPressedEvent;
		public Action ArrowDownKeyPressedEvent;

		void Start()
		{
			gameObject.AddComponent<HotkeyCoroutine>()
				.StartCoroutine(KeyCode.DownArrow, 0.1F)
				.KeyPressedEvent += () => ArrowDownKeyPressedEvent?.Invoke();
			gameObject.AddComponent<HotkeyCoroutine>()
				.StartCoroutine(KeyCode.UpArrow, 0.1F)
				.KeyPressedEvent += () => ArrowUpKeyPressedEvent?.Invoke();
		}
	}
}