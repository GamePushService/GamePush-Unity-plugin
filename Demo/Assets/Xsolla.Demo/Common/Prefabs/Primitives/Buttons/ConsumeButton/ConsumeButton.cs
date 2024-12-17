using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class ConsumeButton : MonoBehaviour
	{
		public SimpleTextButton consumeButton;
		public Action onClick { get; set; }
		public ValueCounter counter;

		void Start()
		{
			consumeButton.onClick = () =>
			{
				if (counter > 0)
				{
					onClick?.Invoke();
				}
				else
				{
					XDebug.Log("You try consume item with quantity = 0!");
				}
			};
		}

		public void ShowCounter(bool show)
		{
			counter.gameObject.SetActive(show);
		}
	}
}