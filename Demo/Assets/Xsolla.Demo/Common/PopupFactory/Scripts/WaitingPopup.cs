using System;
using System.Collections;
using UnityEngine;

namespace Xsolla.Demo.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/SuccessPopup")]
	public class WaitingPopup : MonoBehaviour, IWaitingPopup
	{
		private Action _callback;
		private Func<bool> _condition;
		
		private void Start()
		{
			StartCoroutine(WaitingCoroutine());
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		private IEnumerator WaitingCoroutine()
		{
			while (true)
			{
				yield return new WaitUntil(() => _condition?.Invoke() ?? false);
				_callback?.Invoke();
				Destroy(gameObject, 0.1F);
				yield break;
			}
		}

		public IWaitingPopup SetCloseCondition(Func<bool> condition)
		{
			_condition = condition;
			return this;
		}

		public IWaitingPopup SetCloseHandler(Action handler)
		{
			_callback = handler;
			return this;
		}
	}
}