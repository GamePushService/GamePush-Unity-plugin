using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public abstract class LoginAuthorization : MonoBehaviour
	{
		public abstract void TryAuth(object[] args, Action onSuccess, Action<Error> onError);
	}
}