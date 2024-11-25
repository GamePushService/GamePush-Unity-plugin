using System.Xml;

namespace Xsolla.Core.Editor
{
	public class FindByTag : IFindCriteria<XmlNode>
	{
		private readonly string tag;

		public FindByTag(string tag)
		{
			this.tag = tag;
		}

		public bool MatchesCriteria(XmlNode xmlNode)
		{
			if (xmlNode == null)
			{
				return false;
			}

			return tag.Equals(xmlNode.Name);
		}
	}
}