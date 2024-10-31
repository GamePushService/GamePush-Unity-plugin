using System.Collections.Generic;

namespace Xsolla.Core.Editor.AutoFillSettings
{
	public class SettingsTree
	{
		private readonly TreeNode _root;
		public TreeNode Root => _root;

		public SettingsTree()
		{
			_root = new TreeNode(null,null,null);
		}

		public class TreeNode
		{
			public readonly dynamic value;
			public readonly string  valueView;
			public readonly TreeNode parent;
			public TreeNode[] children;

			public TreeNode(dynamic value, string valueView, TreeNode parent)
			{
				this.value = value;
				this.valueView = valueView;
				this.parent = parent;
			}
		}

		public List<string[]> GenerateTableView(int[] selection)
		{
			var result = new List<string[]>(selection.Length);
			var curNode = _root;
			for (int i = 0; i < selection.Length; i++)
			{
				var children = curNode.children;
				if (children == null || children.Length == 0)
					break;

				var subView = new string[children.Length];
				for (int j = 0; j < children.Length; j++)
					subView[j] = children[j].valueView;

				result.Add(subView);

				var nextIndex = selection[i];
				if (nextIndex >= children.Length || nextIndex < 0)
					break;

				curNode = children[nextIndex];
			}

			return result;
		}

		public dynamic[] GetValues(int[] selection)
		{
			var result = new dynamic[selection.Length];
			var curNode = _root;
			for (int i = 0; i < selection.Length; i++)
			{
				var children = curNode.children;
				if (children == null || children.Length == 0)
					break;

				var nextIndex = selection[i];
				if (nextIndex >= children.Length || nextIndex < 0)
					break;

				var nextNode = children[nextIndex];
				result[i] = nextNode.value;
				curNode = nextNode;
			}

			return result;
		}
	}
}