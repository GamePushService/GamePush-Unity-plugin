using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class XsollaWebCallbacks : MonoBehaviour
	{
		private event Action OnPaymentStatusUpdate;
		private event Action OnPaymentCancel;

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		// Callback for Xsolla Pay Station (do not remove)
		public void PublishPaymentStatusUpdate()
		{
			OnPaymentStatusUpdate?.Invoke();
		}

		// Callback for Xsolla Pay Station (do not remove)
		public void PublishPaymentCancel()
		{
			OnPaymentCancel?.Invoke();
		}

		public static void AddPaymentStatusUpdateHandler(Action action)
		{
			Instance.OnPaymentStatusUpdate += action;
		}

		public static void RemovePaymentStatusUpdateHandler(Action action)
		{
			Instance.OnPaymentStatusUpdate -= action;
		}

		public static void AddPaymentCancelHandler(Action action)
		{
			Instance.OnPaymentCancel += action;
		}

		public static void RemovePaymentCancelHandler(Action action)
		{
			Instance.OnPaymentCancel -= action;
		}

		private static XsollaWebCallbacks _instance;

		private static XsollaWebCallbacks Instance
		{
			get
			{
				if (!_instance)
					_instance = new GameObject("XsollaWebCallbacks").AddComponent<XsollaWebCallbacks>();

				return _instance;
			}
		}
	}
}