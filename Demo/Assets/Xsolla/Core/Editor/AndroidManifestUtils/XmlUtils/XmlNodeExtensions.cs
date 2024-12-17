using System.Xml;

namespace Xsolla.Core.Editor
{
	public static class XmlNodeExtensions
	{
		public static XmlNode FindNodeRecursive(this XmlNode root, IFindCriteria<XmlNode> criteria)
		{
			if (criteria.MatchesCriteria(root))
			{
				return root;
			}

			var currentNode = root.FirstChild;
			while (currentNode != null)
			{
				var node = currentNode.FindNodeRecursive(criteria);
				if (node != null)
				{
					return node;
				}

				currentNode = currentNode.NextSibling;
			}

			return null;
		}

		public static XmlNode FindNodeInChildren(this XmlNode root, IFindCriteria<XmlNode> criteria)
		{
			var currentNode = root.FirstChild;
			while (currentNode != null)
			{
				if (criteria.MatchesCriteria(currentNode))
				{
					return currentNode;
				}

				currentNode = currentNode.NextSibling;
			}

			return null;
		}

		public static void AddAndroidManifestNode(this XmlNode root, XmlDocument xmlDocument, BaseManifestNode node)
		{
			var xmlElement = xmlDocument.CreateElement(node.Tag);

			foreach (var attribute in node.Attributes)
			{
				var split = attribute.Key.Split(':');
				var prefix = split[0];
				var name = split[1];

				var manifest = xmlDocument.FindNodeInChildren(new FindByTag(AndroidManifestConstants.ManifestTag));

				if (manifest != null)
				{
					var xmlNamespace = manifest.GetNamespaceOfPrefix(prefix);
					xmlElement.SetAttribute(name, xmlNamespace, attribute.Value);
				}
			}

			node.ChildNodes.ForEach(childNode => xmlElement.AddAndroidManifestNode(xmlDocument, childNode));
			root.InsertAt(xmlElement, 0);
		}

		public static void InsertAt(this XmlNode node, XmlNode insertingNode, int index = 0)
		{
			if (insertingNode == null)
			{
				return;
			}

			if (index < 0)
			{
				index = 0;
			}

			var childNodes = node.ChildNodes;
			var childrenCount = childNodes.Count;

			if (index >= childrenCount)
			{
				node.AppendChild(insertingNode);
				return;
			}

			var followingNode = childNodes[index];

			node.InsertBefore(insertingNode, followingNode);
		}
	}
}