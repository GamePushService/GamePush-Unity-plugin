using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Xsolla.XsollaBrowser
{
	public class MainThreadExecutor : MonoBehaviour
	{
		private readonly ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

		private void Update()
		{
			while (actions.TryDequeue(out var action))
			{
				action?.Invoke();
			}
		}

		public void Enqueue(Action action)
		{
			actions?.Enqueue(action);
		}
	}
}