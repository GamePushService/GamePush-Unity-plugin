using System.Collections.Generic;

namespace Xsolla.Core.Editor
{
	public class ActivityNode : BaseManifestNode
	{
		public ActivityNode(string name) : base(
			AndroidManifestConstants.ActivityTag,
			AndroidManifestConstants.ApplicationTag,
			new Dictionary<string, string> {{AndroidManifestConstants.NameAttribute, name}}) { }
	}
}