using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class LoginProxyActionHolder : MonoBehaviour
	{
		public Action<LoginPageEnterController, object> ProxyAction { get; set; }
		public object ProxyActionArgument { get; set; }
	}
}
