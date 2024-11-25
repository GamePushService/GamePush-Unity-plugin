using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class UserCatalog : MonoSingleton<UserCatalog>
	{
		public bool IsUpdated { get; private set; } = true;
	}
}
