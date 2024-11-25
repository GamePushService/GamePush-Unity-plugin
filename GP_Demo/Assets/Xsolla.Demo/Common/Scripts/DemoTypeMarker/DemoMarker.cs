namespace Xsolla.Demo
{
    public partial class DemoMarker
    {
		private static DemoMarker _marker;
		public static DemoMarker Marker
		{
			get
			{
				if (_marker == null)
					_marker = new DemoMarker();

				return _marker;
			}
		}

		public static bool IsStorePartAvailable => (Marker is IStoreDemoMarker);
		public static bool IsInventoryPartAvailable => (Marker is IInventoryDemoMarker);
		public static bool IsLoginPartAvailable => (Marker is ILoginDemoMarker);

		public static bool IsStoreDemo		=> IsStorePartAvailable && IsInventoryPartAvailable && IsLoginPartAvailable;
		public static bool IsInventoryDemo	=> !IsStorePartAvailable && IsInventoryPartAvailable && IsLoginPartAvailable;
		public static bool IsLoginDemo		=> !IsStorePartAvailable && !IsInventoryPartAvailable && IsLoginPartAvailable;
	}
}
