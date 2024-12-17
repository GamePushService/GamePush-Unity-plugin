using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(SimpleButton))]
	public class AvatarChoiceButton : MonoBehaviour
	{
		[SerializeField] private Image AvatarImage;

		public static event Action<Sprite> AvatarPicked;

		private void Awake()
		{
			GetComponent<SimpleButton>().onClick += RaiseAvatarPicked;
		}

		private void RaiseAvatarPicked()
		{
			AvatarPicked?.Invoke(AvatarImage.sprite);
		}
	}
}