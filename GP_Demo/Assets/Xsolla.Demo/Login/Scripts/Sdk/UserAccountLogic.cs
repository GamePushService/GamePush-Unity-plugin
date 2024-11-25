using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.UserAccount;

namespace Xsolla.Demo
{
	public class UserAccountLogic : MonoSingleton<UserAccountLogic>
	{
		private const int NETWORKS_CACHE_TIMEOUT = 5;

		#region Social

		private bool IsEditorOrStandalone
		{
			get
			{
				if (Application.isEditor)
					return true;

				switch (Application.platform)
				{
					case RuntimePlatform.LinuxEditor:
					case RuntimePlatform.OSXEditor:
					case RuntimePlatform.WindowsEditor:
						return true;
				}

				return false;
			}
		}

		public void LinkSocialProvider(SocialProvider socialProvider, Action<SocialProvider> onSuccess, Action<Error> onError = null)
		{
			if (!IsEditorOrStandalone)
			{
				var errorMessage = "LinkSocialProvider: This functionality is not supported elswere except Editor and Standalone build";
				XDebug.LogError(errorMessage);
				onError?.Invoke(new Error(ErrorType.MethodIsNotAllowed, errorMessage: errorMessage));
				return;
			}

			Action<LinkSocialProviderLink> urlCallback = url =>
			{
				var browser = XsollaWebBrowser.InAppBrowser;
				if (browser == null)
				{
					var message = "LinkSocialProvider: Can not obtain in-built browser";
					XDebug.LogError(message);
					onError?.Invoke(new Error(ErrorType.MethodIsNotAllowed, errorMessage: message));
					return;
				}

				browser.Open(url.url);
				browser.AddInitHandler(() =>
				{
					browser.AddUrlChangeHandler(newUrl =>
					{
						XDebug.Log($"URL = {newUrl}");

						if (ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.token, out _))
						{
							browser.Close();
							HotkeyCoroutine.Unlock();
							onSuccess?.Invoke(socialProvider);
							return;
						}

						if (ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.error_code, out string errorCode) &&
						    ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.error_description, out string errorDescription))
						{
							browser.Close();
							HotkeyCoroutine.Unlock();
							onError?.Invoke(new Error(statusCode: errorCode, errorMessage: errorDescription));
						}
					});
				});
			};

			XsollaUserAccount.LinkSocialProvider(socialProvider, urlCallback, onError);
		}

		private List<LinkedSocialNetwork> _networksCache;
		private DateTime _networksCacheTime;
		private bool _networksCacheInProgress;

		public void PurgeSocialProvidersCache() => _networksCache = null;

		public void GetLinkedSocialProviders(Action<List<LinkedSocialNetwork>> onSuccess, Action<Error> onError = null)
		{
			if (_networksCacheInProgress)
			{
				StartCoroutine(WaitLinkedSocialProviders(onSuccess));
				return;
			}

			if ((DateTime.Now - _networksCacheTime).Seconds > NETWORKS_CACHE_TIMEOUT || _networksCache == null)
			{
				_networksCacheInProgress = true;
				XsollaUserAccount.GetLinkedSocialProviders(networks =>
				{
					_networksCache = networks.items;
					_networksCacheTime = DateTime.Now;
					onSuccess?.Invoke(_networksCache);
					_networksCacheInProgress = false;
				}, error =>
				{
					if (_networksCache == null)
						_networksCache = new List<LinkedSocialNetwork>();
					onError?.Invoke(error);
					_networksCacheInProgress = false;
				});
			}
			else
			{
				onSuccess?.Invoke(_networksCache);
			}
		}

		private IEnumerator WaitLinkedSocialProviders(Action<List<LinkedSocialNetwork>> onSuccess)
		{
			yield return new WaitWhile(() => _networksCacheInProgress);
			onSuccess?.Invoke(_networksCache);
		}

		#endregion
	}
}