using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public abstract class LoginPageController : MonoBehaviour
	{
		public Action OnSuccess { get; set; }
		public Action<Error> OnError { get; set; }
		public bool IsInProgress { get; protected set; }

		protected bool ValidateEmail(string email)
		{
			if (!string.IsNullOrEmpty(email))
			{
				var emailPattern = "^[a-zA-Z0-9-_.+]+[@][a-zA-Z0-9-_.]+[.][a-zA-Z]+$";
				var regex = new Regex(emailPattern);
				return regex.IsMatch(email);
			}
			else
			{
				return false;
			}
		}
	}
}
