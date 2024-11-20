namespace Xsolla.Demo
{
	public class UserProfileEntryPhoneValueConverter : BaseUserProfileValueConverter
	{
		public override string Convert(string value)
		{
			if (!string.IsNullOrEmpty(value) && value.Length == 12)
				return value.Insert(2, " ").Insert(6, " ").Insert(10, " ").Insert(13, " ");
			else
				return value;
		}

		public override string ConvertBack(string value)
		{
			if(!string.IsNullOrEmpty(value))
				return value.Replace(" ", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty);
			else
				return value;
		}
	}
}
