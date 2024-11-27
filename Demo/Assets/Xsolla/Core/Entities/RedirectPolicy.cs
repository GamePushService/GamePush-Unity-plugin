using System;

namespace Xsolla.Core
{
	[Serializable]
	public class RedirectPolicy
	{
		public string return_url;
		public string redirect_conditions;
		public int delay;
		public string status_for_manual_redirection;
		public string redirect_button_caption;
	}
}