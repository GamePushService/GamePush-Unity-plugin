using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class StoreTabsHotkey : MonoBehaviour
	{
		public Action TabKeyPressedEvent;
		public Action LeftArrowKeyPressedEvent;
		public Action RightArrowKeyPressedEvent;

		void Start()
		{
			gameObject.AddComponent<HotkeyCoroutine>()
				.StartCoroutine(KeyCode.Tab)
				.KeyPressedEvent += () => TabKeyPressedEvent?.Invoke();
			gameObject.AddComponent<HotkeyCoroutine>()
				.StartCoroutine(KeyCode.LeftArrow)
				.KeyPressedEvent += () => LeftArrowKeyPressedEvent?.Invoke();
			gameObject.AddComponent<HotkeyCoroutine>()
				.StartCoroutine(KeyCode.RightArrow)
				.KeyPressedEvent += () => RightArrowKeyPressedEvent?.Invoke();
		}
	}
}