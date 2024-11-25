using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;
using Xsolla.Demo.Popup;
using Xsolla.UserAccount;

namespace Xsolla.Demo
{
	public class UserProfileAvatarManager : MonoBehaviour
	{
		[SerializeField] private GameObject SetBlock;
		[SerializeField] private GameObject UnsetBlock;
		[SerializeField] private GameObject EditBlock;

		[SerializeField] private SimpleButton[] EditButtons;
		[SerializeField] private SimpleButton DeleteButton;

		private void Awake()
		{
			AvatarChoiceButton.AvatarPicked += SetNewAvatar;

			foreach (var button in EditButtons)
				button.onClick += () => SetState(AvatarState.Edit);

			DeleteButton.onClick += () => SetNewAvatar(null);
		}

		private void OnDestroy()
		{
			AvatarChoiceButton.AvatarPicked -= SetNewAvatar;
		}

		private IEnumerator Start()
		{
			string pictureUrl = null;
			bool? isUserPictureUrlObtained = null;

			XsollaAuth.GetUserInfo(
				info =>
				{
					pictureUrl = info.picture;
					isUserPictureUrlObtained = true;
				},
				error =>
				{
					StoreDemoPopup.ShowError(error);
					isUserPictureUrlObtained = false;
				});

			yield return new WaitWhile(() => isUserPictureUrlObtained == null);

			if (isUserPictureUrlObtained == true && !string.IsNullOrEmpty(pictureUrl))
				SetState(AvatarState.Set);
			else
				SetState(AvatarState.Unset);
		}

		private void SetNewAvatar(Sprite newAvatar)
		{
			if (newAvatar != null)
			{
				UploadPicture(newAvatar);
				SetState(AvatarState.Set);
			}
			else
			{
				DeletePicture();
				SetState(AvatarState.Unset);
			}
		}

		private void SetState(AvatarState avatarState)
		{
			if (SetBlock)
				SetBlock.SetActive(avatarState == AvatarState.Set);
			if (UnsetBlock)
				UnsetBlock.SetActive(avatarState == AvatarState.Unset);
			if (EditBlock)
				EditBlock.SetActive(avatarState == AvatarState.Edit);
		}

		private void UploadPicture(Sprite sprite)
		{
			bool? isImageUploaded = null;
			ShowWaiting(() => isImageUploaded == null);

			byte[] data = ConvertToData(sprite, out string boundary);

			XsollaUserAccount.UploadUserPicture(
				data,
				boundary,
				imageInfo =>
				{
					var packedInfo = ParseUtils.FromJson<UserImageUpload>(imageInfo);

					if (packedInfo != null)
					{
						var pictureUrl = packedInfo.picture;
						SetPictureUrlToInfo(pictureUrl,
							onSuccess: () =>
							{
								RefreshImageLoaders();
								isImageUploaded = true;
							},
							onError: () => isImageUploaded = false);
					}
					else
						XDebug.LogError($"Could not parse server response: {imageInfo}");
				},
				error =>
				{
					isImageUploaded = false;
					StoreDemoPopup.ShowError(error);
				});
		}

		private void DeletePicture()
		{
			bool? isPictureDeleted = null;
			ShowWaiting(() => isPictureDeleted == null);

			XsollaUserAccount.DeleteUserPicture(
				() =>
				{
					SetPictureUrlToInfo(null,
						onSuccess: () =>
						{
							isPictureDeleted = true;
							RefreshImageLoaders();
						},
						onError: () => isPictureDeleted = false);
				},
				error =>
				{
					isPictureDeleted = false;
					StoreDemoPopup.ShowError(error);
				});
		}

		private void ShowWaiting(Func<bool> waitWhile)
		{
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => !waitWhile.Invoke());
		}

		private void SetPictureUrlToInfo(string pictureUrl, Action onSuccess = null, Action onError = null)
		{
			XsollaAuth.GetUserInfo(
				info =>
				{
					info.picture = pictureUrl;
					onSuccess?.Invoke();
				},
				error =>
				{
					StoreDemoPopup.ShowError(error);
					onError?.Invoke();
				});
		}

		private byte[] ConvertToData(Sprite sprite, out string boundary)
		{
			boundary = $"{new string('-', 27)}{DateTime.Now.Ticks}";
			var beginBoundary = $"\r\n--{boundary}\r\n";
			var endBoundary = $"\r\n--{boundary}--\r\n";

			var texture = sprite.texture;
			var binaryData = texture.EncodeToPNG();

			var pictureHeader = "Content-Disposition: form-data;name=\"picture\";filename=\"avatar.png\"\r\nContent-Type: \r\n\r\n";

			List<byte> uploadContent = new List<byte>();
			uploadContent.AddRange(Encoding.Default.GetBytes(beginBoundary));
			uploadContent.AddRange(Encoding.Default.GetBytes(pictureHeader));
			uploadContent.AddRange(binaryData);
			uploadContent.AddRange(Encoding.Default.GetBytes(endBoundary));

			return uploadContent.ToArray();
		}

		private void RefreshImageLoaders()
		{
#if UNITY_6000
			var avatarLoaders = FindObjectsByType<UserAvatarLoader>(FindObjectsSortMode.None);
#else
			var avatarLoaders = FindObjectsOfType<UserAvatarLoader>();
#endif

			if (avatarLoaders != null)
				foreach (var loader in avatarLoaders)
				{
					if (loader)
						loader.Refresh();
				}
		}

		private enum AvatarState
		{
			Set,
			Unset,
			Edit
		}
	}
}