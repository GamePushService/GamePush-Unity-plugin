namespace Xsolla.Demo
{
	public class UserProfileEntryGenderValueConverter : BaseUserProfileValueConverter
	{
		public override string Convert(string value)
		{
			switch (value)
			{
				case UserProfileGender.MALE_SHORT:
					return UserProfileGender.MALE;
				case UserProfileGender.FEMALE_SHORT:
					return UserProfileGender.FEMALE;
				case UserProfileGender.OTHER_LOWERCASE:
					return UserProfileGender.OTHER;
				case UserProfileGender.PREFER_NOT_LOWERCASE:
					return UserProfileGender.PREFER_NOT;
				default:
					return value;
			}
		}

		public override string ConvertBack(string value)
		{
			switch (value)
			{
				case UserProfileGender.MALE:
					return UserProfileGender.MALE_SHORT;
				case UserProfileGender.FEMALE:
					return UserProfileGender.FEMALE_SHORT;
				case UserProfileGender.OTHER:
					return UserProfileGender.OTHER_LOWERCASE;
				case UserProfileGender.PREFER_NOT:
					return UserProfileGender.PREFER_NOT_LOWERCASE;
				default:
					return value;
			}
		}
	}
}
