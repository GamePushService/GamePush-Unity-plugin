#if (UNITY_EDITOR || UNITY_STANDALONE)
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.Core.Browser
{
	internal class Keyboard2DBehaviour : MonoBehaviour
	{
		private IXsollaBrowserKeyboardInput keyboardInput;

		public event Action EscapePressed;

		private static readonly List<KeyCode> SystemKeys = new List<KeyCode> {
			KeyCode.Escape
		};

		// @formatter:off
		private static readonly Dictionary<KeyCode, string> ModificationKeys = new Dictionary<KeyCode, string>{
			{KeyCode.LeftShift, "Shift"},
			{KeyCode.RightShift, "Shift"},
			{KeyCode.LeftAlt, "Alt"},
			{KeyCode.RightAlt, "Alt"},
			{KeyCode.LeftControl, "Control"},
			{KeyCode.RightControl, "Control"}
		};
		
		private static readonly Dictionary<KeyCode, string> NumpadKeys = new Dictionary<KeyCode, string>{
			{KeyCode.Keypad0, "Digit0"},
			{KeyCode.Keypad1, "Digit1"},
			{KeyCode.Keypad2, "Digit2"},
			{KeyCode.Keypad3, "Digit3"},
			{KeyCode.Keypad4, "Digit4"},
			{KeyCode.Keypad5, "Digit5"},
			{KeyCode.Keypad6, "Digit6"},
			{KeyCode.Keypad7, "Digit7"},
			{KeyCode.Keypad8, "Digit8"},
			{KeyCode.Keypad9, "Digit9"}
		};
		// @formatter:on 

		private static readonly List<KeyCode> AllKeyCodes = Enum.GetValues(typeof(KeyCode)).OfType<KeyCode>()
			.Where(key => key < KeyCode.Mouse0)
			.Except(SystemKeys)
			.Except(ModificationKeys.Keys)
			.Except(NumpadKeys.Keys)
			.ToList();

		private void Awake()
		{
			keyboardInput = GetComponent<XsollaBrowser>().Input.Keyboard;
		}

		private void Update()
		{
			if (InputProxy.GetKeyUp(KeyCode.Escape))
			{
				EscapePressed?.Invoke();
			}

			foreach (var pair in ModificationKeys)
			{
				if (InputProxy.GetKeyDown(pair.Key))
					keyboardInput.KeyDown(pair.Value);

				if (InputProxy.GetKeyUp(pair.Key))
					keyboardInput.KeyUp(pair.Value);
			}

			foreach (var pair in NumpadKeys)
			{
				if (InputProxy.GetKeyDown(pair.Key))
					keyboardInput.PressKey(pair.Value);
			}

			AllKeyCodes.ForEach(code =>
			{
				if (InputProxy.GetKeyDown(code))
				{
					var key = KeysConverter.Convert(code);
					keyboardInput.PressKey(key);
				}
			});
		}
	}
}
#endif