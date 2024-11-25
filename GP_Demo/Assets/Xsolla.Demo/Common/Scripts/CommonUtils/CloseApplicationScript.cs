using UnityEditor;
using UnityEngine;

namespace Xsolla.Demo
{
	public class CloseApplicationScript : MonoBehaviour
	{
		[SerializeField] private SimpleButton exitButton = default;

		void Start()
		{
			if (exitButton != null)
				exitButton.onClick = () =>
				{
					Application.Quit();
	#if UNITY_EDITOR
					EditorApplication.isPlaying = false;
	#endif
				};
		}
	}
}
