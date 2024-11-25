using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor
	{
		private const string CALLBACK_URL_LABEL = "Callback URL";
		private const string CALLBACK_URL_TOOLTIP = "URI to redirect the user to after signing up, logging in, or password reset.\n" +
		                                            "For OAuth 2.0 authorization must be identical to the OAuth 2.0 redirect URI specified in Publisher Account in Login >Security > OAuth 2.0 settings.\n" +
		                                            "For JWT authorization must be identical to the callback URL specified in Publisher Account in Login settings.\n" +
		                                            "Required if there are several сallback URIs.";
		private const string OAUTH_CLIENT_ID_LABEL = "OAuth Client ID";
		private const string OAUTH_CLIENT_ID_TOOLTIP = "To get the ID, set up an OAuth client in Publusher account in the " +
		                                               "\"Login -> your Login project -> Secure -> OAuth 2.0\" section.";

		private bool LoginSettings()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GroupAreaStyle);
			EditorGUILayout.LabelField("Login", GroupHeaderStyle);

			var changed = false;

			var oauthClientId = EditorGUILayout.IntField(new GUIContent(OAUTH_CLIENT_ID_LABEL, OAUTH_CLIENT_ID_TOOLTIP), XsollaSettings.OAuthClientId);
			if (oauthClientId != XsollaSettings.OAuthClientId)
			{
				XsollaSettings.OAuthClientId = oauthClientId;
				changed = true;
			}

			if (oauthClientId <= 0)
				DrawErrorBox("OAuth Client ID has incorrect value");

			var callback = EditorGUILayout.TextField(new GUIContent(CALLBACK_URL_LABEL, CALLBACK_URL_TOOLTIP), XsollaSettings.CallbackUrl);
			if (callback != XsollaSettings.CallbackUrl)
			{
				XsollaSettings.CallbackUrl = callback;
				changed = true;
			}

			var regex = new Regex(@"^[^\s].+[^\s]$");
			if (!string.IsNullOrEmpty(callback) && (!regex.IsMatch(callback) || !Uri.IsWellFormedUriString(callback, UriKind.Absolute)))
				DrawErrorBox("Callback URL has incorrect value");

			EditorGUILayout.EndVertical();
			return changed;
		}
	}
}