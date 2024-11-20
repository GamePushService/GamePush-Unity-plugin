using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class PaystationUrlBuilder
	{
		private readonly string PaymentToken;
		private readonly bool IsSandBox;
		private readonly int PaystationVersion;

		public PaystationUrlBuilder(string paymentToken)
		{
			PaymentToken = paymentToken;
			IsSandBox = XsollaSettings.IsSandbox;
			PaystationVersion = XsollaSettings.PaystationVersion;
		}

		public string Build()
		{
			var url = GetPaystationBasePath() + GetPaystationVersionPath();

			return new UrlBuilder(url)
				.AddParam(GetTokenQueryKey(), PaymentToken)
				.AddParam("engine", "unity")
				.AddParam("engine_v", Application.unityVersion)
				.AddParam("sdk", "store")
				.AddParam("sdk_v", Constants.SDK_VERSION)
				.Build();
		}

		private string GetPaystationBasePath()
		{
			return IsSandBox
				? "https://sandbox-secure.xsolla.com/"
				: "https://secure.xsolla.com/";
		}

		private string GetPaystationVersionPath()
		{
			switch (PaystationVersion)
			{
				case 3:  return "paystation3";
				case 4:  return "paystation4";
				default: throw new Exception($"Unknown Paystation version: {PaystationVersion}");
			}
		}

		private string GetTokenQueryKey()
		{
			switch (PaystationVersion)
			{
				case 3:  return "access_token";
				case 4:  return "token";
				default: throw new Exception($"Unknown Paystation version: {PaystationVersion}");
			}
		}
	}
}