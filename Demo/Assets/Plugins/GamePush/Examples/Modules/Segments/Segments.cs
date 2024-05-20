using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GamePush;
using Examples.Console;

public class Segments : MonoBehaviour
{
    [SerializeField] private TMP_InputField _segmentTag;
    [Space]
    [SerializeField] private Button _buttonList;
    [SerializeField] private Button _buttonHas;

    private void OnEnable()
    {
        _buttonList.onClick.AddListener(List);
        _buttonHas.onClick.AddListener(Has);

        GP_Segments.OnSegmentEnter += OnEnter;
        GP_Segments.OnSegmentLeave += OnLeave;
    }

    private void OnDisable()
    {
        _buttonList.onClick.RemoveListener(List);
        _buttonHas.onClick.RemoveListener(Has);

        GP_Segments.OnSegmentEnter -= OnEnter;
        GP_Segments.OnSegmentLeave -= OnLeave;
    }

    public void List()
    {
        string list = GP_Segments.List();
        ConsoleUI.Instance.Log("Segments:\n " + list);
    }

    public void Has()
    {
        bool has = GP_Segments.Has(_segmentTag.text);
        ConsoleUI.Instance.Log($"Player in segment {_segmentTag.text}:\n {has}");
    }

    public void OnEnter(string tag)
    {
        ConsoleUI.Instance.Log("Enter segment: " + tag);
    }

    public void OnLeave(string tag)
    {
        ConsoleUI.Instance.Log("Leave segment: " + tag);
    }
}
