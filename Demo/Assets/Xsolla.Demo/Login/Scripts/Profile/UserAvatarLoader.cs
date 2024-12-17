using UnityEngine;
using UnityEngine.UI;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class UserAvatarLoader : MonoBehaviour
	{
		[SerializeField] private Image UserAvatarHolder = default;
		[SerializeField] private Sprite AvatarPlaceholder = default;

		public void Refresh() => Start();

		private void Start()
		{
			XsollaAuth.GetUserInfo(
				info => { DrawAvatar(info.picture); },
				XDebug.LogError);
		}

		private void DrawAvatar(string avatarUrl)
		{
			if (string.IsNullOrEmpty(avatarUrl))
			{
				if (UserAvatarHolder)
					UserAvatarHolder.sprite = AvatarPlaceholder;
			}
			else
			{
				ImageLoader.LoadSprite(avatarUrl,
					avatar =>
					{
						if (UserAvatarHolder)
							UserAvatarHolder.sprite = avatar;
					});
			}
		}
	}
}