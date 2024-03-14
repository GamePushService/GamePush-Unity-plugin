using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Examples.Console;
using UnityEngine.UI;
using TMPro;
using GamePush;

namespace Examples.Images
{
    public class Images : MonoBehaviour
    {
        [SerializeField] private Button _fetchButton;
        [SerializeField] private Button _uploadButton;
        [SerializeField] private Button _chooseButton;

        private void OnEnable()
        {
            _fetchButton.onClick.AddListener(Fetch);
            _uploadButton.onClick.AddListener(Upload);
            _chooseButton.onClick.AddListener(Choose);
        }

        private void OnDisable()
        {
            _fetchButton.onClick.RemoveListener(Fetch);
            _uploadButton.onClick.RemoveListener(Upload);
            _chooseButton.onClick.RemoveListener(Choose);
        }

        public void Fetch() => GP_Images.Fetch();
        public void Upload() => GP_Images.Upload();
        public void Choose() => GP_Images.Choose();
    }
}
