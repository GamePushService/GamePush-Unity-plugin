using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush;

namespace Examples.Logger
{
    public class Logger : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _logTitle;
        [SerializeField] private TMP_InputField _logText;
        [Space]
        [SerializeField] private Button _buttonLog;
        [SerializeField] private Button _buttonInfo;
        [SerializeField] private Button _buttonWarn;
        [SerializeField] private Button _buttonError;

        private void OnEnable()
        {
            _buttonLog.onClick.AddListener(Log);
            _buttonInfo.onClick.AddListener(Info);
            _buttonWarn.onClick.AddListener(Warn);
            _buttonError.onClick.AddListener(Error);
        }

        private void OnDisable()
        {
            _buttonLog.onClick.RemoveListener(Log);
            _buttonInfo.onClick.RemoveListener(Info);
            _buttonWarn.onClick.RemoveListener(Warn);
            _buttonError.onClick.RemoveListener(Error);
        }

        public void Log()
        {
            GP_Logger.Log(_logTitle.text, _logText.text);
        }

        public void Info()
        {
            GP_Logger.Info(_logTitle.text, _logText.text);
        }

        public void Warn()
        {
            GP_Logger.Warn(_logTitle.text, _logText.text);
        }

        public void Error()
        {
            GP_Logger.Error(_logTitle.text, _logText.text);
        }
    }
}
