using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Examples.Console;
using UnityEngine.UI;
using TMPro;
using GamePush;
using GamePush.Utilities;

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
        [SerializeField] private TMP_InputField _inputTags;
        [SerializeField] private TMP_InputField _inputUrl;
        [SerializeField] private TMP_InputField _inputW;
        [SerializeField] private TMP_InputField _inputH;
        [Space]
        [SerializeField] private Button _setImageButton;
        [SerializeField] private Image _image;

        private void OnEnable()
        {
            _fetchButton.onClick.AddListener(Fetch);
            _fetchMoreButton.onClick.AddListener(FetchMore);
            _uploadButton.onClick.AddListener(Upload);
            _uploadUrlButton.onClick.AddListener(UploadUrl);
            _chooseButton.onClick.AddListener(Choose);
            _resizeButton.onClick.AddListener(Resize);

            _setImageButton.onClick.AddListener(SetImage);

            GP_Images.OnImagesCanLoadMore += CanLoadMore;
        }

        private void OnDisable()
        {
            _fetchButton.onClick.RemoveListener(Fetch);
            _fetchMoreButton.onClick.RemoveListener(FetchMore);
            _uploadButton.onClick.RemoveListener(Upload);
            _uploadUrlButton.onClick.RemoveListener(UploadUrl);
            _chooseButton.onClick.RemoveListener(Choose);
            _resizeButton.onClick.RemoveListener(Resize);

            _setImageButton.onClick.RemoveListener(SetImage);

            GP_Images.OnImagesCanLoadMore -= CanLoadMore;
        }

        private string[] GetTags()
        {
            string[] tagsText = _inputTags.text.Split(",");
            List<string> tags = new List<string>();

            foreach(string tag in tagsText)
            {
                tags.Add(tag.Trim());
            }

            return tags.ToArray();
        }

        private ImagesFetchFilter GetFilter()
        {
            ImagesFetchFilter filter = new ImagesFetchFilter();
            filter.tags = GetTags();
            return filter;
        }
        

        public void Fetch() => GP_Images.Fetch(GetFilter(), OnImagesFetch, OnImagesError);
        public void FetchMore() => GP_Images.FetchMore(GetFilter(), OnImagesFetch, OnImagesError);
        
        public void Upload() => GP_Images.Upload(GetTags(), OnImagesUpload, OnImagesError);
        public void UploadUrl()=>  GP_Images.UploadUrl(_inputUrl.text, GetTags(), OnImagesUpload, OnImagesError);

        public void Choose() => GP_Images.Choose(OnImagesChoose, OnImagesError);

        public void Resize()
        {
            ImageResizeData resizeData = new ImageResizeData();
            resizeData.url = _inputUrl.text;
            resizeData.width = int.Parse(_inputW.text);
            resizeData.height = int.Parse(_inputH.text);

            GP_Images.Resize(resizeData, OnImagesResize, OnImagesError);
        }
          
        public async void SetImage()
        {
            string url = GP_Images.FormatToPng(_inputUrl.text);
            ConsoleUI.Instance.Log(url);
            await UtilityImage.DownloadImageAsync(url, _image);
        }

        private void OnImagesFetch(List<ImageData> images)
        {
            foreach(ImageData image in images)
            {
                ConsoleUI.Instance.Log("ID: " + image.id);
                ConsoleUI.Instance.Log("Tags: " + string.Join(",", image.tags));
                ConsoleUI.Instance.Log("URL: " + image.src);
                ConsoleUI.Instance.Log(" ");
            }
        }

        private void OnImagesUpload(ImageData image)
        {
            _inputUrl.text = image.src;
            ConsoleUI.Instance.Log("Upload image");
            ConsoleUI.Instance.Log("ID: " + image.id);
            ConsoleUI.Instance.Log("PlayerID: " + image.playerId);
            ConsoleUI.Instance.Log(" ");
        }

        private void OnImagesChoose(string result)
        {
            _inputUrl.text = result;
            ConsoleUI.Instance.Log("result: " + result);
        }

        private void OnImagesResize(string result)
        {
            _inputUrl.text = result;
            ConsoleUI.Instance.Log("Resize image: " + result);
        }

        private void OnImagesError(string error)
        {
            ConsoleUI.Instance.Log("ERROR: " + error);
        }

        public void CanLoadMore(bool can)
        {
            ConsoleUI.Instance.Log("Can load more: " + can);
            ConsoleUI.Instance.Log(" ");
        }
    }
}
