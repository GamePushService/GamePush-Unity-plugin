using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Xsolla.Demo;

public static class CurrencyFormatUtil
{
	const string CURRENCY_FORMAT_FILE_NAME = "currency-format";

	static readonly Dictionary<string, CurrencyFormat> CurrencyFormats;

	static CurrencyFormatUtil()
	{
		var currenciesFile = (TextAsset) Resources.Load(CURRENCY_FORMAT_FILE_NAME);
		if (currenciesFile == null)
		{
			XDebug.LogWarning("Can not find or load currencies file");
			return;
		}

		CurrencyFormats = JsonConvert.DeserializeObject<Dictionary<string, CurrencyFormat>>(currenciesFile.text);
	}

	public static CurrencyFormat GetCurrencyFormat(string currencyCode)
	{
		if (CurrencyFormats != null && CurrencyFormats.ContainsKey(currencyCode))
		{
			return CurrencyFormats[currencyCode];
		}

		return null;
	}
}