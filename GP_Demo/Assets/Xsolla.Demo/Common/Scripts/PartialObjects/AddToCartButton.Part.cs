using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public partial class AddToCartButton : MonoBehaviour
	{
#pragma warning disable 0067
		public static event Action OnCursorEnter;
		public static event Action OnCursorExit;
#pragma warning restore 0067
	}
}
