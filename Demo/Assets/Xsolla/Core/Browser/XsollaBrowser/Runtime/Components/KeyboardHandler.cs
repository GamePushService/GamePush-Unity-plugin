using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Xsolla.XsollaBrowser
{
	public class KeyboardHandler : MonoBehaviour, IBrowserHandler
	{
		private BrowserPage Page;
		private Dictionary<KeyCode, string> NavigationKeys;
		private Dictionary<char, string> ControlCharacters;

		private void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus && Page != null)
				Page.UpKeyAsync("alt");
		}

		public void Run(BrowserPage page, CancellationToken cancellationToken)
		{
			Page = page;

			NavigationKeys = new Dictionary<KeyCode, string> {
				[KeyCode.Escape] = "Escape",
				[KeyCode.LeftArrow] = "ArrowLeft",
				[KeyCode.RightArrow] = "ArrowRight",
				[KeyCode.UpArrow] = "ArrowUp",
				[KeyCode.DownArrow] = "ArrowDown",
				[KeyCode.Home] = "Home",
				[KeyCode.End] = "End",
				[KeyCode.PageUp] = "PageUp",
				[KeyCode.PageDown] = "PageDown"
			};

			ControlCharacters = new Dictionary<char, string> {
				['\n'] = "Enter",
				['\r'] = "Enter",
				['\b'] = "Backspace",
				['\t'] = "Tab"
			};

			StartCoroutine(TrackInputLoop(cancellationToken));
		}

		public void Stop()
		{
			StopAllCoroutines();
		}

		private IEnumerator TrackInputLoop(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				if (Input.anyKey)
				{
					if (!Input.GetKeyDown(KeyCode.Escape))
						ProcessInput();
				}

				yield return null;
			}
		}

		private void ProcessInput()
		{
			if (ProcessNavigation())
				return;

			if (ProcessClipboard())
				return;

			if (ProcessControlCharacters())
				return;

			ProcessText();
		}

		private void ProcessText()
		{
			foreach (var character in Input.inputString)
			{
				Page.SendCharacterAsync(character.ToString());
			}
		}

		private bool ProcessControlCharacters()
		{
			foreach (var symbol in Input.inputString)
			{
				if (ControlCharacters.TryGetValue(symbol, out var key))
				{
					Page.PressKeyAsync(key);
					return true;
				}
			}

			return false;
		}

		private bool ProcessNavigation()
		{
			foreach (var kvp in NavigationKeys)
			{
				if (Input.GetKey(kvp.Key))
				{
					Page.DownKeyAsync(kvp.Value);
					return true;
				}
			}

			return false;
		}

		private bool ProcessClipboard()
		{
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
			if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKeyDown(KeyCode.V))
			{
				PasteFromClipboard();
				return true;
			}
#else
			if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.V))
			{
				PasteFromClipboard();
				return true;
			}
#endif
			return false;
		}

		private void PasteFromClipboard()
		{
			var text = GUIUtility.systemCopyBuffer;
			if (!string.IsNullOrEmpty(text))
				Page.SendCharacterAsync(text);
		}
	}
}