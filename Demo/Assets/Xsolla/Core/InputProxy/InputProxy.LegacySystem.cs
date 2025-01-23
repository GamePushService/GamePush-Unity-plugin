#if !ENABLE_INPUT_SYSTEM
using UnityEngine;

namespace Xsolla.Core
{
	public static class InputProxy
	{
		public static bool GetKeyDown(KeyCode code)
		{
			return Input.GetKeyDown(code);
		}

		public static bool GetKeyUp(KeyCode code)
		{
			return Input.GetKeyUp(code);
		}

		public static Vector3 MousePosition => Input.mousePosition;
	}
}

#endif