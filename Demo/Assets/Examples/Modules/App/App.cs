using System;
using UnityEngine;
using UnityEngine.UI;
using GamePush;
using Examples.Console;

namespace Examples.App
{
    public class App : MonoBehaviour
    {

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
            string result = GP_App.ReviewRequest();
            ConsoleUI.Instance.Log("Review result: " + result);
        }

        public void CanReview()
        {
            string result = GP_App.CanReview();
            ConsoleUI.Instance.Log("Can Review: " + result);
        }

        public void AppShortcut()
        {
            string result = GP_App.AddShortcut();
            ConsoleUI.Instance.Log("Shortcut result: " + result);
        }

        public void CanAppShortcut()
        {
            string result = GP_App.CanAddShortcut();
            ConsoleUI.Instance.Log("Can Shortcut: " + result);
        }
    }
}