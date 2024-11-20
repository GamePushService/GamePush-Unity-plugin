using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Browser
{
	public class PreloaderScript : MonoBehaviour
	{
		private Text text;

		// Start is called before the first frame update
		private void Start()
		{
			text = transform.Find("Text").GetComponent<Text>();
		}

		public void SetPercent(int value)
		{
			SetText($"Update{Environment.NewLine}{value}%");
		}

		public void SetText(string value)
		{
			if (text != null)
				text.text = value;
		}
	}
}