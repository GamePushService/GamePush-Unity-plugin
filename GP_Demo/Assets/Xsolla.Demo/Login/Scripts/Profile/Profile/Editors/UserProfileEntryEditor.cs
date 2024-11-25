using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class UserProfileEntryEditor : MonoBehaviour
	{
		public event Action<string> UserProfileEntryEdited;

		protected void RaiseEntryEdited(string value)
		{
			UserProfileEntryEdited?.Invoke(value);
		}

		public abstract void SetInitial(string value);
	}
}
