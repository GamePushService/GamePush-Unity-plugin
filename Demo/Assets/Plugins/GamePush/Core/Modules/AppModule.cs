using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;

namespace GamePush.Core
{
    public class AppModule
    {
        public event Action<int> OnReviewResult;
        public event Action<string> OnReviewClose;
        public event Action<bool> OnAddShortcut;

        private Project _project;
        private PlatformConfig _platformConfig;

        public void Init(Project project, PlatformConfig platformConfig)
        {
            _project = project;
            _platformConfig = platformConfig;
        }

        public string Title() => _project.name;

        public string Description() => _project.description;

        public string ProjectIcon() => _project.icon;

        public string AppLink() => _platformConfig.gameLink;

        public void ReviewRequest()
        {
            string error = "Not supported";
            OnReviewClose?.Invoke(error);
        }

        public bool IsAlreadyReviewed() => false;
        public bool CanReview() => false;

        public void AddShortcut()
        {
            OnAddShortcut?.Invoke(false);
        }

        public bool CanAddShortcut() => false;
    }
}
