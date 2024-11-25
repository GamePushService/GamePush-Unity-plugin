using System;

namespace Xsolla.Demo
{
	public interface IGroup
	{
		string Id { get; set; }

		string Name { get; set; }

		Action<string> OnGroupClick { get; set; }

		void Select(bool trigger = true);
		void Deselect();
		bool IsSelected();
	}
}