using UnityEngine;

namespace Xsolla.Demo
{
	public class SimpleButtonWithObjects : SimpleButton
	{
		[SerializeField] GameObject[] NormalStateObjects = default;
		[SerializeField] GameObject[] HoverStateObjects = default;
		[SerializeField] GameObject[] PressedStateObjects = default;

		protected override void OnNormal()
		{
			base.OnNormal();

			EnableObjects(NormalStateObjects);
			DisableObjects(HoverStateObjects);
			DisableObjects(PressedStateObjects);
		}

		protected override void OnHover()
		{
			base.OnHover();

			DisableObjects(NormalStateObjects);
			EnableObjects(HoverStateObjects);
			DisableObjects(PressedStateObjects);
		}

		protected override void OnPressed()
		{
			base.OnPressed();

			DisableObjects(NormalStateObjects);
			DisableObjects(HoverStateObjects);
			EnableObjects(PressedStateObjects);
		}

		void EnableObjects(GameObject[] objects)
		{
			SetObjectsActive(objects, state: true);
		}

		void DisableObjects(GameObject[] objects)
		{
			SetObjectsActive(objects, state: false);
		}

		void SetObjectsActive(GameObject[] objects, bool state)
		{
			if (objects == null || objects.Length == 0)
				return;
			else
			{
				for (int i = 0; i < objects.Length; i++)
					objects[i].SetActive(state);
			}
		}
	}
}
