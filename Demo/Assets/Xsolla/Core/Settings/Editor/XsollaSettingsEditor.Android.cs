using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor
	{
		private const string FACEBOOK_APP_ID_LABEL = "Facebook App ID";
		private const string FACEBOOK_APP_ID_TOOLTIP = "Application ID from your Facebook developer page";

		private const string FACEBOOK_CLIENT_TOKEN_LABEL = "Facebook Client Token";
		private const string FACEBOOK_CLIENT_TOKEN_TOOLTIP = "Facebook client token (can be obtained on Facebook developer page). Used for native user authentication via Facebook Android application";

		private const string GOOGLE_SERVER_ID_LABEL = "Google Server ID";
		private const string GOOGLE_SERVER_ID_TOOLTIP = "Server ID from your Google developer page";

		private const string WECHAT_APP_ID_LABEL = "WeChat App ID";
		private const string WECHAT_APP_ID_TOOLTIP = "Application ID from your WeCHat developer page";

		private const string QQ_APP_ID_LABEL = "QQ App ID";
		private const string QQ_APP_ID_TOOLTIP = "Application ID from your QQ developer page";

		private bool AndroidSettings()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GroupAreaStyle);
			EditorGUILayout.LabelField("Android", GroupHeaderStyle);

			var changed = false;

			var facebookAppId = EditorGUILayout.TextField(new GUIContent(FACEBOOK_APP_ID_LABEL, FACEBOOK_APP_ID_TOOLTIP), XsollaSettings.FacebookAppId);
			if (facebookAppId != XsollaSettings.FacebookAppId)
			{
				XsollaSettings.FacebookAppId = facebookAppId;
				changed = true;
			}

			var facebookClientToken = EditorGUILayout.TextField(new GUIContent(FACEBOOK_CLIENT_TOKEN_LABEL, FACEBOOK_CLIENT_TOKEN_TOOLTIP), XsollaSettings.FacebookClientToken);
			if (facebookClientToken != XsollaSettings.FacebookClientToken)
			{
				XsollaSettings.FacebookClientToken = facebookClientToken;
				changed = true;
			}

			var googleServerId = EditorGUILayout.TextField(new GUIContent(GOOGLE_SERVER_ID_LABEL, GOOGLE_SERVER_ID_TOOLTIP), XsollaSettings.GoogleServerId);
			if (googleServerId != XsollaSettings.GoogleServerId)
			{
				XsollaSettings.GoogleServerId = googleServerId;
				changed = true;
			}

			var wechatAppId = EditorGUILayout.TextField(new GUIContent(WECHAT_APP_ID_LABEL, WECHAT_APP_ID_TOOLTIP), XsollaSettings.WeChatAppId);
			if (wechatAppId != XsollaSettings.WeChatAppId)
			{
				XsollaSettings.WeChatAppId = wechatAppId;
				changed = true;
			}

			var qqAppId = EditorGUILayout.TextField(new GUIContent(QQ_APP_ID_LABEL, QQ_APP_ID_TOOLTIP), XsollaSettings.QqAppId);
			if (qqAppId != XsollaSettings.QqAppId)
			{
				XsollaSettings.QqAppId = qqAppId;
				changed = true;
			}

			EditorGUILayout.EndVertical();
			return changed;
		}
	}
}