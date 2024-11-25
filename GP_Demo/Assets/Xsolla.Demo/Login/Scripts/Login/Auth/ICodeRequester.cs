using System;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public interface ICodeRequester
	{
		void RequestCode(Action<string> onCode);

		void RaiseOnError(Error error);
	}
}