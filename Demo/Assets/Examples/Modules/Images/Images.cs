using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Examples.Console;
using UnityEngine.UI;
using TMPro;
using GamePush;
using GP_Utilities;

namespace Examples.Images
{
    public class Images : MonoBehaviour
    {
        [SerializeField] private Button _uploadButton;
        [SerializeField] private Button _uploadUrlButton;
        [SerializeField] private Button _chooseButton;
        [SerializeField] private Button _fetchButton;
        [SerializeField] private Button _fetchMoreButton;
        [SerializeField] private Button _resizeButton;
        [Space]
        [SerializeField] private Button _setImageButton;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_InputField _inputUrl;
        [SerializeField] private TMP_InputField _inputTags;

        private void OnEnable()
        {
            _fetchButton.onClick.AddListener(Fetch);
            _uploadButton.onClick.AddListener(Upload);
            _uploadUrlButton.onClick.AddListener(Upload);
            _chooseButton.onClick.AddListener(Choose);

            _setImageButton.onClick.AddListener(SetImage);
        }

        private void OnDisable()
        {
            _fetchButton.onClick.RemoveListener(Fetch);
            _uploadButton.onClick.RemoveListener(Upload);
            _chooseButton.onClick.RemoveListener(Choose);

            _setImageButton.onClick.RemoveListener(SetImage);
        }

        private string[] GetTags()
        {
            return _inputTags.text.Split(",");
        }

        public void Fetch() => GP_Images.Fetch();
        public void Upload() => GP_Images.Upload(GetTags());
        public void UploadUrl() => GP_Images.UploadUrl(_inputUrl.text, GetTags());
        public void Choose() => GP_Images.Choose();

        public async void SetImage()
        {
            await GP_Utility.DownloadImageAsync(_inputUrl.text, _image);
        }


    }
}
