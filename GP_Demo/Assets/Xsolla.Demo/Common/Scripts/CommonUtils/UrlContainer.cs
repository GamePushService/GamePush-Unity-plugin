using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class UrlContainer : MonoBehaviour
	{
		[SerializeField] public string[] Urls = default;

		public string GetUrl(UrlType urlType)
		{
			var index = (int)urlType;
			return Urls[index];
		}

		private void Awake()
		{
			CheckConsistency();
		}

		private void CheckConsistency()
		{
			var typeNames = Enum.GetNames(typeof(UrlType));
			var numberOfElementsInEnum = typeNames.Length;
			var urlsArrayLength = Urls.Length;

			if (urlsArrayLength != numberOfElementsInEnum)
				XDebug.LogError($"URLs length is {urlsArrayLength} while there are {numberOfElementsInEnum} UrlTypes defined. Expect the IndexOutOfRangeException.");

			for (int i = 0; i < urlsArrayLength; i++)
			{
				if (string.IsNullOrEmpty(Urls[i]))
					XDebug.Log($"URL value for '{typeNames[i]}' is null or empty.");
			}
		}
	}
}
