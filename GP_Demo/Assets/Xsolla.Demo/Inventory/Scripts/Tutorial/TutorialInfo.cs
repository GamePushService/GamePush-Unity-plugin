using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public enum ActivationScreen
	{
		None = 0,
		Inventory = 1,
		MainMenu = 2
	}

	[CreateAssetMenu(fileName = "TutorialInfo", menuName = "Xsolla/ScriptableObjects/TutorialInfo", order = 1)]
	public class TutorialInfo : ScriptableObject
	{
		[Serializable]
		public class TutorialStep
		{
			public string title;
			public string description;
			public string prevButtonText;
			public string nextButtonText;
			public string highlightElementId;
			public ActivationScreen screenToActivate;
			public string associatedDocumentation;

			public string FormattedTitle => title.Replace("\\n", "\n");
			public string FormattedDescription => description.Replace("\\n", "\n");
		}

		public List<TutorialStep> tutorialSteps;
	}
}