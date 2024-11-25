using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(SimpleTextButton))]
	public class FriendSystemSocialNetwork : MonoBehaviour
	{
		[SerializeField] private Image background = default;
		[SerializeField] private Image logo = default;
		[SerializeField] private Image linkIcon = default;
		[SerializeField] private Image addIcon = default;
		[SerializeField] private SocialProvider provider = SocialProvider.None;

		public enum State
		{
			Linked,
			Unlinked
		}

		public event Action<SocialProvider, State> StateChanged;

		public SocialProvider Provider => provider;
		public SimpleButton Button => GetComponent<SimpleButton>();

		private void Start()
		{
			RefreshState();
		}

		public void RefreshState()
		{
			if (provider == SocialProvider.None)
				return;

			UserAccountLogic.Instance.GetLinkedSocialProviders(
			onSuccess: networks =>
			{
				if (networks.Any(n => n.provider.Equals(provider.ToString().ToLowerInvariant())))
					SetState(State.Linked);
				else
					SetState(State.Unlinked);
			},
			onError: StoreDemoPopup.ShowError);
		}

		private void SetVisibility(MonoBehaviour component, bool visibility)
		{
			if(component != null)
				component.gameObject.SetActive(visibility);
		}

		private void SetImageOpacity(Image image, float opacity)
		{
			var tmpColor = image.color;
			tmpColor.a = opacity;
			image.color = tmpColor;
		}

		private void SetState(State state)
		{
			switch (state)
			{
				case State.Linked:
				{
					SetVisibility(background, true);
					SetVisibility(logo, true);
					SetVisibility(linkIcon, true);
					SetVisibility(addIcon, false);

					SetImageOpacity(logo, 0.7F);
					GetComponent<SimpleTextButton>().onClick = UnlinkSocialProvider;
					break;
				}
				case State.Unlinked:
				{
					SetVisibility(background, false);
					SetVisibility(logo, true);
					SetVisibility(linkIcon, false);
					SetVisibility(addIcon, true);

					SetImageOpacity(logo, 1.0F);
					GetComponent<SimpleTextButton>().onClick = LinkSocialProvider;
					break;
				}
				default:
				{
					XDebug.LogWarning($"Unexpected state: '{state}'");
					return;
				}
			}

			StateChanged?.Invoke(provider, state);
		}

		private void UnlinkSocialProvider()
		{
			XDebug.Log("We can not unlink social provider yet. So do nothing.");
		}

		private void LinkSocialProvider()
		{
			var supported = true;

#if UNITY_EDITOR
			switch (EditorUserBuildSettings.activeBuildTarget)
			{
				case BuildTarget.StandaloneOSX:
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
				case BuildTarget.StandaloneLinux64:
					//Do nothing
					break;
				default:
					supported = false;
					break;
			}

#elif (!UNITY_STANDALONE)
			supported = false;
#else
#endif
			if (supported)
			{
				Action<SocialProvider> onSuccessLink = _ =>
				{
					UserAccountLogic.Instance.PurgeSocialProvidersCache();
					UserFriends.Instance.UpdateFriends(RefreshState,StoreDemoPopup.ShowError);
				};
				UserAccountLogic.Instance.LinkSocialProvider(provider, onSuccess: onSuccessLink, onError: StoreDemoPopup.ShowError);
			}
			else
			{
				StoreDemoPopup.ShowError(new Error(errorMessage:"Social account linking is not supported for this platform"));
			}
		}
	}
}
