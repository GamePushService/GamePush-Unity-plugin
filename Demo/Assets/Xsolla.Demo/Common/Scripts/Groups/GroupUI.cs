using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class GroupUI : MonoBehaviour, IGroup
	{
		[SerializeField] MenuButton menuButton = default;

		void Awake()
		{
			menuButton.onClick = ((s) =>
			{
				if (OnGroupClick != null)
					OnGroupClick.Invoke(s);
			});
		}

		public string Id
		{
			get { return menuButton.Id; }
			set { menuButton.Id = value; }
		}

		public string Name
		{
			get { return menuButton.Text; }
			set { menuButton.Text = value; }
		}

		public Action<string> OnGroupClick { get; set; }

		public void Select(bool trigger = true)
		{
			menuButton.Select(trigger);
		}

		public void Deselect()
		{
			menuButton.Deselect();
		}

		public bool IsSelected()
		{
			return menuButton.IsSelected;
		}
	}
}
