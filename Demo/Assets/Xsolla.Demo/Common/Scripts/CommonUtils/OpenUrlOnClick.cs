using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class OpenUrlOnClick : MonoBehaviour
	{
		[SerializeField] private SimpleButton Button = default;
		[SerializeField] private UrlType UrlType = default;

		private string _url;

		public string URL
		{
			get => _url;
			set => _url = value;
		}

		private void Awake()
		{
			Button.onClick += OpenUrl;
			URL = DemoController.Instance.UrlContainer.GetUrl(UrlType);
		}

		private void OpenUrl()
		{
			XsollaWebBrowser.Open(URL, true);
		}
	}
}
