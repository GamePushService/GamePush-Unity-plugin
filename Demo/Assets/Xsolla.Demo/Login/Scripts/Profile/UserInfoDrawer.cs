using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class UserInfoDrawer : MonoBehaviour
	{
		[SerializeField] private Text UserEmailText = default;
		[SerializeField] private Text UserTagText = default;

		private const int USER_INFO_LIMIT = 42;

		private IEnumerator Start()
		{
			if (!UserEmailText)
			{
				XDebug.LogWarning("Can not draw user info because 'UserEmailText' field is null or empty!");
				yield break;
			}

			yield return new WaitWhile(() => !XsollaToken.Exists);

			var busy = true;
			XsollaAuth.GetUserInfo(info =>
			{
				DrawInfo(info);
				busy = false;
			}, _ => busy = false);

			yield return new WaitWhile(() => busy);
			gameObject.SetActive(false);
			gameObject.SetActive(true);
		}

		private void DrawInfo(UserInfo info)
		{
			string nameToDraw;

			if (!string.IsNullOrEmpty(info.nickname))
				nameToDraw = info.nickname;
			else if (!string.IsNullOrEmpty(info.email))
				nameToDraw = info.email;
			else
				nameToDraw = string.Empty;

			if (nameToDraw.Length > USER_INFO_LIMIT)
				nameToDraw = nameToDraw.Substring(0, USER_INFO_LIMIT);

			UserEmailText.text = nameToDraw;

			UserTagText.text = string.IsNullOrEmpty(info.tag) ? string.Empty : $"#{info.tag}";
			UserTagText.gameObject.SetActive(!string.IsNullOrEmpty(info.tag));
		}

		public void Refresh()
		{
			StartCoroutine(Start());
		}
	}
}