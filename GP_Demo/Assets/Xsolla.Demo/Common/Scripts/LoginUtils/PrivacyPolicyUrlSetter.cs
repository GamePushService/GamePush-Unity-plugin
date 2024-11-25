using UnityEngine;

namespace Xsolla.Demo
{
	public class PrivacyPolicyUrlSetter : MonoBehaviour
	{
		[SerializeField] private OpenUrlOnClick PrivacyPolicyOpenScript = default;

		private void Start()
		{
			var currentUrl = PrivacyPolicyOpenScript.URL;
			var currentLanguage = Application.systemLanguage;
			var urlAddition = default(string);

			switch (currentLanguage)
			{
				case SystemLanguage.German:
					urlAddition = "/de/";
					break;
				case SystemLanguage.Korean:
					urlAddition = "/ko/";
					break;
				case SystemLanguage.Chinese:
				case SystemLanguage.ChineseSimplified:
				case SystemLanguage.ChineseTraditional:
					urlAddition = "/zh/";
					break;
				case SystemLanguage.Japanese:
					urlAddition = "/ja/";
					break;
				case SystemLanguage.Russian:
					urlAddition = "/ru/";
					break;
				default:
					urlAddition = "/";
					break;
			}

			PrivacyPolicyOpenScript.URL = string.Format(currentUrl, urlAddition);
		}
	}
}
