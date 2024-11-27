using System.Xml;

namespace Xsolla.Core.Editor
{
	public class FindByChildName : IFindCriteria<XmlNode>
	{
		private readonly string tag;
		private readonly BaseManifestNode childNode;

		public FindByChildName(string tag, BaseManifestNode childNode)
		{
			this.tag = tag;
			this.childNode = childNode;
		}

		public bool MatchesCriteria(XmlNode xmlNode)
		{
			if (xmlNode == null)
			{
				return false;
			}

			if (tag.Equals(xmlNode.Name))
			{
				var child = xmlNode.FindNodeRecursive(new FindByName(childNode));
				return child != null;
			}

			return false;
		}
	}
}