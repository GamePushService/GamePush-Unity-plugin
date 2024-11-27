using System;
using System.Xml;

namespace Xsolla.Core.Editor
{
	public class FindByLabel : IFindCriteria<XmlNode>
	{
		private readonly BaseManifestNode node;

		public FindByLabel(BaseManifestNode node)
		{
			this.node = node;
		}

		public bool MatchesCriteria(XmlNode xmlNode)
		{
			if (xmlNode != null && xmlNode.Attributes != null)
			{
				if (xmlNode.Name.Equals(node.Tag, StringComparison.InvariantCultureIgnoreCase))
				{
					var attributeNode = xmlNode.Attributes.GetNamedItem(AndroidManifestConstants.LabelAttribute);
					return attributeNode != null
					       && node.Attributes.ContainsKey(AndroidManifestConstants.LabelAttribute)
					       && attributeNode.Value.Equals(node.Attributes[AndroidManifestConstants.LabelAttribute]);
				}
			}

			return false;
		}
	}
}