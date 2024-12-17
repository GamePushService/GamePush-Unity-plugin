using System.Collections.Generic;

namespace Xsolla.Core.Editor
{
	public class ActionNode : BaseManifestNode
	{
		public ActionNode(string name) : base(
			AndroidManifestConstants.ActionTag,
			AndroidManifestConstants.IntentFilterTag,
			new Dictionary<string, string> {{AndroidManifestConstants.NameAttribute, name}}) { }
	}
}