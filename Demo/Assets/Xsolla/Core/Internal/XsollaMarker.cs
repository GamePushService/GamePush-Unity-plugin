namespace Xsolla.Core
{
	public interface IXsollaMarkerHasDemoPart { }

	public partial class XsollaMarker
	{
		private static XsollaMarker _instance;

		private static XsollaMarker Marker
		{
			get
			{
				if (_instance == null)
					_instance = new XsollaMarker();
				return _instance;
			}
		}

		public static bool HasDemoPart => Marker is IXsollaMarkerHasDemoPart;
	}
}