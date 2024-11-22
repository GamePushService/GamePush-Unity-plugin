using System;
using UnityEngine;
using UnityEngine.UI;
using GamePush;
using Examples.Console;

namespace Examples.App
{
    public class App : MonoBehaviour
    {
        [SerializeField] private Button _appInfoButton;
        [SerializeField] private Button _reviewRequestButton;
        [SerializeField] private Button _canReviewButton;
        [SerializeField] private Button _appShortcutButton;
        [SerializeField] private Button _canAppShortcutButton;

        private void OnEnable()
        {
            _appInfoButton.onClick.AddListener(GameInfo);
            _reviewRequestButton.onClick.AddListener(ReviewRequest);
            _canReviewButton.onClick.AddListener(CanReview);
            _appShortcutButton.onClick.AddListener(AppShortcut);
            _canAppShortcutButton.onClick.AddListener(CanAppShortcut);
        }

        private void OnDisable()
        {
            _appInfoButton.onClick.RemoveListener(GameInfo);
            _reviewRequestButton.onClick.RemoveListener(ReviewRequest);
            _canReviewButton.onClick.RemoveListener(CanReview);
            _appShortcutButton.onClick.RemoveListener(AppShortcut);
            _canAppShortcutButton.onClick.RemoveListener(CanAppShortcut);
        }

        public void GameInfo()
        {
            string title = GP_App.Title();
            string description = GP_App.Description();
            string url = GP_App.Url();

            ConsoleUI.Instance.Log("App Title: " + title);
            ConsoleUI.Instance.Log("App Description: " + description);
            ConsoleUI.Instance.Log("App URL: " + url);
        }

        public void ReviewRequest()
        {
            GP_App.ReviewRequest(OnReviewResult, OnReviewClose);
            ConsoleUI.Instance.Log("Send review request");
        }

        private void OnReviewResult(int result)
        {
            ConsoleUI.Instance.Log("Review result: " + result);
        }

        private void OnReviewClose(string error)
        {
            ConsoleUI.Instance.Log("Review result: " + error);
        }

        public void CanReview()
        {
            bool result = GP_App.CanReview();
            ConsoleUI.Instance.Log("Can Review: " + result);
        }

        public void AppShortcut()
        {
            GP_App.AddShortcut(OnAddShortcut);
            ConsoleUI.Instance.Log("Send add shortcut request");
        }

        private void OnAddShortcut(bool success)
        {
            ConsoleUI.Instance.Log("Shortcut result: " + success);
        }

        public void CanAppShortcut()
        {
            bool result = GP_App.CanAddShortcut();
            ConsoleUI.Instance.Log("Can Shortcut: " + result);
        }
    }
}