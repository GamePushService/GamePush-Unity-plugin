#if ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Xsolla.Core
{
	public static class InputProxy
	{
		private static readonly Dictionary<KeyCode, ButtonControl> Controls;

		public static bool GetKeyDown(KeyCode code)
		{
			return Controls.ContainsKey(code) && Controls[code].wasPressedThisFrame;
		}

		public static bool GetKeyUp(KeyCode code)
		{
			return Controls.ContainsKey(code) && Controls[code].wasReleasedThisFrame;
		}

		public static Vector3 MousePosition
		{
			get => Mouse.current.position.ReadValue();
		}

		private static void AddKeyboardControls()
		{
			var keys = Keyboard.current.allKeys.ToList();
			var codes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>();

			foreach (var code in codes)
			{
				var codeName = code.ToString();
				codeName = char.ToLower(codeName[0]) + codeName.Substring(1);

				if (codeName.StartsWith("joystick"))
					continue;

				if (codeName.StartsWith("mouse"))
					continue;

				if (TryAddKeyControl(code, keys.FirstOrDefault(x => x.name == codeName)))
					continue;

				if (codeName.StartsWith("alpha"))
				{
					var name = codeName.Replace("alpha", string.Empty);
					if (TryAddKeyControl(code, keys.FirstOrDefault(x => x.name == name)))
						continue;
				}

				if (codeName.StartsWith("keypad"))
				{
					var name = codeName.Replace("keypad", "numpad");
					if (TryAddKeyControl(code, keys.FirstOrDefault(x => x.name == name)))
						continue;
				}

				TryAddKeyControl(KeyCode.Return, keys.FirstOrDefault(x => x.name == "enter"));
				TryAddKeyControl(KeyCode.KeypadEnter, keys.FirstOrDefault(x => x.name == "enter"));
				TryAddKeyControl(KeyCode.BackQuote, keys.FirstOrDefault(x => x.name == "backquote"));
				
				TryAddKeyControl(KeyCode.LeftControl, keys.FirstOrDefault(x => x.name == "leftCtrl"));
				TryAddKeyControl(KeyCode.RightControl, keys.FirstOrDefault(x => x.name == "rightCtrl"));

				TryAddKeyControl(KeyCode.LeftCommand, keys.FirstOrDefault(x => x.name == "leftMeta"));
				TryAddKeyControl(KeyCode.LeftWindows, keys.FirstOrDefault(x => x.name == "leftMeta"));
				TryAddKeyControl(KeyCode.LeftApple, keys.FirstOrDefault(x => x.name == "leftMeta"));

				TryAddKeyControl(KeyCode.RightCommand, keys.FirstOrDefault(x => x.name == "rightMeta"));
				TryAddKeyControl(KeyCode.RightWindows, keys.FirstOrDefault(x => x.name == "rightMeta"));
				TryAddKeyControl(KeyCode.RightApple, keys.FirstOrDefault(x => x.name == "rightMeta"));

				TryAddKeyControl(KeyCode.Print, keys.FirstOrDefault(x => x.name == "printScreen"));
				TryAddKeyControl(KeyCode.Numlock, keys.FirstOrDefault(x => x.name == "numLock"));
				TryAddKeyControl(KeyCode.Menu, keys.FirstOrDefault(x => x.name == "contextMenu"));
			}
		}

		private static void AddMouseControls()
		{
			TryAddKeyControl(KeyCode.Mouse0, Mouse.current.leftButton);
			TryAddKeyControl(KeyCode.Mouse1, Mouse.current.rightButton);
			TryAddKeyControl(KeyCode.Mouse2, Mouse.current.middleButton);
		}

		private static bool TryAddKeyControl(KeyCode code, ButtonControl control)
		{
			if (control == null)
				return false;

			if (Controls.ContainsKey(code))
				return false;

			Controls.Add(code, control);
			return true;
		}

		static InputProxy()
		{
			Controls = new Dictionary<KeyCode, ButtonControl>();
			AddKeyboardControls();
			AddMouseControls();
		}
	}
}

#endif