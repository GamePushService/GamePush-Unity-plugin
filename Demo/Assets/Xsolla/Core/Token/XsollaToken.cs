using UnityEngine;

namespace Xsolla.Core
{
	public static class XsollaToken
	{
		private const string SaveKey = "XsollaSuperToken";

		public static string AccessToken => Instance?.accessToken;

		public static string RefreshToken => Instance?.refreshToken;

		public static bool Exists => Instance != null;

		private static TokenData Instance { get; set; }

		public static void Create(string accessToken)
		{
			Instance = new TokenData {
				accessToken = accessToken
			};

			SaveInstance();
		}

		public static void Create(string accessToken, string refreshToken)
		{
			Instance = new TokenData {
				accessToken = accessToken,
				refreshToken = refreshToken
			};

			SaveInstance();
		}

		private static void SaveInstance()
		{
			if (Instance == null)
				return;

			var json = ParseUtils.ToJson(Instance);
			PlayerPrefs.SetString(SaveKey, json);
		}

		public static bool TryLoadInstance()
		{
			if (!PlayerPrefs.HasKey(SaveKey))
				return false;

			var json = PlayerPrefs.GetString(SaveKey);
			Instance = ParseUtils.FromJson<TokenData>(json);
			return Instance != null;
		}

		public static void DeleteSavedInstance()
		{
			Instance = null;
			PlayerPrefs.DeleteKey(SaveKey);
		}
	}
}