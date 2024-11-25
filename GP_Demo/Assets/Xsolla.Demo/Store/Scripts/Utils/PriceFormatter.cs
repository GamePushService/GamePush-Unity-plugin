using System;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public static class PriceFormatter
	{
		private static string _lastCurrency;
		private const string DEFAULT_CURRENCY = "USD";

		private static string LastCurrency
		{
			get { return _lastCurrency ?? DEFAULT_CURRENCY; }
			set { _lastCurrency = value; }
		}

		public static string FormatPrice(float price)
		{
			return FormatPrice(LastCurrency, price);
		}

		public static string FormatPrice(string currency, float price)
		{
			var currencyFormat = CurrencyFormatUtil.GetCurrencyFormat(currency);
			if (currencyFormat == null)
				return string.Empty;

			var roundDownPrice = Math.Round((decimal) price, currencyFormat.fractionSize, MidpointRounding.AwayFromZero);
			var priceTemplate = currencyFormat.symbol.template;
			var currencySymbol = currencyFormat.symbol.grapheme;

			var formattedPrice = priceTemplate
				.Replace("$", currencySymbol)
				.Replace("1", roundDownPrice.ToString(string.Format("F{0}", currencyFormat.fractionSize)));

			LastCurrency = currency;

			return formattedPrice;
		}
	}
}
