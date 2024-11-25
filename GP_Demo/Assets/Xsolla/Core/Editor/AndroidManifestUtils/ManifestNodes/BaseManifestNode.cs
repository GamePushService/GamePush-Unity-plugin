using System.Collections.Generic;

namespace Xsolla.Core.Editor
{
	public class BaseManifestNode
	{
		public string Tag { get; private set; }
		public string ParentTag { get; private set; }
		public Dictionary<string, string> Attributes { get; }
		public List<BaseManifestNode> ChildNodes { get; }

		public BaseManifestNode(string tag, string parentTag = "", Dictionary<string, string> attributes = null, List<BaseManifestNode> childNodes = null)
		{
			Tag = tag;
			ParentTag = parentTag;
			Attributes = attributes ?? new Dictionary<string, string>();
			ChildNodes = childNodes ?? new List<BaseManifestNode>();
		}

		public void AddChildNode(BaseManifestNode childNode)
		{
			ChildNodes.Add(childNode);
		}

		public void AddAttribute(string key, string value)
		{
			Attributes.Add(key, value);
		}
	}
}