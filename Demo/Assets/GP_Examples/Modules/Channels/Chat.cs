using UnityEngine;
using TMPro;

using GamePush;
using Examples.Console;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    [SerializeField] private TMP_InputField _channelIdInput;
    [SerializeField] private Button _openChatIDButton;
    [SerializeField] private Button _openChatButton;
    [SerializeField] private Button _isMainChatEnabledButton;
    [SerializeField] private Button _mainChatIDButton;

    private void OnEnable()
    {
        _openChatButton.onClick.AddListener(OpenChat);
        _openChatIDButton.onClick.AddListener(OpenChatWithID);
        _isMainChatEnabledButton.onClick.AddListener(IsMainChatEnabled);
        _mainChatIDButton.onClick.AddListener(MainChatID);
    }

    private void OnDisable()
    {
        _openChatButton.onClick.RemoveListener(OpenChat);
        _openChatIDButton.onClick.RemoveListener(OpenChatWithID);
        _isMainChatEnabledButton.onClick.RemoveListener(IsMainChatEnabled);
        _mainChatIDButton.onClick.RemoveListener(MainChatID);
    }


    public void OpenChat()
    {
        GP_Channels.OpenChat(OnOpen, OnClose, OnOpenError);
    }

    public void OpenChatWithID()
    {
        GP_Channels.OpenChat(int.Parse(_channelIdInput.text), OnOpen, OnClose, OnOpenError);
    }

    private void MainChatID()
    {
        int id = GP_Channels.MainChatId();
        ConsoleUI.Instance.Log(id);
    }

    private void IsMainChatEnabled()
    {
        bool isMainChatEnabled = GP_Channels.IsMainChatEnabled();
        ConsoleUI.Instance.Log(isMainChatEnabled);
    }

    private void OnOpen() => ConsoleUI.Instance.Log("ON OPEN CHAT");
    private void OnClose() => ConsoleUI.Instance.Log("ON CLOSE CHAT");
    private void OnOpenError() => ConsoleUI.Instance.Log("ON OPEN CHAT: ERROR");
}
