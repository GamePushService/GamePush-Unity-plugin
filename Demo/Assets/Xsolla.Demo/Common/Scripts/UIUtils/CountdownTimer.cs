using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
    public class CountdownTimer : MonoBehaviour
    {
		[SerializeField] private Text OutputText = default;
		[Tooltip("Must contain %TIMER% marker. Example: 'Expires in %TIMER%'")]
		[SerializeField] private string OutputTemplate = default;
		[Tooltip("Supported values are d (days), h (hours), m (minutes), s (seconds). Example: 'hhh:mm:ss'")]
		[SerializeField] private string TimerTemplate = default;
		[SerializeField] private int DefaultSeconds = default;
		[SerializeField] private TimerAction OnEnableAction = default;
		[SerializeField] private TimerAction OnDisableAction = default;

		private const string TIMER_MARK = "%TIMER%";

		private Coroutine _timerCoroutine;
		private WaitForSeconds _waitOneSecond = new WaitForSeconds(1.0f);
		private int? _lastTimerValue;
		private int? _currentSecondsLeft;
		private string[,] _timerTemplateFormat;

		public event Action TimeIsUp;

		private void Awake()
		{
			_timerTemplateFormat = GenerateTemplateFormat(TimerTemplate);

			if (string.IsNullOrEmpty(TimerTemplate))
				XDebug.LogWarning("TimerTemplate is not set");

			if (!string.IsNullOrEmpty(OutputTemplate) && !OutputTemplate.Contains(TIMER_MARK))
				XDebug.LogWarning("Incorrect OutputTemplate, check that %TIMER% is included");
		}

		public void StartTimer()
		{
			StartTimer(DefaultSeconds);
		}

		public void StartTimer(int seconds)
		{
			StopTimer();

			_lastTimerValue = seconds;
			_currentSecondsLeft = seconds;

			_timerCoroutine = StartCoroutine(TimerCoroutine());
		}

		public void ResetTimer()
		{
			if (_lastTimerValue.HasValue)
				ResetTimer(_lastTimerValue.Value);
			else
				ResetTimer(DefaultSeconds);
		}

		public void ResetTimer(int seconds)
		{
			StartTimer(seconds);
		}

		public void StopTimer()
		{
			if (_timerCoroutine != null)
			{
				StopCoroutine(_timerCoroutine);
				_timerCoroutine = null;
				_currentSecondsLeft = null;
			}
		}

		private string[,] GenerateTemplateFormat(string template)
		{
			var format = new string[4,2];

			if (string.IsNullOrEmpty(template))
				return format;

			var formatChars = new char[] {'d','h','m','s'};

			for (int i = 0; i < formatChars.Length; i++)
			{
				var substring = GetSubstring(formatChars[i],template);
				if (substring != null)
				{
					format[i, 0] = substring;
					format[i, 1] = $"D{substring.Length}";
				}
			}

			return format;
		}

		private string GetSubstring(char value, string input)
		{
			int count = default(int);
			int index = default(int);
			int startIndex = default(int);

			do
			{
				index = input.IndexOf(value, startIndex);

				if (index != -1)
					count += 1;
				else
					break;

				startIndex = index + 1;
			} while (startIndex < input.Length);

			if (count > 0)
				return new string(value, count);
			else
				return null;
		}

		private void OnEnable()
		{
			DoTimerAction(OnEnableAction);
		}

		private void OnDisable()
		{
			DoTimerAction(OnDisableAction);
		}

		private void DoTimerAction(TimerAction timerAction)
		{
			switch (timerAction)
			{
				case TimerAction.None: //Do nothing
					break;
				case TimerAction.Start:
					StartTimer();
					break;
				case TimerAction.Stop:
					StopTimer();
					break;
				case TimerAction.Reset:
					ResetTimer();
					break;
			}
		}

		private IEnumerator TimerCoroutine()
		{
			OutputTimerValue(_currentSecondsLeft);

			while (_currentSecondsLeft.HasValue && _currentSecondsLeft.Value > 0)
			{
				yield return _waitOneSecond;

				if (_currentSecondsLeft.HasValue)
					_currentSecondsLeft -= 1;

				OutputTimerValue(_currentSecondsLeft);
			}

			if (_currentSecondsLeft.HasValue)
				TimeIsUp?.Invoke();
		}

		private void OutputTimerValue(int? secondsToOutput)
		{
			if (!OutputText || !secondsToOutput.HasValue || string.IsNullOrEmpty(TimerTemplate))
				return;

			var commonValue = secondsToOutput.Value;
			var output = TimerTemplate;

			for (int i = 0; i <= 3; i++)
			{
				var formatPart = _timerTemplateFormat[i,0];
				if (formatPart == null)
					continue;

				var value = 0;
				var valueFormat = _timerTemplateFormat[i,1];

				switch (i)
				{
					case 0:
						//Days
						if (commonValue >= 86400)
						{
							value = commonValue / 86400;
							commonValue %= 86400;
						}
						break;
					case 1:
						//Hours
						if (commonValue >= 3600)
						{
							value = commonValue / 3600;
							commonValue %= 3600;
						}
						break;
					case 2:
						//Minutes
						if (commonValue >= 60)
						{
							value = commonValue / 60;
							commonValue %= 60;
						}
						break;
					case 3:
						//Seconds
						value = commonValue;
						break;
				}

				output = output.Replace(formatPart,value.ToString(valueFormat));
			}

			if (!string.IsNullOrEmpty(OutputTemplate))
				OutputText.text = OutputTemplate.Replace(TIMER_MARK, output);
			else
				OutputText.text = output;
		}

		public enum TimerAction
		{
			None, Start, Stop, Reset
		};
	}
}
