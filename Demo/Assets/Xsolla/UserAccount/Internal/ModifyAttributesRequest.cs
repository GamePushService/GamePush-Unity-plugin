using System;
using System.Collections.Generic;
using System.Linq;

namespace Xsolla.UserAccount
{
	[Serializable]
	internal class ModifyAttributesRequest
	{
		public List<UserAttribute> attributes;
		public int publisher_project_id;
		public List<string> removing_keys;

		public ModifyAttributesRequest(List<UserAttribute> attributes, string projectId, List<string> removingKeys)
		{
			this.attributes = attributes != null && attributes.Any() ? attributes : new List<UserAttribute>();
			publisher_project_id = Convert.ToInt32(projectId);
			removing_keys = removingKeys != null && removingKeys.Any() ? removingKeys : new List<string>();
		}
	}
}