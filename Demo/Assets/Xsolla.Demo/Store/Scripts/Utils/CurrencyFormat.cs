using System;

[Serializable]
public class CurrencyFormat
{
	[Serializable]
	public class Symbol
	{
		public string grapheme;
		public string template;
		public bool rtl;
	}
	
	public string name;
	public int fractionSize;
	public Symbol symbol;
	public Symbol uniqSymbol;
}
