using System;
using System.Collections;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class HotkeyCoroutine : MonoBehaviour
	{
		public event Action KeyPressedEvent;
		private KeyCode _keyCode;
		private float _timeout;
		private Coroutine _coroutine;
		private static bool m_isLocked;

		public HotkeyCoroutine StartCoroutine(KeyCode keyCode, float timeout = 0.4F)
		{
			_keyCode = keyCode;
			_timeout = timeout;
			stopCoroutine();
			startCoroutine();
			return this;
		}

		public static void Lock()
		{
			m_isLocked = true;
		}

		public static void Unlock()
		{
			m_isLocked = false;
		}

		public static bool IsLocked()
		{
			return m_isLocked;
		}

		private void OnEnable()
		{
			startCoroutine();
		}

		private void OnDisable()
		{
			stopCoroutine();
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		void startCoroutine()
		{
			if (_coroutine == null)
			{
				_coroutine = StartCoroutine(SomeHotkeyCoroutine());
			}
		}

		void stopCoroutine()
		{
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
				_coroutine = null;
			}
		}

		IEnumerator SomeHotkeyCoroutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(_timeout);
				yield return new WaitUntil(() => InputProxy.GetKeyDown(_keyCode));
				if (!m_isLocked)
					KeyPressedEvent?.Invoke();
			}
		}
	}
}