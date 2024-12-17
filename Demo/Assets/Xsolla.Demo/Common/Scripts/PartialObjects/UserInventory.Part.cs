using System;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class UserInventory : MonoSingleton<UserInventory>
	{
		public bool IsUpdated { get; private set; } = true;

		public void Refresh(Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			RefreshInventory(onSuccess, onError);
		}

		partial void RefreshInventory(Action onSuccess = null, Action<Error> onError = null);
	}
}
