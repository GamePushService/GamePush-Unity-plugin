using System;

namespace Xsolla.Demo
{
	public class UserProfileEntryBirthdayValueConverter : BaseUserProfileValueConverter
	{
		public override string Convert(string value)
		{
			if (DateTime.TryParse(value, out DateTime birthday))
				return birthday.ToString("dd/MM/yyyy").Replace(".","/");
			else
				return value;
		}

		public override string ConvertBack(string value)
		{
			if (DateTime.TryParse(value, out DateTime birthday))
				return birthday.ToString("yyyy-MM-dd");
			else
				return value;
		}
	}
}
