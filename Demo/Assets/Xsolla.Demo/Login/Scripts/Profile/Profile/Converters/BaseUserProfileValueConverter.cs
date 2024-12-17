using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseUserProfileValueConverter : MonoBehaviour
	{
		public abstract string Convert(string value);
		public abstract string ConvertBack(string value);
	}
}
