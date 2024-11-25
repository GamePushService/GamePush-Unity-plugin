namespace Xsolla.Core.Editor
{
	public class DataNode : BaseManifestNode
	{
		public DataNode(string scheme, string host, string pathPrefix) : base(
			AndroidManifestConstants.DataTag,
			AndroidManifestConstants.IntentFilterTag)
		{
			AddAttribute("android:scheme", scheme);
			AddAttribute("android:host", host);

			if (!string.IsNullOrEmpty(pathPrefix) && pathPrefix != "/")
				AddAttribute("android:pathPrefix", pathPrefix);
		}
	}
}