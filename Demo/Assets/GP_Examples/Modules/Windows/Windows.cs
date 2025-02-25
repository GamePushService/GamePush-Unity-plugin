using Examples.Console;
using GamePush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.Windows
{
    public class Windows : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _titleInput;
        [SerializeField] private TMP_InputField _descriptionInput;
        [SerializeField] private TMP_InputField _textConfirmInput;
        [SerializeField] private TMP_InputField _textCancelInput;
        [SerializeField] private Toggle _invertButtonColors;
        [Space]
        [SerializeField] private Button _showConfirmButton;
        [SerializeField] private Button _showDefaultButton;
        
        private void OnEnable()
        {
            _showConfirmButton.onClick.AddListener(ShowConfirm);
            _showDefaultButton.onClick.AddListener(ShowConfirmDefault);
        }

        private void OnDisable()
        {
            _showConfirmButton.onClick.RemoveListener(ShowConfirm);
            _showDefaultButton.onClick.RemoveListener(ShowConfirmDefault);
        }

        private void ShowConfirm()
        {
            ConsoleUI.Instance.Log($"Show confirm window:");
            ConsoleUI.Instance.Log($" Title: {_titleInput.text}");
            ConsoleUI.Instance.Log($" Description: {_descriptionInput.text}");
            ConsoleUI.Instance.Log($" Confirm text: {_textConfirmInput.text}");
            ConsoleUI.Instance.Log($" Cancel text: {_textCancelInput.text}");
            ConsoleUI.Instance.Log($" Invert Colors: {_invertButtonColors.isOn} \n");
            GP_Windows.ShowConfirm(GetConfirmWindowDataData(), OnConfirm);
        }
        
        private void ShowConfirmDefault()
        {
            ConsoleUI.Instance.Log($"Show default confirm window \n");
            GP_Windows.ShowConfirm(OnConfirm);
        }
        
        private void OnConfirm(bool result) => ConsoleUI.Instance.Log("Confirm by player: " + result);

        private ConfirmWindowData GetConfirmWindowDataData()
        {
            return new ConfirmWindowData
            {
                title = _titleInput.text,
                description = _descriptionInput.text,
                textConfirm = _textConfirmInput.text,
                textCancel = _textCancelInput.text,
                invertButtonColors = _invertButtonColors.isOn
            };
        }
    }
}