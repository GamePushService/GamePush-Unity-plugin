using System.Xml;

namespace Xsolla.Core.Editor
{
	public class AndroidManifestWrapper
	{
		private readonly string manifestPath;
		private readonly XmlDocument xmlDocument;

		public AndroidManifestWrapper(string path)
		{
			manifestPath = path;

			xmlDocument = new XmlDocument();
			xmlDocument.Load(path);

			var applicationNode = xmlDocument.FindNodeRecursive(new FindByTag(AndroidManifestConstants.ApplicationTag));
			if (applicationNode == null)
			{
				XDebug.LogError($"Failed to parse AndroidManifest.xml with path {path}");
			}
		}

		public void AddNode(BaseManifestNode node, IFindCriteria<XmlNode> parentNodeCriteria)
		{
			var parentNode = xmlDocument.FindNodeRecursive(parentNodeCriteria);
			if (parentNode != null)
			{
				parentNode.AddAndroidManifestNode(xmlDocument, node);
			}
			else
			{
				XDebug.LogError("Failed to add new node to AndroidManifest.xml since no specified parent node found");
			}
		}

		public void RemoveNode(IFindCriteria<XmlNode> parentNodeCriteria, IFindCriteria<XmlNode> removeCriteria)
		{
			var parentNode = xmlDocument.FindNodeRecursive(parentNodeCriteria);
			if (parentNode != null)
			{
				var xmlNode = parentNode.FindNodeInChildren(removeCriteria);
				if (xmlNode != null)
				{
					parentNode.RemoveChild(xmlNode);
				}
			}
		}

		public bool ContainsNode(IFindCriteria<XmlNode> parentNodeCriteria, IFindCriteria<XmlNode> containsCriteria)
		{
			var parentNode = xmlDocument.FindNodeRecursive(parentNodeCriteria);
			if (parentNode != null)
			{
				var xmlNode = parentNode.FindNodeInChildren(containsCriteria);
				return xmlNode != null;
			}

			return false;
		}

		public void SaveManifest()
		{
			var settings = new XmlWriterSettings {
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace,
				Indent = true,
				IndentChars = "    "
			};

			using (var xmlWriter = XmlWriter.Create(manifestPath, settings))
			{
				xmlDocument.Save(xmlWriter);
			}
		}
	}
}